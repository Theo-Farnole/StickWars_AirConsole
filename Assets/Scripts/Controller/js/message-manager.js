var airconsole;

        function init() {
            airconsole = new AirConsole({
                "orientation": "landscape"
            }); 

            ViewManager.init();
            ViewManager.show("Play");

            airconsole.onMessage = function(from, data) {
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
        }

        function horizontal(amount) {
            airconsole.message(AirConsole.SCREEN, {
                horizontal: amount
            })
        }

        function bPressed(pressed) {
            airconsole.message(AirConsole.SCREEN, {
                bPressed: pressed
            })
        }

        function xPressed(pressed) {
            airconsole.message(AirConsole.SCREEN, {
                xPressed: pressed
            })
        }

        function aPressed(pressed) {
            airconsole.message(AirConsole.SCREEN, {
                aPressed: pressed
            })
        }