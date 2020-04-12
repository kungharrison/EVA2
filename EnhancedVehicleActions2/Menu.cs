using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System.IO;

namespace EnhancedVehicleActions2
{
    class Menu
    {
        private static GameFiber MenusProcessFiber;
        private static UIMenu mainMenu;
        private static MenuPool _menuPool;
        private static UIMenuCheckboxItem speedometerCheckbox;
        private static UIMenuCheckboxItem vehicleLockCheckbox;
        private static UIMenuCheckboxItem alarmKeyCheckbox;
        private static UIMenuCheckboxItem interiorLightKeyCheckbox;
        private static UIMenuListItem vehicleDoorsList;
        private static UIMenuListItem radioStationList;

        //iniFile information gathered
        private static readonly InitializationFile iniFile = EnhancedVehicleActions2.iniFile; //grabs iniFile from EnhancedVehicleActions2
        private static readonly bool isSpeedometerEnabled = iniFile.ReadBoolean("Options", "toggleableSpeedometer", true);
        public static readonly bool isRadioStationEnabled = iniFile.ReadBoolean("Options", "toggleableRadioStation", true);
        private static readonly bool isVehicleLockEnabled = iniFile.ReadBoolean("Options", "toggleableLock", true);
        //private static readonly bool isEngineSystemEnabled = iniFile.ReadBoolean("Options", "toggleableEngine", true);
        private static readonly System.Windows.Forms.Keys actionKey = (System.Windows.Forms.Keys)iniFile.ReadEnum(typeof(System.Windows.Forms.Keys), "KeyBindings", "ActionKey", System.Windows.Forms.Keys.F7);
        private static readonly ControllerButtons controllerActionKey = (ControllerButtons)iniFile.ReadEnum(typeof(ControllerButtons), "ControllerKeyBindings", "ControllerActionKey", ControllerButtons.RightThumb);

        public Menu()
        {
            MenusProcessFiber = new GameFiber(MainLogic);
            _menuPool = new MenuPool();
            mainMenu = new UIMenu("Vehicle Actions", "");
            _menuPool.Add(mainMenu);

            //Add items to mainMenu
            if (isSpeedometerEnabled) //checks if speedometer is enabled to add to pool
            {
                mainMenu.AddItem(speedometerCheckbox = new UIMenuCheckboxItem("Speedometer", false, "Enables/Disables Speedometer"));
            }
            if (isVehicleLockEnabled) //checks if speedometer is enabled to add to pool
            {
                mainMenu.AddItem(vehicleLockCheckbox = new UIMenuCheckboxItem("Vehicle Lock", false, "Locks/Unlocks Vehicle"));
            }
            mainMenu.AddItem(alarmKeyCheckbox = new UIMenuCheckboxItem("Car Alarm", false, "Toggles Car Alarm"));
            mainMenu.AddItem(interiorLightKeyCheckbox = new UIMenuCheckboxItem("Interior Light", false, "Toggles Interior Light"));
            mainMenu.AddItem(vehicleDoorsList = new UIMenuListItem("Vehicle Doors", "Select which door of your vehicle to open and close", "All", "Front left", "Front right", "Rear left", "Rear right", "Hood", "Trunk"));
            if (isRadioStationEnabled) //checks if radio station is enabled to add to pool
            {
                string[] radioList = new string[] { "Off", "Los Santos Rock Radio", "Non-Stop-Pop FM", "Radio Los Santos", "Channel X", "West Coast Talk Radio", "Rebel Radio", "Soulwax FM", "East Los FM", "West Coast Classics", "Blue Ark", "Worldwide FM", "FlyLo FM", "The Lowdown 91.1", "The Lab", "Radio Mirror Park", "Space 103.2", "Vinewood Boulevard Radio", "Blonded Los Santos 97.8", "Los Santos Underground", "Self Radio"};
                mainMenu.AddItem(radioStationList = new UIMenuListItem("Radio", "Sets default radio station to be used on all vehicles when entered", radioList));
            }

            mainMenu.OnCheckboxChange += OnCheckboxChange;
            mainMenu.OnItemSelect += OnItemSelect;
            mainMenu.OnListChange += OnListChange;

            MenusProcessFiber.Start();
            GameFiber.Hibernate();
        }

