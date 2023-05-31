class SummaryBox {

    //#region fields

    elementRef;

    //#endregion

    constructor(elementRef) {
        this.elementRef = elementRef;
    }

    /**
     * Resize textarea to fit content
     * */
    Open() {
        const textarea = this.elementRef.querySelector('textarea');
        this.OnInput();
        textarea.addEventListener('input', this.OnInput.bind(this));        
    }

    /**
     * Resize input following content
     * @param {any} e
     */
    OnInput(e) {
        const textarea = e?.target ?? this.elementRef.querySelector('textarea');        
        if (textarea) {
            textarea.style.height = 'auto';
            if (textarea.scrollHeight > 0) {
                const menu = this.elementRef.querySelector('div.dropdown-menu');
                let maxHeight = null;
                if (menu && menu.style.maxHeight) {
                    maxHeight = parseInt(menu.style.maxHeight.replace('px', ''));
                }
                textarea.style.height = (maxHeight ? Math.min(textarea.scrollHeight, maxHeight)
                    : textarea.scrollHeight) + 'px';
            }            
        }
    }

    /**
     * Initialize new summary box
     * @param {any} elementRef
     */
    static Init(elementRef) {
        if (!elementRef.summary) {
            elementRef.summary = new SummaryBox(elementRef);
        } else {
            setTimeout(() => {
                elementRef.summary.Open();
            }, 100);            
        }
    }

}

Lagoon.LgSummaryBox = SummaryBox.Init;