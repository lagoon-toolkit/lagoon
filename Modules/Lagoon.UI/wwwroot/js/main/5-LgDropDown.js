// LgDropDown namespace

class DropDown {

    //#region fields

    eltRef;
    control;
    menu;
    throttleTimeout = 10;
    throttle = 25;
    isPositioning = false;
    intersectionObserver;
    ignoreRegistrationIntersection;
    resizeObserver = {
        obs1: null,
        ignore1: false,
        obs2: null,
        ignore2: false,
    };
    initialMaxHeight;
    dotNetRef;
    events = [];
    subList = false;
    isOpening = false;
    options = {
        openOnFocus: false,
        overControl: false,
        minWidth: -1,
        minHeight: -1
    };

    //#endregion

    //#region properties

    /**
     * Gets blazor reference
     * */
    get EltRef() {
        return this.eltRef;
    }

    //#endregion

    //#region methods

    /**
     * Constructor
     * @param {any} eltRef
     * @param {any} dotNetRef
     * @param {object} options
     */
    constructor(eltRef, dotNetRef, options) {
        this.eltRef = eltRef;
        this.dotNetRef = dotNetRef;
        this.options = options || this.options;
        if (!this.eltRef) {
            console.error('DropDown : Element reference not exists.');
        }
        this.Init();
    }

    /**
     * Build dropdown JS
     * */
    Init() {
        this.control = this.eltRef.querySelector('div.dropdown-control');
        this.menu = this.eltRef.querySelector("div.dropdown-menu");
        this.initialMaxHeight = this.menu?.style.maxHeight.replace('px', '');
        this.Dispose();
        if (!this.menu) return;
        // Events
        this.AddEvent({ target: document, type: 'scroll', fct: this.OnScroll.bind(this) });
        this.AddEvent({ target: document, type: 'click', fct: this.OnClickOut.bind(this) });
        this.AddEvent({ target: window, type: 'resize', fct: this.OnResize.bind(this) });
        this.AddEvent({ target: this.eltRef, type: 'keydown', fct: this.OnKeyDown.bind(this) });
        this.AddEvent({ target: this.eltRef, type: 'keyup', fct: this.OnKeyUp.bind(this) });
        if (this.options.openOnFocus) {
            this.AddEvent({ target: this.eltRef, type: 'focusin', fct: this.OnFocus.bind(this) });
        }
        // Set observer
        // InterserctionObserver 2 - Root : Null; Track : List
        //    - Keep the dropdown list visible => set the dropdown position
        if (!this.intersectionObserver) {
            this.intersectionObserver = new IntersectionObserver(entry => {
                if (this.ignoreRegistrationIntersection) {
                    this.ignoreRegistrationIntersection = false;
                    return;
                }
                this.SetDropdownPosition();
            }, { threshold: 1 });
        }
        // ResizeObserver 1 - Track : Control
        //    - when button size changed and dropdown list is at the bottom:
        //          set the dropdown position
        if (!this.resizeObserver.obs1) {
            this.resizeObserver.obs1 = new ResizeObserver(entry => {
                if (this.resizeObserver.ignore1) {
                    this.resizeObserver.ignore1 = false;
                    return;
                }
                this.SetDropdownPosition();
            });
        }
        // ResizeObserver 2 - Track : dropdown list
        //    - when dropdown list changed :
        //          set the dropdown position
        if (!this.resizeObserver.obs2) {
            this.resizeObserver.obs2 = new ResizeObserver(entry => {
                if (this.resizeObserver.ignore2) {
                    this.resizeObserver.ignore2 = false;
                    return;
                }
                this.SetDropdownPosition();
            });
        }
    }

    /**
     * Call the dropdown position setter
     */
    SetDropdownPosition() {
        const self = this;
        if (this.throttleTimeout)
            clearTimeout(this.throttleTimeout);
        this.throttleTimeout = setTimeout((function () {
            self.SetPositionNow();
        }), this.throttle);
    }

