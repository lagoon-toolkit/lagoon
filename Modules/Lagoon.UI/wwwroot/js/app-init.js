//#region error interception
function showCriticalError() {
    window.traceCritical = true;
    var div = document.getElementById("blazor-error-ui");
    if (!window.appLoaded) {
        var btnReload = div.getElementsByClassName("reload")[0];
        btnReload.addEventListener('click', async function (e) {
            // Clear storage
            localStorage.clear();
            sessionStorage.clear();
            // Unregister service-worker
            if (navigator.serviceWorker) {
                await navigator.serviceWorker.getRegistrations().then(async function (registrations) {
                    for (const registration of registrations) {
                        if (registration.active) registration.active.postMessage('kill-cache');
                        if (registration.waiting) registration.waiting.postMessage('kill-cache');
                        await registration.unregister();
                    }
                }).catch(function (ex) {
                    console.log('Service Worker unregistration failed: ', ex);
                });
            }
            // Reload page
            window.location.reload();
            e.preventDefault();
            return false;
        });
    }
    // Display the error
    document.getElementById("blazor-error-ui").style.display = 'block';
    // Hide the app div
    //NET6_0_OR_GREATER
    var app = document.getElementById("app");
    if (app) app.style.display = 'none';
    //!NET6_0_OR_GREATER
    var apps = document.getElementsByTagName("app");
    if (apps && apps.length > 0) apps[0].style.display = 'none';
}

// Intercept error messages
(function () {
    var originalErrorFunc = console.error;
    console.error = function () {
        if (arguments.length > 0 && (typeof arguments[0] === 'string')) {
            var msg = arguments[0];
            var crit = msg.lastIndexOf('crit: ', 1);
            if (crit > -1) {
                msg = msg.slice(6 + crit);
                crit = true;
                showCriticalError();
            }
            if (window.traceCritical) {
                var div = document.getElementById('app-critical-detail');
                if (div) {
                    msg.match(/[^\r\n]+/g).forEach((line) => {
                        var text = document.createTextNode(line);
                        var span = document.createElement((line.lastIndexOf(' ', 0) === 0) ? 'span' : 'b');
                        if (line.includes(':line ')) span.className = 'red';
                        span.appendChild(text);
                        div.appendChild(span);
                    });
                }
            }
        }
        originalErrorFunc.apply(console, arguments);
    };
})();

// All other fatal errors
window.onerror = function (err) {
    console.warn(err);
}

//#endregion

// Initialize lagoon module
window.Lagoon = window.Lagoon || {};

// Keep the application build version
Lagoon.Version = document.currentScript.getAttribute('version');

// Get the browser language
Lagoon.getUserLanguage = function() {
    return navigator.language;
}

// Called when the AddLagoonAsync method is succesfully complete
Lagoon.onAppInitialized = function (appRef) {
    window.appLoaded = true;
    Lagoon.WindowManager.watchResize(appRef);
    return Lagoon.WindowManager.getWindowInformation();
}

// Insert script
Lagoon.includeJS = function (url) {
    return new Promise(function (resolve, reject) {
        Lagoon.pendingIJS++;
        var script = document.createElement("script");
        script.async = false;
        script.defer = false;
        script.src = url + (url.includes('?') ? "&" : "?") + 'v=' + Lagoon.Version;
        // When the script in the DOM and ready to be used
        script.onload = function () {
            resolve();
        };
        // if it fails, return reject
        script.onerror = function () {
            reject();
        };
        document.body.appendChild(script);
    });
};

// Append SVG in DOM (else there's delay to render them in chromium)
Lagoon.insertIconLibrary = function (xml) {
    var content = (new XMLSerializer()).serializeToString(xml);
    var div = document.createElement("div");
    div.innerHTML = content;
    // Don't use display:none, else radian items are not working in chrome (v93)
    div.firstChild.setAttribute("style", "position:absolute; height:0; width:0");
    document.body.appendChild(div.firstChild);
}
// Download SVG collection
Lagoon.initIconLibrary = function () {
    fetch("icons.svg")
        .then(response => response.text())
        .then((t) => (new window.DOMParser()).parseFromString(t, "text/xml"))
        .then((x) => { Lagoon.insertIconLibrary(x.documentElement) })
        .catch(err => console.log("Lagoon.initIconLibrary", err));
}

// Ensure browser base path equal to index.html base path
Lagoon.ensureLowerCasePath = function () {
    var currentPath = window.location.pathname;
    let href = window.location.pathname.substr(0, baseHref.length);
    if (!href.endsWith('/')) href += '/';
    href = href.toLocaleLowerCase();
    var newPath = baseHref + window.location.pathname.substr(baseHref.length);
    // If the baseHref only differ in case or trailing slash, redirect to the clean path wich match index.html baseHref
    if (href == baseHref.toLocaleLowerCase() && currentPath != newPath) {
        window.location.pathname = newPath;
        return false;
    } else {
        return true;
    }
}

