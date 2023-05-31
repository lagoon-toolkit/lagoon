Lagoon.LgTabScroll = (function () {

    var throttleTimeout;
    var throttle = 25;

    function NavigationButtonsVisibility(eltRef) {
        if (throttleTimeout) clearTimeout(throttleTimeout);
        throttleTimeout = setTimeout((function () {
            NavigationButtonsVisibilityNow(eltRef);
        }), throttle);
    }

    /**
     * Manage dom element visibility for the tab list
    * @param {any} eltRef element reference
     * */
    function NavigationButtonsVisibilityNow(eltRef) {
        if ($(eltRef).children().length > 0) {
            var leftFirstElt = $(eltRef).children().first().offset().left;
            var lastElt = $(eltRef).children().last();
            var tabLeft = $(eltRef).offset().left;
            var rightLastElt = lastElt.offset().left + lastElt.outerWidth() - tabLeft;
            var tabContainerWidth = $(eltRef).width();
            if (leftFirstElt >= tabLeft && rightLastElt <= tabContainerWidth) {
                // All tabs are visible into the tabcontainerheader
                // Hide left and right navigation button
                $(eltRef).siblings(".scrollTabPrevious").css("display", "none");
                $(eltRef).siblings(".scrollTabNext").css("display", "none");
            } else {
                // Display navigation buttons to scroll tab list
                $(eltRef).siblings(".scrollTabPrevious").css("display", "");
                $(eltRef).siblings(".scrollTabNext").css("display", "");
            }
        } else {
            // Hide left and right navigation button
            $(eltRef).siblings(".scrollTabPrevious").css("display", "none");
            $(eltRef).siblings(".scrollTabNext").css("display", "none");
        }
    }

    return {
        init: function (eltRef) {
            var targetNode = $(eltRef)[0];
            var config = { attributes: true, childList: true };

            // Mutation Observer
            var mutationObsCallback = function (mutationsList) {
                for (var mutation of mutationsList) {
                    if (mutation.type == 'childList') {
                        NavigationButtonsVisibility($(eltRef));
                    }
                }
            };
            eltRef.observerMutation = new MutationObserver(mutationObsCallback);
            eltRef.observerMutation.observe(targetNode, config);

            // Resize Observer
            resizeObsCallback = function (entry) {
                NavigationButtonsVisibility($(eltRef));
            };
            eltRef.observerResize = new ResizeObserver(resizeObsCallback);
            eltRef.observerResize.observe(targetNode);
            // Disabled : Should not auto scroll to the tab header
            //zz this.scrollFollowActiveTab(eltRef);
        },
        /**
         * Scroll with right and left button
         * @param {any} eltRef element reference
         * @param {any} direction scroll direction (right or left)
         * */
        scroll: function (eltRef, direction) {
            if (direction == "right") {
                $(eltRef).animate({
                    scrollLeft: "+=200px"
                }, "slow");
            } else {
                $(eltRef).animate({
                    scrollLeft: "-=200px"
                }, "slow");
            }
        },
        /**
         * Follow the active tab (auto scroll)
         * @param {any} eltRef element reference
         * */
        scrollFollowActiveTab: function (eltRef) {
            var lastTab = $(eltRef).find("li.active");
            if (lastTab.length > 0) {
                lastTab[0].scrollIntoView({ behavior: 'smooth' });
            }
        },
        /**
         * Dispose events listener
         * Disconnect intersection observers
         * Disconnect resize observers
         * @param {any} eltRef element reference
         */
        dispose: function (eltRef) {
            if (eltRef.observerMutation) {
                eltRef.observerMutation.disconnect();
                eltRef.observerMutation = null;
            }
            if (eltRef.observerResize) {
                eltRef.observerResize.disconnect();
                eltRef.observerResize = null;
            }
        }
    }

})();