    /**
    * Set the dropdown position
    * fix the dropdown list opens by expanding the contents of the modal-body (with scroll)
    * instead of opening in front of the modal    
    */
    SetPositionNow() {

        if (this.isPositioning) {
            return;
        }
        const self = this;
        this.isPositioning = true;
        this.intersectionObserver?.disconnect();
        this.resizeObserver.obs1?.disconnect();
        this.resizeObserver.obs2?.disconnect();

        if (this.menu.classList.contains("hide")) {
            this.isPositioning = false;
            return;
        }
        const realControlBounding = this.control.getBoundingClientRect();
        let controlBounding = {
            left: realControlBounding.left,
            top: realControlBounding.top,
            right: realControlBounding.right,
            bottom: realControlBounding.bottom
        };
        const availableSpaceTop = controlBounding.top;
        const viewportWidth = window.innerWidth
            || document.documentElement.clientWidth;
        const availableSpaceBottom = (window.innerHeight
            || document.documentElement.clientHeight) - controlBounding.bottom;

        // Reinit calculated values
        this.menu.style.left = "0";
        this.menu.style.width = "";
        this.menu.style.maxHeight = "";
        this.menu.style.visibility = "hidden";

        // Apply the default positionning (not yet visible while visibility is still hidden)
        this.menu.classList.add("show");
        // Vertical dropdown management
        // TODO replace with vanilla JS        
        const $menu = $(this.menu);
        const $eltRef = $(this.eltRef);        
        const dropdownHeight = $menu.outerHeight(true);
        let calculatedTop = this.options.overControl ? controlBounding.top : controlBounding.bottom;
        let calculatedLeft = controlBounding.left;
        let dropDownMaxHeight;
        // TODO replace with vanilla JS
        let calculatedWidth = $eltRef.width();
        if (availableSpaceBottom > dropdownHeight) {
            // Enougth place at the bottom            
            dropDownMaxHeight = this.initialMaxHeight ? this.initialMaxHeight : availableSpaceBottom;
        } else if (availableSpaceTop > dropdownHeight) {
            // Enougth place at the top            
            calculatedTop = calculatedTop - dropdownHeight;
            dropDownMaxHeight = this.initialMaxHeight ? this.initialMaxHeight : availableSpaceTop;
        } else if (availableSpaceTop > availableSpaceBottom) {
            // More place at the top but not enought so : set je dropdownlist max-height            
            calculatedTop = calculatedTop - availableSpaceTop;
            dropDownMaxHeight = availableSpaceTop;
        } else {
            // More place at the bottom but not enought so : set je dropdownlist max-height
            dropDownMaxHeight = availableSpaceBottom;
        }
        this.menu.style.top = this.AddPx(calculatedTop);        
        if (this.options.minHeight > 0 && !dropDownMaxHeight) {
            this.menu.style.height = this.AddPx(this.options.minHeight);
        } 
        this.menu.style.maxHeight = this.AddPx(parseInt(dropDownMaxHeight) + 2);
        this.menu.style.maxWidth = '50%';
        // Calculate of the horizontal offset caused by element overlay
        const overElts = this.ElementsOver(this.eltRef);
        if (overElts) {
            let maxLeft = controlBounding.left;
            overElts.forEach(elt => {
                const eltBounding = elt.getBoundingClientRect();
                maxLeft = maxLeft < eltBounding.right ? eltBounding.right : maxLeft;
            });
            controlBounding.left = maxLeft;
        }
        // Horizontal dropdown management
        const dropdownWidth = this.options.minWidth > 0 ? this.options.minWidth : $menu.outerWidth();
        calculatedWidth = calculatedWidth > dropdownWidth ?
            calculatedWidth : dropdownWidth;
        // Detect if it's right or left aligned with the button
        if (controlBounding.left < viewportWidth / 2) {
            // Left align
            calculatedLeft = controlBounding.left;
            let overflow = calculatedLeft + calculatedWidth - viewportWidth;
            if (overflow > 0) {
                calculatedWidth -= overflow;
            }
        } else {
            // Right
            calculatedLeft = controlBounding.right - calculatedWidth;
            if (calculatedLeft < 0) {
                calculatedWidth = calculatedWidth + calculatedLeft;
                calculatedLeft = 0;
            }
        }
        // Apply horizontal positionning       
        this.menu.style.left = this.AddPx(calculatedLeft);        
        this.menu.style.width = this.AddPx(calculatedWidth);        
        this.menu.style.visibility = "";

        // Watch the resizing
        this.ignoreRegistrationIntersection = true;
        this.resizeObserver.ignore1 = true;
        this.resizeObserver.ignore2 = true;
        this.intersectionObserver?.observe(this.menu);
        this.resizeObserver.obs1?.observe(this.control);
        this.resizeObserver.obs2?.observe(this.menu);
        // Unlock this method call
        this.isPositioning = false;
        // Focus first focusable inside menu
        if (this.options.openOnFocus) {
            const toFocus = Lagoon.JsUtils.getTabbableChild(this.menu);
            toFocus?.focus();
        }
    }

