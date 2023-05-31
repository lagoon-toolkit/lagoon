window.Lagoon = window.Lagoon || {};

Lagoon.LgNotificationDataStore = (function () {

    // Table which contains all user notifications
    const notifDataStoreName = 'NotificationData';

    // Table which contains pending actions (offline mode)
    const notifPendingActionDataStoreName = 'NotificationPendingActionData';

    // Open IndexedDB and ensure tables creation
    function OpenDb(localDatabaseName) {
        var request = window.indexedDB.open(localDatabaseName, 1);
        request.onupgradeneeded = ev => {
            db = request.result;
            db.createObjectStore(notifDataStoreName, { keyPath: "notificationUserId" });
            db.createObjectStore(notifPendingActionDataStoreName, { keyPath: "notificationUserId" });
        }
        request.onerror = function () {
            console.error("LgNotificationDataStore - Failed to open the database: " + localDatabaseName);
        }
        return request;
    }

    return {

        /**
         * Retrieve a list of notification 
         *
         * @returns list of notifications
         */
        getNotifications: function (localDatabaseName) {
            return new Promise((resolve, reject) => {
                var request = OpenDb(localDatabaseName);
                request.onsuccess = function () {
                    var db = request.result;
                    var trans = db.transaction(notifDataStoreName, 'readonly');
                    // Get the notifications object store
                    var store = trans.objectStore(notifDataStoreName);
                    var notifications = store.getAll();
                    notifications.onsuccess = (r) => {
                        if (r.target.result === undefined) {
                            reject(`${store} not found`);
                        } else {
                            resolve(notifications.result);
                        }
                    }
                    notifications.onerror = (e) => {
                        reject(e);
                    }
                }
            });
        },

        /**
         * Delete one notification
         *
         * @param {Guid*} notificationUserId NotificationUserId
         */
        deleteNotification: function (localDatabaseName, notificationUserId) {
            return new Promise((resolve, reject) => {
                var request = OpenDb(localDatabaseName);
                request.onsuccess = function () {
                    var db = request.result;
                    var trans = db.transaction(notifDataStoreName, 'readwrite');
                    // Get the notifications object store
                    var store = trans.objectStore(notifDataStoreName);
                    // Delete object from store
                    var objectStoreRequest = store.delete(String(notificationUserId));

                    objectStoreRequest.onsuccess = function (event) {
                        resolve(objectStoreRequest.result);
                    };
                    objectStoreRequest.onerror = (e) => {
                        reject(e);
                    }
                }
            });
        },

        /**
         * Delete multiple notifications
         *
         * @param {Guid*} notificationUserId List of NotificationUserId
         */
        deleteNotifications: function (localDatabaseName, notificationUserIds) {
            return new Promise((resolve, reject) => {
                var request = OpenDb(localDatabaseName);
                request.onsuccess = function () {
                    var db = request.result;
                    var trans = db.transaction(notifDataStoreName, 'readwrite');
                    // Get the notifications object store
                    var store = trans.objectStore(notifDataStoreName);
                    notificationUserIds.forEach((notificationUSerId, index) => {
                        // Delete item from keypath
                        query = store.delete(String(notificationUSerId));

                        // handle the error case
                        query.onerror = function (event) {
                            console.log(event.target.errorCode);
                        };
                    });
                    resolve(store.result);
                }
            });
        },

        /**
        * Sync indexeddb table of notification with database (server) 
        *
        * @param {List*} notificatons List of user notification (view model)
        */
        syncNotification: function (localDatabaseName, notificatons) {
            var request = OpenDb(localDatabaseName);
            request.onsuccess = function () {
                var db = request.result;
                // Create a new transaction
                var trans = db.transaction(notifDataStoreName, 'readwrite');

                //Get the notification object store
                var store = trans.objectStore(notifDataStoreName);
                // Loop on notifications to save into indexeddb
                notificatons.forEach((item, index) => {
                    // Add / Update item
                    query = store.put(item, item.NotificationUserId);

                    // handle the error case
                    query.onerror = function (event) {
                        console.log(event.target.errorCode);
                    };
                });

            }
        },

        /**
         * Retrieve a list of all pending actions collected during offline
         *
         * @returns list of pending actions
         */
        getPendingActions: function (localDatabaseName) {
            return new Promise((resolve, reject) => {
                var request = OpenDb(localDatabaseName);
                request.onsuccess = function () {
                    var db = request.result;
                    var trans = db.transaction(notifPendingActionDataStoreName, 'readonly');
                    var store = trans.objectStore(notifPendingActionDataStoreName);
                    var pendingActions = store.getAll();
                    pendingActions.onsuccess = (r) => {
                        if (r.target.result === undefined) {
                            reject(`${store} not found`);
                        } else {
                            resolve(pendingActions.result);
                        }
                    }
                    pendingActions.onerror = (e) => {
                        reject(e);
                    }
                }
            });
        },

        /**
         * Add / Update a pending action for one NotificationUserId
         *
         * @param {Object*} pending action object
         */
        pendingActionsUpdateNotification: function (localDatabaseName, pendingAction) {
            return new Promise((resolve, reject) => {
                var request = OpenDb(localDatabaseName);
                request.onsuccess = function () {
                    var db = request.result;
                    var trans = db.transaction(notifPendingActionDataStoreName, 'readwrite');
                    // Get the notifications object store
                    var store = trans.objectStore(notifPendingActionDataStoreName);
                    // Check if notificationUserId exist into pending actions
                    var pendingNotification = store.put(pendingAction);
                    pendingNotification.onsuccess = (r) => {
                        // Set Notification into indexeddb table
                        var transNotif = db.transaction(notifDataStoreName, 'readwrite');
                        // Get the notifications object store
                        var storeNotif = transNotif.objectStore(notifDataStoreName);
                        var notification = storeNotif.get(pendingAction.notificationUserId);

                        notification.onsuccess = (r) => {
                            notification.result.isRead = pendingAction.isRead;
                            notification.result.updateDate = pendingAction.updateDate;
                            // Update notification into indexeddb table
                            storeNotif.put(notification.result);
                            resolve(storeNotif.result);
                        }

                        notification.onerror = (e) => {
                            reject(e);
                        }
                    }
                    pendingNotification.onerror = (e) => {
                        reject(e);
                    }
                }
            });
        },

        /**
         * Add / Update multiple pending actions
         *
         * @param {Object*} list of pending action object
         */
        pendingActionsUpdateNotifications: function (localDatabaseName, pendingActions) {
            return new Promise((resolve, reject) => {
                var request = OpenDb(localDatabaseName);
                request.onsuccess = function () {
                    var db = request.result;
                    var trans = db.transaction(notifPendingActionDataStoreName, 'readwrite');
                    // Get the notifications object store
                    var store = trans.objectStore(notifPendingActionDataStoreName);

                    pendingActions.forEach((pendingAction, index) => {
                        // Check if notificationUserId exist into pending actions
                        var pendingNotification = store.put(pendingAction);
                        pendingNotification.onsuccess = (r) => {
                            // Set Notification into indexeddb table
                            var transNotif = db.transaction(notifDataStoreName, 'readwrite');
                            // Get the notifications object store
                            var storeNotif = transNotif.objectStore(notifDataStoreName);
                            var notification = storeNotif.get(pendingAction.notificationUserId);

                            notification.onsuccess = (r) => {
                                notification.result.isRead = pendingAction.isRead;
                                notification.result.updateDate = pendingAction.updateDate;
                                // Update notification into indexeddb table
                                storeNotif.put(notification.result);
                            }

                            notification.onerror = (e) => {
                                reject(e);
                            }
                        }
                        pendingNotification.onerror = (e) => {
                            reject(e);
                        }
                    });
                }
            });
        },

        /**
         * Delete one notification => delete pending action
         *
         * @param {Object*} pendingAction pending action to create
         */
        pendingActionsDeleteNotification: function (localDatabaseName, pendingAction) {
            return new Promise((resolve, reject) => {
                var request = OpenDb(localDatabaseName);
                request.onsuccess = function () {
                    var db = request.result;
                    var trans = db.transaction(notifPendingActionDataStoreName, 'readwrite');
                    // Get the notifications object store
                    var store = trans.objectStore(notifPendingActionDataStoreName);
                    // Check if notificationUserId exist into pending actions
                    var pendingNotification = store.put(pendingAction);
                    pendingNotification.onsuccess = (r) => {
                        // Set Notification into indexeddb table
                        var transDelNotif = db.transaction(notifDataStoreName, 'readwrite');
                        // Get the notifications object store
                        var storeDelNotif = transDelNotif.objectStore(notifDataStoreName);
                        // Remove object
                        var objectStoreDelete = storeDelNotif.delete(String(pendingAction.notificationUserId));

                        objectStoreDelete.onsuccess = (e) => {
                            resolve(e);
                        };
                        objectStoreDelete.onerror = (e) => {
                            reject(e);
                        }
                    }
                    pendingNotification.onerror = (e) => {
                        reject(e);
                    }
                }
            });
        },

        /**
         * Delete multiple notifications => delete pending action
         *
         * @param {Object*} pendingActions pending actions to create
         */
        pendingActionsDeleteNotifications: function (localDatabaseName, pendingActions) {
            return new Promise((resolve, reject) => {
                var request = OpenDb(localDatabaseName);
                request.onsuccess = function () {
                    var db = request.result;
                    var trans = db.transaction(notifPendingActionDataStoreName, 'readwrite');
                    // Get the notifications object store
                    var store = trans.objectStore(notifPendingActionDataStoreName);

                    pendingActions.forEach((pendingAction, index) => {
                        // Check if notificationUserId exist into pending actions
                        var pendingNotification = store.put(pendingAction);
                        pendingNotification.onsuccess = (r) => {
                            // Set Notification into indexeddb table
                            var transDelNotif = db.transaction(notifDataStoreName, 'readwrite');
                            // Get the notifications object store
                            var storeDelNotif = transDelNotif.objectStore(notifDataStoreName);
                            // Remove object
                            var objectStoreDelete = storeDelNotif.delete(String(pendingAction.notificationUserId));

                            objectStoreDelete.onerror = (e) => {
                                reject(e);
                            }
                        }
                        pendingNotification.onerror = (e) => {
                            reject(e);
                        }
                    });
                    resolve(store.result);
                }
            });
        },

        /**
         * Delete all pending actions
         */
        pendingActionsDeleteAll: function (localDatabaseName) {
            return new Promise((resolve, reject) => {
                var request = OpenDb(localDatabaseName);
                request.onsuccess = function () {
                    var db = request.result;
                    var trans = db.transaction(notifPendingActionDataStoreName, 'readwrite');
                    // Get the notifications object store
                    var store = trans.objectStore(notifPendingActionDataStoreName);
                    // Check if notificationUserId exist into pending actions
                    var pendingNotification = store.clear();
                    pendingNotification.onsuccess = (e) => {
                        resolve(e);
                    }
                    pendingNotification.onerror = (e) => {
                        reject(e);
                    }
                }
            });
        },

        /**
         * Fully delete the database from IndexedDb 
         * @param {any} localDatabaseName Database name to remove
         */
        deleteDatabase: function (localDatabaseName) {
            return new Promise((resolve, reject) => {
                var req = indexedDB.deleteDatabase(localDatabaseName);
                req.onsuccess = function () {
                    resolve();
                };
                req.onerror = function () {
                    console.error("LgNotificationDataStore - Fail to delete the database '" + localDatabaseName + "' (error)");
                    reject();
                };
                req.onblocked = function () {
                    console.error("LgNotificationDataStore - Fail to delete the database '" + localDatabaseName + "' (blocked)");
                    reject();
                };
            });
        }

    }
})();