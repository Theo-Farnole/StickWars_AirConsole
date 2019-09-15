const BUTTONS_COUNT = 5;

var airconsole;
var touchedElement = new Map();
var activeFunctionMap = new Map();
var disableFunctionMap = new Map();



// #region AIR CONSOLE STARTUP
function init() {
    ViewManager.init();
    ViewManager.show("Load");
    // ViewManager.show("Play");

    airconsole = new AirConsole({
        "orientation": "landscape"
    });

    airconsole.onMessage = function (from, data) {
        if (from == AirConsole.SCREEN) {
            if (data.view) {
                ViewManager.show(data.view);
            }

            if (data.charId != -1) {
                document.body.style.backgroundColor = data.bgColor;

                var sheet = document.styleSheets[0];
                var css_rules_num = sheet.cssRules.length;

                // update tackle button color
                sheet.addRule("#button_b>div", "background-color:" + data.bgColor, css_rules_num);
                css_rules_num = sheet.cssRules.length;

                // update font color
                if (data.charId == "Red" || data.charId == "Blue") {
                    sheet.addRule(".view>p", "color: white", css_rules_num);
                }
            }
        }
    };

    addButton("button_left", true,
        function () {
            horizontal(-1)
        },
        function () {
            horizontal(0)
        });
    addButton("button_right", true,
        function () {
            horizontal(1)
        },
        function () {
            horizontal(0)
        });
    addButton("button_b", false,
        function () {
            bPressed(true)
        },
        function () {
            bPressed(false)
        });
    addButton("button_x", false,
        function () {
            xPressed(true)
        },
        function () {
            xPressed(false)
        });
    addButton("button_a", false,
        function () {
            aPressed(true)
        },
        function () {
            aPressed(false)
        });

    addButton("button_a_menu", false,
        function () {
            aPressed(true)
        },
        function () {
        });
}

// #endregion AIR CONSOLE STARTUP

// #region TOUCH LISTENER
document.addEventListener('touchstart', function (event) {
    event.preventDefault();

    var touches = event.touches;

    for (var i = 0; i < touches.length; i++) {
        var touch = touches[i];
        var identifier = touch.identifier;

        touchedElement.set(identifier, document.elementFromPoint(touch.pageX, touch.pageY));

        var touchedElementId = touchedElement.get(identifier).id;

        // if touched element has an active function
        if (touchedElementId != undefined && activeFunctionMap.get(touchedElementId) != undefined) {
            activeFunctionMap.get(touchedElementId)();

            // add visual feedback
            touchedElement.get(identifier).className += " buttons_active";
        }
    }
}, false);

document.addEventListener('touchmove', function (event) {
    event.preventDefault();

    var touches = event.touches;

    for (var i = 0; i < touches.length; i++) {

        var touch = touches[i];
        var identifier = touch.identifier;
        var newTouchedElement = document.elementFromPoint(touch.pageX, touch.pageY);

        if (touchedElement.get(identifier) !== newTouchedElement) {
            var touchedElementId = touchedElement.get(identifier).id;

            // if old touched element has a disable function
            if (touchedElementId != undefined && disableFunctionMap.get(touchedElementId) != undefined) {
                disableFunctionMap.get(touchedElementId)();

                // remove visual feedback
                touchedElement.get(identifier).className = touchedElement.get(identifier).className.replace(" buttons_active", "");
            }

            touchedElement.set(identifier, newTouchedElement);
            touchedElementId = touchedElement.get(identifier).id;

            // if new touched element has an active function
            if (touchedElementId != undefined && activeFunctionMap.get(touchedElementId) != undefined) {
                activeFunctionMap.get(touchedElementId)();

                // add visual feedback
                touchedElement.get(identifier).className += " buttons_active";
            }
        }
    }
}, false);

document.addEventListener("touchend", function (event) {

    var touches = event.changedTouches;

    for (var i = 0; i < touches.length; i++) {
        var identifier = touches[i].identifier;

        var touchedElementId = touchedElement.get(identifier).id;

        if (touchedElementId != undefined && disableFunctionMap.get(touchedElementId) != undefined) {
            disableFunctionMap.get(touchedElementId)();

            // remove visual feedback
            touchedElement.get(identifier).className = touchedElement.get(identifier).className.replace(" buttons_active", "");
        }

        touchedElement.set(identifier, null);
    }
});


function addButton(id, isDirectional, activeFunction, disableFunction) {

    var obj = document.getElementById(id);

    if (isDirectional == true) {
        activeFunctionMap.set(id, activeFunction);
        disableFunctionMap.set(id, disableFunction);
    } else {
        obj.addEventListener("touchstart", function () {
            obj.className += " buttons_active";
            activeFunction();
        });
        obj.addEventListener("touchend", function () {
            obj.className = obj.className.replace(" buttons_active", "");
            disableFunction();
        });
    }

    obj.addEventListener("mousedown", activeFunction);
    obj.addEventListener("mouseup", disableFunction);
}
// #endregion TOUCH LISTENER

// #region MESSAGE METHODS
function horizontal(amount) {
    console.log("horizontal(" + amount + ")");

    airconsole.message(AirConsole.SCREEN, {
        horizontal: amount
    })
}

function bPressed(pressed) {
    console.log("bPressed(" + pressed + ")");

    airconsole.message(AirConsole.SCREEN, {
        bPressed: pressed
    })
}

function xPressed(pressed) {
    console.log("xPressed(" + pressed + ")");

    airconsole.message(AirConsole.SCREEN, {
        xPressed: pressed
    })
}

function aPressed(pressed) {
    console.log("aPressed(" + pressed + ")");

    airconsole.message(AirConsole.SCREEN, {
        aPressed: pressed
    })
}
//#endregion