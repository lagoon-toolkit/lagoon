(function () {
    window.BlazorInputFile = {
        init: function(elem, componentInstance) {
            elem._blazorInputFileNextFileId = 0;
            elem._blazorFilesById = {};

            elem.addEventListener('change', function handleInputFileChange(event) {
                // Reduce to purely serializable data, plus build an index by ID
                var fileList = Array.prototype.map.call(elem.files, function (file) {
                    var result = {
                        id: ++elem._blazorInputFileNextFileId,
                        lastModified: new Date(file.lastModified).toISOString(),
                        name: file.name,
                        size: file.size,
                        type: file.type,
                        relativePath: file.webkitRelativePath
                    };
                    elem._blazorFilesById[result.id] = result;

                    // Attach the blob data itself as a non-enumerable property so it doesn't appear in the JSON
                    Object.defineProperty(result, 'blob', { value: file });

                    return result;
                });

                componentInstance.invokeMethodAsync('NotifyChange', fileList).then(function () {
                    //reset file value ,otherwise, the same filename will not be trigger change event again
                    elem.value = '';
                }, function (err) {
                    //reset file value ,otherwise, the same filename will not be trigger change event again
                    elem.value = '';
                    throw new Error(err);
                });
            });
        },

        /**
         * Upload select files directly without retrieving Stream or data from c#
         * 
         * @param {DotnetRef} dotnetRef Component used to report progession / complete / error
         * @param {any} element The HTMLInputElement
         * @param {any} uploadUrl Xhr POST destination
         * @param {any} files Files name to send 
         * @param {any} token Optionnal authorization header to add to the request
         */
        uploadWithJs: function (dotnetRef, element, uploadUrl, files, token) {
            return new Promise(function (resolve, reject) {
                // Prepare file to post (multipart data)
                var data = new FormData();
                for (const filename of files) {
                    var file = getFileByName(element, filename);
                    data.append('file-' + file.id, file.blob);
                }
                // Initialise a new XMLHttpRequest
                var xhr = new window.XMLHttpRequest();
                // Used to track upload speed
                xhr.bytesSecond = -1;
                // Follow upload progression
                xhr.upload.addEventListener("progress", function (evt) {
                    if (!xhr.lastUpdate) xhr.lastUpdate = new Date();
                    var ellapsed = new Date() - xhr.lastUpdate;
                    if (ellapsed > 1000) {
                        if (xhr.lastLoaded) {
                            var diff = parseInt(evt.loaded) - xhr.lastLoaded;
                            xhr.bytesSecond = (1000 * diff / ellapsed);
                        }
                        xhr.lastLoaded = parseInt(evt.loaded);
                        xhr.lastUpdate = new Date();
                    }
                    if (evt.lengthComputable) {
                        dotnetRef.invokeMethodAsync("UploadProgressionAsync", parseInt(evt.loaded * 100 / evt.total), parseInt(xhr.bytesSecond));
                    }
                }, false);
                // Keep reference to the XHR to be able to cancel the upload
                element.XHR = xhr;
                // Prepare XHR options
                var xhrOptions = {
                    url: uploadUrl,
                    data: data,
                    cache: false,
                    contentType: false,
                    processData: false,
                    method: 'POST',
                    success: function (data) {
                        // Reset XHR
                        element.XHR = null;
                        // Complete callback
                        dotnetRef.invokeMethodAsync("UploadCompleteAsync");
                        resolve(1);
                    },
                    error: function (xhr, textStatus, errorThrown) {
                        // Error callback
                        dotnetRef.invokeMethodAsync("UploadFailedAsync", xhr.status, xhr.responseText, false);
                        resolve(-1);
                    },
                    xhr: function () {
                        return xhr;
                    },
                };
                // Add the token if provided
                if (token) {
                    xhrOptions.headers = { 'authorization': 'Bearer ' + token };
                }
                // Send the POST request
                jQuery.ajax(xhrOptions);
            });
        },

        /**
         * Download a file
         * 
         * @param {object} dotnetRef Dotnet ref
         * @param {string} fileId File identifier
         * @param {any} fileName File name
         * @param {any} downloadUrl Download file url
         * @param {any} token Optional token to send with the request
         */
        downloadWithJs: function (dotnetRef, element, fileId, fileName, downloadUrl, token) {
            return new Promise(function (resolve, reject) {
                // Initialise a new XMLHttpRequest
                var xhr = new window.XMLHttpRequest();
                xhr.open("GET", downloadUrl, true);
                xhr.responseType = 'blob';
                // Follow upload progression
                xhr.addEventListener("progress", function (evt) {
                    if (!xhr.lastUpdate) xhr.lastUpdate = new Date();
                    var ellapsed = new Date() - xhr.lastUpdate;
                    if (ellapsed > 1000) {
                        if (xhr.lastLoaded) {
                            var diff = parseInt(evt.loaded) - xhr.lastLoaded;
                            xhr.bytesSecond = (1000 * diff / ellapsed);
                        }
                        xhr.lastLoaded = parseInt(evt.loaded);
                        xhr.lastUpdate = new Date();
                    }
                    if (evt.lengthComputable) {
                        dotnetRef.invokeMethodAsync("DownloadProgressionAsync", fileId, parseInt(evt.loaded * 100 / evt.total));
                    }
                }, false);
                // Add the token if provided
                if (token) {
                    xhr.setRequestHeader('authorization', 'Bearer ' + token);
                }
                // Download complete
                xhr.onload = function (event) {
                    var blob = xhr.response;
                    if (xhr.status == 200) {
                        // Send the received file to the browser
                        Lagoon.JsFileUtils.OpenBlob(blob, fileName);
                        // Process successfully complete
                        resolve(1);
                    } else {
                        var reader = new FileReader();
                        reader.onload = function (evt) {
                            console.log(evt.target.result);
                            // Error callback
                            dotnetRef.invokeMethodAsync("UploadFailedAsync", xhr.status, evt.target.result, true);
                            resolve(-1);
                        };
                        reader.readAsText(blob);
                    }
                };
                // Errors handling
                xhr.onerror = function (e) {
                    resolve(-1);
                };
                xhr.onabort = function (e) {
                    resolve(-1);
                }
                // Keep a reference to the XHR to be able to abort the download 
                if (!element._downloadXHR) element._downloadXHR = {};
                element._downloadXHR[fileId] = xhr;
                // Send the request
                xhr.send();
            });
        },

        /**
         * Cancel an upload in progress
         * 
         * @param {HTMLInputElement} element Input element dom reference
         * @param {boolean} upload To cancel an upload in progress
         * @param {boolean} element To cancel all download in progress
         */
        cancelUpload: function (element, upload, download) {
            if (upload && element.XHR) {
                // Stop upload
                element.XHR.abort();
                element.XHR = null;
            }
            // Stop all potential download
            if (download && element._downloadXHR) {
                for (var index in element._downloadXHR) {
                    element._downloadXHR[index].abort();
                }
                // Reset download tracker
                element._downloadXHR = {};
            }
        },

        /**
         * Cancel a download
         * 
         * @param {HTMLInputElement} element input ref
         * @param {any} fileId file identifier
         */
        cancelDownload: function (element, fileId) {
            if (element._downloadXHR && element._downloadXHR[fileId]) {
                element._downloadXHR[fileId].abort();
                delete element._downloadXHR[fileId];
            }
        },

        /**
         * Remove a file from selection ()
         * 
         * @param {HTMLInputElement} element input ref
         * @param {int} fileId File index
         */
        removeFile: function (element, fileId) {
            var originalFile = getFileById(element, fileId);
            // Free the blob url if set
            if (originalFile.blobUrl) {
                URL.revokeObjectURL(originalFile.blobUrl);
            }
            delete element._blazorFilesById[fileId];
        },

        toImageFile(elem, fileId, format, maxWidth, maxHeight) {
            var originalFile = getFileById(elem, fileId);

            return new Promise(function (resolve) {
                var originalFileImage = new Image();
                originalFileImage.onload = function () { resolve(originalFileImage); };
                originalFileImage.src = URL.createObjectURL(originalFile.blob);
            }).then(function (loadedImage) {
                return new Promise(function (resolve) {
                    var desiredWidthRatio = Math.min(1, maxWidth / loadedImage.width);
                    var desiredHeightRatio = Math.min(1, maxHeight / loadedImage.height);
                    var chosenSizeRatio = Math.min(desiredWidthRatio, desiredHeightRatio);

                    var canvas = document.createElement('canvas');
                    canvas.width = Math.round(loadedImage.width * chosenSizeRatio);
                    canvas.height = Math.round(loadedImage.height * chosenSizeRatio);
                    canvas.getContext('2d').drawImage(loadedImage, 0, 0, canvas.width, canvas.height);
                    canvas.toBlob(resolve, format);
                });
            }).then(function (resizedImageBlob) {
                var result = {
                    id: ++elem._blazorInputFileNextFileId,
                    lastModified: originalFile.lastModified,
                    name: originalFile.name, // Note: we're not changing the file extension
                    size: resizedImageBlob.size,
                    type: format,
                    relativePath: originalFile.relativePath
                };

                elem._blazorFilesById[result.id] = result;

                // Attach the blob data itself as a non-enumerable property so it doesn't appear in the JSON
                Object.defineProperty(result, 'blob', { value: resizedImageBlob });

                return result;
            });
        },

        /**
         * Return a local blob url (blob:http://...) for the specified file.
         * Should be used to display image in UI without having to retrieve all bytes data from c# and convert to base64
         * 
         * @param {HTMLInputElement} elem Input wich contain select file by the user
         * @param {int} fileId The file identifier
         */
        toBlobUrl: function (elem, fileId) {
            var originalFile = getFileById(elem, fileId);

            if (!originalFile.blobUrl) {
                originalFile.blobUrl = URL.createObjectURL(originalFile.blob);
            }

            return originalFile.blobUrl;
        },

        readFileData: function readFileData(elem, fileId, startOffset, count) {
            var readPromise = getArrayBufferFromFileAsync(elem, fileId);

            return readPromise.then(function (arrayBuffer) {
                var uint8Array = new Uint8Array(arrayBuffer, startOffset, count);
                var base64 = uint8ToBase64(uint8Array);
                return base64;
            });
        },

        ensureArrayBufferReadyForSharedMemoryInterop: function ensureArrayBufferReadyForSharedMemoryInterop(elem, fileId) {

            return getArrayBufferFromFileAsync(elem, fileId).then(function (arrayBuffer) {
                getFileById(elem, fileId).arrayBuffer = arrayBuffer;
            });
        },

        readFileDataSharedMemory: function readFileDataSharedMemory(readRequest) {
            // This uses various unsupported internal APIs. Beware that if you also use them,
            // your code could become broken by any update.
            var inputFileElementReferenceId = Blazor.platform.readStringField(readRequest, 0);
            var inputFileElement = document.querySelector('[_bl_' + inputFileElementReferenceId + ']');
            var fileId = Blazor.platform.readInt32Field(readRequest, 4);
            var sourceOffset = Blazor.platform.readUint64Field(readRequest, 8);
            var destination = Blazor.platform.readInt32Field(readRequest, 16);
            var destinationOffset = Blazor.platform.readInt32Field(readRequest, 20);
            var maxBytes = Blazor.platform.readInt32Field(readRequest, 24);

            var sourceArrayBuffer = getFileById(inputFileElement, fileId).arrayBuffer;
            var bytesToRead = Math.min(maxBytes, sourceArrayBuffer.byteLength - sourceOffset);
            var sourceUint8Array = new Uint8Array(sourceArrayBuffer, sourceOffset, bytesToRead);

            var destinationUint8Array = Blazor.platform.toUint8Array(destination);
            destinationUint8Array.set(sourceUint8Array, destinationOffset);

            return bytesToRead;
        }
    };

    function getFileById(elem, fileId) {
        var file = elem._blazorFilesById[fileId];
        if (!file) {
            throw new Error('There is no file with ID ' + fileId + '. The file list may have changed');
        }

        return file;
    }

    function getFileByName(elem, filename) {
        for (var index in elem._blazorFilesById) {
            if (elem._blazorFilesById[index].name == filename) {
                return elem._blazorFilesById[index];
            }
        }
        throw new Error('There is no file with name ' + filename + '. The file list may have changed');
    }

    function getArrayBufferFromFileAsync(elem, fileId) {
        var file = getFileById(elem, fileId);

        // On the first read, convert the FileReader into a Promise<ArrayBuffer>
        if (!file.readPromise) {
            file.readPromise = new Promise(function (resolve, reject) {
                var reader = new FileReader();
                reader.onload = function () { resolve(reader.result); };
                reader.onerror = function (err) { reject(err); };
                reader.readAsArrayBuffer(file.blob);
            });
        }

        return file.readPromise;
    }

    var uint8ToBase64 = (function () {
        // Code from https://github.com/beatgammit/base64-js/
        // License: MIT
        var lookup = [];

        var code = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
        for (var i = 0, len = code.length; i < len; ++i) {
            lookup[i] = code[i];
        }

        function tripletToBase64(num) {
            return lookup[num >> 18 & 0x3F] +
                lookup[num >> 12 & 0x3F] +
                lookup[num >> 6 & 0x3F] +
                lookup[num & 0x3F];
        }

        function encodeChunk(uint8, start, end) {
            var tmp;
            var output = [];
            for (var i = start; i < end; i += 3) {
                tmp =
                    ((uint8[i] << 16) & 0xFF0000) +
                    ((uint8[i + 1] << 8) & 0xFF00) +
                    (uint8[i + 2] & 0xFF);
                output.push(tripletToBase64(tmp));
            }
            return output.join('');
        }

        return function fromByteArray(uint8) {
            var tmp;
            var len = uint8.length;
            var extraBytes = len % 3; // if we have 1 byte left, pad 2 bytes
            var parts = [];
            var maxChunkLength = 16383; // must be multiple of 3

            // go through the array every three bytes, we'll deal with trailing stuff later
            for (var i = 0, len2 = len - extraBytes; i < len2; i += maxChunkLength) {
                parts.push(encodeChunk(
                    uint8, i, (i + maxChunkLength) > len2 ? len2 : (i + maxChunkLength)
                ));
            }

            // pad the end with zeros, but make sure to not forget the extra bytes
            if (extraBytes === 1) {
                tmp = uint8[len - 1];
                parts.push(
                    lookup[tmp >> 2] +
                    lookup[(tmp << 4) & 0x3F] +
                    '=='
                );
            } else if (extraBytes === 2) {
                tmp = (uint8[len - 2] << 8) + uint8[len - 1];
                parts.push(
                    lookup[tmp >> 10] +
                    lookup[(tmp >> 4) & 0x3F] +
                    lookup[(tmp << 2) & 0x3F] +
                    '='
                );
            }

            return parts.join('');
        };
    })();
})();
