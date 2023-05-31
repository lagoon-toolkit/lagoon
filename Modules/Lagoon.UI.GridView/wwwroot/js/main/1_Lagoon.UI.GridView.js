window.Lagoon = window.Lagoon || {};

/**
 * JS of the gridview
 **/
class LgGridView {

    constructor(id, container, dotNetObjectReference) {
        this.Id = id;
        this.Container = container;
        const frame = container.closest('.frameRoot.gridview');
        this.Frame = {
            self: frame,
            header: frame.querySelector('.frameHeader'),
            body: frame.querySelector('.frameContent')
        };
        this.ObjectReference = dotNetObjectReference;
        this.Body = this.Container.querySelector('div.gridview-body-container');
        this.HeaderContainer = this.Container.querySelector('div.gridview-header-scroll');
        this.Header = this.Container.querySelector('div.gridview-header');
        this.ColTotal = parseInt(container.getAttribute('aria-colcount'));
        this.RowTotal = container.querySelectorAll('div.gridview-row').length;
        this.InitEvents();
        this.InitObserver();
        this.LastFocus = null;
        this.CurrentFocus = null;
        this.GroupOffset = 0;
        this.PreviousFocus = null;
        this.Visible = this.Frame.body.classList.contains('show');
        this.MustResize = !this.Visible;
        this.debug = false;
        return this;
    }

    /**
     * Initialize mutation observer on grid
     * */
    InitObserver() {
        const self = this;
        const config = { childList: true, subtree: true };
        this.Observer = new MutationObserver((e) => {
            self.ColTotal = parseInt(self.Container.getAttribute('aria-colcount'));
            self.RowTotal = self.Container.querySelectorAll('div.gridview-row').length;
        });
        this.Observer.observe(self.Container, config);

        this.HeaderOffsetWidth = 0;
        this.ResizeObserver = new ResizeObserver(e => {
            if (self.HeaderOffsetWidth !== self.Header.offsetWidth) {
                self.HeaderOffsetWidth = self.Header.offsetWidth;
                self.ObjectReference.invokeMethodAsync('RefreshCssVariablesAsync');
            }
        });
        this.ResizeObserver.observe(this.Header);
        // Detect browser resize to resize gridview columns
        let endResizeTimer;
        this.ResizeBrowserObserver = new ResizeObserver(e => {
            if (self.Visible) {
                clearTimeout(endResizeTimer);
                endResizeTimer = setTimeout(() => {
                    self.ObjectReference.invokeMethodAsync('JsResizeColumnsAsync');
                }, 200);
            } else {
                // Detect if resize is made on hidden gridview
                self.MustResize = true;
            }
        });
        this.ResizeBrowserObserver.observe(document.body);
        // Detect gridview visibility to resize gridview columns
        this.IntersectionObserver = new IntersectionObserver(e => {
            const isIntersecting = e.length > 0 ? e[0].isIntersecting : false;
            // Detect if resize not apply on gridview hidden
            if (!self.Visible && isIntersecting && self.MustResize) {
                self.MustResize = false;
                self.ObjectReference.invokeMethodAsync('JsResizeColumnsAsync');
            }
            self.Visible = isIntersecting;
        }, {
            root: document,
            threshold: 1.0
        });
        this.IntersectionObserver.observe(self.Container);
    }

    /**
     * Initialize Gridview events
     **/
    InitEvents() {
        this.Events = [];
        // Synchronize header and body horizontal scroll
        this.AddEvent({
            target: this.Body,
            type: 'scroll',
            callback: this.ScrollBody.bind(this),
            capture: false
        });
        this.AddEvent({
            target: this.HeaderContainer,
            type: 'scroll',
            callback: this.ScrollHeader.bind(this),
            capture: false
        });
        // Column resizer
        this.InitResizer();

        // Stop propagation of cell click event in edit
        this.ClickEventPropagation();

        // Manage key in body
        this.AddEvent({
            target: this.Frame.body,
            type: 'keydown',
            callback: this.BodyKeysManager.bind(this),
            capture: true
        });
    }

