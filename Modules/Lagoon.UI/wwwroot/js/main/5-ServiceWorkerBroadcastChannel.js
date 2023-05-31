Lagoon.ServiceWorker = (function () {

    // id of sw banner div
    const bannerId = 'appNewVersion';

    // Root path from origin of the current application
    const baseHref = document.getElementsByTagName('base')[0].getAttribute('href');

    // Initialisation state
    var _isInit = false;

    // c# ref on LgApplication
    var _appRef;

    var _applicationRootName;

    var _register = function () {
        // Register the serviceworker for the app scope
        return navigator.serviceWorker.register('service-worker.js');
    }

    // Two-way communication initalisation
    var _init = function (appRef, applicationRootName) {
        if (!_isInit) {
            navigator.serviceWorker.addEventListener('message', event => {
                // We keep only message targetting this app service worker
                if (event.data && event.data.scope === baseHref) {
                    _receiveMessage(event.data.msg);
                }
            });
            _appRef = appRef;
            _applicationRootName = applicationRootName;
            _isInit = true;
        }
    }

    // Receive a message from the service worker
    var _receiveMessage = function (message) {
        if (message == 'new-version-found') {
            // Show the 'New app available button'
            _swNewVersion();

        } else if (message == 'new-version-active') {
            // Ensure banner is hidden when a sw become active
            _removeBanner();

        } else if (message.startsWith('new-version-inprogress-')) {
            // Show sw installtion progression
            var progress = message.split('-')[3];
            _swInstallVersion(progress);

        } else if (message == 'new-version-problem') {
            // Show the 'An error occured please reload'
            _swFailedVersion();

        } else if (message == 'reload-page') {
            // Reload application when user click on 'SkipWaiting' or when app failed to install
            window.location.reload();

        } else if (message.startsWith('navigate-to-tab§')) {
            _appRef.invokeMethod("NavForSw", message.split('§')[1]);
        }
    }

    // New version available banner
    var _swNewVersion = function() {
        _showSwBanner(
            Lagoon.JsDicoManager.GetDico('appNewVersion'),
            'new-application-version',
            () => {
                // Rq: After few second the waiting SW will be sleeped by the navigator
                // BroadcastChannel can't wake up a sleep SW, so we use the on-way channel to communicate with the sleeping SW
                _register().then(function (registration) {
                    if (registration && registration.waiting) {
                        registration.waiting.postMessage('skip-waiting');
                    }
                });
                // Remove the banner (on first install we don't have to skip-waiting)
                _removeBanner();
            },
            Lagoon.JsDicoManager.GetDico('appReload')
        );
    }

    // Installing new sw banner
    var _swInstallVersion = function (progression) {
        _showSwBanner(
            Lagoon.JsDicoManager.GetDico('appNewVersionInProgress') + '(' + progression + ' %)',
            'new-application-version',
            null,
            Lagoon.JsDicoManager.GetDico('appReload')
        );
    }

    // SW failed banner
    var _swFailedVersion = function () {
        _showSwBanner(
            Lagoon.JsDicoManager.GetDico('appNewVersionError'),
            'new-application-version new-application-version-error',
            () => { window.location.reload(); },
            Lagoon.JsDicoManager.GetDico('appReload')
        );
    }

    // SW unavailable
    var _swNotAvailable = function() {
        _showSwBanner(
            Lagoon.JsDicoManager.GetDico('appSwNotAvailable'),
            'new-application-version new-application-version-error',
            () => { _removeBanner(); },
            Lagoon.JsDicoManager.GetDico('appCloseBanner')
        );
    }

    // Create sw banner
    var _showSwBanner = function (text, cssClass, action, textAction) {
        // Banner container
        let newVersionToast = document.createElement('div');
        newVersionToast.id = bannerId;
        newVersionToast.className = cssClass;
        // Message
        let paragraph = document.createElement('span');
        let span = document.createElement('span');
        span.textContent = text + ' ';
        paragraph.appendChild(span);
        newVersionToast.appendChild(paragraph);
        // Optional action
        if (action) {
            let updateButton = document.createElement('a');
            updateButton.textContent = textAction;
            updateButton.addEventListener('click', action);
            paragraph.appendChild(updateButton);
        }
        // Add to the body
        _removeBanner();
        let body = document.getElementsByTagName('body')[0];
        body.appendChild(newVersionToast);
    }

    // Remove sw info banner
    var _removeBanner = function () {
        var prev = document.getElementById(bannerId);
        if (prev) prev.remove();
    }

    // Return true if there is an active service worker
    var _isSwActive = function () {
        return 'serviceWorker' in navigator
                && navigator.serviceWorker.controller != null
                && navigator.serviceWorker.controller.state == 'activated';
    }

    // Return the indexeddb shared by the service-worker and the application
    var _openDb = function () {
        return Lagoon.SharedServiceWorker.GetDb(_applicationRootName);
    }

    return {

        /**
         * Lagoon service-worker initialisation 
         */
        Init: function (appRef, applicationRootName) {
            if (navigator.serviceWorker)
            {
                _init(appRef, applicationRootName);
                // SW registration
                _register().then(function (registration) {
                    // If we have an active and a waiting service-worker we have to show the "Update new version button"
                    // This would happen if skipWaiting() isn't being called, and there are still old tabs open.
                    if (registration && registration.waiting) {
                        // Show the "New app available button"
                        _swNewVersion();
                    }
                }).catch(err => {
                    console.log('ServiceWorker -> Failed to install new version', err);
                });
            }
            else
            {
                // Show service-worker not available banner
                _swNotAvailable();
            }
        },

        /**
         * Remove all registered SW 
         **/
        Unregister: function () {
            if (navigator.serviceWorker)
            {
                try
                {
                    navigator.serviceWorker.getRegistrations().then(function (registrations) { for (let registration of registrations) { registration.unregister(); } });
                }
                catch (e)
                {
                    console.log('ServiceWorker -> Failed to unregister');
                }
            }
        },

        /**
         * Subscribe to the service-worker channel message 
         **/
        SubscribeUiUpdate: function (appRef, applicationRootName) {
            if (navigator.serviceWorker) {
                _init(appRef, applicationRootName);
            }
        },

        /**
         * Subscribe to PushNotification
         * 
         * @param serverKey {string} The public vapid key (V.A.P.ID Volontary Application IDentifier)
         **/
        SubscribePushNotification: function (serverKey) {
            return new Promise((resolve, reject) => {
                if (_isSwActive()) {
                    _register()
                        .then((reg) => {
                            reg.pushManager.getSubscription().then(function (sub) {
                                if (sub === null) {
                                    // No subscription registered, try to subscribe to PushEvent
                                    reg.pushManager.subscribe({
                                        userVisibleOnly: true,
                                        applicationServerKey: serverKey
                                    }).then(function (sub) {
                                        // subscription
                                        var subObj = sub.toJSON();
                                        console.log('SubscribePushNotification - success : ', subObj);

                                        _openDb().then(db => {
                                            var trans = db.transaction('RegisteredSubcription', 'readwrite');
                                            var store = trans.objectStore('RegisteredSubcription');
                                            store.put({
                                                Endpoint: subObj.endpoint,
                                                P256dh: subObj.keys.p256dh,
                                                Auth: subObj.keys.auth,
                                                Type: "Subscription"
                                            });
                                            resolve({
                                                Endpoint: subObj.endpoint,
                                                P256dh: subObj.keys.p256dh,
                                                Auth: subObj.keys.auth
                                            });
                                        }).catch(e => {
                                            reject(e);
                                        })
                                    }).catch(function (e) {
                                        console.error("SubscribePushNotification - Unable to subscribe to push notification: ", e);
                                        reject();
                                    });
                                } else {
                                    // The navigator has already an active WebPush subscription so just return the existing subsciption infos
                                    console.log('The navigator has already an active WebPush subscription, return the existing subscription', sub);
                                    var subObj = sub.toJSON();
                                    resolve({
                                        Endpoint: subObj.endpoint,
                                        P256dh: subObj.keys.p256dh,
                                        Auth: subObj.keys.auth
                                    });
                                }
                            });
                        });
                } else {
                    // sw not registered
                    reject('ServiceWorker not registered');
                }
            });
        },

        /**
         * Unsubscribe to WebPush notification
         * @returns The existing registration endpoint or empty if no subscription found
         **/
        UnsubscribePushNotification: function () {
            return new Promise((resolve, reject) => {
                if (_isSwActive()) {
                    _openDb().then(db => {
                        var trans = db.transaction('RegisteredSubcription', 'readwrite');
                        var store = trans.objectStore('RegisteredSubcription');
                        store.delete('Subscription');
                    });

                    _register()
                        .then((reg) => {
                            reg.pushManager.getSubscription().then(function (sub) {
                                if (sub != null) {
                                    var oldEndpoint = sub.toJSON().endpoint;
                                    sub.unsubscribe().then((successful) => {
                                        // Successfully unsubscribed
                                        resolve(oldEndpoint);
                                    }).catch((e) => {
                                        // Unsubscribing failed
                                        reject();
                                    })
                                }
                                else {
                                    // Assume success result if no existing registration
                                    resolve();
                                }
                            });
                        });
                } else {
                    // Assume success result if no sw registered (we can't have an active subscription without a registered sw)
                    resolve();
                }
            });
        },

        /**
         * Return true if there is an active WebPush notification subscription 
         **/
        HasSubscribedToWebPushNotification: function () {
            return new Promise((resolve, reject) => {
                if (_isSwActive()) {
                    _register()
                        .then((reg) => {
                            reg.pushManager.getSubscription().then(function (sub) {
                                resolve(sub != null);
                            });
                        });
                } else {
                    // Assume success result if no sw registered (we can't have an active subscription without a registered sw)
                    resolve(false);
                }
            });
        },

        /**
         * Check if push notification has been accepted 
         **/
        PushNotificationAllowed: function () {
            return Notification.permission === "granted";
        },

        /**
         * Check if there is an active service worker 
         **/
        IsServiceWorkerActive: function () {
            return _isSwActive();
        },

        GetDb: function () {
            return "ok";
        }

    }

})();