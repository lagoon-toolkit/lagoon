// LgTextBox namespace
Lagoon.LgTextBox = (function () {	
    return {
        InitInputMask: function (eltRef, dotnetRef, autoUnmask, clearIncomplete, email) {
            var searchTimer = null;
            if (eltRef.inputmask === undefined) {
                $(eltRef).inputmask({
                    autoUnmask: autoUnmask,
                    clearIncomplete: clearIncomplete,
                    onBeforePaste: function (valuePassed, opt) {
                        setTimeout(() => {
                            if ($(eltRef)[0].inputmask.isComplete()) {
                                dotnetRef.invokeMethodAsync("OnJsOnCompleteInputMaskAsync", $(eltRef).val());
                            }
                        }, 100);
                    },
                    oncomplete: function () {
                        // Clear timer
                        clearTimeout(searchTimer);
                        // Start a new timer
                        searchTimer = setTimeout(() => {
                            dotnetRef.invokeMethodAsync("OnJsOnCompleteInputMaskAsync", $(eltRef).val());
                        }, 500);
                    }
                }).on('change', function (e) {
                    var newVal = $(eltRef).val();
                    if (email) {
                        newVal = $.trim(newVal).replace(/^[\s_@.]+$/g, '');
                    }
                    dotnetRef.invokeMethodAsync("OnJsChangeAsync", newVal);
                });
            }
        },

        InitPrefixSuffixPadding: function (eltRef) {
            var parentNode = eltRef?.parentNode;
            if (parentNode) {
                var prefixElement = parentNode.getElementsByClassName('form-input-prefix')[0];
                var suffixElement = parentNode.getElementsByClassName('form-input-suffix')[0];
                var label = parentNode.parentNode.getElementsByClassName('lbl')[0];
                if (prefixElement) {
                    var leftPrefix = parseInt($(prefixElement).css('left'));
                    eltRef.style.setProperty('padding-left', (prefixElement.offsetWidth + leftPrefix * 2) + 'px');
                    // associated label
                    if (label && parseInt($(label).css('padding-left'))) {
                        label.style.setProperty('padding-left', (prefixElement.offsetWidth + leftPrefix * 2.2) + 'px');
                    }
                }
                else {
                    eltRef.style.removeProperty('padding-left');
                    label?.style.removeProperty('padding-left');
                }
                if (suffixElement) {
                    var rightSuffix = parseInt($(suffixElement).css('right'));
                    eltRef.style.setProperty('padding-right', (suffixElement.offsetWidth + rightSuffix * 2) + 'px');
                }
                else {
                    eltRef.style.removeProperty('padding-right');
                }
            }
        }
    }
})();