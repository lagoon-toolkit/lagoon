class LgTopMenuWrapper {

    /**
    * Initialize this class instance.
    * @param {string} id ID of the top level menu.
    */
    constructor(id) {
        this.menu = document.getElementById(id);
        // 
        this.throttle = 500;
        // Create an observer instance to watch the header bar resize
        this.resizeObserver = new ResizeObserver(this.resizeHandler.bind(this));
        // Create an observer instance to watch the add or remove menu items
        this.itemsObserver = new MutationObserver(this.itemListChanged.bind(this));
    }

    /**
     * Dispose topMenuXrapper
     * */
    Dispose() {
        this.resizeObserver?.disconnect();
        this.resizeObserver = null;
        this.itemsObserver?.disconnect();
        this.itemsObserver = null;
        this.menu = null;
        Lagoon.topMenuWrapper = null;
    }


    /**
    * Update menu wrapping when the window is resized or the toolbar expand.
    * @param {ResizeObserverEntry[]} entries An array of ResizeObserverEntry objects that can be used to access the new dimensions of the element after each change.
    * @param {ResizeObserver} observer The ResizeObserver which invoked this method.
    */
    resizeHandler() {
        // Ignore the first resize notification due to "observe" registration
        // (Not a bug, it's a feature, see https://github.com/WICG/resize-observer/issues/38#issuecomment-290557457)
        if (this.ignoreRegistrationResize) {
            this.ignoreRegistrationResize = false;
            return;
        }
        this.wrap(false);
    }

    /**
    * Update menu wrapping when top level menu item list changed. (Raised when items are added in an AuthorizedView).
    * @param {MutationRecord[]} mutationsList An array of MutationRecord objects, describing each change that occurred.
    * @param {MutationObserver} observer The MutationObserver which invoked this method.
    */
    itemListChanged() {
        this.wrap(true);
    };

    /**
    * Move first level overflowing menu items to the wrapper sub menu.
    * @param {bool} reset true to reset the cached menu items width.
    */
    wrap(reset) {
        this.hideOverflow(true);
        if (this.throttleTimeout) clearTimeout(this.throttleTimeout);
        this.throttleTimeout = setTimeout((function () {
            // wrap menu if menu exist in DOM
            if ($('.menu-wrapper').length) {
                this.wrapNow(reset);
            }
        }).bind(this), this.throttle);
    }

    /**
    * Move first level overflowing menu items to the wrapper sub menu.
    * @param {bool} reset true to reset the cached menu items width.
    */
    wrapNow(reset) {
        // Ignore the first resize notification due tu "observe" registration (see https://github.com/WICG/resize-observer/issues/38#issuecomment-290557457)
        if (this.isWrapping) {
            return;
        }
        this.isWrapping = true;
        // Disable the resize observer
        this.resizeObserver.disconnect();
        // Disable the mutation observer
        this.itemsObserver.disconnect();
        // Get the menu item wrapper
        let mw = $('.menu-wrapper');
        // Get the menu bar
        let div = mw.closest('div');
        this.hideOverflow(true);
        // Get the menu wraper item list
        let ul = mw.children('ul');
        // Get available width
        let aw = div[0].getBoundingClientRect().width;
        // Get the navbar width
        let cw = Math.round(div[0].parentNode.getBoundingClientRect().width);
        let isMobile = aw === 0 || Math.round(aw) === cw;
        // Load menu item list
        let d = mw.data('wrapper');
        if (reset || !d) {
            d = { items: [], moved: [], tw: 0 };
            mw.prevAll('li').each(function (idx, li) {
                let w = isMobile ? 1 : li.getBoundingClientRect().width;
                d.tw += w;
                d.items.push({ li: li, w: w, n: li.nextSibling });
            });
            if (!isMobile) mw.data('wrapper', d);
        }
        let smw = (!isMobile) && aw < d.tw;
        if (smw) {
            // The menu must be wrapped
            mw.css('visibility','visible').show();
            aw -= mw[0].getBoundingClientRect().width;
        } else {
            // The menu don't need to be wrapped
            mw.hide();
        }
        let tw = d.tw;
        d.items.forEach(function (mi) {
            let toMove = (!isMobile) && aw < tw;
            if (toMove) {
                tw -= mi.w;
                if (!mi.isMoved) {
                    ul.prepend(mi.li);
                    mi.li.className = mi.li.className.replace('dropdown', 'dropdown-submenu');
                    mi.isMoved = true;
                }
            } else {
                if (mi.isMoved) {
                    $(mi.li).insertBefore(mi.n);
                    mi.li.className = mi.li.className.replace('dropdown-submenu', 'dropdown');
                    mi.isMoved = false;
                }
            }
        });
        this.hideOverflow(false);
        // Start observing the menu item list for configured mutations
        this.itemsObserver.observe(mw.closest('ul')[0], { attributes: false, childList: true, subtree: false });
        // Start observing the navbar width
        this.ignoreRegistrationResize = true;
        this.resizeObserver.observe(this.menu);
        this.resizeObserver.observe(document.body);
        // After the first wrap, we reduce the time between refreshing
        this.throttle = 40;
        // The wraping is complete
        this.isWrapping = false;
    }

    /**
    * Hide or show the overflow. We must hide the overflow while we calculate the menu items positions.
    * @param {bool} hidden true to hide the overflow.
    */
    hideOverflow(hidden) {
        this.menu.classList.toggle('overflow-hidden', hidden);
    }

}

/**
* Move first level overflowing menu items to the wrapper sub menu.
*/
Lagoon.wrapTopMenu = function () {
    if (!Lagoon.topMenuWrapper) {
        Lagoon.topMenuWrapper = new LgTopMenuWrapper('divMainMenu');
    }
    Lagoon.topMenuWrapper.wrap(true);
}

/**
 * Dispose topMenuWrapper
 * @param {any} eltRef
 */
Lagoon.wrapTopMenuDispose = () => {
    Lagoon.topMenuWrapper?.Dispose();
};
