//////Dictionary  that contains button Id and the associated shortcut key 
////var buttonDictionary = new Object();

//////The onkeydown event occurs when the user is pressing a key (on the keyboard)
////document.onkeydown = function (e) {
////    e = e || window.event;//Get event
////    //console.log("Key Pressed: " + e.key + "\n" + "CTRL key pressed: " + e.ctrlKey + "\n");
////    if (e.ctrlKey) {
////        getButtonIdsAssociatedToShortcutkeyPressed(e.key).forEach(function (btn) {
////            btn.click();
////            e.preventDefault();
////        });
////    }
////}

//////Add button to Dictionary
////var addButtonToDictionary = function (buttonId, shortcutKey) {
////    buttonDictionary[buttonId] = shortcutKey;
////    //for (var btn in buttonDictionary) {
////    //    var val = buttonDictionary[btn];
////    //    console.log("buttonDictionary   " + btn + val);
////    //}
////}

////var getButtonIdsAssociatedToShortcutkeyPressed = function (shortcutKey) {
////    var buttons = [];
////    for (var elem in buttonDictionary) {
////        if (buttonDictionary[elem].localeCompare(shortcutKey) == 0) {
////            var btnById = document.getElementById(elem);
////            if (btnById) {
////                if (isButtonFocused(btnById)) {
////                    return [btnById];
////                }
////                if (isButtonParentFocused(btnById)) {
////                    buttons.push(btnById);
////                }
////            }
////        }
////    }
////    return buttons;
////}

////var isButtonFocused = function (button) {
////    return document.activeElement === button;
////}

////var isButtonParentFocused = function (button) {
////    var buttonParent = button.parentElement;
////    while (buttonParent) {
////        if (document.activeElement === buttonParent) {
////            return true;
////        }

////        buttonParent = buttonParent.parentElement;
////    }

////    return false;
////}