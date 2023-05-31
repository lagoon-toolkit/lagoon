// LgTimeBox namespace
Lagoon.LgTimeBox = (function () {
	
    return {
        // Init timer clock picker
        InitTimePicker: (objectRef, eltRef, txtOk, txtCancel) => {
            if (!eltRef.picker) {                               
                $(eltRef).clockTimePicker({
                    i18n: {
                        okButton: txtOk,
                        cancelButton: txtCancel
                    },
                    colors: {
                        buttonTextColor: Lagoon.LgTimeBox.GetTimeColors("--timebox-bt-color"),
                        clockFaceColor: Lagoon.LgTimeBox.GetTimeColors("--timebox-cf-color"),
                        clockInnerCircleTextColor: Lagoon.LgTimeBox.GetTimeColors("--timebox-cict-color"),
                        clockInnerCircleUnselectableTextColor: Lagoon.LgTimeBox.GetTimeColors("--timebox-cicut-color"),
                        clockOuterCircleTextColor: Lagoon.LgTimeBox.GetTimeColors("--timebox-coct-color"),
                        clockOuterCircleUnselectableTextColor: Lagoon.LgTimeBox.GetTimeColors("--timebox-cocut-color"),
                        hoverCircleColor: Lagoon.LgTimeBox.GetTimeColors("--timebox-hc-color"),
                        popupBackgroundColor: Lagoon.LgTimeBox.GetTimeColors("--timebox-pb-color"),
                        popupHeaderBackgroundColor: Lagoon.LgTimeBox.GetTimeColors("--timebox-phb-color"),
                        popupHeaderTextColor: Lagoon.LgTimeBox.GetTimeColors("--timebox-pht-color"),
                        selectorColor: Lagoon.LgTimeBox.GetTimeColors("--timebox-s-color"),
                        selectorNumberColor: Lagoon.LgTimeBox.GetTimeColors("--timebox-sn-color"),
                        signButtonColor: Lagoon.LgTimeBox.GetTimeColors("--timebox-sbu-color"),
                        signButtonBackgroundColor: Lagoon.LgTimeBox.GetTimeColors("--timebox-sba-color")
                    },
                    onChange: (newValue, oldValue) => {
                        objectRef.invokeMethodAsync("SetTimeBoxValueAsync", newValue);
                    }
                });                
                eltRef.picker = true;
            }
        },
        // Return CSS variable value
        GetTimeColors: (cssVar) => {
            const color = getComputedStyle(document.documentElement).getPropertyValue(cssVar);            
            return color;
        },
        // Init time input mask
        InitTimeMask: (objectRef, eltRef, ignoreSeconds) => {
            var completeTimer = null;
            if (eltRef.inputmask === undefined) {
                eltRef.yinit = true;
                // Time mask
                var completMask = 'HH:MM';
                if (!ignoreSeconds) completMask += ':ss';
                // Callback used to transmit event to c# side
                var doCallback = function () {
                    // Clear timer
                    clearTimeout(completeTimer);
                    // Start a new timer
                    completeTimer = setTimeout(() => {
                        objectRef.invokeMethodAsync("SetTimeBoxValueAsync", $(eltRef).val());
                    }, 500);
                };
                // Apply time mask on input control
                $(eltRef).inputmask({
                    alias: "datetime",
                    inputFormat: completMask,
                    clearIncomplete: true,
                    oncomplete: doCallback,
                    onincomplete: doCallback
                });
            }
        }
    }

})();