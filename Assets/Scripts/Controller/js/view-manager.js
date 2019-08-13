//the view manager finds all div containers with the "view" class and lets me show one view while hiding all others
//see also: https://github.com/AirConsole/airconsole-view-manager
var ViewManager = {
    views: {},
    current_view_id: null,

    init: function() {
        var views = document.querySelectorAll('.view');
        for (var i = 0; i < views.length; i++) {
        this.views[views[i].id] = views[i];
        }
        return this;
    }, 

    show: function(id) {
        console.log("Change view with " + id);

        var view = this.views[id];
        if (view) {
        this.current_view_id = id;
        this.hideAll();
        view.style.display = "block";
        } else {
        console.warn("Could not find view with ID:", id);
        }
        return this;
    },

    hideAll: function() {
        for (var key in this.views) {
        this.views[key].style.display = "none";
        }
    }
};