    /**
     * Add events and keep reference
     * @param {any} params     
     */
    AddEvent(params) {
        this.Events.push({
            target: params.target,
            type: params.type,
            callback: params.callback,
            capture: params.capture
        });
        params.target.addEventListener(params.type, params.callback, params.capture);
    }

    /**
     * Dispose events and observer
     * */
    Dispose() {
        // Remove observer
        this.Observer?.disconnect();
        this.Observer = null;
        this.ResizeObserver?.disconnect();
        this.ResizeObserver = null;
        this.ResizeBrowserObserver?.disconnect();
        this.ResizeBrowserObserver = null;
        this.IntersectionObserver?.disconnect();
        this.IntersectionObserver = null;
        // Remove events        
        this.Events?.forEach((params) => {
            params.target?.removeEventListener(params.type, params.callback, params.capture);
        });
    }

    /**
     * Scroll header synchronization
     * @param {any} e
     */
    ScrollHeader(e) {
        this.Body.scrollLeft = e.target.scrollLeft;
        // Calculation row
        const calc = this.Container.querySelector('div.gridview-calc-scroll');
        if (calc) calc.scrollLeft = e.target.scrollLeft;
    }

    /**
     * Scroll body synchronization
     * @param {any} e
     */
    ScrollBody(e) {
        // Header
        this.HeaderContainer.scrollLeft = e.target.scrollLeft;
        // Calculation Row
        const calc = this.Container.querySelector('div.gridview-calc-scroll');
        if (calc) calc.scrollLeft = e.target.scrollLeft;
    }

    /**
     * Manage key down and up events inside gridview
     * @param {any} e
     */
    GridViewKeysManager(e) {
        if (e.ctrlKey && e.altKey) {
            //TODO add shortcut
            this.GroupOffset = this.Container?.dataset.grouplvl ?
                parseInt(this.Container?.dataset.grouplvl) : 0;
            switch (e.key) {
                case 'b': // Select first body cell
                    this.CurrentFocus = this.CellMove(0, 0, 0 + this.GroupOffset, 0);
                    break;
            }
            if (this.CurrentFocus) {
                this.CurrentFocus.focus();
                e.preventDefault();
            }
        }
    }