        public static void MainLogic()
        {
            GameFiber.StartNew(delegate
            {
                while (true)
                {
                    if (Game.IsKeyDown(actionKey))
                    {
                        mainMenu.Visible = !mainMenu.Visible;
                    }
                    if (Game.IsControllerButtonDownRightNow(controllerActionKey))
                    {
                        mainMenu.Visible = true;
                    }
                    _menuPool.ProcessMenus();
                    GameFiber.Yield();
                }
            });
        }

        public static void OnCheckboxChange(UIMenu sender, UIMenuCheckboxItem checkbox, bool Checked)
        {
            if (sender == mainMenu)
            {
                if (checkbox == speedometerCheckbox)
                {
                    if (Checked)
                    {
                        EnhancedVehicleActions2.isSpeedometerEnabled = true;
                    }
                    else
                    {
                        EnhancedVehicleActions2.isSpeedometerEnabled = false;
                        Game.DisplaySubtitle("");
                    }
                }
                else if (checkbox == alarmKeyCheckbox)
                {
                    EnhancedVehicleActions2.isAlarmEnabled = Checked;
                    EnhancedVehicleActions2.ToggleAlarm();
                }
                else if (checkbox == interiorLightKeyCheckbox)
                {
                    EnhancedVehicleActions2.isInteriorLightsEnabled = Checked;
                    EnhancedVehicleActions2.ToggleInteriorLights();
                }
                else if (checkbox == vehicleLockCheckbox)
                { 
                    if (Game.LocalPlayer.Character.Tasks.CurrentTaskStatus == TaskStatus.NoTask && !Game.LocalPlayer.Character.IsGettingIntoVehicle)
                    {
                        Game.LogTrivial("" + Game.LocalPlayer.Character.Tasks.CurrentTaskStatus);
                        EnhancedVehicleActions2.isVehicleLocked = Checked;
                        EnhancedVehicleActions2.ToggleVehicleLock();
                    }
                    else
                    {
                        vehicleLockCheckbox.Checked = !Checked;
                    }
                }
            }
        }

        public static void OnListChange(UIMenu sender, UIMenuListItem list, int index)
        {
            if (sender == mainMenu && list == radioStationList)
            {
                EnhancedVehicleActions2.intendedRadio = index - 1;
                EnhancedVehicleActions2.ToggleDefaultRadio();
               //iniFile.Write("Options", "defaultRadioStation", "None");
            }
        }

        public static void OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender == mainMenu && selectedItem == vehicleDoorsList)
            {
                if (vehicleDoorsList.SelectedItem.DisplayText == "All")
                {
                    EnhancedVehicleActions2.ActivateVehicleDoors();
                }
                else if (vehicleDoorsList.SelectedItem.DisplayText == "Front left")
                {
                    EnhancedVehicleActions2.ActivateVehicleDoors(0);
                }
                else if (vehicleDoorsList.SelectedItem.DisplayText == "Front right")
                {
                    EnhancedVehicleActions2.ActivateVehicleDoors(1);
                }
                else if (vehicleDoorsList.SelectedItem.DisplayText == "Rear left")
                {
                    EnhancedVehicleActions2.ActivateVehicleDoors(2);
                }
                else if (vehicleDoorsList.SelectedItem.DisplayText == "Rear right")
                {
                    EnhancedVehicleActions2.ActivateVehicleDoors(3);
                }
                else if (vehicleDoorsList.SelectedItem.DisplayText == "Hood")
                {
                    EnhancedVehicleActions2.ActivateVehicleDoors(4);
                }
                else if (vehicleDoorsList.SelectedItem.DisplayText == "Trunk")
                {
                    EnhancedVehicleActions2.ActivateVehicleDoors(5);
                }
            }
        }
    }
}