Lagoon.LoadingParams = {
    LoadResourcesProgression: 0,
    OriginalLoadingAnimation: '',
    GetOriginalLoadingAnimation: function () {
        return this.OriginalLoadingAnimation;
    }
}

// Show download progression (only called by a modified 'blazor.webassembly.js')
Lagoon.loadResourceCallback = function (total, name, response) {
    if (name.endsWith('.dll'))
    {
        Lagoon.LoadingParams.LoadResourcesProgression++;
        const value = parseInt((Lagoon.LoadingParams.LoadResourcesProgression * 100.0) / total);
        const pct = value + '%';

        const divLoading = document.getElementsByClassName("app-loading")[0];
        if (pct != '100%') {
            divLoading.innerHTML = '<div class="app-loading-detail">' + Lagoon.LoadingParams.OriginalLoadingAnimation + '<div class="app-loading-detail-download"></div><div class="app-loading-detail-download-progression"><div class="app-loading-detail-download-progressionbar" style="width:' + pct + '">&nbsp;</div></div></div>';
        } else {
            divLoading.innerHTML = '<div class="app-loading-detail">' + Lagoon.LoadingParams.OriginalLoadingAnimation + '<div class="app-loading-detail-loading"></div></div>';
        }
    }
}

// Init app when the DOM is ready
document.addEventListener("DOMContentLoaded", function () {
    // We keep application base relative URL (used in jsUtils)
    window.baseHref = document.getElementsByTagName('base')[0].getAttribute('href');
    // We load only authentication JS script when index.html is opened in the hidden iframe
    if ((window !== parent) && (baseHref === parent.baseHref)) {
        Lagoon.includeJS("_content/Microsoft.AspNetCore.Components.WebAssembly.Authentication/AuthenticationService.js");
        return;
    }
    // Check url
    if (Lagoon.ensureLowerCasePath()) {
        // Load 'blazor-error-ui' content
        var divError = document.getElementById("blazor-error-ui");
        if (divError) {
            divError.innerHTML = '<div class="big-msg"><h1 class="lblTitle">An unexpected error has occurred</h1><h2 class="lblTitle">Please contact an administrator.</h2><a id="btnReload" href="./">Reload</a><a id="btnDetail" href="#" onclick="document.getElementById(\'app-critical-detail\').style.display=\'block\';return false">Detail</a><div id="app-critical-detail"></div></div>';
        }
        // Load loading content
        var divLoading = document.getElementsByClassName("app-loading")[0];
        if (divLoading) {
            var loadingAnimation = divLoading.getElementsByClassName('app-loading-animation');
            if (loadingAnimation.length > 0) {
                Lagoon.LoadingParams.OriginalLoadingAnimation = loadingAnimation[0].outerHTML;
            }
            divLoading.innerHTML = '<div class="app-loading-detail">' + Lagoon.LoadingParams.OriginalLoadingAnimation + '<div class="app-loading-detail-initialisation"></div></div>';
        }
        // Load additionals javascripts
        Lagoon.includeJS("_content/Lagoon.UI/js/jquery.js");
        Lagoon.includeJS("_content/Lagoon.UI/js/popper.js");
        Lagoon.includeJS("_content/Lagoon.UI/js/bootstrap.js");
        Lagoon.includeJS("_content/Lagoon.UI/js/bootstrap-datepicker.js");
        Lagoon.includeJS("_content/Lagoon.UI/js/inputmask.js");
        Lagoon.includeJS("_content/Lagoon.UI/js/jquery.mask.js");
        Lagoon.includeJS("_content/Lagoon.UI/js/main.min.js");
        Lagoon.includeJS("_content/Lagoon.UI/js/toastr.js");
        Lagoon.includeJS("_content/Lagoon.UI/js/inputfile.js");
        Lagoon.includeJS("_content/Lagoon.UI/js/jquery-clock-timepicker.js");
        Lagoon.includeJS("_content/Lagoon.UI/js/service-worker-shared.js");
        Lagoon.includeJS("_content/Microsoft.AspNetCore.Components.WebAssembly.Authentication/AuthenticationService.js");
        Lagoon.includeJS("_framework/blazor.webassembly.js").then(
            function () {
                // Dotnet resolver between JS Object and C# Object.
                DotNet.attachReviver(Lagoon.JsObjectManager.Reviver);
            }
        );
        // Load icon library in DOM
        Lagoon.initIconLibrary();
    }
});