    /**
     * Manage key between cells movement in body
     * @param {any} e
     */
    BodyKeysManager(e) {
        const cell = e.target.closest('.gridview-cell');
        const isActive = this.IsActiveElement(e.target);
        const hasShift = e.shiftKey;
        const hasCtrl = e.ctrlKey;
        const hasAlt = e.altKey;
        const isEditRow = e.target.closest('.gridview-row.gridview-row-edit,.gridview-row.gridview-row-add');
        const inDropDown = e.target?.closest('div.dropdown-menu');
        const isPageSizeCbo = e.target?.closest('div.gv-paging-cbo');
        let prevent = false;
        let moveIndex = null;
        let focusedInput = 1;
        let siblingElement = null;
        let lostFocus = false;
        this.CurrentFocus = null;

        this.GroupOffset = this.Container?.dataset.grouplvl ?
            parseInt(this.Container?.dataset.grouplvl) : 0;
        // authorize NVDA shortcut. Check if keydown come from dropdown or pager   
        if ((hasAlt && hasCtrl) || inDropDown || isPageSizeCbo) {
            return true;
        }
        if (cell) {
            const cellInfo = this.GetPosition(cell);
            switch (e.key) {
                case 'Tab':
                    moveIndex = hasShift ? -1 : 1;
                    focusedInput = moveIndex;
                    if (!this.IsCell(e.target)) {
                        siblingElement = this.GetSiblingInput(cell, e.target, moveIndex);
                    }
                    if (!siblingElement) {
                        this.CurrentFocus = this.CellMove(cellInfo.row, 0, cellInfo.col,
                            moveIndex, true, isEditRow ? 0 : moveIndex);
                    } else {
                        this.CurrentFocus = siblingElement;
                    }
                    prevent = (this.CurrentFocus != null || isEditRow);
                    lostFocus = isActive;
                    break;
                case 'ArrowUp':
                    moveIndex = -1;
                case 'ArrowDown':
                    // Not up or down mode in edit row
                    if (!isEditRow && (!isActive || hasShift)) {
                        moveIndex = moveIndex ? moveIndex : 1;
                        this.CurrentFocus = this.CellMove(cellInfo.row, moveIndex, cellInfo.col,
                            0, false, moveIndex);
                        prevent = true;
                    }
                    lostFocus = isActive && hasShift;
                    break;
                case 'ArrowLeft':
                    moveIndex = -1;
                    focusedInput = -1;
                case 'ArrowRight':
                    if (!isActive || hasShift) {
                        moveIndex = moveIndex ? moveIndex : 1;
                        // Select the sibling active element in cell                                              
                        if (!this.IsCell(e.target)) {
                            siblingElement = this.GetSiblingInput(cell, e.target, moveIndex);
                        }
                        // Move cell if there is no active element                        
                        if (!siblingElement) {
                            this.CurrentFocus = this.CellMove(cellInfo.row, 0, cellInfo.col
                                , moveIndex, false, moveIndex, false, true);
                        } else {
                            this.CurrentFocus = siblingElement;
                        }
                        prevent = true;
                    }
                    lostFocus = isActive && hasShift;
                    break;
                case 'Home':
                    if (!isEditRow && (!isActive || hasShift)) {
                        this.CurrentFocus = this.CellMove(0, 0, 0 + this.GroupOffset, 0);
                        prevent = true;
                    }
                    break;
                case 'End':
                    if (!isEditRow && (!isActive || hasShift)) {
                        this.CurrentFocus = this.CellMove(this.RowTotal - 1, 0,
                            this.ColTotal - 1 + this.GroupOffset, 0, false, -1);
                        prevent = true;
                    }
                    break;
                case 'Enter':
                case ' ':
                    // Sort on header                    
                    if (e.target?.classList.contains('gridview-sort') ||
                        e.target?.classList.contains('gridcell-group-title')) {
                        e.target.click();
                    }
                    break;
            }
        } else {
            // Disable TAB move and select first cell
            if (e.key == 'Tab' && !hasShift) {
                this.CurrentFocus = this.CellMove(0, 0, 0 + this.GroupOffset, 0);
                prevent = true;
            }
        }
        if (this.debug) console.log('XXX CELL', this.CurrentFocus, cell);
        if (this.CurrentFocus) {
            if (this.IsCell(this.CurrentFocus)) {
                const cellInput = this.GetCellInput(this.CurrentFocus, focusedInput == 1, focusedInput == -1);
                if (this.debug) console.log('XXX CELLINPUT', cellInput);
                if (cellInput) {
                    this.CurrentFocus = cellInput;
                }
            }
            if (Lagoon.JsUtils.ElementIsBelow(this.CurrentFocus)) {
                this.Body.scrollLeft = 0;
            }
            this.CurrentFocus.focus();
        }
        if (prevent) {
            e.preventDefault();
            if (!lostFocus) {
                e.stopPropagation();
            } else {
                // Special case for datepicker                
                const $target = $(e.target);
                if ($target.hasClass('dtb') && $target.datepicker) {
                    $target.datepicker('hide');
                }
            }
        }
        return true;
    }

    /**
     * Indicate if element is gridview cell
     * @param {any} element
     */
    IsCell(element) {
        if (!element) return true;
        return element.classList.contains('gridview-cell')
    }

    /**
     * Get row and column index
     * @param {any} cell gridview cell     
     */
    GetPosition(cell) {
        const parentRow = cell.closest('div.gridview-row');
        const rows = this.Container.querySelectorAll('div.gridview-row');
        const rowIndex = Array.prototype.indexOf.call(rows, parentRow);

        // Get order from style
        let gridColumn = window.getComputedStyle(cell, null)?.gridColumnStart;
        let colIndex = 0;
        if (!gridColumn || isNaN(gridColumn)) {
            colIndex = Array.prototype.indexOf.call(parentRow.querySelectorAll('div.gridview-cell[tabindex]'), cell);
        } else {
            colIndex = parseInt(gridColumn) - 1;
            colIndex = colIndex;
        }
        return {
            row: rowIndex > -1 ? parseInt(rowIndex) : 0,
            col: colIndex
        };
    }

