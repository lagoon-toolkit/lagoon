window.Lagoon = window.Lagoon || {};

class JsColumnOptions {

    //#region

    dragContainerPos;
    dragElement;
    modalBody;
    modalBodyRect;

    //#endregion

    /**
     * Constructor
     * @param {any} eltRef
     */
    constructor(eltRef, jsRef) {
        this.reference = eltRef;
        this.jsReference = jsRef;
        this.InitEvents();
    }
    /**
     * Init event listeners
     * */
    InitEvents() {
        if (!this.events) {
            this.events = new Lagoon.EventManager(this);
        } else {
            this.events.RemoveAll();
        }
        this.events.Add(this.reference, "keydown", this.KeydownEvent.bind(this), true);
        this.events.Add(this.reference, "mousedown", this.MouseDownEvent.bind(this));
    }
    /**
     * Manage keydown event
     * @param {any} event
     */
    KeydownEvent(event) {
        if (event.ctrlKey && (event.key == "ArrowDown" || event.key == "ArrowUp")) {
            event.preventDefault();
            event.stopPropagation();
            const container = event.target.closest("li[data-index]");
            if (container) {
                let index = container.dataset.index;
                this.jsReference.invokeMethodAsync("KeyPress", event.key, index);
            }
        }
    }

    /**
     * Drag start event
     * @param {any} event
     */
    MouseDownEvent(event) {
        const target = event.target.closest('li[data-draggable=true]');
        if (target && event.target.matches('svg.drag-icon')) {
            this.dragElement = {
                element: target,
                placeholder: null,
                height: 0,
                y: 0,
                toIndex: 0
            };
            // Calculate the mouse position
            this.modalBody = this.reference.closest('div.modal-body');
            this.modalBodyTop = this.modalBody.getBoundingClientRect().top;
            this.dragContainerPos = this.reference.getBoundingClientRect();
            const rect = this.dragElement.element.getBoundingClientRect();
            this.dragElement.height = rect.height;
            this.dragElement.toIndex = this.dragElement.element.dataset.index;
            this.dragElement.y = rect.top - this.modalBodyTop + this.modalBody.scrollTop;
            this.dragElement.height = this.dragElement.element.offsetHeight;
            this.dragElement.width = this.dragElement.element.querySelector('div')?.offsetWidth;
            // Lock cursor
            document.exitPointerLock = document.exitPointerLock ||
                document.mozExitPointerLock;
            document.exitPointerLock();
            this.modalBody.requestPointerLock =
                this.modalBody.requestPointerLock ||
                this.modalBody.mozRequestPointerLock;
            this.modalBody.requestPointerLock();
            if ("onpointerlockchange" in document) {
                this.events.Add(document, 'pointerlockchange', this.PointerLockChangeEvent.bind(this));
            } else if ("onmozpointerlockchange" in document) {
                this.events.Add(document, 'mozpointerlockchange', this.PointerLockChangeEvent.bind(this));
            }
            // Add         
            this.events.Add(document, "mouseup", this.MouseUpEvent.bind(this));
        }
    }

    /**
     * Pointer lock change event
     * @param {any} event
     */
    PointerLockChangeEvent(event) {
        this.events.RemoveByTargetType(document, 'mousemove');
        if (document.pointerLockElement === this.modalBody) {
            this.events.Add(document, "mousemove", this.MouseMoveEvent.bind(this));
        }
    }

    /**
     * Remove drag events, styles and validate change
     * @param {any} event
     */
    MouseUpEvent(event) {
        if (this.dragElement) {
            // Release cursor
            this.events.RemoveByTargetType(document, 'mouseup');
            this.events.RemoveByTargetType(document, 'mousemove');
            document.exitPointerLock = document.exitPointerLock ||
                document.mozExitPointerLock;
            document.exitPointerLock();
            this.events.RemoveByTargetType(document, 'pointerlockchange');
            // Validate move            
            this.dragElement.element.style.removeProperty('top');
            this.dragElement.element.style.removeProperty('left');
            this.dragElement.element.style.removeProperty('position');
            this.dragElement.element.style.removeProperty('z-index');
            this.dragElement.element.classList.remove('dragging');
            this.dragElement.placeholder?.remove();
            // Call blazor move method
            const fromIndex = this.dragElement.element.dataset.index;
            const toIndex = this.dragElement.toIndex;            
            this.jsReference.invokeMethodAsync('MoveColumnJS', fromIndex, toIndex);
            this.dragElement = null;
        }
    }

    /**
     * Mouse move event used in drag and drop
     * @param {any} event
     */
    MouseMoveEvent(event) {
        if (!this.dragElement.element) return false;
        // Set position for dragging element        
        this.dragElement.element.style.position = 'absolute';
        // Limit y axis top movement (-10 to move over the first element)
        this.dragElement.y = Math.max(-10, this.dragElement.y + event.movementY);
        this.dragElement.element.style.top = `${parseInt(this.dragElement.y)}px`;
        this.dragElement.element.style.zIndex = 100;
        this.dragElement.element.classList.add('dragging');
        // Set placeholder
        if (!this.dragElement.placeholder) {
            this.dragElement.placeholder = document.createElement('div');
            this.dragElement.placeholder.classList.add('placeholder');
            this.dragElement.placeholder.style.height = `${this.dragElement.height}px`;
            this.dragElement.placeholder.style.width = `${this.dragElement.width}px`;
            this.dragElement.element.parentNode.insertBefore(
                this.dragElement.placeholder,
                this.dragElement.element.nextSibling
            );
        }
        // Get previous and next column
        const prev = this.dragElement.element.previousElementSibling;
        const next = this.dragElement.placeholder.nextElementSibling;
        // Scroll follow
        Lagoon.JsUtils.ScrollFollowing(this.dragElement.element, this.modalBody, 50);
        // User moves item to the top
        if (prev && this.IsAbove(this.dragElement.element, prev)) {
            this.dragElement.toIndex = prev.dataset?.index;
            this.Swap(this.dragElement.placeholder, this.dragElement.element);
            this.Swap(this.dragElement.placeholder, prev);            
            return;
        }
        // User moves item to the bottom
        if (next && this.IsAbove(next, this.dragElement.element)) {
            this.dragElement.toIndex = next.dataset?.index;
            this.Swap(next, this.dragElement.placeholder);
            this.Swap(next, this.dragElement.element);            
        }
    }

    /**
     * Check if first element is over another
     * @param {any} nodeA
     * @param {any} nodeB
     */
    IsAbove(nodeA, nodeB) {
        const rectA = nodeA.getBoundingClientRect();
        const rectB = nodeB.getBoundingClientRect();

        return rectA.top + rectA.height / 2 < rectB.top + rectB.height / 2;
    }

    /**
     * Swap two html element
     * @param {any} nodeA
     * @param {any} nodeB
     */
    Swap(nodeA, nodeB) {
        const parentA = nodeA.parentNode;
        const siblingA = nodeA.nextSibling === nodeB ? nodeA : nodeA.nextSibling;
        nodeB.parentNode.insertBefore(nodeA, nodeB);
        parentA.insertBefore(nodeB, siblingA);
    }

    /**
     * Dispose class
     * */
    Dispose() {
        this.events.RemoveAll();
    }

    /**
     * Init new columns options 
     * @param {any} eltRef
     * @param {any} jsRef
     */
    static Init(eltRef, jsRef) {
        if (eltRef && eltRef.nodeName && !eltRef.columnOptions) {
            eltRef.columnOptions = new JsColumnOptions(eltRef, jsRef);
        }
    }

}

window.Lagoon.JsColumnOptions = JsColumnOptions;