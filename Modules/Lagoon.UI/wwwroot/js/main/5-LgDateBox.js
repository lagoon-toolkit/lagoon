// LgDateox namespace
Lagoon.LgDateBox = (function () {
    
    return {
        /**
         * 
         * @param {any} eltRef
         * @param {any} hndEltRef
         * @param {any} dtbFormat
         * @param {any} toRefresh
         * @param {number} dateBoxKind enum dateBoxKind (Days(0), Month(1), Year(2), Week(3))
         */
        InitDatepicker: function (csRef, eltRef, dtbFormat, dtbFormatDisplay, toRefresh, value, startDate, endDate, dateBoxKind) {
            if (toRefresh) {
                // Destroy => forced by language changed
                $(eltRef).datepicker('destroy');
            }
            // Datapicker already initialized 
            if ($(eltRef).data().datepicker) {
                if (value != $(eltRef).data('lastValue')) {
                    if (dtbFormat != 'yyyy') {
                        $(eltRef).datepicker("setDate", this.strToDate(value));
                    } else {
                        $(eltRef).datepicker("setDate", value);
                    }
                }
                if (startDate) {
                    $(eltRef).datepicker("setStartDate", this.strToDate(startDate));
                }
                if (endDate) {
                    $(eltRef).datepicker("setEndDate", this.strToDate(endDate));
                }
                return;
            }

            const isWeekPicker = dateBoxKind === 3;

            // We keep the current selected value
            $(eltRef).data('lastValue', value);

            var daysShort = [];
            var daysMin = [];
            Lagoon.JsDicoManager.GetDico("DayNames").split(",").forEach(function (value) {
                daysShort.push(value.substr(0, 3));
                daysMin.push(value.substr(0, 2));
            });
            // Calendar translation
            $.fn.datepicker.dates['lg'] = {
                days: Lagoon.JsDicoManager.GetDico("DayNames").split(","),
                daysShort: daysShort,
                daysMin: daysMin,
                months: Lagoon.JsDicoManager.GetDico("MonthNames").split(","),
                monthsShort: Lagoon.JsDicoManager.GetDico("MonthNamesShort").split(","),
                today: "Today",
                clear: "Clear",
                weekStart: 1
            };

            const self = this;
            $(eltRef).datepicker({
                orientation: "auto",
                format: dtbFormatDisplay,
                assumeNearbyYear: true,
                zIndexOffset: 1071,
                autoclose: true,
                language: 'lg',
                forceParse: false,
                todayHighlight: true,
                weekModeFormat: 'ww/yyyy',
                weekMode: isWeekPicker,
                calendarWeeks: isWeekPicker,
                minViewMode: isWeekPicker ? 0 : dateBoxKind, // set week to days to select Monday
                updateViewDate: startDate || endDate ? false : true,
                beforeShowMonth: function (ev) {
                    const evDate = new Date(ev);
                    const currentDate = new Date();
                    if (dateBoxKind === 1 && evDate.getFullYear() === currentDate.getFullYear() && evDate.getMonth() === currentDate.getMonth()) {
                        return "current";
                    }
                    return "";
                },
                beforeShowYear: function (ev) {
                    if (dateBoxKind === 2 && new Date(ev).getFullYear() === new Date().getFullYear()) {
                        return "current";
                    }
                    return "";
                }
            }).on('show', function (ev) {
                if (isWeekPicker) {
                    var element = document.getElementsByClassName("datepicker");
                    $(element).addClass('week-picker');
                }
            }).on('changeDate', function (ev) {
                var newValue = $(eltRef).data('datepicker').getFormattedDate(dtbFormat);
                // Display the week number
                if (isWeekPicker) {
                    csRef.invokeMethodAsync("GetWeekNumberAsync", newValue)
                        .then(result => {
                            $(eltRef).val(result);
                        });
                }
                if (newValue != $(eltRef).data('lastValue')) {
                    $(eltRef).data('lastValue', newValue);
                    csRef.invokeMethodAsync("SetDateBoxValueAsync", newValue);
                }
            }).on('clearDate', function (ev) {
                // prevent onChange event
                if ($(eltRef).val() == "" && $(eltRef).data('lastValue') != "") {
                    $(eltRef).data('lastValue', null);
                    csRef.invokeMethodAsync("SetDateBoxValueAsync", "");
                }                
            }).on('keydown', function (e) {
                var dp = $(eltRef).data("datepicker");
                var dpIsVible = $(".datepicker").length;
                var keycode = e.keyCode ? e.keyCode : e.which;

                // Enter
                if (keycode == "13" && dpIsVible) {
                    var newValue = dp.focusDate ? dp.focusDate : self.strToDate(dp.getFormattedDate(dtbFormat));
                    $(eltRef).datepicker('setDate', newValue);
                    $(eltRef).datepicker('update'); // reset focus.
                    $(eltRef).datepicker('hide');
                    e.preventDefault();
                }

                // Arrow down
                if (keycode == "40" && !dpIsVible) {
                    $(eltRef).datepicker('show');
                }
            });

            if (startDate) {
                $(eltRef).datepicker("setStartDate", this.strToDate(startDate));
            }
            if (endDate) {
                $(eltRef).datepicker("setEndDate", this.strToDate(endDate));
            }
        },

        strToDate: function (str) {
            // We use the split to convert to local timezone and not utc, new Date(str) asume that str is an utc date
            var a = str.split("-");
            return a.length === 3 ? new Date(a[0], a[1] - 1, a[2]) : null;
        },

        dateToStr: function (date) {
            return date.toISOString().substring(0, 10);
        }
    }

})();