var airconsole;

function init() {
    airconsole = new AirConsole({
        "orientation": "landscape"
    }); 

    ViewManager.init();
    ViewManager.show("Play");

    airconsole.onMessage = function(from, data) 
    {
        if (from == AirConsole.SCREEN)
        {
            if (data.view)
            {
                ViewManager.show(data.view);
            }
            
            if (data.bgColor)
            {
                document.body.style.backgroundColor = data.bgColor;
            } 
        }
    };

    // to avoid play on awake
    // replace "horizontal(-1)" by
    // function () { horizontal(-1)}
    addButton("button_left", horizontal(-1), horizontal(0));
    addButton("button_right", horizontal(1), horizontal(0));
    addButton("button_b", bPressed(true), bPressed(false));
    addButton("button_x", xPressed(true), xPressed(false));
    addButton("button_a", aPressed(true), aPressed(false));
}


function addButton(id, activeFunction, disableFunction)
{
    setEventHandler(id, "ontouchstart", activeFunction);
    setEventHandler(id, "ontouchend", disableFunction);

    setEventHandler(id, "onmousedown", activeFunction);
    setEventHandler(id, "onmouseup", disableFunction);
}

function setEventHandler(obj, name, fn) {
    if (typeof obj == "string") {
        obj = document.getElementById(obj);
    }
    if (obj.addEventListener) {
        return(obj.addEventListener(name, fn));
    } else if (obj.attachEvent) {
        return(obj.attachEvent("on" + name, function() {return(fn.call(obj));}));
    }
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