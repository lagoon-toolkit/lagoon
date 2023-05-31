Lagoon.HtmlEditor = {

    // Load HtmlEditor
    Load: function (editorObject, options) {
        if (editorObject == null) {
            // If the component is disposed before the first render
            // the editorObject will be null and we don't have to try to 
            // initialise the Jodit editor
            return null;
        }
        // Remove external plugins
        options.beautifyHTMLCDNUrlsJS = [];
        options.sourceEditorCDNUrlsJS = [];

        options.controls = {
            fontsize: {
                list: Jodit.atom(options.fontSizeList)
            },
            font: {
                list: Jodit.atom(options.fontFamilyList)
            }
        };

        if (options.buttons == null)
        {
            options.buttons = Jodit.defaultOptions.buttons;
        }
        // Create the html editor
        return Jodit.make(editorObject, options);
    },

    // Destroy editor
    Unload: function (joditRef) {
        try {
            joditRef.destruct();
        } catch (ex) {}
    },

    // Return editor content
    GetData: function (editorObject) {
        return editorObject.value;
    }
};