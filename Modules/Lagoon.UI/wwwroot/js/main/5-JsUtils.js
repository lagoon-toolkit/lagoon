// JsUtils namespace
Lagoon.JsUtils = (function () {
    // Container in which the bootstrap tooltips are added
    var $tooltipContainer;

    //Element with tooltip 
    var elemWithTooltip;

    //MutationObserver to detect element's dom removal
    var mutationObserver;

    //MutationObserver to listen for changes to the title attribute 
    var attributeObserver;

    // Tooltip initialization
    var initTooltip = function () {
        // Customize the sanitizer
        // https://getbootstrap.com/docs/4.3/getting-started/javascript/#sanitizer
        var myDefaultWhiteList = $.fn.tooltip.Constructor.Default.whiteList;
        myDefaultWhiteList['*'].push("style");
        myDefaultWhiteList.table = [];
        myDefaultWhiteList.tr = [];
        myDefaultWhiteList.td = ['colspan', 'rowspan'];
        myDefaultWhiteList.th = myDefaultWhiteList.td;
        // Tooltip container (all tooltip generated  by Bootstrap/Popper will be added in this container)
        var divTooltip = document.createElement("div");
        divTooltip.id = "tooltip-container";
        document.body.appendChild(divTooltip);
        $tooltipContainer = $(divTooltip);
        // Used to detected if a tooltip must be destroyed (the target element has been removed/regenerated)
        mutationObserver = new MutationObserver(function (e) {
            if (elemWithTooltip && !document.body.contains(elemWithTooltip)) {
                attributeObserver.disconnect();
                elemWithTooltip = null;
                $tooltipContainer.empty();
            }
        });
        // When the title or disabled attribute change for the element currently tracked, re-create a new tooltip
        attributeObserver = new MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                if (mutation.type === "attributes" && (mutation.attributeName === "title" || mutation.attributeName === "disabled") && $(elemWithTooltip).attr("title")) {
                    // Ensure the destroyed
                    $tooltipContainer.empty();
                    // Re-create the tooltip if it's title has change
                    if (document.body.contains(elemWithTooltip) && $(elemWithTooltip).is(":hover")) {
                        $(elemWithTooltip).attr('data-original-title', $(elemWithTooltip).attr("title"));
                        $(elemWithTooltip).attr("title", '');
                        showBootstrapTooltips(elemWithTooltip);
                    }
                }
            });
        });
        // Manually trigger the bootstrap tooltip on mouseenter
        document.body.addEventListener('mouseenter', showTooltip, true);
    };

    var showTooltip = function (event) {
        var el = event.target;
        // Check if the targer element has the text-truncate class which truncate the text if there is not enought space
        // Enable the tooltip visualization based on element content only if required (eg. if the text is truncated)
        if (el.classList?.contains('text-truncate')) {
            // using the constant '2' to take into account potential borders not necessary present on the current element (eg. could be done on a parentNode)
            // but if the text overflows by two pixels it remains readable even without the tooltip
            if (el.offsetWidth <= (el.scrollWidth - 2) || el.offsetHeight <= (el.scrollHeight - 2)) {
                // add title attribute which will be used as tooltip content
                el.setAttribute('title', el.innerText);
            } else {
                // remove title attribute: the size could have changed since the last showTooltip
                // and it's not necessary to show a tooltip for this element
                el.removeAttribute('title');
                el.removeAttribute('data-original-title');
            }
        }
        // Check for tooltip value
        if ($(el).attr("title") || $(el).attr("data-original-title")) {
            if ($(el).attr("title")) {
                $(el).attr('data-original-title', $(el).attr("title"));
                $(el).attr('title', '');
            }
            // Wait for a second before showing tooltip
            setTimeout(function () {
                if ($(el).is(":hover") && el != elemWithTooltip) {
                    // Ensure original tooltip (title attribute is removed)
                    if ($(el).attr('title')) {
                        $(el).attr('data-original-title', $(el).attr("title"));
                        $(el).attr('title', '');
                    }
                    // Track the element wich trigger the tooltip
                    elemWithTooltip = el;
                    // Check for title attribute update
                    attributeObserver.observe(el, {
                        attributes: true, //configure it to listen to attribute changes
                    });
                    // The target element coulb be removed/regenerated, use observer to detect orphan tooltip
                    mutationObserver.observe(document.body, { childList: true, subtree: true });
                    // Show the tooltip
                    showBootstrapTooltips(el);
                    // Restore title attribute when tooltip is hidden
                    $(el).off('hide.bs.tooltip').on('hide.bs.tooltip', function () {
                        elemWithTooltip = null;
                        attributeObserver.disconnect();
                        $(el).attr('title', $(el).attr("data-original-title"));
                        mutationObserver.disconnect();
                    });
                }
            }, 1000);
        }
    };

    //show Bootstrap tooltips
    var showBootstrapTooltips = function (el) {
        // Ensure there is only one tooltip to show
        $tooltipContainer.empty();

        var htmlTooltip = ($(el).attr('data-tooltip-html') == "true");

        // Show the element tooltip
        $(el)
            .tooltip({
                placement: function (sender, e) {
                    if ($(e).attr("disabled")) {
                        if (e.tagName != "INPUT") { 
                            throw "Tooltips for .disabled or disabled elements must be triggered on a wrapper element."
                        }
                    }
                    // position define on element attribute
                    if ($(e).attr("data-tooltip-pos") != null) {
                        return $(e).attr("data-tooltip-pos");
                    } else {
                        // position auto
                        return "auto";
                    }
                },
                html: htmlTooltip,
                customClass: htmlTooltip ? "" : "tooltip-text",
                trigger: 'hover',
                delay: { show: 1000, hide: 250 },//Delay showing does not work but hiding does
                container: $tooltipContainer
            }).tooltip('show');
    }

    /**
     * Initialize accessibility for main menu
     */
    var initMenuAccess = function () {
        // focus first item on open
        $(document).on('shown.bs.dropdown', (e) => {
            const firstItem = $(e.target).find("[role=menuitem]").first();
            firstItem.focus();
        });
    }

    // OnLoad => Intialize bootstrap tooltip
    $(function () {
        initTooltip();
        initMenuAccess();
    });

    return {

        // Set focus on tag by its selector
        focusElement(selector, parent = null) {
            let elt;
            elt = !parent ? $(selector) : $(parent).find(selector);
            if (elt.length > 0) elt.focus();
        },

        // Set focus on element by its blazor reference
        focusElementByReference(elementReference, selector = null, delay) {
            if (elementReference instanceof HTMLElement) {
                setTimeout(() => {
                    let element = elementReference;
                    if (selector) {
                        element = elementReference.querySelector(selector);
                    }
                    element?.focus();
                }, delay); // To fix focus problem in LgDropDown (Textbox isn't visible on AfterRender)
            }
        },

        // Set focus on tag by its Id
        focusElementById(elementId) {
            var elt = document.getElementById(elementId);
            if (elt) {
                if (this.isTabbable(elt)) {
                    elt.focus();
                }
                else {
                    // Focus first tabbable element
                    const focusableChildren = this.tabbableElement(elt);
                    if (focusableChildren.length > 0) {
                        focusableChildren[0].focus();
                    }
                }
            }
        },

        /**
         * Move the focus following direction
         * @param {any} container         
         * @param {MoveFocusDirection} direction
         */
        moveFocus(container, direction) {
            // Get focusable element of the parent
            let index = 0;
            let focusElt;
            let focusCtn;
            let forceIndex;
            const activeElement = document.activeElement;
            switch (direction) {
                case -1: // First
                    focusCtn = container;
                    forceIndex = -1;
                    break;
                case 0: //up
                    focusCtn = container;
                    moveIndex = -1;
                    break;
                case 2: // down
                    focusCtn = container;
                    moveIndex = 1;
                    break;
                case 1: // right
                    focusCtn = activeElement.parentnode;
                    moveIndex = 1;
                    break;
                case 3: // left
                    focusCtn = activeElement.parentnode;
                    moveIndex = -1;
                    break;
                case 4: // Last
                    focusCtn = container;
                    forceIndex = 1;
                    break;
                default:
                    break;
            }
            if (!focusCtn) return;
            const elements = this.tabbableElement(focusCtn) ? this.tabbableElement(focusCtn) : [];
            let destIndex = 0;
            if (!forceIndex) {
                elements.every(elt => {
                    if (elt === activeElement) {
                        return false;
                    }
                    index++;
                    return true;
                });
                destIndex = index + moveIndex;
            } else {
                destIndex = forceIndex > 0 ? elements.length - 1 : 0;
            }
            focusElt = elements[Math.min(elements.length - 1, Math.max(0, destIndex))];
            focusElt?.focus();
        },

        /**
         * Return all tabbable elements inside container
         * @param {any} container
         */
        tabbableElement: (container) => {
            if (!container) return [];
            const tabbableElts = [
                'input',
                'select',
                'textarea',
                'a[href]',
                'button:not([disabled])',
                '[tabindex]',
                'audio[controls]',
                'video[controls]',
                '[contenteditable]:not([contenteditable="false"])',
                'details>summary:first-of-type',
                'details',
            ];
            const children = container.querySelectorAll(tabbableElts.join(','));
            let tabbableChildren = [];
            children.forEach((elt) => {
                // Reject hidden and not focusable element
                if (Lagoon.JsUtils.isTabbable(elt)) {
                    tabbableChildren.push(elt);
                }
            });
            return tabbableChildren;
        },

        /**
         * Indicate if element can receive Tab focus          
         * @param {any} element
         */
        isTabbable(element) {
            return $(element).is(':visible') && element.tabIndex >= 0;
        },

        /**
         * Get the first tabbable previous element
         * @param {any} child
         */
        getTabbable(child) {
            try {
                let parent = child.parentNode;
                const focusable = $(parent).find('button, a, input, select, textarea, [tabindex]:not([tabindex="-1"])');

                if (focusable.length) {
                    let focusableElt = null;
                    focusable.each((idx, elt) => {
                        if (this.isTabbable(elt) && document.body.contains(elt)) {
                            focusableElt = elt;
                            return false;
                        }
                    });
                    if (focusableElt) {
                        return focusableElt;
                    }
                }
                return this.getTabbable(parent);
            } catch (err) {
                return null;
            }
        },

        /**
         * Get first child tabbable element
         * @param {any} parent
         */
        getTabbableChild(parent) {
            try {
                const focusable = $(parent).find('button, a, input, select, textarea, [tabindex]:not([tabindex="-1"])');
                if (focusable.length) {
                    let focusableElt = null;
                    focusable.each((idx, elt) => {
                        if (this.isTabbable(elt) && document.body.contains(elt)) {
                            focusableElt = elt;
                            return false;
                        }
                    });
                    if (focusableElt) {
                        return focusableElt;
                    }
                }
            } catch (err) {
                return null;
            }
        },

        // Detect if element is overflowed
        hasOverflow(selector) {
            var data = { x: false, y: false };
            var elt = $(selector);

            if (elt.prop('scrollWidth') > elt.width()) {
                data.x = true;
            }

            if (elt.prop('scrollHeight') > elt.height()) {
                data.y = true;
            }
            return data;
        },

        // Filter management on text input
        onFilterKey(element, mode, chars) {
            $(element).keypress(function (event) {
                // Don't handle "Enter" key
                if (event.keyCode == 13) return true;
                var code = event.charCode ? event.charCode : event.keyCode;
                if (!mode) mode = '';
                var reg = new RegExp('[' + mode + chars + ']');
                if (reg.exec(String.fromCharCode(code))) return true;
                event.preventDefault();
            });
        },

        // Handle modal to draggable modal
        draggableModal(element) {
            $(element).draggable({
                handle: ".modal-header"
            });
        },

        initToastrOption(toastrOptionJson) {
            // Initialize app toastr options
            toastr.options = {
                "closeButton": toastrOptionJson.closeButton,
                "debug": toastrOptionJson.debug,
                "newestOnTop": toastrOptionJson.newestOnTop,
                "progressBar": toastrOptionJson.progressBar,
                "positionClass": toastrOptionJson.positionClass,
                "preventDuplicates": toastrOptionJson.preventDuplicates,
                "onclick": toastrOptionJson.onclick,
                "showDuration": toastrOptionJson.showDuration,
                "hideDuration": toastrOptionJson.hideDuration,
                "timeOut": toastrOptionJson.timeOut,
                "extendedTimeOut": toastrOptionJson.extendedTimeOut,
                "showEasing": toastrOptionJson.showEasing,
                "hideEasing": toastrOptionJson.hideEasing,
                "showMethod": toastrOptionJson.showMethod,
                "hideMethod": toastrOptionJson.hideMethod,
                "escapeHtml": toastrOptionJson.escapeHtml
            };
        },

        hideAllToastr: function() {
            var container = toastr.getContainer()[0];
            if (container) {
                container.querySelectorAll(".toastr").forEach(
                    function (toastElement) {
                        $(toastElement).data('delayedHideToast')();
                    }
                );
            }
        },

        /**
         * Create/Modify a cookie
         * @param {string} name Cookie name
         * @param {string} value Cookie value
         * @param {number} days Cookie expiration (null if no expiration)
         */
        CreateCookie: function (name, value, days) {
            var expires;
            if (days) {
                // Build cookie expiration date
                var date = new Date();
                date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                expires = "; expires=" + date.toGMTString();
            }
            else {
                // No expiration provided: cookie stored in session
                expires = "";
            }
            // Add cookie
            document.cookie = name + "=" + value + expires + "; path=" + window.baseHref + ";SameSite=Lax";
        },

        /**
         * Retrieve a cookie value
         * @param {string} name Cookie name
         * @returns Cookie value if found, null otherwise
         */
        GetCookie: function (name) {
            // Split cookie string and get all individual name=value pairs in an array
            var cookieArr = document.cookie.split(";");

            // Loop through the array elements
            for (var i = 0; i < cookieArr.length; i++) {
                var cookiePair = cookieArr[i].split("=");

                /* Removing whitespace at the beginning of the cookie name
                and compare it with the given string */
                if (name == cookiePair[0].trim()) {
                    // Decode the cookie value and return
                    return decodeURIComponent(cookiePair[1]);
                }
            }

            // Return null if not found
            return null;
        },

        // On click html element content => stop propagation on some elements
        InitClickControl(elt) {
            $(elt).click(function (event) {
                var arrayInputs = ['input', 'textarea', 'select', 'option'];
                var targetName = event.target.localName;
                if (arrayInputs.indexOf(targetName) >= 0) {
                    event.stopPropagation();
                }
            });
        },

        /**
         * Change language of the document
         * @param {string} lang The ISO 639-1 two-letter code for the language of the culture.
         * @param {string} culture The culture name in the format languagecode2-country/regioncode2.
         */
        ChangeLang: function (xLangCookieName, lang, culture) {
            // Change the HTML document attribute "lang"
            let currentLang = lang ? lang : document.documentElement.lang;
            document.documentElement.lang = currentLang;
            // Update http cookie (server should answer in the same language)
            this.CreateCookie(xLangCookieName, culture, 365 * 5);
        },

        /**
         * Change document title
         * @param {string} title
         */
        ChangePageTitle: function (title) {
            document.title = title;
        },

        /**
         * Display message for screen reader only
         */
        ChangeSrText: function (message, temporary, timer) {
            const srArea = $("#divMainSrOnly");
            srArea.text(message);
        },

        /***
         * Return the base href
         **/
        GetBaseHref: function () {
            return window.baseHref;
        },

        /**
         * Return true if localstorage contain a key with "LgCgu" + application base path 
         **/
        HasAcceptedCgu: function (eulaKey, eulaDicoKey, eulaVersion) {
            var result = localStorage.getItem(eulaKey);
            if (result != "") {
                var eulaDico = localStorage.getItem(eulaDicoKey);
                if (eulaDico) {
                    eulaDico = JSON.parse(eulaDico);
                    // If only one item, there is no eula to show
                    // so we consider that the eula has been accepted
                    if (eulaDico.length == 1) {
                        console.error('LgEula: Eula are configured but no local eula found.');
                        return true;
                    }
                    // Check the current validated version
                    for (var i in eulaDico) {
                        if (eulaDico[i].Id == "EulaVersion") {
                            return result == eulaDico[i].Value;
                        }
                    }
                }
                return result == eulaVersion;
            }
            return false;
        },

        /**
         * Return the list of languageKey supported by CGU 
         **/
        GetCguSupportedLanguage: function (eulaDicoKey) {
            var lng = [];
            var result = localStorage.getItem(eulaDicoKey);
            if (result) {
                result = JSON.parse(result);
                for (var i in result) {
                    if (result[i].Id != "EulaVersion") {
                        lng.push(result[i].Id);
                    }
                }
            }
            return lng;
        },

        /**
         * Create a LgCgu key in local storage 
         **/
        AcceptCgu: function (cguKey, eulaVersion) {
            localStorage.setItem(cguKey, eulaVersion);
        },

        // Get apple-touch-icon for LgLoaderView
        getAppleTouchIcon() {
            // We keep application base relative URL (used in jsUtils)
            var appleToucheIcon = $('link[rel="apple-touch-icon"]').attr('href');

            return appleToucheIcon;
        },

        /**
         * Get content width of element
         * @param {any} selector
         */
        GetElementScrollWidth(selector, minWidth = 40) {
            var element = $(selector).first();
            if (element) {
                var htmlElement = element.get(0);
                if (htmlElement) {
                    var sizingDiv = document.createElement('div');
                    sizingDiv.innerHTML = element.html();
                    var computedStyle = window.getComputedStyle(htmlElement);
                    sizingDiv.style = computedStyle || {};
                    sizingDiv.style.display = 'inline-block';
                    document.body.appendChild(sizingDiv);
                    var width = $(sizingDiv).width();
                    document.body.removeChild(sizingDiv);
                    return Math.round(width);
                }
            }

            return minWidth;
        },

        //Get Element Offset Width
        GetElementOffsetWidth(containerSelector, elementSelector) {
            const container = document?.querySelector(containerSelector);
            var element = container?.querySelector(elementSelector);
            if (element)
                return element.offsetWidth;

            return 0;
        },

        /**
         * Get selector width 
         * @param {any} selector
         */
        GetElementWidth(selector) {
            return $(selector).width();
        },

        /**
         * Limit event call frequency
         * @param {any} func callback function
         * @param {any} limit time limit in millisecond
         */
        Throttle(func, limit) {
            let inThrottle
            return function () {
                const args = arguments
                const self = this
                if (!inThrottle) {
                    func.apply(self, args)
                    inThrottle = true
                    setTimeout(() => inThrottle = false, limit)
                }
            }
        },

        // Disable response for 'Enter' key on HtmlElement
        DisableEnterKey: function (element) {
            if (element.IsEnterKeyDisabled) return;
            element.IsEnterKeyDisabled = true;
            element.addEventListener('keypress', e => {
                if (e.which == 13 && e.target?.type !== 'textarea') { // Enter Key
                    e.preventDefault();
                }
            });
        },

        // Delete an IndexedDB by it's name
        DeleteDatabase: function (dbName) {
            return new Promise((resolve, reject) => {
                var req = window.indexedDB.deleteDatabase(dbName);
                req.onsuccess = function () {
                    resolve();
                };
                req.onerror = function () {
                    console.error("DeleteDatabase - Fail to delete the IndexedDB named '" + dbName + "' (error)");
                    resolve();
                };
                req.onblocked = function () {
                    console.error("DeleteDatabase - Fail to delete the IndexedDB named '" + dbName + "' (blocked)");
                    resolve();
                };
            });
        },

        /**
         * Apply migration for localStorage & indexedDb keys
         * @param {any} keyPattern Storage key pattern
         */
        CleanupLocalStorage: function (keyPattern) {
            return new Promise((resolve, reject) => {
                // Flag to avoid applying the migration script multiple times
                var migrationKey = keyPattern.replace('{0}', 'Migration');

                if (localStorage.getItem(migrationKey) == null) {
                    try {
                        // Clear localStorage old keys
                        var localKeys = Object.keys(localStorage);
                        for (const key of localKeys) {
                            var content = localStorage.getItem(key);

                            // Lagoon keys (deleting without migration)
                            if (key.startsWith("LgAppSettings") ||
                                key.startsWith("LgApplicationClaims") ||
                                key.startsWith("LgErrorsContainer")) {

                                // Remove it from localStorage
                                localStorage.removeItem(key);

                                // Eula keys migration
                            } else if (key.startsWith("LgEulaDico/")) {
                                // Eula migration
                                var newKey = keyPattern.replace('{0}', "EulaDico");
                                localStorage.removeItem(key);
                                localStorage.setItem(newKey, content);

                            } else if (key.startsWith("LgEula/")) {
                                var newKey = keyPattern.replace('{0}', "EulaState");
                                localStorage.removeItem(key);
                                localStorage.setItem(newKey, content);
                            }
                            // GridView keys migration
                            else if (key.startsWith("http") && key.includes("gridview-")) {
                                var newKey = 'undefined';
                                // Key without app url prefix
                                var oldKey = key.substr(key.lastIndexOf('/') + 1);
                                var splittedKey = oldKey.split('-');
                                var stateId = splittedKey[1];

                                if (key.includes('-profileslist')) {                                    
                                    newKey = keyPattern.replace('{0}', "GridView-ListProfile-" + stateId);
                                    // Replacing value in 'id' property for each stored profile
                                    content = JSON.parse(content);
                                    for (const obj of content) {
                                        var idProfileIndex = obj.id.split('-');
                                        var idProfile = idProfileIndex[idProfileIndex.length - 1];
                                        obj.id = stateId + "-" + idProfile;
                                    }
                                    content = JSON.stringify(content);

                                } else if (key.includes('-profile-')) {
                                    var profileIndex = splittedKey[3];
                                    newKey = keyPattern.replace('{0}', "GridView-Profile-" + stateId + "-" + profileIndex);
                                    content = JSON.parse(content);
                                    var idProfileIndex = content.id.split('-');
                                    var idProfile = idProfileIndex[idProfileIndex.length - 1];
                                    content.id = stateId + "-" + idProfile;
                                    content = JSON.stringify(content);

                                } else if (key.includes('-lastprofile')) {
                                    newKey = keyPattern.replace('{0}', "GridView-LastProfile-" + stateId);
                                    content = JSON.parse(content);
                                    // Replacing value in 'id' property
                                    var idProfileIndex = content.id.split('-');
                                    var idProfile = idProfileIndex[idProfileIndex.length - 1];
                                    content.id = stateId + "-" + idProfile;
                                    content = JSON.stringify(content);

                                } else {
                                    console.error(' !!! NOT Migrated  !! => ' + key);
                                }
                                console.log('Migration "' + key + '" to "' + newKey + '"');
                                // Rename key                   
                                localStorage.removeItem(key);
                                localStorage.setItem(newKey, content);
                            }
                        }
                        // Rq: Firefox does not expose the databases function wich allow to retrieve the list of databases. 
                        // Currently the is no way to retrieve the list of databases in Firefox
                        if (typeof indexedDB.databases == "function") {
                            // Clear old IndexedDB
                            indexedDB.databases().then(databases => {
                                for (const db of databases) {
                                    if (db.name.startsWith("NotificationDataStore") || db.name.startsWith("TabDataStore")) {
                                        console.log('deleting : ' + db.name);
                                        window.indexedDB.deleteDatabase(db.name);
                                    }
                                }
                            });
                        }
                        // Store the migration version
                        localStorage.setItem(migrationKey, "1");
                    } catch (ex) {
                        console.error('An error occured while upgrading localStorage keys.', ex);
                    }
                }
                resolve();
            });
        },

        /**
         * Ensure element always visible in scroll container
         * @param {any} element element to follow
         * @param {any} container scrolling container         
         */
        ScrollFollowing: function (element, container, offset = 0) {
            const eleTop = element.offsetTop - offset;
            const eleBottom = eleTop + element.clientHeight + offset;
            const containerTop = container.scrollTop;
            const containerBottom = containerTop + container.clientHeight;
            if (eleTop < containerTop) {
                // Scroll to the top of container
                container.scrollTop -= containerTop - eleTop;
            } else if (eleBottom > containerBottom) {
                // Scroll to the bottom of container
                container.scrollTop += eleBottom - containerBottom;
            }
        },
        
        /**
        * Detect if html element is under another html element
        * @param {any} elt element to test
        */
        ElementIsBelow(elt) {
            const boundingRect = elt.getBoundingClientRect();
            // adjust coordinates to get more accurate results
            const left = boundingRect.left + 1
            const right = boundingRect.right - 1
            const top = boundingRect.top + 1
            const bottom = boundingRect.bottom - 1
            if (document.elementFromPoint(left, top) !== elt) return true
            if (document.elementFromPoint(right, top) !== elt) return true
            if (document.elementFromPoint(left, bottom) !== elt) return true
            if (document.elementFromPoint(right, bottom) !== elt) return true
            return false
        },
        /**
        * RegEx - Escape special char from text
        * @param {any} text text to escape
        */
        RegExpEscape: function (text) {
            return text.replace(/[-[\]{}()*+!<=:?.\/\\^$|#\s,]/g, '\\$&');
        }      

    }

})();