
class LgAutoHideMenu {

    /**
    * Initialize this class instance.
    * @param {string} eltRef Dotnet ElementRef of the menuitem.
    */
    constructor(eltRef) {
        // Menu item element 
        this.menuItem = $(eltRef);
        // Timer
        this.throttle = 500;
        // Create an observer instance to watch the add or remove menu items
        this.itemsObserver = new MutationObserver(this.itemListChanged.bind(this));
    }

    /**
     * Dispose auto hide element & mutation observer
     * */
    Dispose() {
        this.itemsObserver?.disconnect();
        this.itemsObserver = null;
        this.menuItem = null;
        Lagoon.hideDropdown = null;
    }

    /**
    * Update menuitem when sub menuitem list changed. (Raised when items are added in an AuthorizedView).
    * @param {MutationRecord[]} mutationsList An array of MutationRecord objects, describing each change that occurred.
    * @param {MutationObserver} observer The MutationObserver which invoked this method.
    */
    itemListChanged(e) {
        this.hide(true);
    };

    /**
    * Hide sub menuitem when empty.
    * @param {bool} reset true to reset the cached menu items width.
    */
    hide(reset) {
        if (this.throttleTimeout) clearTimeout(this.throttleTimeout);
        this.throttleTimeout = setTimeout((function () {
            // wrap menu if menu exist in DOM
            this.hideNow(reset);
        }).bind(this), this.throttle);
    }

    /**
    * Hide sub level if do not contrains children.
    * @param {bool} reset true to reset the cached menuitems items .
    */
    hideNow(reset) {
        // Disable the mutation observer
        this.itemsObserver.disconnect();
        // Check if menuitem has ul children
        if ($(this.menuItem).children('ul').length > 0) {
            var li = $(this.menuItem).children('ul').children('li');
            // Check if menuitem has ul as child whithout any li children
            if (li.length == 0) {
                // Remove ul element
                $(this.menuItem).children('ul').remove();
                // Remove class dropdown-toggle on 'a' element
                $(this.menuItem).find('a').removeClass('dropdown-toggle');
                // Remove dropdown class
                $(this.menuItem).removeClass('dropdown');
                // Rmove class for sub dropdown
                $(this.menuItem).removeClass('dropdown-submenu');
            }
        }
        // Start observing the menu item list for configured mutations
        this.itemsObserver.observe(this.menuItem[0], { attributes: false, childList: true, subtree: false });
    }
}

/**
 * Hide sub level when empty
 * @param {any} eltRef element reference
*/
Lagoon.autoHideDropdownMenuItem = function (eltRef) {
    if (!eltRef.hideDropdown) {
        eltRef.hideDropdown = new LgAutoHideMenu(eltRef);
    }
    eltRef.hideDropdown.hide(true);
}

/**
 * Dispose auto hide mutation observer
 * @param {any} eltRef
 */
Lagoon.autoHideDropdownMenuItemDispose = (eltRef) => {
    eltRef.hideDropdown?.Dispose();
};