    /**
     * Return active input in cell
     * @param {any} cell cell to check
     * @param {bool} first return only the first
     * @param {bool} last return only the last
     */
    GetCellInput(cell, first = true, last = false) {
        const inputsFinded = cell.querySelectorAll(`
            input:not([disabled="disabled"]):not(:disabled),
            button:not([disabled="disabled"]):not(:disabled),
            textarea:not([disabled="disabled"]):not(:disabled),
            select:not([disabled="disabled"]):not(:disabled),
            a:not([disabled="disabled"]):not(:disabled),
            div[tabindex]`);
        if (inputsFinded.length > 0) {
            let inputs = [];
            // Remove hidden            
            inputsFinded.forEach(elt => {
                // TODO find solution without JQuery                
                if ($(elt).is(':visible') && elt.tabIndex != -1) {
                    inputs.push(elt);
                }
            });
            if (first) {
                return inputs[0];
            }
            if (last) {
                return inputs[inputs.length - 1];
            }
            return inputs;
        }
        return null;
    }

    /**
     * Indicate if the element is interactive
     * @param {any} element element to test
     */
    IsActiveElement(element) {
        const tags = ['INPUT', 'TEXTAREA', 'SELECT'];
        const tagName = element.tagName;
        const isDisabled = element.hasAttribute('disabled');
        const isRadioCheck = element.getAttribute('type') == 'checkbox'
            || element.getAttribute('type') == 'radio';
        // Menu and menu item
        const openDropDown = (element.dataset.toggle && element.getAttribute('aria-expanded') == 'true'
            || element.closest('.dropdown-menu'));
        return !isDisabled && !isRadioCheck && (openDropDown || tags.indexOf(tagName) > -1);
    }

    /**
     * Return previous or next input
     * @param {any} cell cell parent
     * @param {any} current current input
     * @param {any} position position of the sibling
     */
    GetSiblingInput(cell, current, position) {
        const inputs = this.GetCellInput(cell, false);
        let siblingIndex = 0;
        if (!inputs || inputs.length == 0) return null;
        if (current) {
            siblingIndex = Array.prototype.indexOf.call(inputs, current);
            if (siblingIndex + position < inputs.length) {
                siblingIndex += position;
            } else {
                return null;
            }
        }
        return inputs[siblingIndex];
    }

    /**
     * Move cell focus
     * @param {int} row current row index
     * @param {int} rowMove row move value
     * @param {int} col current column index
     * @param {int} colMove column move value
     * @param {bool} tabMode tabulation move mode
     * @param {int} nextCell force to get next cell     
     * @param {bool} limitRow indicate if move is limited to row
     */
    CellMove(row, rowMove, col, colMove, tabMode = false, nextCell = 1, limitRow = false) {
        let realCol = col + colMove;
        let realRow = row + rowMove;

        if (tabMode) {
            if (realRow < 0 || realRow > this.RowTotal - 1) {
                return null;
            }
        } else {
            // Check min limits
            realCol = Math.max(realCol, this.GroupOffset);
            realRow = Math.max(realRow, 0);
            // Check max limits
            realCol = Math.min(realCol, this.ColTotal - 1 + this.GroupOffset);
            realRow = Math.min(realRow, this.RowTotal - 1);
        }
        const finalCell = this.GetCell(realRow, realCol, nextCell, limitRow);
        if (finalCell) {
            return finalCell;
        }
        return null;
    }

