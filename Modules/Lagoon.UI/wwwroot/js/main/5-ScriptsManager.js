window.Lagoon = window.Lagoon || {};

// ScriptsManager namespace
window.Lagoon.ScriptsManager = (function () {

    // Store list of what scripts we've completly loaded
    var loadedScripts = [];

    // Store list of script that are being loaded
    var loadingScripts = [];

    // Return the public object
    return {

        /**
         * Add script tag (if not already exist) into the DOM
         * @param {*} scriptPath 
         * @returns A promise that completes when the script loads
         */
        Add: function (scriptPath, addAppVersion) {
            if (addAppVersion === true) {
                var version = Lagoon.Version || (new Date()).getTime();
                scriptPath += scriptPath.includes('?') ? "&" : "?";
                scriptPath += "v=" + version;
            }
            // if already loaded we can ignore
            if (loadedScripts[scriptPath]) {
                // return 'empty' promise
                return new Promise(function (resolve, reject) {
                    resolve(scriptPath);
                });
            }
            // if script is knowed but still loading, attach to onload event
            else if (loadingScripts[scriptPath] != null) {
                return new Promise(function (resolve, reject) {
                    // Track the original onLoad event
                    var parentOnLoadEvent = loadingScripts[scriptPath].onload;
                    // Replace it
                    loadingScripts[scriptPath].onload = function () {
                        // Send complete to asker & parent(s)
                        resolve(scriptPath);
                        parentOnLoadEvent();
                    }
                });
            }
            // Try to load the script
            return new Promise(function (resolve, reject) {
                // Create JS library script element
                var script = document.createElement("script");
                script.src = scriptPath;
                script.type = "text/javascript";
                // Mark the script as 'loading in progress'
                loadingScripts[scriptPath] = script;

                // When the script in the DOM and ready to be used
                script.onload = function () {
                    // Flag script as loaded
                    loadedScripts[scriptPath] = true;
                    // Remove script from loading
                    loadingScripts[scriptPath] = null;
                    // Send complete to the parent
                    resolve(scriptPath);
                };
                // if it fails, return reject
                script.onerror = function () {
                    reject(scriptPath);
                };
                // scripts will load at end of body
                document.body.appendChild(script);
            });
        }

    }

})();