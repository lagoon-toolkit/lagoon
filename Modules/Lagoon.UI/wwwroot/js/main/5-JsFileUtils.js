// JsObjectManager namespace
Lagoon.JsFileUtils = (function () {

    /**
     * Open or download (if target is false), a file from an url.
     * @param {string} url File url endpoint.
     * @param {string} filename Filename url endpoint.
     * @param {string} token The current user token.
     * @param {string} target The name of the browser's window.
     */
    var openAnonymousUri = function (url, filename, target) {
        // Check if the file must be open in a window of the browser
//x        console.log("openAnonymousUri", target);
        if (target && (target !== "_download")) {
            // Ask to the navigator to open file
            window.open(url, target);
        } else {
            // Create & trigger click on 'a' element to download file
            var link = document.createElement('a');
            link.download = filename ? filename : "";
            link.href = url;
            link.target = "_blank";
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
    }

    /**
     * Open or download (if target is false), a file from the server by adding authentication bearer as cookie.
     * @param {string} url File url endpoint.
     * @param {string} filename Filename url endpoint.
     * @param {string} token The current user token.
     * @param {string} target The name of the browser's window.
     */
    var openAuthenticatedUri = function (url, filename, token, target) {

        var cookieName = "lgn-" + (new Date()).getTime().toString(36);
        // Add token to a temp cookie
        if (token) {
            var tknCookie = cookieName + "T=" + encodeURIComponent(token) + "; path=" + window.baseHref + url.split("?")[0] + "; SameSite=Strict";
            document.cookie = tknCookie;
        }
        // Add the cookie name to the url
        url += (url.indexOf("?", 0) < 0 ? "?" : "&") + "lgndld=" + cookieName;
//x        console.log("openAuthenticatedUri", target);
        if (target && (target !== "_download")) {
            // Ask to the navigator to open file
            window.open(url, target);
        }
        else {
            // Add the cookie to wait the download starts
            var waiter = cookieName + "=wait" + "; path=/; SameSite=Lax";
            document.cookie = waiter;
            // Run the download
            var msg = toastr.info('<div class="toastDownloads"><svg class="bi i-a-Loading"><use xlink:href="#i-a-Loading"></use></svg><span>'
                + Lagoon.JsDicoManager.GetDico('PreparingDownload') + '</span></div> ',
                false, { closeButton: false, extendedTimeOut: 0, tapToDismiss: false, timeOut: 0 })[0];
            var html = '<body onload="document.getElementById(\'link\').click()"><a id="link" href="' + url + '" download';
            if (filename) {
                html += ' = "' + filename + '"';
            }
            html += '>Download</a></body>';
            var iframe = document.createElement('iframe');
            iframe.style.display = 'none';
            iframe.toastr = msg;
            iframe.onload = function () {
                var isError = true;
                try {
                    isError = iframe.contentDocument.location.host !== '';
                } catch { }
                // The download failed, the frame has been loaded
                if (isError) {
                    document.cookie = waiter + ";expires=Thu, 13 Apr 1979 12:00:00 GMT";
                    toastr.error(Lagoon.JsDicoManager.GetDico('PreparingDownloadFailed'), null, { timeOut: 0 });
                    iframe.toastr.parentElement.removeChild(iframe.toastr);
                    iframe.parentElement.removeChild(iframe);
                }
            };
            document.body.appendChild(iframe);
            iframe.srcdoc = html;
            // Wait until the cookie is removed when the download starts
            (function waitStart() {
                if (document.cookie.indexOf(cookieName + '=') < 0) {
                    // Download started, we close the iframe
                    iframe.toastr.parentElement.removeChild(iframe.toastr);
                    iframe.parentElement.removeChild(iframe);
                } else {
                    setTimeout(function () { waitStart() }, 1000);
                }
            }());
        }
        // Drop the bearer token after download
        if (token) {
            setTimeout(function () { document.cookie = tknCookie + ";expires=Thu, 13 Apr 1979 12:00:00 GMT"; }, 1000);
        }
    };

    var _tempChunks = {};

    return {

        /**
         * Open or download a file from an url.
         * @param {string} url File url endpoint.
         * @param {string} filename Filename url endpoint.
         * @param {string} target The name of the browser's window (false for download as attachment).
         * @param {string} token The current user token.
         */
        OpenURL: function (url, filename, target, token) {
            // Check if the query target a resource on the server application
            var fullURL = (new URL(url, document.baseURI)).href;
            if (token && fullURL.startsWith(document.baseURI)) {
                // Open the URL after adding the bearer token as cookie
                openAuthenticatedUri(url, filename, token, target);
            } else {
                openAnonymousUri(url, filename, target);
            }
        },

        /**
         * Open or download a file from an url.
         * @param {object} obj A File, Blob, or MediaSource object. 
         * @param {string} filename The name of the file (optional for File objects).
         * @param {string} target The name of the window in which the file should be opened.
         */
        OpenBlob: function (obj, filename, target) {
            var bUrl = window.URL.createObjectURL(obj);
            // "A File object is a Blob object with a name attribute..." https://www.w3.org/TR/FileAPI/#file-section
            if (!filename && obj.name) {
                // We get the file name from the file object
                filename = obj.name;
            }
            openAnonymousUri(bUrl, filename, target);
            window.URL.revokeObjectURL(bUrl);
        },

        /**
         * Open or download a C# byte array as file.
         * @param {string} hArgs The file name and the target separated by '|'. (InvokeUnmarshalled supports 3 parameters max)
         * @param {byte[]} hContents The file contents.
         * @param {byte[]} hBOM The BOM for the file.
         */
        OpenBlobUnmarshalled: function (hArgs, hContents, hBOM) {
            // Convert the parameters to actual JS types
            const args = BINDING.conv_string(hArgs).split('|');
            const target = args.length > 1 ? args[1] : false;
            const contentType = args.length > 2 ? args[2] : "";
            // Create the file object
            var fileBits = [];
            if (hBOM) fileBits.push(Blazor.platform.toUint8Array(hBOM));
            fileBits.push(Blazor.platform.toUint8Array(hContents));
            const file = new File(fileBits, args[0], { type: contentType });
            this.OpenBlob(file, false, target);
        },

        /**
         * Receive a file chunk by chunk.
         * Should be used to write a (File)Stream directly to JS without having to pass an entire file
         * 
         * @param {any} id An unique identifier
         * @param {any} bytes An bunch of bytes
         * @param {any} offsetSize Start offset
         */
        SetBytesChunk: function (id, bytes, offsetSize) {
            // Convert the parameters to actual JS types
            const idStr = BINDING.conv_string(id);
            const offsetSizeStr = BINDING.conv_string(offsetSize);
            const bytesArray = Blazor.platform.toUint8Array(bytes);
            const offset = parseInt(offsetSizeStr.split(";")[0]);
            const size = parseInt(offsetSizeStr.split(";")[1]);
            // Check if this identifier is know
            if (_tempChunks[idStr] === undefined) {
                _tempChunks[idStr] = {
                    bytes: new Uint8Array(size),
                }
            }
            // Copy bytes to buffer
            var index = 0;
            for (var i = 0; i < bytesArray.length; i++) {
                _tempChunks[idStr].bytes[index + offset] = bytesArray[i];
                index++;
            }
        },

        /**
         * Complete 
         * 
         * @param {any} id
         * @param {any} filename
         * @param {any} contentType
         */
        CompleteBytesChunk: function (id, filename, contentType) {
            const file = new File([_tempChunks[id].bytes], filename, { type: contentType });
            // Download the file
            this.OpenBlob(file, filename);
            // Free memory
            delete _tempChunks[id];
        }

    }

})();