// LgGlobalSearch namespace
Lagoon.LgGlobalSearch = (function () {
    var lastElementFocus = null;

    /**
     * Set focus with keyboard
     * @param {any} eltRef
     * @param {any} elementToFocus
     */
    var setFocus = function (eltRef, elementToFocus) {
        // remove tabindex on previous element
        if ($(lastElementFocus).length) {
            $(lastElementFocus).removeClass('selected');
        }
        // Set focus on the element        
        const element = $(elementToFocus);
        if (element.hasClass('gs-item-content')) {
            $(eltRef).focus();
            element.addClass('selected');
        } else {
            element.focus();
        }
        lastElementFocus = elementToFocus;
    }

    var closeDropDown = function (dotnetRef) {
        dotnetRef.invokeMethodAsync("CloseDropdown");
    }

    return {
        /**
         * Highlight founded text into each result DOM elements
        * @param {any} eltRef search element reference
        * @param {any} searchedText searched text
         * */
        highlightFoundedText: function (eltRef, searchedText, filterTextSearchMode) {
            let cssClassContainer = ".highlightable";
            
            // Take all matches and insensitive case insensitive match (ignores case of[a - zA - Z])
            if (filterTextSearchMode == 1) {
                // Enum cs => StartWith
                searchedText = "^" + searchedText;
            } else if (filterTextSearchMode == 2) {
                // Enum cs => EndWith
                searchedText = searchedText + "$";
            }
            let regex = new RegExp(Lagoon.JsUtils.RegExpEscape(searchedText), 'gi');

            $(eltRef).find('.gs-item-content:not(.gs-item-content-seeAll)').each(function () {
                $(this).find(cssClassContainer).each(function () {
                    var eltHighlightable = this;
                    if (eltHighlightable) {
                        var text = eltHighlightable.innerHTML;
                        text = text.replace(/(<span class="highlight">|<\/span>)/gim, '');
                        var newText = text.replace(regex, '<span class="highlight">$&</span>');
                        eltHighlightable.innerHTML = newText;
                    }
                });
            });
        },
        /**
        * Reset focus 
        * @param {any} eltRef search element reference
        */
        resetFocus: function (eltRef) {
            if (lastElementFocus != null) {
                $(lastElementFocus).removeClass('selected');
            }
            // Set to current selected option
            $(eltRef).find('.gs-item-content[aria-selected="true"]').addClass('selected');
            lastElementFocus = null;
        },
        /**
         * Init the Select keyboard control with focus management
         * @param {any} eltRef Blazor reference to the select component
         * @param {any} hasSearchBox Select has a searchbox
         */
        keyBoardController: function (eltRef, dotnetRef) {
            //dotnetRef = dotnetRef;
            // Default : focus on first element selected or first element of the list
            let dropdownExpanded;

            $(eltRef).on('keydown', function (e) {
                dropdownExpanded = $(eltRef).find(".dropdown-menu").hasClass("show");
                const optionActive = dropdownExpanded ? eltRef.querySelector(".gs-item-content.selected") : null;
                const activeElement = optionActive || document.activeElement;
                const activeElementJq = $(activeElement);

                let stop = false;

                if (dropdownExpanded) {
                    //pressed key is a non-char
                    //e.g. 'esc', 'backspace', 'up arrow'
                    const keyValue = e.key.toLowerCase();
                    switch (keyValue) {
                        //// Esc
                        case "escape":
                            setFocus(eltRef, $(eltRef).find(".dropdown"));
                            closeDropDown(dotnetRef);
                            break;
                        // ARROW DOWN
                        case "arrowdown":
                            // Focus into searchbox                                                                          
                            if (lastElementFocus == null) {
                                setFocus(eltRef, $(eltRef).find('.gs-item-content:first'));
                            } else if (activeElementJq.next('.gs-item-content').length) {
                                setFocus(eltRef, activeElementJq.next('.gs-item-content'));
                            }
                            stop = true;
                            break;
                        // ARROW UP
                        case "arrowup":
                            if (activeElementJq.prev('.gs-item-content').length) {
                                setFocus(eltRef, activeElementJq.prev('.gs-item-content'));
                            } else if (lastElementFocus == null) {
                                setFocus(eltRef, $(eltRef).find('.gs-item-content:last'));
                            }
                            stop = true;
                            break;
                        case "home":
                            if ($(eltRef).find(".gs-item-content:first").length) {
                                setFocus(eltRef, $(eltRef).find(".gs-item-content:first"));
                                stop = true;
                            }
                            break;
                        case "end":
                            if ($(eltRef).find(".gs-item-content:last").length) {
                                setFocus(eltRef, $(eltRef).find(".gs-item-content:last"));
                                stop = true;
                            }
                            break;
                        case "tab":
                            lastElementFocus = null;
                            closeDropDown(dotnetRef);
                            break;
                        case "enter":
                            if (activeElementJq.hasClass('selected')) {
                                const event = new Event('click', {
                                    bubbles: true
                                });
                                activeElement.dispatchEvent(event);
                                stop = true;
                            } else {
                                dotnetRef.invokeMethodAsync("OnSearchAsync");
                            }

                            setFocus(eltRef, $(eltRef).find(".dropdown"));
                            closeDropDown(dotnetRef);
                            lastElementFocus = null;
                            break;
                    }
                    if (stop) {
                        e.preventDefault();
                        e.stopPropagation();
                    }
                } 
            });
        }
    }

})();