    /**
     * Get cell by row and column index
     * @param {any} row row index - base 1
     * @param {any} col column index
     * @param {bool} limitRow indicate if move is limited to row
     * @param {int} nextCell            
     */
    GetCell(row, col, nextCell = 1, limitRow = false) {
        const rows = this.Container.querySelectorAll('div.gridview-row');
        const currentRow = rows[row];
        let currentCol = col;
        let cell = null;
        const isHeaderGroup = currentRow.classList.contains('gridview-header-group');

        // Jump header group row
        if (isHeaderGroup) {
            row = nextCell > 0 ? row + 1 : row - 1
        }
        if (row < 0 || row >= rows.length) {
            return null;
        }
        cell = this.GetCellByOrder(rows[row], currentCol);
        if (cell) {
            return cell;
        }
        if ((row || row === 0) && nextCell != 0) {
            // Get next or previous cell         
            const maxCols = this.ColTotal - 1 + this.GroupOffset;
            const cells = currentRow.querySelectorAll('div.gridview-cell');
            const isFooter = currentRow.classList.contains('gridview-footer');

            if (!limitRow && (currentCol < 0 || currentCol > maxCols)) {
                //Row change                                 
                const newRow = nextCell > 0 ? row + 1 : row - 1
                const newCol = nextCell > 0 ? 0 : maxCols;
                return this.GetCell(newRow, newCol, nextCell, limitRow);
            } else {
                // Footer row case                
                if (isFooter && (currentCol >= 0 && currentCol < cells.length)) {
                    return cells[currentCol];
                }
                // Next or previous cell                
                cell = this.GetCell(row, nextCell > 0 ? currentCol + 1 : currentCol - 1, nextCell, limitRow);
                if (cell) {
                    return cell;
                }
            }
            return null;
        }
        return null;
    }

    /**
     * Indicate if element is displayed
     * @param {any} elt element to check
     */
    IsVisible(elt) {
        if (!elt) return false;
        if (elt.classList.contains('sr-only')) return false;
        // cell with pointer event disabled not visible
        const pointerEventNone = window.getComputedStyle(elt, null)?.pointerEvents == "none";
        if (pointerEventNone) return false;
        return $(elt).is(':visible');
        //const eltStyle = window.getComputedStyle(elt, null);        
        //return eltStyle?.display != 'none' && eltStyle?.visibility != 'hidden';
    }

    /**
     * Get cell following order
     * @param {any} row current row
     * @param {any} order order to find
     */
    GetCellByOrder(row, order) {
        const cells = row.querySelectorAll('div.gridview-cell');
        let currentOrder = order + 1;
        for (var i = 0; i < cells.length; i++) {
            const gridColumn = window.getComputedStyle(cells[i], null)?.gridColumnStart;
            if (this.IsVisible(cells[i]) && gridColumn && parseInt(gridColumn) == currentOrder) {
                return cells[i];
            }
        }
        return null;
    }

    /**
     * Initialize resize column event
     * */
    InitResizer() {
        const self = this;
        let updateFunc
        let currentWidth = 0;
        let minWidth = 0;
        let maxWidth = 0;

        this.AddEvent({
            target: document,
            type: 'pointerlockchange',
            callback: e => {
                const pointerTarget = document.pointerLockElement;
                try {
                    updateFunc = self.Throttle((e) => {
                        let gridView = e.target.closest('div.gridview-container');
                        if ('gridview-' + self.Id == gridView.id) {
                            currentWidth = self.UpdateColumnsWidth(e, currentWidth, minWidth, maxWidth);
                        }
                    }, 20);
                    pointerTarget.removeEventListener("mousemove", updateFunc, false);
                    if (pointerTarget && pointerTarget.classList.contains('gridview-resizer')) {
                        pointerTarget.addEventListener("mousemove", updateFunc, false);
                    }
                } catch (e) { }
            },
            capture: false
        });
        // Stop click event on header to avoid sort
        this.AddEvent({
            target: this.HeaderContainer,
            type: 'click',
            callback: (e) => {
                if (e.target.classList.contains('gridview-resizer')) {
                    e.preventDefault();
                    e.stopPropagation();
                }
            },
            capture: false
        });
        // Call resizing
        this.AddEvent({
            target: this.HeaderContainer,
            type: 'mousedown',
            callback: (e) => {
                if (e.target.classList.contains('gridview-resizer')) {
                    e.preventDefault();
                    e.stopPropagation();
                    const resizer = e.target;
                    const cell = resizer.closest('div.gridview-cell');
                    currentWidth = cell.offsetWidth
                    minWidth = resizer.dataset.minWidth;
                    maxWidth = resizer.dataset.maxWidth;
                    resizer.requestPointerLock();
                }
            },
            capture: false
        });
        // Free resize pointer
        this.AddEvent({
            target: this.HeaderContainer,
            type: 'mouseup',
            callback: e => {
                if (e.target.classList.contains('gridview-resizer')) {
                    e.preventDefault();
                    e.stopPropagation();
                    const resizer = e.target;
                    document.exitPointerLock();
                    resizer.removeEventListener("mousemove", updateFunc, false);
                    currentWidth = 0;
                    self.ObjectReference.invokeMethodAsync('UpdateCurrentProfileJsAsync');
                }
            },
            capture: false
        });
    }

