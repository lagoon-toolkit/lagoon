// LgModal namespace
Lagoon.LgModal = (function () {

    return {        
        // Initialize modal
        Init: function (element, dotNetReference, isDraggable, allowDragLeave) {
            const modal = $(element);
            const modalDialog = modal.find('.modal-dialog').first();
            const modalContent = modal.find('.modal-content').first();
            if (modal.data('modal')) return;            
            let modalData = {
                reference: dotNetReference,
                event: this.KeyEvent              
            };            
            if (!this.lastFocused) {
                this.lastFocused = {};
            }
            this.lastFocused[element.id] = this.GetActiveElement();
            modal.on('keydown.lg.modal', modalData.event);
            // Fix JS error "Event X is already tracked" when Esc call confirmation modal
            // TODO check if needed in Net6+
            setTimeout(() => {
                modal.focus();
            }, 1);
            modal.data('modal', modalData);
            element._allowDragLeave = allowDragLeave;
            if (isDraggable) {
                element._fnStartDrag = function (e) {
                    // Don't start drag process on toolbar
                    var target = $(e.target).first();
                    if (target.hasClass('modal-header') || target.hasClass('modal-title')) {
                        var originX = e.clientX;
                        var originY = e.clientY;
                        var originalLeftPosition = modalDialog[0].offsetLeft;
                        var originalTopPosition = modalDialog[0].offsetTop;
                        var originalWidth = modalDialog[0].offsetWidth;
                        var originalHeight = modalContent[0].offsetHeight;
                        // Ensure the modal have a fixed width before starting the drag process
                        modalDialog
                            .css('width', originalWidth + 'px')
                            .css('user-select', 'none')
                            .css('opacity', '0.5');
                        // Function used to constraint the drag inside the window
                        element._fnConstaint = function (left, top) {
                            var result = { left: left, top: top }
                            var showButton = false;
                            if (!element._allowDragLeave) {
                                if (left < 0) {
                                    result.left = 0;
                                    showButton = true;
                                } else if (left + originalWidth > window.innerWidth) {
                                    result.left = window.innerWidth - originalWidth;
                                    showButton = true;
                                }
                                if (top < 0) {
                                    result.top = 0;
                                    showButton = true;
                                } else if (top + originalHeight > window.innerHeight) {
                                    result.top = window.innerHeight - originalHeight;
                                    showButton = true;
                                }
                                if (showButton) {
                                    $('.btn-modal-dragr', this).show();
                                }
                            } else {
                                // even if we allow the modal overflow, ensure that a part (50px) of the modal is still in the window
                                // so we can start a new drag process with the visible part
                                if (left + 50 > window.innerWidth) {
                                    result.left = window.innerWidth - 50;
                                } else if (left - 50 < originalWidth * -1) {
                                    result.left = (originalWidth * -1) + 50;
                                }

                                if (top + 50 > window.innerHeight) {
                                    result.top = window.innerHeight - 50;
                                } else if (top - 50 < originalHeight * -1) {
                                    result.top = (originalHeight * -1) + 50;
                                }
                            }

                            return result;
                        }



                        var fnMove = function (e) {
                            // Compute the new top/left with 'click origin' and current mouse position
                            var newTop = originalTopPosition - (originY - e.clientY);
                            var newLeft = originalLeftPosition - (originX - e.clientX);
                            var pos = element._fnConstaint(newLeft, newTop);
                            modalDialog.css('margin', pos.top + 'px ' + pos.left + 'px');
                        };
                        var fnUp = function (e) {
                            $(window).off('mousemove', fnMove);
                            $(window).off('mouseup', fnUp);
                            modal.css('user-select', 'initial');
                            modalDialog.css('opacity', '1');
                        };
                        $(window)
                            .on('mousemove', fnMove)
                            .on('mouseup', fnUp);
                    }
                };
                // Function used to constraint the modal in the visible area
                element._fnOnResize = function () {
                    if (element._fnConstaint) {
                        var leftPosition = modalDialog[0].offsetLeft;
                        var topPosition = modalDialog[0].offsetTop;
                        var pos = element._fnConstaint(leftPosition, topPosition);
                        modalDialog.css('margin', pos.top + 'px ' + pos.left + 'px')
                    }
                }
                modal
                    .find('.modal-header')
                        .css('cursor', 'move')
                        .on('mousedown', element._fnStartDrag);
                // On window resize, ensure the modal is still is the visible area
                $(window).resize(element._fnOnResize);
            }               

            // Keep focus inside modal
            const self = this;
            $(document).on('keydown', function (e) {                
                if (e.code == "Tab") {
                    // Get children list with all actives elements
                    if (!self.FocusableChildren) {
                        self.FocusableChildren = Lagoon.JsUtils.tabbableElement(modal[0]);
                    }
                    const currentIndex = self.FocusableChildren.indexOf(document.activeElement);                    
                    // Check if TAB go out of modal
                    const isFirstLast = e.shiftKey ? currentIndex == 0 : currentIndex == self.FocusableChildren.length - 1;
                    if (currentIndex < 0 || isFirstLast) {
                        e.preventDefault();
                        const focusedElement = e.shiftKey ? self.FocusableChildren.at(-1) : self.FocusableChildren.at(0);                        
                        focusedElement?.focus();
                    }
                }
            });
        },

        // Update the drag mode (to allow or not the modal to leave partially the visible area)
        UpdateDragLeave: function (element, allowDragLeave) {
            element._allowDragLeave = allowDragLeave;
            // Launch a resize to ensure that the modal is still in the visible area
            $(window).trigger('resize');
        },

        // Free events handler
        Dispose: function (id) {
            const modal = $('#' + id);
            const modalData = modal.data('modal');
            if (modal.length == 0) return;
            if(modalData) {                
                modal.removeData('modal');
            }
            modal.off('keydown.lg.modal');
            $(document).off('keydown');
            if (modal[0]._fnOnResize) {
                $(window).off('resize', modal[0]._fnOnResize);
                modal[0]._fnOnResize = null;
            }
            if (modal[0]._fnStartDrag) {
                modal.find('.modal-header').off('mousedown', modal[0]._fnStartDrag);
                modal[0]._fnStartDrag = null;
            }
            // Focus last element focused or nearest before open modal            
            const lastFocused = this.lastFocused[id];            
            if (lastFocused) {
                let elementToFocus = null;
                // Timeout used to fix "Unknown edit type: 0"
                setTimeout(() => {                    
                    const lastFocusedInDom = document.body.contains(lastFocused.Element);                    

                    if (lastFocusedInDom && Lagoon.JsUtils.isTabbable(lastFocused.Element)) {
                        // Case of element not removed                            
                        elementToFocus = lastFocused.Element;
                    } else {
                        // Case of element focusable is not visible                        
                        elementToFocus = Lagoon.JsUtils.getTabbable(lastFocused.Element);
                    }
                    // Case of element focusable has been changed (StateHasChanged)
                    if (!elementToFocus) {
                        elementToFocus = this.GetElementFocusable(lastFocused);
                    }                    
                    elementToFocus?.focus();
                }, 100);
                delete this.lastFocused[id];                
            }               
        },

        // Manage keys
        KeyEvent: function (e) {            
            // Escape close
            const keyCode = e.code || e.originalEvent?.code;            
            if (keyCode === "Escape") {
                const modalData = $(e.currentTarget).data('modal');
                if (modalData) {
                    e.preventDefault();
                    modalData.reference.invokeMethodAsync('CloseFromJsAsync');
                }
            }
        },

        /**
         * Return focused element and its parents
         * */
        GetActiveElement() {
            const activeElement = document.activeElement;
            const getParents = el => {
                for (var parents = []; el; el = el.parentElement) {                    
                    if (el.tagName === "APP") {
                        break;
                    }
                    const index = el.parentElement ?
                        Array.prototype.indexOf.call(el.parentElement.children, el)
                        : -1;                    
                    parents.push({
                        Elt: el,
                        Index: index
                    });
                }

                return parents;
            };
            
            return {
                Element: activeElement,
                Parents: getParents(activeElement)
            };
            return activeElement;
        },

        /**
         * Get element focusable in DOM Tree
         * @param {any} element
         */
        GetElementFocusable(element) {
            if (element.Parents) {
                try {
                    const parents = element.Parents.reverse();                                                        
                    let elementToFocus = null;
                    let parent = null;

                    for (var i = 0; i < parents.length; i++) {
                        // Find the first parent element of the first removed element from DOM
                        if (!parent && !document.body.contains(parents[i].Elt)) {                            
                            parent = parents[i - 1].Elt;
                        }
                        // Find element by index
                        if (parent) {
                            elementToFocus = parent.children[parents[i].Index];
                            parent = elementToFocus;
                        }
                    }                                                           
                    return elementToFocus;
                } catch (e) {
                    console.log("GetElementFocusable - Error", e);
                    return null;
                }                
            }
        }
    };
})();