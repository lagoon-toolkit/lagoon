// LgColorPicker namespace
Lagoon.LgColorPicker = (function () {
    var lastElementFocus = null;

    // Default focus after keyboard dropdown toggle
    var initToggleFocus = function (eltRef, last = false, first = true) {
        if (last) {
            setFocus(eltRef, $(eltRef).find(".option-item:last"));
        } else if (first) {
            setFocus(eltRef, $(eltRef).find(".option-item:first"));
        } else {
            let optionSel = $(eltRef).find(".option-selected:first");
            if (!optionSel.length) {
                optionSel = $(eltRef).find(".option-item:first");
            }
            setFocus(eltRef, optionSel);
        }
    }

    /**
     * Set focus with keyboard
     * @param {any} eltRef
     * @param {any} elementToFocus
     */
    var setFocus = function (eltRef, elementToFocus) {
        // remove tabindex on previous element
        if ($(lastElementFocus).length) {
            $(lastElementFocus).removeClass('option-item-selected');
        }
        // Set focus on the element        
        const element = $(elementToFocus);
        if (element.hasClass('option-item')) {
            element.focus();
            eltRef?.setAttribute('aria-activedescendant', element.attr('id'));
            // Remove all other selection
            element.closest('.color-picker-content')
                .find('.option-item.option-item-selected')
                .removeClass('option-item-selected');
            element.addClass('option-item-selected');
            // Scroll to have selected element always visible            
            if (!isVisible(element[0], eltRef.querySelector('div.color-picker-content'))) {
                element[0].scrollIntoView({ behavior: 'smooth', block: 'nearest' });
            }
        } else {
            element.focus();
        }
        lastElementFocus = elementToFocus;
    }

    /**
     * Check if element is visible in container
     * @param {any} element
     * @param {any} container
     */
    var isVisible = function (element, container) {
        const { bottom, height, top } = element.getBoundingClientRect();
        const containerRect = container.getBoundingClientRect();

        return top <= containerRect.top ? containerRect.top - top <= height : bottom - containerRect.bottom <= height;
    }

    return {
        /**
         * Focus initialization 
         * @param {any} eltRef  Select reference
         */
        initFocus: function (eltRef, last = false, first = false) {
            lastElementFocus = null;
            // Delay for dropdown opening latency
            setTimeout(function () {
                // Init focus into dropdown list
                initToggleFocus(eltRef, last, first);
            }, 25);
        },
        /**
        * Reset focus 
        * @param {any} eltRef  Select reference
        */
        resetFocus: function (eltRef) {
            if (lastElementFocus != null) {
                $(lastElementFocus).removeClass('select-option-selected');
            }
            // Set to current selected option
            $(eltRef).find('.select-option-item[aria-selected="true"]').addClass('select-option-selected');
            lastElementFocus = null;
        },
        /**
         * Init the Select keyboard control with focus management
         * @param {any} eltRef Blazor reference to the select component
         * @param {any} hasSearchBox Select has a searchbox
         */
        keyBoardController: function (eltRef, hasShowInput) {
            // Default : focus on first element selected or first element of the list
            let dropdownExpanded;
            lastElementFocus = null;
            const self = this;

            $(eltRef).on('keydown', function (e) {
                dropdownExpanded = $(eltRef).find(".dropdown-menu").hasClass("show");
                const optionActive = dropdownExpanded ? eltRef.querySelector("div.option-item-selected") : null;
                const activeElement = optionActive || document.activeElement;
                const activeElementJq = $(activeElement);
                const widthItem = $(eltRef).find(".option-item").outerWidth();
                const widthDropdown = $(eltRef).find(".dropdown-menu").width();
                const nbItemsPerRow = widthDropdown / widthItem;

                let stop = false;

                if (dropdownExpanded) {
                    // Manage option selection when list is open
                    if (String.fromCharCode(e.keyCode).match(/(\w|\s)/g) && hasShowInput && e.key != "Enter" && e.key != "Tab") {
                        setFocus(eltRef, $(eltRef).find(".input-color input"));
                    } else {
                        //pressed key is a non-char
                        //e.g. 'esc', 'backspace', 'up arrow'
                        const keyValue = e.key.toLowerCase();
                        switch (keyValue) {
                            case "arrowdown":
                                var nextIndex = (parseInt(activeElementJq.index()) + nbItemsPerRow + 1);

                                if ($(eltRef).find('.option-item:nth-child(' + nextIndex + ')').length) {
                                    setFocus(eltRef, $(eltRef).find('.option-item:nth-child(' + nextIndex + ')'));
                                    stop = true;
                                }
                                break;
                            case "arrowup":
                                var prevIndex = (parseInt(activeElementJq.index()) - nbItemsPerRow + 1);

                                if ($(eltRef).find('.option-item:nth-child(' + prevIndex + ')').length) {
                                    setFocus(eltRef, $(eltRef).find('.option-item:nth-child(' + prevIndex + ')'));
                                    stop = true;
                                }
                                break;
                            case "arrowright":
                                if (activeElementJq.next('.option-item').length) {
                                    setFocus(eltRef, activeElementJq.next('.option-item'));
                                    stop = true;
                                }
                                break;
                            case "arrowleft":
                                if (activeElementJq.prev('.option-item').length) {
                                    setFocus(eltRef, activeElementJq.prev('.option-item'));
                                    stop = true;
                                }
                                break;
                            case "home":
                                if ($(eltRef).find(".option-item:first").length) {
                                    setFocus(eltRef, $(eltRef).find(".option-item:first"));
                                    stop = true;
                                }
                                break;
                            case "end":
                                if ($(eltRef).find(".option-item:last").length) {
                                    setFocus(eltRef, $(eltRef).find(".option-item:last"));
                                    stop = true;
                                }
                                break;
                            case "enter":
                                if (activeElementJq.hasClass('option-item')) {
                                    const event = new Event('click', {
                                        bubbles: true
                                    });
                                    activeElement.dispatchEvent(event);
                                    stop = true;
                                }
                                break;
                        }
                        if (stop) {
                            e.preventDefault();
                            e.stopPropagation();
                        }
                    }
                }

            });
        }
    }

})();