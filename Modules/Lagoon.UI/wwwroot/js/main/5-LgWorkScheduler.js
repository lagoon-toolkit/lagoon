// LgWorkScheduler namespace used by LgWorkScheduler component
Lagoon.LgWorkScheduler = (function () {

    const _class_wst = 'wst';
    const _class_wk_row = 'wk-row';
    const _class_wk_row_highlight = 'wk-row-highlight';
    const _class_wst_group = 'wst-group';
    const _class_wst_group_first = 'wst-first';
    const _class_wst_group_second = 'wst-second';
    const _class_wst_today = 'wst-today';

    /**
     * Scoll synchro between elementReference and targetHorizontal + targerVertical
     *
     * @param {DotNetObjectReference} dotnetRef Used to invoke c# methods
     * @param {bool} virtualScrolling True if virtual scrolling, false otherwise
     * @param {HtmlDivElement} schedulerReference Element from wich scroll position will be retrieved
     * @param {HtmlDivElement} timelineReference Apply scrollLeft from schedulerReference to this div
     * @param {HtmlDivElement} projectListReference Apply scrollTop from schedulerReference to this div
     */
    var _scrollSynchro = function (dotnetRef, virtualScrolling, schedulerReference, timelineReference, projectListReference, mainContainer) {
        var lastYPosition;
        var onScrollSynchro = function () {
            var scrollPositionX = schedulerReference.scrollLeft;
            var scrollPositionY = schedulerReference.scrollTop;
            timelineReference.scrollLeft = scrollPositionX;
            projectListReference.scrollTop = scrollPositionY;
            var mainContainerHeight = $(mainContainer).height();
            if (virtualScrolling && lastYPosition != parseInt(scrollPositionY)) {
                lastYPosition = parseInt(scrollPositionY);
                dotnetRef.invokeMethod('OnScrollChanged', parseInt(scrollPositionY), parseInt(mainContainerHeight));
            }
            schedulerReference.dispatchEvent(new Event("mousemove"));
        };
        schedulerReference.addEventListener('scroll', onScrollSynchro, { passive: false });
        return onScrollSynchro;
    };

    /**
     * Center inside their parents all elements with '_class_wst_group' css class.
     * Used to maintain visibility of first timeline row when an horizontal scroll is applied
     * 
     * @param {HtmlDivElement} schedulerContainer Area containing scroll
     * @param {HtmlDivElement} timelineContainer Element in which we have to look for 
     */
    var _scrollTimeline = function (schedulerContainer, timelineContainer) {
        var onScrollTimeline = function () {
            var scrollPositionX = schedulerContainer.scrollLeft;
            var textWidth = 50;
            var visibleWidth = schedulerContainer.offsetWidth;
            var totalWidth = 0;
            var timeHeaderGroups = timelineContainer.getElementsByClassName(_class_wst_group);
            var currentLevel = _class_wst_group_first;
            for (const timeHeader of timeHeaderGroups) {
                timeHeader.style.paddingLeft = '0px';
                textWidth = timeHeader.children[0].clientWidth / 2;

                if (!timeHeader.classList.contains(currentLevel) && timeHeader.classList.contains(_class_wst_group_second)) {
                    currentLevel = _class_wst_group_second;
                    totalWidth = 0;
                }

                if (scrollPositionX <= totalWidth && totalWidth + timeHeader.offsetWidth <= scrollPositionX + visibleWidth) {
                    // Start & End visible
                    timeHeader.style.paddingLeft = (timeHeader.offsetWidth / 2) - textWidth + 'px';
                }
                else if (scrollPositionX <= totalWidth && totalWidth + timeHeader.offsetWidth > scrollPositionX + visibleWidth) {
                    // Only Start visible
                    timeHeader.style.paddingLeft = (0) + 'px';

                    var visible = scrollPositionX + visibleWidth - totalWidth;
                    if (visible > 0) {
                        visible = (visible / 2) - textWidth;
                        timeHeader.style.paddingLeft = visible + 'px';
                    }
                }
                else if (scrollPositionX > totalWidth && totalWidth + timeHeader.offsetWidth <= scrollPositionX + visibleWidth && totalWidth + timeHeader.offsetWidth > scrollPositionX) {
                    // Only End visible
                    var hidden = scrollPositionX - totalWidth;
                    hidden += ((timeHeader.offsetWidth - hidden) / 2) - textWidth;
                    if (hidden + textWidth > timeHeader.offsetWidth) hidden = timeHeader.offsetWidth - textWidth;
                    timeHeader.style.paddingLeft = hidden + 'px';

                } else if (scrollPositionX > totalWidth && scrollPositionX + visibleWidth < totalWidth + timeHeader.offsetWidth) {
                    // Start & End are invisible but visible area is inside Start & End
                    var visible = (scrollPositionX + (visibleWidth / 2)) - totalWidth - textWidth;
                    timeHeader.style.paddingLeft = visible + 'px';

                } else {
                    // Not visible
                    timeHeader.paddingLeft = '0px';
                }
                // Keep total width position
                totalWidth += timeHeader.offsetWidth;
            }
        }
        schedulerContainer.addEventListener('scroll', onScrollTimeline);
        onScrollTimeline();
        return onScrollTimeline;
    };

    /**
     * On window size change, update available space 
     * 
     * @param {HtmlDivElement} mainContainer 
     * @param {bool} virtualScrolling True if virtual scrolling, false otherwise
     * @param {HtmlDivElement} projectOuterContainer
     * @param {HtmlDivElement} timelineAndScheduleContainer 
     * @param {HtmlDivElement} schedulerContainer 
     */
    var _sizeChange = function (dotnetRef, virtualScrolling, rowHeight, mainContainer, projectOuterContainer, timelineAndScheduleContainer, schedulerContainer) {
        var mainHeight = 0;
        var onSizeChange = function () {
            var mainContainerWidth = $(mainContainer).width();
            var mainContainerHeight = $(mainContainer).height();
            var projectWidth = $(projectOuterContainer).width();
            if (mainContainerWidth > 0 && mainContainerHeight > 0) {
                // Use all space available
                timelineAndScheduleContainer.style.width = (mainContainerWidth - projectWidth) + 'px';
                schedulerContainer.style.height = (mainContainerHeight - rowHeight) + 'px';
                // When virtualScrolling is enabled, we have to draw to show/hide potential new lines
                if (virtualScrolling && mainHeight != mainContainerHeight) {
                    //_delay(function () {
                    mainHeight = mainContainerHeight;
                    dotnetRef.invokeMethod('OnHeightChanged', parseInt(mainContainerHeight));
                    //}, 50);
                }
                schedulerContainer.dispatchEvent(new Event("scroll"));
            }
        };
        window.addEventListener('resize', onSizeChange);
        // Trigger 'resize' for first rendering
        setTimeout(function () { onSizeChange(); }, 20);
        return onSizeChange;
    };

    /**
     * Allow to resize project list width by dragging an element
     * 
     * @param {HtmlDivElement} gripperElement Element used to fire resize
     * @param {HtmlDivElement} mainContainer Main container used to detect if drag out of main area
     */
    var _initGripper = function (gripperElement, mainContainer, timelineAndScheduleContainer, schedulerContainer) {
        var mainContainerWidth;
        // Resize project list on mouse move
        var mouseMoveEvent = function (e) {
            var basePosition = mainContainer.getBoundingClientRect();
            var newProjectWidth = e.clientX - basePosition.left;
            // TODO: Passer la ref de la div a resizer
            gripperElement.parentNode.parentNode.style.width = newProjectWidth + 'px';
            timelineAndScheduleContainer.style.width = (mainContainerWidth - newProjectWidth) + 'px';
            // Trigger 'scroll' event on schedulerContainer to for timeline label to center inside their parent with the new space available
            schedulerContainer.dispatchEvent(new Event("scroll"));
        }
        // Stop resizing
        var mouseUpEvent = function (e) {
            mainContainer.onmousemove = null;
            mainContainer.onmouseup = null;
            gripperElement.onmouseup = null;
            mainContainer.style.cursor = 'default';
        }
        // Init. resizing when mouse down on gripper element 
        gripperElement.onmousedown = function (e) {
            mainContainerWidth = $(mainContainer).width();
            // Initialize resizing on mouse move
            mainContainer.onmousemove = mouseMoveEvent;
            mainContainer.onmouseup = mouseUpEvent;
            mainContainer.style.cursor = 'col-resize';
            // Stop resizing on mouse up
            gripperElement.onmouseup = mouseUpEvent;
        }
    };

    /**
     * Hight element from project list, scheduler view and timeline according to mouse position
     * 
     * @param {HtmlDivElement} schedulerContainer 
     * @param {HtmlDivElement} projectInnerContainer
     * @param {HtmlDivElement} timelineContainer
     */
    var _highlightColumnOnMouseMove = function (schedulerContainer, projectInnerContainer, timelineContainer) {
        schedulerContainer.onmousemove = function (e) {
            var basePosition = schedulerContainer.getBoundingClientRect();
            var x = e.clientX - basePosition.left + schedulerContainer.scrollLeft;
            var y = e.clientY - basePosition.top + schedulerContainer.scrollTop;
            // Highlight project row on mouse move
            var totalHeight = 0;
            var rows = projectInnerContainer.getElementsByClassName(_class_wk_row);
            for (const row of rows) row.classList.remove(_class_wk_row_highlight);
            for (var i in rows) {
                totalHeight += rows[i].offsetHeight;
                if (y < totalHeight) {
                    rows[i].classList.add(_class_wk_row_highlight);
                    break;
                }
            }
            // add wk-row-highlight on item with wst class according to 'x' mouse position
            var scheduleHighlight = function (element) {
                var totalWidth = 0;
                var columns = element.getElementsByClassName(_class_wst);
                for (const column of columns) column.classList.remove(_class_wk_row_highlight);
                for (const column of columns) {
                    totalWidth += $(column).width();
                    if (x < totalWidth) {
                        column.classList.add(_class_wk_row_highlight);
                        break;
                    }
                }
            }
            // Highlight column in scheduler view
            scheduleHighlight(schedulerContainer);
            // Highlight secondary timeline 
            scheduleHighlight(timelineContainer);
        };
        // Remove hightlight when cursor leave the component
        schedulerContainer.onmouseleave = function () {
            // Remove wk-row-highlight class from 'element'
            var clearHighlight = function (element) {
                var objects = element.getElementsByClassName(_class_wk_row_highlight);
                for (const obj of objects) obj.classList.remove(_class_wk_row_highlight);
            }
            // Clear highlight column in scheduler view
            clearHighlight(projectInnerContainer);
            // Clear highlight in schedule view
            clearHighlight(schedulerContainer);
            // Clear highlight secondary timeline 
            clearHighlight(timelineContainer);
        }
    };

    /**
     * Drag and drop manager
     * 
     * @param {c#} dotnetRef
     * @param {HTMLElement} mainContainer
     * @param {HTMLElement} schedulerContainer
     * @param {HTMLElement} projectOuterContainer
     * @param {string} externalDraggableIdentifier
     * @param {int} timelineHeight
     * @param {int} rowHeight
     * @param {int} paddingRow
     * @param {int} stepSize
     */
    var _initDragAndDrop = function (dotnetRef, mainContainer, schedulerContainer, projectOuterContainer, externalDraggableIdentifier, timelineHeight, rowHeight, paddingRow, stepSize, dayStep, isAgendaMode) {
        // Retrieve elements from the planning which can be dragged
        var movableItems = $(schedulerContainer).find("div[data-move]");
        // Since blazor can reuse some HTMLElement remove and re-add the 'mousedown' event handler according to the attribute 'data-move'
        movableItems.off('mousedown');
        var hoveredRowId;
        var wkRows = $(schedulerContainer).find('.wk-row');

        // Clear all event handler and temp var after a 'drop' event
        var cleanDragDropProcess = function () {
            // Clean up drag process
            mainContainer._currentDraggedMoved = false;
            mainContainer.onmousemove = null;
            mainContainer.onmouseup = null;
            mainContainer.onmouseleave = null;
            $(mainContainer).removeClass('wk-grabbing-grip');
            window.onmouseup = null;
            this.onmouseup = null;
            mainContainer.style.cursor = 'default';
            // Remove the cloned element
            $(mainContainer._currentDraggedElement).remove();
            // Restaure visibility of the original item
            mainContainer._currentDraggedOrigin.css({ opacity: '1' });
            $(window).off('mousemove', mainContainer._moveFromOutsideToSchedulerAreaFunction);
            // Clean temp vars
            mainContainer._currentDraggedElement = null;
            mainContainer._currentDraggedOffsetX = null;
            mainContainer._currentDraggedOriginX = null;
            mainContainer._currentDraggedLeftPosition = null;
            mainContainer._currentDraggedOriginY = null;
            mainContainer._currentDraggedRowHeight = null;
            mainContainer._currentDraggedXAllowed = null;
            mainContainer._currentDraggedYAllowed = null;
            mainContainer._currentDraggedOrigin = null;
            mainContainer._currentDraggedOriginRowId = null;
            mainContainer._currentDraggedIsExternal = null;
            mainContainer._currentDraggedExternalIdentifier = null;
        }
        var basePosition = mainContainer.getBoundingClientRect();
        // Function used to move the current selected item according to mouse position and Scheduler contraint
        var mouseMoveEvent = function (e) {
            var projectWidth = $(projectOuterContainer).width() | 0;
            // Retrieve the rowId based on current mouse position
            var currentMouseTop = e.clientY - basePosition.top - timelineHeight + schedulerContainer.scrollTop;
            var divider = Math.floor(currentMouseTop / mainContainer._currentDraggedRowHeight);
            hoveredRowId = $(wkRows.get(divider)).attr('data-row-id');
            // Check if the current dragged item can be moved horizontally
            if (mainContainer._currentDraggedXAllowed) {
                // Compute the new left position according to the mouse coordinates
                var newItemLeft = e.clientX - projectWidth - basePosition.left + schedulerContainer.scrollLeft - mainContainer._currentDraggedOffsetX;
                if (isAgendaMode) {
                    newItemLeft += mainContainer._currentDraggedOffsetX;
                }
                if (mainContainer._currentDraggedIsExternal) {
                    newItemLeft += projectWidth - schedulerContainer.scrollLeft;
                }
                // Start the drag process only after a little move (10px min)
                if (Math.abs(mainContainer._currentDraggedOriginX - newItemLeft) > 10 || mainContainer._currentDraggedMoved || mainContainer._currentDraggedIsExternal) {
                    // Check and apply move step consttraint (free, hour, day, ...)
                    // rq: when agenda mode, x-axis is always constraint by a step of one day
                    var currentStepSize = isAgendaMode ? dayStep : stepSize;
                    if (currentStepSize > 0) {
                        // Constraint the drag on predefined position according to stepSize
                        if (mainContainer._currentDraggedIsExternal) {
                            // Rq: If we are in external dragging mode the X origin is not the same as an internal dragging
                            var divider = Math.floor((newItemLeft + schedulerContainer.scrollLeft - projectWidth) / currentStepSize);
                            // to constraint the drag to a full visible 'column' (to avoid the glitch switch intern/extern dragging)
                            if (schedulerContainer.scrollLeft > 0 && divider > 0) {
                                var shift = Math.floor(schedulerContainer.scrollLeft / currentStepSize) + 1;
                                divider -= shift;
                                if (divider < 0) {
                                    mainContainer._glitch = true;
                                    return;
                                }
                            }
                            var remainder = (newItemLeft - projectWidth) % currentStepSize;
                            if (remainder / 2 > currentStepSize) {
                                // it should not be possible
                                debugger;
                                divider += 1;
                            }
                            newItemLeft = currentStepSize * divider;
                            // Apply the left offset due to external dragging
                            newItemLeft += projectWidth;
                            if (schedulerContainer.scrollLeft > 0) {
                                newItemLeft += currentStepSize - (schedulerContainer.scrollLeft % currentStepSize);
                            }
                        } else {
                            var divider = Math.floor(newItemLeft / currentStepSize);
                            var remainder = newItemLeft % currentStepSize;
                            if (remainder / 2 > currentStepSize) {
                                divider += 1;
                            }
                            newItemLeft = currentStepSize * divider;
                        }
                    }
                    $(mainContainer._currentDraggedElement).css('left', newItemLeft + 'px');
                    mainContainer._currentDraggedMoved = true;
                    mainContainer._currentDraggedLeftPosition = newItemLeft;
                }
            }
            // Check if the current dragged item can be moved vertically
            if (mainContainer._currentDraggedYAllowed) {
                // Compute the new top position according to the mouse coordinates
                var divider = 0, newTop = 0;
                if (isAgendaMode) {
                    // re-adjust the position according to the position where the user has started the drag process
                    currentMouseTop -= mainContainer._currentDraggedOffsetY;
                    // in agenda mode, compute the top position according to stepSize (not necessary in a row)
                    divider = Math.floor(currentMouseTop / stepSize);
                    newTop = stepSize * divider;
                    if (mainContainer._currentDraggedIsExternal) {
                        newTop -= paddingRow;
                    }
                    mainContainer._currentAgendaTop = newTop;
                } else {
                    // in timeline mode, compute the top position according to row position
                    divider = Math.floor(currentMouseTop / mainContainer._currentDraggedRowHeight);
                    newTop = mainContainer._currentDraggedRowHeight * divider;
                }
                if (mainContainer._currentDraggedIsExternal) {
                    newTop += timelineHeight - schedulerContainer.scrollTop; // if element dragged from the outside, the base is the mainContainer so we have to add the timeline height
                }
                if (Math.abs(newTop - mainContainer._currentDraggedOriginY) > 10 || mainContainer._currentDraggedMoved || mainContainer._currentDraggedIsExternal) {
                    $(mainContainer._currentDraggedElement).css('top', newTop + 'px');
                    mainContainer._currentDraggedMoved = true;
                }
                else if (isNaN(mainContainer._currentDraggedOriginY)) {
                    mainContainer._currentDraggedOriginY = newTop;
                }
            }
            // When the dragged element is at the begin or end of the schedule are, move the scroll horizontally
            // to allow the user to see area hidden by the scroll
            if (false && !$(e.target).hasClass('wk-row')) {
                var scrollAutoSpace = 50; // TODO WKS Ne pas utiliser un nb fixe, faire un calcul proportionnel => plus on s'approche de la fin et plus on scrool vite
                var scrollMove = 100;
                var xx = e.clientX - $(projectOuterContainer).width() - mainContainer.getBoundingClientRect().left - e.offsetX;
                if (xx < scrollAutoSpace && schedulerContainer.scrollLeft > 0) {
                    schedulerContainer.scrollTo({
                        left: schedulerContainer.scrollLeft - scrollMove,
                        behavior: 'smooth'
                    });
                } else if (xx + $(e.target).width() + scrollAutoSpace > $(schedulerContainer).width() && schedulerContainer.scrollLeft < $(schedulerContainer).children().first().width()) {
                    schedulerContainer.scrollTo({
                        left: schedulerContainer.scrollLeft + scrollMove,
                        behavior: 'smooth'
                    });
                }
            }
            // Don't let parent(s) element(s) handle the mouseMove event
            e.stopPropagation();
            return false;
        }
        // Function used to complete drag / drop process
        var mouseUpEvent = function (e) {
            var finalLeft = mainContainer._currentDraggedIsExternal
                ? mainContainer._currentDraggedLeftPosition - $(projectOuterContainer).width() + schedulerContainer.scrollLeft
                : mainContainer._currentDraggedLeftPosition;


            // Dropped on invalid destination
            if (((!hoveredRowId && !isAgendaMode) || finalLeft < 0)) {
                if (mainContainer._currentCloneOrigin != null) {
                    $(mainContainer._currentDraggedElement).animate({
                        left: mainContainer._currentCloneOrigin.left,
                        top: mainContainer._currentCloneOrigin.top
                    }, 250, 'swing', function () {
                        dotnetRef.invokeMethod('CancelDragProcess');
                        cleanDragDropProcess();
                    });
                } else {
                    dotnetRef.invokeMethod('CancelDragProcess');
                    cleanDragDropProcess();
                }

                mainContainer._lastEndDrop = new Date();
                return false;

                // If the element has been moved
            } else if (mainContainer._currentDraggedMoved) {
                // Dropped item come from outside of datasource
                if (mainContainer._currentDraggedIsExternal == true) {
                    if (isAgendaMode) {
                        dotnetRef
                            .invokeMethod('CompleteDragProcessAsync',
                                '' + (mainContainer._currentAgendaTop + paddingRow),
                                finalLeft,
                                mainContainer._currentDraggedExternalIdentifier,
                                $(mainContainer._currentDraggedElement).height());
                    } else {
                        dotnetRef
                            .invokeMethod('CompleteDragProcessAsync',
                                hoveredRowId,
                                finalLeft,
                                mainContainer._currentDraggedExternalIdentifier,
                                null);
                    }

                    // Dropped item from the datasource   
                } else {
                    if (isAgendaMode) {
                        // in agenda mode, send the new top coordinate to compute the final date
                        dotnetRef
                            .invokeMethod('CompleteDragProcessAsync',
                                '' + mainContainer._currentAgendaTop,
                                finalLeft,
                                null, null);
                    } else {
                        // in timeline mode, task are dropped inside a row (so returning the rowId)
                        dotnetRef
                            .invokeMethod('CompleteDragProcessAsync',
                                mainContainer._currentDraggedYAllowed ? hoveredRowId : mainContainer._currentDraggedOriginRowId,
                                finalLeft,
                                null, null);
                    }

                }
            }
            else {
                dotnetRef
                    .invokeMethod('OnDragItemClickAsync');
            }
            cleanDragDropProcess();
            mainContainer._lastEndDrop = new Date();
            return false;
        }
        // If the scheduler contain movable items
        if (movableItems.length > 0) {
            // Handle mousedown event on each element which allow-move is true
            movableItems.each(function (e) {
                // Check allowed drag type for this item
                var dataMove = $(this).attr('data-move'); // TODO  KWS
                var moveX = dataMove.includes('x');
                var moveY = dataMove.includes('y');
                if (moveX || moveY) {
                    // Start the drag process when mousedown event fire on the draggable element
                    $(this).on('mousedown', function (e) {
                        // Subscribe to all required events to manage the drag/drop (mousedown / mousemove / mouseup)
                        // at the mainContainer level (we don't have to display an item dragged from the Scheduler outside the mainContainer)
                        mainContainer.onmousemove = mouseMoveEvent;
                        mainContainer.onmouseup = mouseUpEvent;
                        mainContainer.onmouseleave = function (e) {
                            hoveredRowId = undefined;
                        }
                        window.onmouseup = function (e) { // TODO WKS A vérifier
                            hoveredRowId = undefined;
                            mouseUpEvent(e);
                        }
                        // Compute the original top position
                        // the cloned task will be positioned outside the 'wk-row' container
                        // with an absolute x/y from the scheduleContainer
                        var prevElement = $(this).closest('.wk-row');
                        hoveredRowId = prevElement.attr('data-row-id');
                        //var rowCount = prevElement.prevAll('.wk-row').length;
                        //var originY = (rowCount * rowHeight);
                        var currentMouseTop = e.clientY - basePosition.top - timelineHeight + schedulerContainer.scrollTop;
                        var divider = Math.floor(currentMouseTop / mainContainer._currentDraggedRowHeight);
                        var originY = mainContainer._currentDraggedRowHeight * divider;

                        // Clone the div to move at the same position and hide (opacity 0.5) the original element
                        var cloned = $(this).clone().css({ position: 'absolute', top: originY + 'px' });
                        //zz Add an indicator as an overlay to show exact start time for the current dragged element
                        //zz var topIndicator = $('<span></span>').addClass('wk-drag-indicator').text('topIndicator');
                        //zz var topIndicatorContainer = $('<div></div>').addClass('wk-drag-indicator-container').append(topIndicator);
                        //zz topIndicatorContainer.append($('<div></div>').addClass('wk-rowhour-border'));
                        //zz cloned.append(topIndicatorContainer);
                        $(schedulerContainer).append(cloned);
                        $(this).css({ opacity: '0.5' });
                        // Var used to track current drag state
                        mainContainer._currentCloneOrigin = { top: originY + 'px', left: $(this).css('left') };
                        mainContainer._currentDraggedElement = cloned.get(0);
                        mainContainer._currentDraggedOffsetX = e.offsetX;
                        mainContainer._currentDraggedOffsetY = e.offsetY;
                        mainContainer._currentDraggedOriginX = parseInt($(this).css('left').replace('px', ''));
                        mainContainer._currentDraggedLeftPosition = mainContainer._currentDraggedOriginX;
                        mainContainer._currentDraggedOriginY = originY;
                        mainContainer._currentDraggedRowHeight = rowHeight;
                        mainContainer._currentDraggedXAllowed = moveX;
                        mainContainer._currentDraggedYAllowed = moveY;
                        mainContainer._currentDraggedOrigin = $(this);
                        mainContainer._currentDraggedOriginRowId = hoveredRowId;
                        mainContainer._currentDraggedIsExternal = false;
                    });
                }
            })
        }
        // If the LgWorkScheduler have an external draggable identifier set
        if (externalDraggableIdentifier) {
            // Retrieve all elements from the DOM wich contain this attribute
            var edi = $('[wks-drag-id=' + externalDraggableIdentifier + ']');
            if (edi.length > 0) {
                // Add an event handler for firing the drag process on all items found
                edi.each(function (i) {
                    // Start the drag when mousedown on the target item
                    $(this).off('mousedown').on('mousedown', function (e) {
                        // Clone and hide the target
                        var target = $(e.currentTarget);
                        var targetWidth = target.attr('data-wks-duration');
                        var clonedTarget = target.parent().clone();
                        clonedTarget.css({ height: '100%', cursor: 'grabbing' });
                        var tartgetClonedContainer = $('<div />').css({
                            width: (isAgendaMode ? dayStep : targetWidth) + 'px',
                            height: (isAgendaMode ? targetWidth : (rowHeight - (paddingRow * 2))) + 'px',
                            lineHeight: (rowHeight - (paddingRow * 2)) + 'px',
                            marginTop: paddingRow + 'px',
                            marginBottom: paddingRow + 'px',
                            position: 'absolute',
                            top: '0px',
                            left: '0px',
                            textAlign: 'initial',
                            cursor: 'grabbing'
                        }).addClass('wk-bar');
                        clonedTarget.find('.wk-grag-grip').addClass('wk-grabbing-grip');
                        $(mainContainer).addClass('wk-grabbing-grip');
                        tartgetClonedContainer.append(clonedTarget);
                        target.parent().css('opacity', '0.5');
                        // Add the cloned item at mainContainer level to allow overflow and display the dragged item outiside the mainContainer area
                        // rq: schedulerContainer don't allow overflow so we won't see the item if the mouse is not hover scheduleContainer
                        $(mainContainer).append(tartgetClonedContainer);
                        // Compute the position for the cloned element (it should be at the same position)
                        var basePosition = mainContainer.getBoundingClientRect();
                        var delta = 5;
                        var newX = e.clientX - basePosition.left - e.offsetX;
                        var newY = e.clientY - basePosition.top - e.offsetY - paddingRow - delta;
                        var currentOffsetX = e.offsetX;
                        var currentOffsetY = e.offsetY;
                        tartgetClonedContainer.css({
                            top: newY + 'px',
                            left: newX + 'px'
                        });
                        // Set vars to track drag state
                        mainContainer._currentDraggedElement = tartgetClonedContainer.get(0);
                        mainContainer._currentDraggedOffsetX = currentOffsetX;
                        mainContainer._currentDraggedOffsetY = e.offsetY;
                        mainContainer._currentDraggedOriginX = 0;
                        mainContainer._currentDraggedLeftPosition = mainContainer._currentDraggedOriginX;
                        mainContainer._currentDraggedOriginY = 0;
                        mainContainer._currentDraggedRowHeight = rowHeight;
                        mainContainer._currentDraggedXAllowed = true;
                        mainContainer._currentDraggedYAllowed = true;
                        mainContainer._currentDraggedOrigin = target.parent();
                        mainContainer._currentDraggedOriginRowId = '';
                        mainContainer._currentDraggedIsExternal = true;
                        mainContainer._currentCloneOrigin = { top: newY + 'px', left: newX + 'px' };
                        mainContainer._currentDraggedExternalIdentifier = target.attr('wks-task-id');
                        hoveredRowId = '';
                        // Disable the d&g handler on the main container since we are using an onmousemove relative 
                        mainContainer.onmousemove = null;
                        // Function used to drag an item from outside to the mainContainer
                        mainContainer._moveFromOutsideToSchedulerAreaFunction = function (e) {
                            var basePosition = mainContainer.getBoundingClientRect();
                            var actualLeft = parseInt($(mainContainer._currentDraggedElement).css('left').replace('px'));
                            var actualTop = parseInt($(mainContainer._currentDraggedElement).css('top').replace('px'));

                            // The dragged element is positionned inside the main container of the LgWorkScheduler
                            // Check if the dragged element is hover the scheduler container (excluding projectContainer and timelineContainer)
                            if (!mainContainer._glitch && actualLeft >= $(projectOuterContainer).outerWidth() && actualTop > timelineHeight) {
                                mouseMoveEvent(e);
                                e.stopPropagation();

                                return true;
                            }
                            else {
                                if (mainContainer._glitch) mainContainer._glitch = false;

                                hoveredRowId = null;
                                var newX = e.clientX - basePosition.left - currentOffsetX;
                                var newY = e.clientY - basePosition.top - currentOffsetY - paddingRow - delta;
                                tartgetClonedContainer.css({
                                    top: newY + 'px',
                                    left: newX + 'px'
                                });
                            }
                        };
                        mainContainer.onmouseup = mouseUpEvent;

                        var fnInvalidMouseUpHandler = function () {
                            $(window).off('mousemove', mainContainer._moveFromOutsideToSchedulerAreaFunction);
                            $(window).off('mouseup', fnInvalidMouseUpHandler);
                            mainContainer.onmouseup = null;
                            if (mainContainer._currentDraggedElement) {
                                mouseUpEvent(e);
                            }
                        }
                        $(window)
                            .on('mousemove', mainContainer._moveFromOutsideToSchedulerAreaFunction)
                            .on('mouseup', fnInvalidMouseUpHandler);
                    });
                });
            }
        }
    }

    //#region Utilitaries

    /**
     * Callback tempo
     */
    var _delay = (function () {
        var timer = 0;
        return function (callback, ms) {
            clearTimeout(timer);
            timer = setTimeout(callback, ms);
        };
    })();

    //#endregion

    return {

        /**
         * LgWorkScheduler initialisation.
         * 
         * @param {DotNetObjectReference} dotnetRef Used to invoke c# methods
         * @param {bool} virtualScrolling True if virtual scrolling, false otherwise
         * @param {bool} showMouseIndicator True if virtual scrolling, false otherwise
         * @param {int} timelineHeight Height of the timeline
         * @param {int} rowHeight Height of each row
         * @param {int} paddingRow Padding applied in each row
         * @param {int} dragStep Constraint for horizontal move
         * @param {HtmlDivElement} mainContainer Global component container
         * @param {HtmlDivElement} schedulerContainer Scheduler container 
         * @param {HtmlDivElement} timelineContainer Timeline container
         * @param {HtmlDivElement} projectOuterContainer  Project list + toolbar container
         * @param {HtmlDivElement} projectInnerContainer  Project list container
         * @param {HtmlDivElement} timelineAndScheduleContainer Right container
         * @param {HtmlDivElement} resizeProjectGripper Div used to resize project width
         */
        Init: function (dotnetRef, virtualScrolling, showMouseIndicator, timelineHeight, rowHeight, paddingRow, dragStep, dayStep, isAgendaMode, mainContainer, schedulerContainer, timelineContainer, projectOuterContainer, projectInnerContainer, timelineAndScheduleContainer, resizeProjectGripper, externalIdentifier) {
            var onSizeCallback = _sizeChange(dotnetRef, virtualScrolling, timelineHeight, mainContainer, projectOuterContainer, timelineAndScheduleContainer, schedulerContainer);
            var onScrollSynchroCallback = _scrollSynchro(dotnetRef, virtualScrolling, schedulerContainer, timelineContainer, projectInnerContainer, mainContainer);
            var onScrollTimelineCallback = _scrollTimeline(schedulerContainer, timelineContainer);
            if (resizeProjectGripper) _initGripper(resizeProjectGripper, mainContainer, timelineAndScheduleContainer, schedulerContainer);
            if (showMouseIndicator) _highlightColumnOnMouseMove(schedulerContainer, projectInnerContainer, timelineContainer);
            _initDragAndDrop(dotnetRef, mainContainer, schedulerContainer, projectOuterContainer, externalIdentifier, timelineHeight, rowHeight, paddingRow, dragStep, dayStep, isAgendaMode);
            // keep the c# ref on the mainContainer HTMLElement to easy retrieve it from anywhere
            mainContainer.dotnetRef = dotnetRef;

            // Allow scoll top/down on projectContainer (which have an overflow:hidden)
            // will result in a scroll in the main schedule container
            var fnScrollOnProject = function (e) {
                schedulerContainer.scrollTo({
                    top: $(schedulerContainer).scrollTop() + e.originalEvent.deltaY,
                    behavior: 'smooth'
                });
            };
            $(projectInnerContainer).on('wheel', fnScrollOnProject);

            return {


                /**
                 * Re-apply the drag and drop manager after an OnAfterRender (the DOM could have changed since the last initialisation)
                 *
                 * @param {int} rowHeight The height of one row
                 * @param {int} paddingRow The padding inside a row
                 * @param {int} dragStep The drag step to use (hour by hour, day by day, ...)
                 */
                RefreshDragDrop: function (rowHeight, paddingRow, dragStep, dayStep, isAgendaMode) {
                    _initDragAndDrop(dotnetRef, mainContainer, schedulerContainer, projectOuterContainer, externalIdentifier, timelineHeight, rowHeight, paddingRow, dragStep, dayStep, isAgendaMode);
                },

                /**
                 * Remove all event listener 
                 */
                Dispose: function () {
                    window.removeEventListener('resize', onSizeCallback);
                    schedulerContainer.removeEventListener('scroll', onScrollSynchroCallback);
                    schedulerContainer.removeEventListener('scroll', onScrollTimelineCallback);
                    $(projectInnerContainer).off('wheel', fnScrollOnProject);
                },

                /**
                 * Center timeline text according to scroll position 
                 **/
                CenterTimeline: function () {
                    schedulerContainer.dispatchEvent(new Event("scroll"));
                    window.dispatchEvent(new Event("resize"));
                },

                /**
                 * Scroll to element position with '_class_wst' css class
                 * 
                 * @param {HtmlDivElement} schedulerContainer Container containing scroll
                 * @param {HtmlDivElement} timelineContainer Container containing today position flagged with '_class_wst' css class
                 */
                ScrollToToday: function () {
                    var found = false;
                    var width = 0;
                    var cols = timelineContainer.getElementsByClassName(_class_wst);
                    for (const col of cols) {
                        width += col.offsetWidth;
                        if (col.classList.contains(_class_wst_today)) {
                            found = true;
                            break;
                        }
                    }
                    if (found) {
                        // Scroll smoothly to today position
                        schedulerContainer.scrollTo({
                            top: schedulerContainer.scrollTop,
                            left: width - (schedulerContainer.offsetWidth / 2),
                            behavior: 'smooth'
                        });
                    }
                },

                /**
                 * Move the horizontal scrollbar to the right 
                 */
                ScrollToEnd: function () {
                    var width = schedulerContainer.getElementsByClassName('schedule-draw-area')[0].style.width.replace('px', '');
                    schedulerContainer.scrollTo({ left: width, behavior: 'smooth' })
                },

                /**
                 * Move the horizontal scrollbar to the left 
                 */
                ScrollToStart: function () {
                    schedulerContainer.scrollTo({ left: 0, behavior: 'smooth' })
                }

            };
        },

        /**
         * Stop tracking window resize event.
         * 
         * @param {JsObjectReference} ref Reference to an js object returned by Init 
         */
        Dispose: function (ref) {
            if (ref) {
                ref.Dispose();
            }
        },

        /**
         * Stop tracking window resize event.
         * 
         * @param {JsObjectReference} ref Reference to an js object returned by Init 
         */
        CenterTimeline: function (ref) {
            if (ref) {
                ref.CenterTimeline();
            }
        },

        /**
         * Scroll to element position with '_class_wst' css class
         * 
         * @param {HtmlDivElement} schedulerContainer Container containing scroll
         * @param {HtmlDivElement} timelineContainer Container containing today position flagged with '_class_wst' css class
         */
        ScrollToToday: function (ref) {
            if (ref) {
                ref.ScrollToToday();
            }
        },

        /**
         * Scroll the horizontal scrollbar to the right
         * 
         */
        ScrollToEnd: function (ref) {
            if (ref) {
                ref.ScrollToEnd();
            }
        },

        /**
         * Scroll the horizontal scrollbar to the left
         * 
         */
        ScrollToStart: function (ref) {
            if (ref) {
                ref.ScrollToStart();
            }
        },

        /**
         * Re-apply the drag and drop manager after an OnAfterRender (the DOM could have changed since the last initialisation)
         * @param {any} ref 
         * @param {any} rowHeight
         * @param {any} paddingRow
         * @param {any} dragStep
         * @param {any} isAgendaMode
         */
        RefreshDragDrop: function (ref, rowHeight, paddingRow, dragStep, dayStep, isAgendaMode) {
            if (ref) {
                ref.RefreshDragDrop(rowHeight, paddingRow, dragStep, dayStep, isAgendaMode);
            }
        },

        /**
         * Event fired by onclick defined in razor view (cf. SchedulerViewItem.razor)
         * 
         * @param {MouseEvent} e Js MouseEvent data 
         * @param {string} key row identifier 
         */
        OnEmptyAreaClick: function (e, key) {
            var row = $(e.target);
            if (row.hasClass('wk-row')) {
                var mainContainer = row.closest('.mainContainer').get(0);
                var dotnetRef = mainContainer.dotnetRef;
                var ellapsed = Math.abs(mainContainer._lastEndDrop - new Date());
                // This even can fire when a drag process is completed (click and drag are both based on onmousedown)
                // So check if at least 500ms have passed since the last 'drop' event to know if it's a click or the end of a drag
                if (dotnetRef && (isNaN(ellapsed) || ellapsed > 500)) {
                    //dotnetRef.invokeMethod('OnInternalEmptyItemClickedAsync', e.offsetX, key);
                    dotnetRef.invokeMethod('OnInternalEmptyItemClickedAsync', e.offsetX, row.attr('data-row-id'));
                }
            }
        },

        OnGrabbing: function (e) {
            $(e.target).css('cursor', 'grabbing');
            var fnRestoreCustor = function () {
                $(e.target).css('cursor', 'grab');
                $(window).off('mouseup', fnRestoreCustor);
            };
            $(window).on('mouseup', fnRestoreCustor);
        }

    }

})();