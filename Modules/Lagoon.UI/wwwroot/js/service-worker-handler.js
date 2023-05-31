// Retrieve the current application base HREF
var swHref = self.location.pathname;
var baseHref = swHref.substring(0, swHref.lastIndexOf('/')) + "/";
// Listen for SW Events
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));
self.addEventListener('push', event => onPushNotificationReceived(event));
self.addEventListener('notificationclick', event => onPushNotificationClicked(event));
self.addEventListener('pushsubscriptionchange', event => onPushSubscriptionChanged(event));
// Cache name & Assets type to cache
const cacheNamePrefix = self.applicationRootName.toLowerCase() + '-offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;


//#region Lagoon specific

// Active console trace
if (self.swdebug === undefined) self.swdebug = false;

// Send message to all windows that have registered the serviceworker
self.postMessage = function (msg) {
    //includeUncontrolled : The first time the application is launched, it's not controlled by the service worker
    self.clients.matchAll({ includeUncontrolled: true, type: 'window' }).then(clients => {
        clients.forEach(client => client.postMessage({ scope: baseHref, msg: msg }));
    });
}

// Send by Lagoon when user click in the 'Reload' button in the SW banner
self.onmessage = async function (e) {
    if (e) {
        if (e.data == 'skip-waiting') {
            swlog('SkipWaiting received by the SW. Version:' + self.assetsManifest.version);
            self.skipWaiting();
            setTimeout(function () { self.postMessage('reload-page'); }, 500);
        } else if (e.data == 'kill-cache') {
            await onSwCacheKill();
        }
    }
}

// Send message to lagoon => new version available to install
const notifyNewVersion = function () {
    swlog('All resources successfully downloaded');
    self.postMessage('new-version-found');
}

// Send message to lagoon => downloading new sw version
const notifyInstallingVersion = function (progression) {
    var message = 'new-version-inprogress-' + progression;
    swlog('Install in progress: ' + message);
    self.postMessage(message);
}

// Send message to lagoon => new sw version failed
const notifyInstallProblem = function () {
    swlog('A new version is available BUT FAILED TO INSTALL');
    self.postMessage('new-version-problem');
}

const notifyActive = function () {
    swlog('Activated');
    self.postMessage('new-version-active');
}

// Debug purpose
const swlog = function (message, arg) {
    if (self.swdebug) console.log('LagoonServiceWorkerHandler: ' + message, arg)
}

//#endregion

//#region SW Events (install, activate, fetch)

/**
 * New SW update available (detected by browser when service-worker.js content change)
 * Rq: Blazor publish process add a comment a the end of this file with SW 'version', calculated with the hash of all resources to force navigator update
 * This SW 'version' can also be found in the auto-generated 'service-worker-assets.js' file => cf. self.assetsManifest.version
 * @param {*} event 
 */
async function onInstall(event) {
    // Installation started
    notifyInstallingVersion(0);
    // Track install state
    var isInstallSuccess = true;

    // For all matching items from the assets manifest create a request
    const assetsRequests = self.assetsManifest.assets
        .map(asset => asset.hash ? new Request(asset.url, { integrity: asset.hash }) : new Request(asset.url));

    // Open the cache and add all required request
    var progress = 1;
    var total = assetsRequests.length;
    const cache = await caches.open(cacheName);
    for (const request of assetsRequests) {
        swlog('SW. Adding with default settings --> ' + request.url);
        notifyInstallingVersion(Math.round(100 * progress / total));
        progress++;

        await cache
            // Try to add request in cache with the default SW behavior :try to get the request from a potential previous cache if possible
            .add(request)
            .catch(async function (ex) {
                swlog('An error occured while trying to add resource to cache : ' + request.url + '(' + ex.message + '). Retrying with cache:"reload" and service-worker version ' + self.assetsManifest.version + ' integrity ' + request.hash);

                // If an error occured (previous cache corrupted / response cached by the navigator / response cached by a middleware-proxy between client-server )
                // Fetch the ressource with current asset version, cache reload and keep integrity checking
                var ticks = (new Date).getTime() * 10000 + 621355968000000000;
                await fetch(request.url + '?v=' + ticks, { cache: 'reload', integrity: request.integrity })
                    .then(async function (response) {
                        if (!response.ok) {
                            isInstallSuccess = false;
                            swlog('RETRY FAILED for request [' + response.url + ']', response);
                            throw 'SW. STOP INSTALL';
                        } else {
                            // Add the resources in the cache storage
                            swlog('RETRY SUCCESS for request [' + request.url + ']', response);
                            await cache
                                .put(request, response)
                                .catch(ex => {
                                    swlog('ERROR in CACHE.PUT !', ex);
                                    //isInstallSuccess = false;
                                    //throw 'SW. STOP INSTALL';
                                });
                        }
                    })
                    .catch(e => {
                        //
                        swlog('SW. An error occured when fetching resource --> ', e);
                       // isInstallSuccess = false;
                    });
            });

        // Don't try to fetch other resources if one is not available
        if (!isInstallSuccess) { break; }
    }

    if (!isInstallSuccess) {
        notifyInstallProblem();
        // Throw an exception, so the navigator will consider that installation failed (and will retry for the next sw registration)
        throw new Error('SW. STOP INSTALL');
    } else {
        notifyNewVersion();
    }
}

/**
 * Event fired when a new SW become active (first SW install or when replacing an existing SW)
 * @param {*} event TODO
 */
async function onActivate(event) {
    notifyActive();

    // Delete unused caches
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
}

