window.refresh = function () {
    dashboardControl.reloadData();
}

window.dashboardEvents = {
    onBeforeRender: (args) => {
        // Registers the Parameter item and the Dashboard Panel.
        dashboardControl = args.component;

        currentDashBoardId = dashboardControl._controlOptions.dashboardId;
        
        //Adding ParameterItemExtension from parameter-item.js
        //dashboardControl.registerExtension(new ParameterItemExtension(dashboardControl));

        ////dashboardControl.registerExtension(new SaveAsDashboardExtension(dashboardControl));
        ////dashboardControl.registerExtension(new DeleteDashboardExtension(dashboardControl));

        // Registers the Dashboard Panel. //from parameter-item-ver2.js
        ////https://docs.devexpress.com/Dashboard/403047/web-dashboard/integrate-dashboard-component/dashboard-component-for-blazor/javascript-customization?utm_source=SupportCenter&utm_medium=website&utm_campaign=docs-feedback&utm_content=T1078022
        //try {
        //    // try catch in case parameter-item-ver2.js is not loaded
        //    dashboardControl.registerExtension(new ParameterCustomItem(dashboardControl));
        //}
        //catch (err) {
        //    //document.getElementById("demo").innerHTML = err.message;
        //}

        // Adds Dashboards (in Dashboards folder) viewer ability
        if (currentDashBoardId != undefined && currentDashBoardId == "DashboardEdit") {
            dashboardControl.registerExtension(new DevExpress.Dashboard.DashboardPanelExtension(dashboardControl));
        }

        if (currentDashBoardId != undefined && currentDashBoardId != "DashboardEdit") {
            // Removes the "New..." menu item from the dashboard menu.
            var toolbox = dashboardControl.findExtension('toolbox');
            //// HR Note: error occurs when switching to viewonly mode. Added toolbox check for undefined
            if (toolbox != undefined) {
                var createItem = toolbox.menuItems().filter(item => item.id === "create-dashboard")[0];
                toolbox.menuItems.remove(createItem);
            }
        }
    },
    // Adds a new custom toolbar item to the dashboard item caption.
    extensions: {
        viewerApi: {
            onItemCaptionToolbarUpdated: function (e) {
                if (e.itemName === "gridDashboardItem1") {
                    e.options.stateItems.push({
                        type: "menu",
                        icon: "baseCircle",
                        menu: {
                            type: "icons",
                            items: ["greenCircle", "yellowCircle", "redCircle"],
                            selectionMode: "none",
                            title: "Circles",
                            itemClick: function (itemData) {
                                alert(itemData.toString());
                            }
                        }
                    });
                }
            }
        }
    }
}