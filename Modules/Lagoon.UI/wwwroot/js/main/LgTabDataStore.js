Lagoon.LgTabDataStore = (function () {

    // Table which contains all user tabs
    const tabDataStoreName = 'TabData';

    // Open IndexedDB and ensure tables creation
    function OpenDb(localDatabaseName) {
        var request = window.indexedDB.open(localDatabaseName, 1);
        request.onupgradeneeded = ev => {
            db = request.result;
            db.createObjectStore(tabDataStoreName);
        }
        request.onerror = function () {
            console.error("LgTabDataStore - Failed to open the database: " + localDatabaseName);
        }
        return request;
    }

    return {
        /**
         * Retrieve a list of tab 
         *
         * @returns list of tab
         */
        getTabs: function (localDatabaseName) {
            return new Promise((resolve, reject) => {
                var request = OpenDb(localDatabaseName);
                request.onsuccess = function () {
                    var db = request.result;
                    var trans = db.transaction(tabDataStoreName, 'readonly');
                    // Get the tab object store
                    var store = trans.objectStore(tabDataStoreName);
                    var tabs = store.getAll();
                    tabs.onsuccess = (r) => {
                        if (r.target.result === undefined) {
                            resolve(null);
                        } else {
                            var cursor = tabs.result;
                            if (cursor) {
                                resolve(cursor[0]);
                            } else {
                                resolve(null);
                            }
                        }
                    }
                    tabs.onerror = (e) => {
                        reject(e);
                    }
                }
            });
        },
        /**
        * Sync indexeddb table of tabs with database (server) 
        *
        * @param {List*} tabs List of tabs
        */
        saveTabs: function (localDatabaseName, tabs) {
            var request = OpenDb(localDatabaseName);
            request.onsuccess = function () {
                var db = request.result;
                // Create a new transaction
                var trans = db.transaction(tabDataStoreName, 'readwrite');
                // Get the tab object store
                var store = trans.objectStore(tabDataStoreName);

                // Add / Update item
                query = store.put(tabs, "1");

                // handle the error case
                query.onerror = function (event) {
                    console.log(event.target.errorCode);
                };

            }
        }
    }
})();