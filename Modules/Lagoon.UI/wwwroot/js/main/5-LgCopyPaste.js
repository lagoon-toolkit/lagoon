// LgCopyPaste namespace
Lagoon.LgCopyPaste = (function () {
    return {

        /**
         * Copy value in clipboard
         * @param {string} value
         */
        Copy(value) {
            navigator.clipboard.writeText(value).then(
                function () { },
                function (err) {
                    console.error('Copy: Could not copy text : ', err);
                }
            );
        },

        /**
         * Get value from clipboard
         */
        TryPaste(refDotNet) {
            try {
                navigator.clipboard.readText().then(
                    value => {
                        refDotNet.invokeMethodAsync("OnPasteAsync", value);
                    },
                    function (err) {
                        refDotNet.invokeMethodAsync("ShowPasteModal");
                    }
                );
            }
            catch (err) {
                refDotNet.invokeMethodAsync("ShowPasteModal");
            }
        },

        AddPasteHandler(modalId, refDotNet) {
            document.querySelector("#" + modalId).dotnetref = refDotNet;
        },

        /**
         * Get value from clipboard
         */
        HandlePaste(modal, e) {
            var clipboardData = e.clipboardData || window.clipboardData;
            pastedData = clipboardData.getData('Text');
            var refDotNet = modal.dotnetref;
            refDotNet.invokeMethodAsync("OnPasteAsync", pastedData);
        }

    }
})();