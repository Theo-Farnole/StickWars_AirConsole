var airconsole;

function init() {
    airconsole = new AirConsole({
        "orientation": "landscape"
    });

    ViewManager.init();
    ViewManager.show("Play");

    airconsole.onMessage = function (from, data) {
        if (from == AirConsole.SCREEN) {
            if (data.view) {
                ViewManager.show(data.view);
            }

            if (data.bgColor) {
                document.body.style.backgroundColor = data.bgColor;
            }
        }
    };

    addButton("button_left",
        function () {
            horizontal(-1)
        },
        function () {
            horizontal(0)
        });
    addButton("button_right",
        function () {
            horizontal(1)
        },
        function () {
            horizontal(0)
        });
    addButton("button_b",
        function () {
            bPressed(true)
        },
        function () {
            bPressed(false)
        });
    addButton("button_x",
        function () {
            xPressed(true)
        },
        function () {
            xPressed(false)
        });
    addButton("button_a",
        function () {            
            aPressed(true)
        },
        function () {
            aPressed(false)
        });
}


function addButton(id, activeFunction, disableFunction) {

    var obj = document.getElementById(id);

    obj.addEventListener("touchstart", activeFunction);
    obj.addEventListener("touchend", disableFunction);

    obj.addEventListener("mousedown", activeFunction);    
    obj.addEventListener("mouseup", disableFunction);
}

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