    /**
     * Check and add event
     * @param {any} param
     */
    AddEvent(param) {
        // Check if event exists
        const exists = this.events.find(e => e.target == param.target && e.type == param.type);
        if (!exists) {
            this.events.push(param);
            param.target.addEventListener(param.type, param.fct, true);
        }
    }

    /**
     * Focus management
     * */
    OnFocus(e) {
        // Focus come from out of the dropdown only
        if (e.target?.closest('.dropdown-control') && !e.relatedTarget?.closest('.lgDropdown')) {
            this.dotNetRef.invokeMethodAsync("ToggleListAsync");
        }
    }

    /**
     * Lost focus management
     * @param {any} e
     */
    OnBlur(e) {
        if (e.target?.closest('.dropdown-control') && !e.relatedTarget?.closest('.lgDropdown')) {
            this.CloseDropDown();
        }
    }

    /**
     * Click outside dropdown
     * @param {any} e
     */
    OnClickOut(e) {
        const inSubList = e.srcElement?.closest('.datepicker') != null;
        if (!this.eltRef.contains(e.target) && !inSubList) {
            this.CloseDropDown(true);
            // Force calendar to close
            const datepickers = this.menu.querySelectorAll('input.dtb');
            if (datepickers) {
                datepickers.forEach((picker) => {
                    $(picker).datepicker('destroy');
                });
            }
        }
    }

    /**
     * DropDown opened actions
     * */
    OnOpened() {
        if (this.menu.classList.contains("hide")) {
            return;
        }
        this.SetDropdownPosition();
    }

    /**
     * Focus first focusable element in control area
     * */
    FocusControl() {
        Lagoon.JsUtils.getTabbableChild(this.control)?.focus();
    }

    /**
     * Dropdown closed actions
     * */
    OnClosed() {
        this.FocusControl();
    }

    /**
     * Rise blazor closing method
     * @param {boolean} disableFocus
     */
    CloseDropDown(disableFocus = false) {
        if (!this.menu.classList.contains("hide")) {
            this.dotNetRef.invokeMethodAsync("CloseDropdownAsync", disableFocus);
        }
    }

    /**
     * Scroll document event
     * @param {any} e
     */
    OnScroll(e) {
        // Do not catch event when scrolling into the dropdown list        
        if (!e.target.closest('div.dropdown-menu')) {
            this.CloseDropDown();
        }
    }

    /**
     * Navigator resize
     * @param {any} e
     */
    OnResize(e) {
        this.CloseDropDown();
    }

