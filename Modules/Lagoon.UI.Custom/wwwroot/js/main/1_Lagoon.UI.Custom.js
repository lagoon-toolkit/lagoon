function ready(callback) {
    // in case the document is already rendered
    if (document.readyState != 'loading') callback();
    // modern browsers
    else if (document.addEventListener) document.addEventListener('DOMContentLoaded', callback);
    // IE <= 8
    else document.attachEvent('onreadystatechange', function () {
        if (document.readyState == 'complete') callback();
    });
}

ready(function () {
    var lblClass = "lblActive";
    $(document).on("focusin", "input, textarea", function () {
        var parentElt = $(this).parent();
        if (parentElt.closest('.suggest-box').length > 0) {
            // specific case for suggestbox
            var eltLabel = parentElt.closest('.suggest-box').find('label');
        } else {
            // other inputs
            var eltLabel = parentElt.next();
        }

        if (!eltLabel.hasClass(lblClass) && this.type != "checkbox" && this.type != "radio" && !$(this).parent().hasClass('.colorPickerBox')) {
            eltLabel.addClass(lblClass);
        }
    });

    $(document).on("focusout", "input, textarea", function () {
        var eltLabel = $(this).parent().next();
        if ($(this).val() == "" && eltLabel.hasClass(lblClass) && ($(this).attr('placeholder') == "" || $(this).attr('placeholder') == undefined)) {
            eltLabel.removeClass(lblClass);
        }
    });
});