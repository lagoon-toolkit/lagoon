// LgSelect namespace
Lagoon.LgSelect = (function () {
    var lastElementFocus = null;
    var throttleTimeout;
    var throttle = 25;

    // Default focus after keyboard dropdown toggle
    var initToggleFocus = function (eltRef, last = false, first = true) {        
        if (last) {            
            setFocus(eltRef, $(eltRef).find(".select-option-item:last"));
        } else if (first) {            
            setFocus(eltRef, $(eltRef).find(".select-option-item:first"));
        } else {            
            let optionSel = $(eltRef).find(".select-option-selected:first");
            if (!optionSel.length) {
                optionSel = $(eltRef).find(".select-option-item:first");
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
            $(lastElementFocus).removeClass('select-option-selected');
        }
        // Set focus on the element        
        const element = $(elementToFocus);        
        if (element.hasClass('select-option-item')) {
            $(eltRef).focus();            
            eltRef?.setAttribute('aria-activedescendant', element.attr('id'));
            // Remove all other selection
            element.closest('.select-dropdown-item-list')
                .find('.select-option-item.select-option-selected')
                .removeClass('select-option-selected');
            element.addClass('select-option-selected');
            // Scroll to have selected element always visible            
            if (!isVisible(element[0], eltRef.querySelector('div.select-dropdown-item-list'))) {
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

    /**
         * Set the Select dropdown position
         * fix the dropdown list opens by expanding the contents of the modal-body (with scroll)
         * instead of opening in front of the modal
         * @param {any} eltRef Blazor reference to the select component
         */
    var setDropdownPositionNow = function (eltRef) {

        if (eltRef.isPositioning) {
            return;
        }

        eltRef.isPositioning = true;
        eltRef.intersectionObserver?.disconnect();
        eltRef.resizeObserver1?.disconnect();
        eltRef.resizeObserver2?.disconnect();

        var buttonElement = $(eltRef).find("button.dropdown-toggle");
        var dropdownElement = $(eltRef).find(".select-dropdown-menu");
        if (dropdownElement.hasClass("hide")) {
            eltRef.isPositioning = false;
            return;
        }
        var buttonBounding = buttonElement[0].getBoundingClientRect();
        var availableSpaceTop = buttonBounding.top;
        var viewportWidth = window.innerWidth || document.documentElement.clientWidth;
        var availableSpaceBottom = (window.innerHeight || document.documentElement.clientHeight) - buttonBounding.bottom;
        var dropdownHeight;
        var calculatedTop;
        var calculatedLeft = buttonBounding.left;
        var calculatedWidth = $(eltRef).width();
        var dropDownMaxHeight;

        if (eltRef.initialMaxHeight === undefined) {
            eltRef.initialMaxHeight = dropdownElement.css("max-height").replace('px', '');
        }
        // Reinit calculated values
        dropdownElement.css({
            left: "0",
            width: "",
            "max-height": "",
            visibility: "hidden"
        });
        // Apply the default positionning (not yet visible while visibility is still hidden)
        dropdownElement.addClass("show");
        // Vertical dropdown management
        dropdownHeight = dropdownElement.outerHeight(true);
        if (availableSpaceBottom > dropdownHeight) {
            // Enougth place at the bottom
            calculatedTop = buttonBounding.bottom;
            dropDownMaxHeight = eltRef.initialMaxHeight;
        } else if (availableSpaceTop > dropdownHeight) {
            // Enougth place at the top
            calculatedTop = buttonBounding.top - dropdownHeight;
            dropDownMaxHeight = eltRef.initialMaxHeight;
        } else if (availableSpaceTop > availableSpaceBottom) {
            // More place at the top but not enought so : set je dropdownlist max-height
            calculatedTop = buttonBounding.top - availableSpaceTop;
            dropDownMaxHeight = availableSpaceTop;
        } else {
            // More place at the bottom but not enought so : set je dropdownlist max-height
            calculatedTop = buttonBounding.bottom;
            dropDownMaxHeight = availableSpaceBottom;
        }
        var optionsVertical = {
            top: calculatedTop,
            "max-height": (parseInt(dropDownMaxHeight)) + 2 + 'px'
        };
        dropdownElement.css(optionsVertical);
        // Horizontal dropdown management
        var dropdownWidth = dropdownElement.outerWidth();
        calculatedWidth = calculatedWidth > dropdownWidth ? calculatedWidth : dropdownWidth;
        // Detect if it's right or left aligned with the button
        if (buttonBounding.left < viewportWidth / 2) {
            // Left align
            calculatedLeft = buttonBounding.left;
            var overflow = calculatedLeft + calculatedWidth - viewportWidth;
            if (overflow > 0) {
                calculatedWidth -= overflow;
            }
        } else {
            // Right
            calculatedLeft = buttonBounding.right - calculatedWidth;
            if (calculatedLeft < 0) {
                calculatedWidth = calculatedWidth + calculatedLeft;
                calculatedLeft = 0;
            }
        }
        // Apply horizontal positionning
        dropdownElement.css({
            left: calculatedLeft,
            width: calculatedWidth,
            visibility: ""
        });
        // Watch the resizing
        eltRef.ignoreRegistrationIntersection = true;
        eltRef.ignoreResizeObserver1 = true;
        eltRef.ignoreResizeObserver2 = true;
        eltRef.intersectionObserver.observe(dropdownElement[0]);
        eltRef.resizeObserver1.observe(buttonElement[0]);
        eltRef.resizeObserver2.observe(dropdownElement[0]);
        // Unlock this method call
        eltRef.isPositioning = false;
    }

    /**
     * Set the Select dropdown position
     * fix the dropdown list opens by expanding the contents of the modal-body (with scroll)
     * instead of opening in front of the modal
     * @param {any} eltRef Blazor reference to the select component
     */
    var setDropdownPosition = function (eltRef) {
        if (throttleTimeout) clearTimeout(throttleTimeout);
        throttleTimeout = setTimeout((function () {
            setDropdownPositionNow(eltRef);
        }), throttle);
    }

    return {
        /**
         * Intersection Observer on the select dropdown list
         * @param {any} eltRef Element reference
         * @param {any} dotNetRef DotNet reference
         */
        onOpening: function (eltRef, dotNetRef) {

            var self = this;
            var dropdownElement = $(eltRef).find(".select-dropdown-menu");
            if (dropdownElement.hasClass("hide")) {
                return;
            }

            this.closeDropDown = function () {
                var dropdownElement = $(eltRef).find(".select-dropdown-menu");
                dotNetRef.invokeMethodAsync("CloseDropdownAsync");
            }

            // Scroll listener
            eltRef.onScroll = function (e) {
                // Do not catch event when scrolling into the dropdown list
                if (!$(e.target).hasClass('select-dropdown-item-list')) {
                    self.closeDropDown();
                }
            };
            document.addEventListener('scroll', eltRef.onScroll, true);

            // Focus out listener
            eltRef.onFocusOut = function (e) {
                dotNetRef.invokeMethodAsync("TryCloseListAsync");
            };
            document.addEventListener('focusout', eltRef.onFocusOut, true);

            // InterserctionObserver 2 - Root : Null; Track : List
            //    - Keep the dropdown list visible => set the dropdown position
            if (!eltRef.intersectionObserver) {
                eltRef.intersectionObserver = new IntersectionObserver(entry => {
                    if (eltRef.ignoreRegistrationIntersection) {
                        eltRef.ignoreRegistrationIntersection = false;
                        return;
                    }
                    setDropdownPosition(eltRef);
                }, { threshold: 1 });
            }

            // ResizeObserver 1 - Track : Button
            //    - when button size changed and dropdown list is at the bottom:
            //          set the dropdown position
            if (!eltRef.resizeObserver1) {
                eltRef.resizeObserver1 = new ResizeObserver(entry => {
                    if (eltRef.ignoreResizeObserver1) {
                        eltRef.ignoreResizeObserver1 = false;
                        return;
                    }
                    setDropdownPosition(eltRef);
                });
            }

            // ResizeObserver 2 - Track : dropdown list
            //    - when dropdown list changed :
            //          set the dropdown position
            if (!eltRef.resizeObserver2) {
                eltRef.resizeObserver2 = new ResizeObserver(entry => {
                    if (eltRef.ignoreResizeObserver2) {
                        eltRef.ignoreResizeObserver2 = false;
                        return;
                    }
                    setDropdownPosition(eltRef);
                });
            }

            // Init position
            setDropdownPosition(eltRef);
        },
        /**
         * Dispose events listener
         * Disconnect intersection observers
         * Disconnect resize observers
         * @param {any} eltRef element reference
         */
        dispose: function (eltRef) {
            if (eltRef.intersectionObserver) {
                eltRef.intersectionObserver.disconnect();
                eltRef.intersectionObserver = null;
            }
            if (eltRef.resizeObserver1) {
                eltRef.resizeObserver1.disconnect();
                eltRef.resizeObserver1 = null;
            }
            if (eltRef.resizeObserver2) {
                eltRef.resizeObserver2.disconnect();
                eltRef.resizeObserver2 = null;
            }
            if (eltRef.onScroll) {
                document.removeEventListener('scroll', eltRef.onScroll, true);
                eltRef.onScroll = null;
            }
            if (eltRef.onFocusOut) {
                document.removeEventListener('focusout', eltRef.onFocusOut, true);
                eltRef.onFocusOut = null;
            }
        },
        /**
         * Disconnect the intersection observer 
         * Remove scroll event listener
         * @param {any} eltRef element reference
         */
        onClosing: function (eltRef) {
            this.dispose(eltRef);
        },
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
        keyBoardController: function (eltRef, hasSearchBox) {
            // Default : focus on first element selected or first element of the list
            let dropdownExpanded;
            lastElementFocus = null;
            const self = this;            
            let closeOnSelect = !$(eltRef).find('.select-dropdown-menu').attr('aria-multiselectable');
            
            $(eltRef).on('keydown', function (e) {                
                dropdownExpanded = $(eltRef).find(".select-dropdown-menu").hasClass("show");
                const optionActive = dropdownExpanded ? eltRef.querySelector("a.select-option-item.select-option-selected") : null;
                const activeElement = optionActive || document.activeElement;                
                const activeElementJq = $(activeElement);
                let stop = false;

                if (dropdownExpanded) {                    
                    // Manage option selection when list is open
                    if (String.fromCharCode(e.keyCode).match(/(\w|\s)/g) && hasSearchBox && e.key != "Enter" && e.key != "Tab") {
                        setFocus(eltRef, $(eltRef).find(".input-searchbox input"));
                    } else {
                        //pressed key is a non-char
                        //e.g. 'esc', 'backspace', 'up arrow'
                        const keyValue = e.key.toLowerCase();                        
                        switch (keyValue) {
                            //// Esc
                            case "escape":
                                stop = true;
                                setFocus(eltRef, $(eltRef).find(".dropdown"));
                                self.closeDropDown();
                                break;
                            // ARROW DOWN
                            case "arrowdown":                                
                                // Focus into searchbox                                                                          
                                if (activeElementJq.closest('.select-dropdown-search').length) {                                    
                                    setFocus(eltRef, $(eltRef).find('.select-option-item:first'));
                                    stop = true;
                                }
                                else if (activeElementJq.next('.select-option-item').length) {                                    
                                    setFocus(eltRef, activeElementJq.next('.select-option-item'));
                                    stop = true;
                                } else if (lastElementFocus == null) {                                    
                                    initToggleFocus(eltRef);
                                    stop = true;
                                }                                
                                break;
                            // ARROW UP
                            case "arrowup":                                
                                if (activeElementJq.prev('.select-option-item').length) {
                                    setFocus(eltRef, activeElementJq.prev('.select-option-item'));
                                    stop = true;
                                } else if (lastElementFocus == null) {
                                    initToggleFocus(eltRef);
                                    stop = true;
                                }                                
                                break;        
                            case "home":                                
                                if ($(eltRef).find(".select-option-item:first").length) {
                                    setFocus(eltRef, $(eltRef).find(".select-option-item:first"));
                                    stop = true;
                                }                                
                                break;
                            case "end":                                
                                if ($(eltRef).find(".select-option-item:last").length) {
                                    setFocus(eltRef, $(eltRef).find(".select-option-item:last"));
                                    stop = true;
                                }                                
                                break;                                                                                        
                            case "enter":                                
                                if (activeElementJq.hasClass('select-option-item')) {
                                    const isSelected = activeElementJq.attr('aria-selected');
                                    const event = new Event('click', {
                                        bubbles: true
                                    });
                                    activeElement.dispatchEvent(event);
                                    if (closeOnSelect) {
                                        self.closeDropDown();
                                    } else if (isSelected) {
                                        setFocus(eltRef, lastElementFocus);
                                    }
                                    stop = true;
                                }
                                break;
                            case "tab":
                                self.closeDropDown();
                                break;
                        }
                        if (stop) {
                            e.preventDefault();
                            e.stopPropagation();
                        }
                    }
                } else {
                    // Manage delete for multiple selection
                    switch (e.key.toLowerCase()) {                        
                        // Delete
                        case "delete":
                            if (activeElementJq.hasClass('select-element')) {
                                var prevSelectedElt = null;
                                if (activeElementJq.prev('.select-element').length) {
                                    prevSelectedElt = activeElementJq.prev('.select-element');
                                }
                                // trigger delete item 
                                var buttonDeletion = activeElementJq.find(".select-element-remove");
                                const eventDeletion = new Event('click', {
                                    bubbles: true
                                });
                                buttonDeletion[0].dispatchEvent(eventDeletion);
                                if (prevSelectedElt != null) {
                                    // if there is an other selected element : select it
                                    setFocus(eltRef, prevSelectedElt);
                                } else {
                                    // select dropdown 
                                    setFocus(eltRef, $(eltRef).find(".dropdown"));
                                }
                            } else if (activeElementJq.hasClass('dropdown')) {
                                 // trigger delete selection                                 
                                var buttonReset = activeElementJq.find(".select-reset-btn");                                
                                const eventReset = new Event('click', {
                                    bubbles: true
                                });
                                buttonReset[0].dispatchEvent(eventReset);
                            }
                            break;
                        // ARROW LEFT
                        case "arrowleft":                            
                            if (activeElementJq.hasClass('select-element') && activeElementJq.prev('.select-element').length) {
                                setFocus(eltRef, activeElementJq.prev('.select-element'));
                            }                            
                        break;
                        // ARROW RIGHT
                        case "arrowright":                            
                            if (activeElementJq.hasClass('select-element') && (activeElementJq.next('.select-element').length)) {
                                setFocus(eltRef, activeElementJq.next('.select-element'));
                            }
                            else if ($(eltRef).find('.select-element').length) {
                                setFocus(eltRef, $(eltRef).find('.select-element:first'));
                            }                            
                        break;
                    }
                }
               
            });
        }
    }

})();