    /**
     * Stop propagation of cell click event in edit
     * */
    ClickEventPropagation() {
        this.ClickPropagation = e => {
            const editCell = e.target.closest('div.gridview-cell-edit');
            const editRow = e.target.closest('div.gridview-row-edit');
            const isActiveElt = e.target.closest('button') != null
                || e.target.closest('a') != null
                || e.target.closest('.dropdown-menu:not(.select-dropdown-menu)')
                || e.target.closest('.summarybox');
            if ((editCell || editRow) && !isActiveElt) {
                e.stopPropagation();
            }
        };
        this.AddEvent({
            target: this.Container,
            type: 'click',
            callback: this.ClickPropagation.bind(this),
            capture: false
        });
    }

    /**
     * Call blazor update column width method
     * @param {any} e event
     * @param {any} width new column width
     * @param {any} minWidth minimal column width
     * @param {any} maxWidth maximal column width
     */
    UpdateColumnsWidth(e, width, minWidth, maxWidth) {
        const self = this;
        const key = e.target.dataset.key;
        width += e.movementX;

        width = Math.max(width, minWidth);
        if (maxWidth > 0 && maxWidth >= minWidth) {
            width = Math.min(width, maxWidth);
        }
        self.ObjectReference.invokeMethodAsync('UpdateColumnWidthAsync', key, width);
        return width;
    }

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
    }

    /**
     * Set focus on last focused element
     **/
    SetLastFocus() {
        if (this.LastFocus) {
            setTimeout((e) => {
                const cell = this.GetCell(this.LastFocus.row, this.LastFocus.col);
                cell?.focus();
            }, 50);
        }
    }

    /**
     * Focus element in the gridview by selector
     * @param {string} selector
     */
    SetFocus(selector) {
        const element = this.Container.querySelector(selector);
        const input = element.querySelector('div.dropdown.cbo,input,textarea,select,button,div.summarybox[tabindex="0"]');
        setTimeout(() => {
            (input || element)?.focus();
            if (input && input.select) {
                input.select();
            }
        }, 0);
    }

    /**
        * Add new GridView
        * @param {string} id
        */
    static AddGridView = (id, dotNetObjectReference) => {
        try {
            const container = document.querySelector('#gridview-' + id);
            const datasetGrid = container.dataset.grid;
            if (!datasetGrid) {
                $(container).data('grid', new Lagoon.GridView(id, container, dotNetObjectReference));
            }
            return true;
        } catch (error) {
            console.error(error);
        }
        return false;
    }

    /**
     * Call JS method
     * @param {any} id
     * @param {any} methodName
     * @param {any} data
     */
    static Action = (id, methodName, data) => {
        try {
            const container = $('#gridview-' + id);
            const gridview = container.data('grid');
            if (gridview && methodName) {
                return gridview[methodName](data);
            }
            return true;
        } catch (error) {
            console.error(error);
        }
        return false;
    }

    /**
     * Focus in row management
     * @param {any} rowRef
     * @param {any} index
     * @param {any} editMode
     */
    static SetFocus = (rowRef, index, editMode) => {
        const cell = rowRef.querySelector('.gridview-cell[data-col="' + index + '"]');
        const input = cell.querySelector('input,textarea,select,button:not(.reset-btn),div.summarybox[tabindex="0"]');
        let isDate = false;

        if (input) {
            // Timeout for LgDatebox to display calendar
            setTimeout(() => {
                input.focus();
                // Select value in cell edit mode
                if (editMode == 1 && input.select) {
                    input.select();
                }
            }, 40);
            // Add lost focus to save data        
            isDate = input.classList.contains('dtb');
            if (isDate) {
                // Check open and close date picker calendar                
                $(input).on('show', (e) => {
                    if (e.target) {
                        e.target.dataset.open = true;
                    }
                });
                $(input).on('hide', (e) => {
                    // Delay to not focusout before change date value
                    // Set to 100ms to have time to rise reset button event
                    setTimeout(() => {
                        const event = new Event('focusout', {
                            bubbles: true
                        });
                        const target = e.target;
                        if (e.target) {
                            target.dataset.open = false;
                            target.dispatchEvent(event);
                        }
                    }, 100);
                });
            }
        }
        if (!rowRef.hasFocus) {
            // Used to detect save in edit inline mode
            rowRef.addEventListener('focusout', (e) => {
                let leavingParent = null;
                let stopEvent = false;
                let cellRef = null;
                // Get parent cell
                if (e.target?.classList.contains('gridview-cell')) {
                    cellRef = e.target;
                } else {
                    cellRef = e.target?.closest('.gridview-cell');
                }
                // Leaving cell if focused target (relatedTarget) is out of cell or is focused target is null
                // relatedTarget = null is to fix the case of closing modal of the SummaryBoxCell                
                leavingParent = !cellRef.contains(e.relatedTarget);
                // Special case for datepicker, select and summarybox                
                if (e.target) {
                    stopEvent = e.target.classList.contains('dropdown-toggle') ||
                        e.target.closest('.summarybox.summarybox-open') != null ||
                        (e.target.classList.contains('dtb') && e.target.dataset.open == 'true');
                }
                if (!leavingParent || stopEvent) {
                    e.preventDefault();
                    e.stopPropagation();
                }
            }, true);
            rowRef.hasFocus = true;
        }
        return true;
    }

    /**
     * Add sticky emulation for Firefox only
     * @param {string} gridId
     * @param {boolean} bottom
     */
    static Sticky = (gridId, bottom) => {
        const isFirefox = /firefox/i.test(navigator.userAgent);
        if (isFirefox) {
            const grid = document.querySelector('#gridview-' + gridId);
            const element = grid?.querySelector('div.gridview-row-add');
            if (element && !element.sticky) {
                grid.sticky = new StickyReplace(element, bottom);
            }
        }
        return true;
    }

    /**
    * Remove sticky JS
    * @param {string} gridId
    */
    static StickyDispose = (gridId) => {
        const isFirefox = /firefox/i.test(navigator.userAgent);
        if (isFirefox) {
            const grid = document.querySelector('#gridview-' + gridId);
            if (grid?.sticky) {
                grid.sticky.Dispose();
                grid.sticky = null;
            }
        }
        return true;
    }

    /**
    * Get body and scrollbar width
    * @param {any} elementId
    */
    static GetTotalWidth(elementId) {
        const bodyWidth = $(`.gridview-${elementId} .gridview-body-container`).width();
        const headerScroll = document.querySelector(`.gridview-${elementId} .gridview-header-scroll`);
        const headerStyle = headerScroll ? window.getComputedStyle(headerScroll) : false;
        const scrollbarWidth = headerStyle ? headerStyle.paddingRight?.replace('px', '') : 0;
        const body = bodyWidth ? parseInt(bodyWidth) : 0;
        const scrollbar = scrollbarWidth ? parseInt(scrollbarWidth) : 0;
        return body - scrollbar;
    }
}
window.Lagoon.GridView = LgGridView;