    /**
     * Keyboard managment
     * @param {any} e
     */
    OnKeyDown(e) {
        this.isOpening = false;
        if (this.menu.classList.contains("hide")
            && (e.key == "Enter" || e.key == " " || e.key == "ArrowUp" || e.key == "ArrowDown")) {
            this.isOpening = true;
            this.dotNetRef.invokeMethodAsync("ToggleListAsync");
        } else if (e.key == 'Escape' || e.key == "Tab") {
            e.stopPropagation();
            this.CloseDropDown();
        }
    }
    OnKeyUp(e) {
        if (!this.menu.classList.contains("hide") && !this.isOpening && e.key !== 'Escape') {
            // Keyup event is used to fix JS error with others JS events 
            //  when dropdown is hidden
            this.dotNetRef.invokeMethodAsync("KeyUpAsync", {
                key: e.key,
                code: e.code,
                location: e.location,
                repeat: e.repeat,
                ctrlKey: e.ctrlKey,
                shiftKey: e.shiftKey,
                altKey: e.altKey,
                metaKey: e.metaKey,
                type: e.type,
            });
        }
        this.isOpening = false;
    }

    /**
     * Check if element is visible in container
     * @param {any} element
     * @param {any} container
     */
    IsVisible(element, container) {
        const { bottom, height, top } = element.getBoundingClientRect();
        const containerRect = container.getBoundingClientRect();

        return top <= containerRect.top ? containerRect.top - top <= height : bottom - containerRect.bottom <= height;
    }

    /**
     * Add px unit to value
     * @param {any} value
     */
    AddPx(value) {
        if (typeof value == 'number' || value.slice(-2) !== 'px') {
            return value + 'px';
        }
        return value;
    }

    /**
     * Remove JS object
     * */
    Dispose() {
        if (this.intersectionObserver) {
            this.intersectionObserver.disconnect();
            this.intersectionObserver = null;
        }
        if (this.resizeObserver.obs1) {
            this.resizeObserver.obs1.disconnect();
            this.resizeObserver.obs1 = null;
        }
        if (this.resizeObserver.obs2) {
            this.resizeObserver.obs2.disconnect();
            this.resizeObserver.obs2 = null;
        }
        this.events.forEach((evt) => {
            evt.target.removeEventListener(evt.type, evt.fct, true);
        });
        this.events = [];
    }

    /**
    * Gets all elements over the tested element
    * @param {any} elt
    */
    ElementsOver(elt) {
        const boundingRect = elt.getBoundingClientRect();
        const left = boundingRect.left + 1;
        const right = boundingRect.right - 1;
        const top = boundingRect.top + 1;
        const overElts = [];

        for (let x = left; x <= right; x++) {
            const eltToPos = document.elementFromPoint(x, top);
            if (eltToPos) {
                const isExclude = eltToPos?.closest('.lgDropdown')
                    || eltToPos.classList.contains('tooltip')
                    || eltToPos.classList.contains('lblActive');
                if (!isExclude && eltToPos !== elt && !overElts.includes(eltToPos)) {
                    overElts.push(eltToPos);
                }
            }
        }
        if (overElts.length > 0) return overElts;
        return null;
    }

    //#endregion

}

/**
 * DropDown initialization method used by Blazor
 * @param {any} eltRef
 * @param {any} dotNetRef
 * @param {object} options
 */
Lagoon.LgDropDown = (eltRef, dotNetRef, options) => {
    const dropDown = eltRef.DropDown;
    if (!dropDown || dropDown.EltRef !== eltRef) {
        eltRef.DropDown = new DropDown(eltRef, dotNetRef, options);
    }
};

/**
 * Init
 * @param {any} eltRef
 */
Lagoon.LgDropDownInit = (eltRef) => {
    eltRef.DropDown?.Init();
};

/**
 * Dispose dropdown
 * @param {any} eltRef
 */
Lagoon.LgDropDownDispose = (eltRef) => {
    eltRef.DropDown?.Dispose();
};

/**
 * Open/close dropdown
 * @param {any} eltRef
 * @param {any} show
 */
Lagoon.LgDropDownShowChanged = (eltRef, show) => {
    if (show) {
        eltRef.DropDown?.OnOpened();
    } else {
        eltRef.DropDown?.OnClosed();
    }
};