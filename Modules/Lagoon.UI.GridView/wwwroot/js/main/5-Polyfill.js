/**
 * Replace css position fixed
 * Used to fix problem with Firefox
 * */
class StickyReplace {

    element;
    parent;
    elementHeight;
    parentMaxHeight;
    bottom;

    constructor(element, bottom) {
        this.element = element;
        if (this.element) {            
            this.element.style.setProperty('position', 'absolute', 'important');            
            this.parent = this.element.parentElement;
            this.bottom = bottom;
            if (this.parent) {
                if (this.bottom) {
                    this.element.style.bottom = - this.parent.scrollTop + 'px';
                    // Padding to not hide last line
                    this.parent.style.paddingBottom = this.element.offsetHeight + 'px';
                    this.Resize();                    
                    this.parent.addEventListener('resize', this.Resize.bind(this));
                } else {
                    this.element.style.top = this.parent.scrollTop + 'px';
                    // Padding to not hide first line
                    this.parent.style.paddingTop = this.element.offsetHeight + 'px';
                }
                this.parent.addEventListener('scroll', this.Scroll.bind(this));
            }            
        }
        return this;
    }

    /**
     * Calculate height element
     * */
    Resize() {
        this.elementHeight = this.element.offsetHeight;
        this.parentMaxHeight = this.parent.scrollHeight - this.parent.offsetHeight;        
    }

    /**
     * Scroll event
     * @param {any} e
     */
    Scroll(e) {
        if (this.bottom) {
            this.element.style.bottom = - Math.min(this.parent.scrollTop, this.parentMaxHeight) + 'px';
        } else {
            this.element.style.top = this.parent.scrollTop + 'px';
        }        
    }

    /**
     * Dispose events
     * */
    Dispose() {                
        this.parent.style.paddingBottom = '0';
        this.parent.style.paddingTop = '0';
        this.parent.removeEventListener('scroll', this.Scroll.bind(this));
        this.parent.removeEventListener('resize', this.Resize.bind(this));
    }

}