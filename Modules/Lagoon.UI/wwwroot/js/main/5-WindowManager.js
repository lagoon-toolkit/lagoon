Lagoon.WindowManager = (function () {

    var appRef;
    var resizeTimeout;

    return {

        // Subscibe to window resize events
        watchResize: function (dotnetRef) {
            appRef = dotnetRef;
            window.addEventListener('resize', this.windowResize.bind(this))
        },

        // On resized invoke service
        windowResize: function () {
            if (resizeTimeout) clearTimeout(resizeTimeout);
            var wndMng = this;
            resizeTimeout = setTimeout(function () {
                appRef.invokeMethodAsync("WindowResizeAsync", wndMng.getWindowInformation())
            }, 300);
        },

        // Navigate to the specified URI
        getWindowInformation: function () {
            return {
                height: window.innerHeight,
                width: window.innerWidth,
                portrait: window.matchMedia("(orientation: portrait)").matches,
                mobile: window.matchMedia("(max-width: 767.98px)").matches
            };
        }
    }

})();