async function onSwCacheKill() {
    swlog('SW: CacheKill Received');

    const cacheKeys = await caches.keys();
    for (const key of cacheKeys) {
        if (key.startsWith(cacheNamePrefix))
        {
            swlog('CacheKill: Deleting cache ' + key);
            await caches.delete(key);
        }
    }
}

/**
 * When a SW is activated it can handle all fetch event
 * @param {*} event Fetch event
 * @returns A response from cache if found or the result of a network fetch (xhr) 
 */
async function onFetch(event) {
    let cachedResponse = null;
    if (event.request.method === 'GET') {
        // Open the local cache
        const cache = await caches.open(cacheName);
        // For all navigation requests, try to serve index.html from cache
        // If you need some URLs to be server-rendered, edit the following check to exclude those URLs
        const shouldServeIndexHtml = event.request.mode === 'navigate'
            && !event.request.url.toLowerCase().includes('/connect/')
            && !event.request.url.toLowerCase().includes('/identity/');
        if (shouldServeIndexHtml) {
            cachedResponse = await cache.match('index.html');
        } else {
            // Remove all queryString from the request for searching in the local cache (will be keept if only fetch required)
            const cleanRequest = event.request.url ? event.request.url.split('?')[0] : event.request;
            cachedResponse = await cache.match(cleanRequest);
        }
    }
    // Return the cached response and if any try to make an http call
    return cachedResponse || fetch(event.request);
}

//#endregion

//#region WebPush Notification

function OpenDb() {
    return Lagoon.SharedServiceWorker.GetDb(self.applicationRootName);
}

/**
 * A WebPush notification has been received 
 **/
function onPushNotificationReceived(e) {
    const obj = e.data.json();
    // rq: copy options to avoid display 'null' as value 
    // (could be avoided if null property wasn't serialized)
    var options = {};
    if (obj.actions != null) options.actions = obj.actions;
    if (obj.badge != null) options.badge = obj.badge;
    if (obj.body != null) options.body = obj.body;
    if (obj.data != null) options.data = obj.data;
    if (obj.icon != null) options.icon = obj.icon;
    if (obj.image != null) options.image = obj.image;
    if (obj.renotify != null) options.renotify = obj.renotify;
    if (obj.requireInteraction != null) options.requireInteraction = obj.requireInteraction;
    if (obj.tag != null) options.tag = obj.tag;
    if (obj.silent != null) options.silent = obj.silent;
    if (obj.vibrate != null) options.vibrate = obj.vibrate;
    // Test afficher ou pas les notifications reçus si l'app est au premier plan
    // A ajouter en param mais : la 'doc' préconnise de toujours finir par un affichage de notification
    // au risque de voir la subscription invalidée
    if (false) {
        // Show the notification only if the application is not loaded
        e.waitUntil(
            self.clients.matchAll({ includeUncontrolled: true, type: 'window' }).then(clients => {
                if (clients.length > 0) {
                    console.log('onPushNotificationReceived received but application seem active');
                } else {
                    self.registration.showNotification(obj.title, options);
                }
            })
        );
    } else {
        // Always show the notification
        e.waitUntil(
            self.registration.showNotification(obj.title, options)
        );
    }
}

/**
 * A WebPush notification has been clicked (on the notification itself or an action button) 
 **/
function onPushNotificationClicked(e) {
    // On Android we need to close the notif 
    e.notification.close(); 
    e.waitUntil(
        clients.matchAll({ type: 'window' }).then(windowClients => {
            // If the application is active and notification have navigation data
            if (e.notification.data && windowClients.length > 0) {
                self.postMessage('navigate-to-tab§' + e.notification.data);
                return Promise.resolve();
            // Check if there is already a window/tab open with the target URL
            } else if (windowClients.length > 0) {
                for (var i = 0; i < windowClients.length; i++) {
                    var client = windowClients[i];
                    if (client.url.startsWith(self.registration.scope) && 'focus' in client) {
                        return client.focus();
                    }
                }
            // If not, then open the target URL in a new window/tab.
            } else {
                var url = baseHref + (e.notification.data ? e.notification.data : '');
                return clients.openWindow(url);
            }

        })
    );
}

/**
 * The browser has changed the notification endpoint and we have to update the existing subscription saved on server-side.
 * rq: the app. could not be loaded so we send directly the new subscription info with the previous one to update the subscription on server side
 **/
function onPushSubscriptionChanged(e) {
    const saveSubscriptionAndSendToServer = function(data) {
        return OpenDb().then(db => {
            // Save/Override the new registration (debug purpose, should be removed)
            var trans = db.transaction('RegisteredSubcription', 'readwrite');
            var store = trans.objectStore('RegisteredSubcription');
            store.put(data);
            // Retrieve the current subscription from indexeddb (it's has been saved on subscription)
            // to be able to send the old and new endpoint on server-side
            var prevSubscription = store.get('Subscription');
            // Send to new subscription to server-side
            return fetch("LgWebPushNotification", {
                method: "PUT",
                headers: {
                    "Content-type": "application/json",
                },
                body: JSON.stringify({
                    OldEndpoint: prevSubscription.Endpoint,
                    Endpoint: data.Endpoint,
                    P256dh: data.P256dh,
                    Auth: data.Auth
                })
            });
        });
    }
    e.waitUntil(
        saveSubscriptionAndSendToServer({
            OldEndpoint: e.oldSubscription ? e.oldSubscription.endpoint : null,
            Endpoint: e.newSubscription ? e.newSubscription.endpoint : null,
            P256dh: e.newSubscription ? e.newSubscription.toJSON().keys.p256dh : null,
            Auth: e.newSubscription ? e.newSubscription.toJSON().keys.auth : null,
            Type: 'RenewSubscription'
        })
    );
}

//#endregion
