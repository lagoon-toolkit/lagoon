//rq: when imported by the service-worker, the 'window.Lagoon' namespace is not available
var Lagoon = Lagoon || {};

/**
 * Scripts shared by Lagoon.ServiceWorker and the service-worker-handler
 **/
Lagoon.SharedServiceWorker = (function () {

    return {

        /**
         * Return the IndexedDb used to store active and renewed subscription
         * @param {String} appName Application name (used as a database name prefix)
         **/
        GetDb: function (appName) {
            return new Promise((resolve, reject) => {
                var request = indexedDB.open(appName + "/ServiceWorkerStore", 1);
                request.onupgradeneeded = ev => {
                    db = request.result;
                    // table added in db v2
                    db.createObjectStore('RegisteredSubcription', { autoIncrement: false, keyPath: "Type" });
                }
                request.onsuccess = function () {
                    resolve(request.result);
                }
                request.onerror = function (e) {
                    console.error("Failed to open the SharedServiceWorker database");
                    reject(e)
                }
                request.onblocked = (e) => {
                    console.error('Failed to open the SharedServiceWorker database (blocked):', e);
                    reject(e);
                };
            });
        }
    }

})()