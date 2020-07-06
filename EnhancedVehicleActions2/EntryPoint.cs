using EnhancedVehicleActions2.Functions;
using Rage;
using Rage.Native;
using System;
using System.Security.Policy;

[assembly: Rage.Attributes.Plugin("EnhancedVehicleActions2", Description = "Enhanced vehicle actions and more", Author = "holexion")]

namespace EnhancedVehicleActions2
{
    public class EntryPoint
    {
        //iniFile contains player keybindings

        public static InitializationFile iniFile = new InitializationFile("plugins/EnhancedVehicleActions2.ini");
        public static void Main()
        {
            //grabs options from iniFile
            bool isTireRetainmentEnabled = iniFile.ReadBoolean("Options", "tireRetainment", true);
            bool isSpeedometerEnabled = iniFile.ReadBoolean("Options", "toggleableSpeedometer", true);
            bool isDoorSystemEnabled = iniFile.ReadBoolean("Options", "doorSystem", true);
            bool isBrakeLightsEnabled = iniFile.ReadBoolean("Options", "brakeLights", true);


            if (isTireRetainmentEnabled)
            {
                TireRetainment();
            }
            if (isSpeedometerEnabled)
            {
                Speedometer();
            }
            if (isBrakeLightsEnabled)
            {
                BrakeLights();
            }
            if (isDoorSystemEnabled)
            {
                _ = new DoorSystem();
            }

            VehicleIndicators(); //activates vehicle indicators
            KeyPolling(); //activates key polling
            _ = new Menu(); //activates menu
            Game.LogTrivial("Enhanced Vehicle Actions 2 sucessfully loaded");
        }

        public static void KeyPolling()
        {
            //grabs keybindings from iniFile
            System.Type keysType = typeof(System.Windows.Forms.Keys);
            System.Windows.Forms.Keys rightBlinkerKey = (System.Windows.Forms.Keys)iniFile.ReadEnum(keysType, "KeyBindings", "RightBlinker", System.Windows.Forms.Keys.OemCloseBrackets);
            System.Windows.Forms.Keys leftBlinkerKey = (System.Windows.Forms.Keys)iniFile.ReadEnum(keysType, "KeyBindings", "LeftBlinker", System.Windows.Forms.Keys.OemOpenBrackets);
            System.Windows.Forms.Keys hazardsKey = (System.Windows.Forms.Keys)iniFile.ReadEnum(keysType, "KeyBindings", "Hazards", System.Windows.Forms.Keys.OemPipe);
            System.Windows.Forms.Keys engineKey = (System.Windows.Forms.Keys)iniFile.ReadEnum(keysType, "KeyBindings", "Engine", System.Windows.Forms.Keys.None);

            //grabs controller keybindings from iniFile
            System.Type controllerType = typeof(ControllerButtons);
            //ControllerButtons altKey = (ControllerButtons)iniFile.ReadEnum(controllerType, "ControllerKeyBindings", "AlternativeKey", ControllerButtons.A);
            ControllerButtons contRightBlinkerKey = (ControllerButtons)iniFile.ReadEnum(controllerType, "ControllerKeyBindings", "RightBlinker", ControllerButtons.RightShoulder);
            ControllerButtons contLeftBlinkerKey = (ControllerButtons)iniFile.ReadEnum(controllerType, "ControllerKeyBindings", "LeftBlinker", ControllerButtons.LeftShoulder);

            GameFiber.StartNew(delegate
            {
                while (true)
                {
                    if (Game.IsKeyDown(rightBlinkerKey) || (Game.IsControllerButtonDown(contRightBlinkerKey))) //&& !Game.IsControllerButtonDown(altKey)))
                    {
                        intendedStatus = VehicleIndicatorLightsStatus.RightOnly;
                    }
                    if (Game.IsKeyDown(leftBlinkerKey) || (Game.IsControllerButtonDown(contLeftBlinkerKey))) //&& !Game.IsControllerButtonDown(altKey)))
                    {
                        intendedStatus = VehicleIndicatorLightsStatus.LeftOnly;
                    }
                    if (Game.IsKeyDown(hazardsKey) || (Game.IsControllerButtonDown(contRightBlinkerKey) && Game.IsControllerButtonDownRightNow(contLeftBlinkerKey))) //&& !Game.IsControllerButtonDown(altKey)))
                    {
                        intendedStatus = VehicleIndicatorLightsStatus.Both;
                    }
                    if (Game.LocalPlayer.Character.IsGettingIntoVehicle) //if player is getting into vehicle, activate
                    {
                        ToggleDefaultRadio(); //change radio to default
                    }
                    if (Game.IsKeyDown(engineKey))
                    {
                        int mode = Engine.mode;
                        if (mode == 1)
                        {
                            Engine.mode = 2;
                        }
                        else if (mode == 2)
                        {
                            Engine.mode = 1;
                        }
                    }
                    /*
                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Scroll)) //DEBUG MODE ONLY
                    {
                        Rage.Native.NativeFunction.Natives.ROLL_UP_WINDOW(Game.LocalPlayer.Character.CurrentVehicle, 0);
                        Rage.Native.NativeFunction.Natives.ROLL_UP_WINDOW(Game.LocalPlayer.Character.CurrentVehicle, 1);
                        Rage.Native.NativeFunction.Natives.ROLL_UP_WINDOW(Game.LocalPlayer.Character.CurrentVehicle, 2);
                        Rage.Native.NativeFunction.Natives.ROLL_UP_WINDOW(Game.LocalPlayer.Character.CurrentVehicle, 3);
                    }
                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Pause)) //DEBUG MODE ONLY
                    {
                        Rage.Native.NativeFunction.Natives.ROLL_DOWN_WINDOWS(Game.LocalPlayer.Character.CurrentVehicle);
                    }
                    
                    if (Game.IsControlJustPressed(0, GameControl.VehicleExit) && Game.LocalPlayer.Character.IsInAnyVehicle(false))
                    {
                        Game.LocalPlayer.Character.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                        Game.LocalPlayer.Character.Tasks.PlayAnimation("veh@low@front_ds@exit_to_aim_1h", "ds_get_out_north", 2f, AnimationFlags.SecondaryTask | AnimationFlags.UpperBodyOnly);
                    }
                    if (Game.IsControlJustReleased(75, GameControl.VehicleExit) && Game.LocalPlayer.Character.IsInAnyVehicle(false))
                    {
                        ExitVehicle(false);
                    }
                    */
                    GameFiber.Yield();
                }
            });
        }

        public static void BrakeLights()
        {
            GameFiber.StartNew(delegate
            {
                while (true)
                {
                    Vehicle playerVehicle = Game.LocalPlayer.Character.CurrentVehicle;
                    Vehicle lastPlayerVehicle = Game.LocalPlayer.Character.LastVehicle;

                    if (playerVehicle.Exists() && playerVehicle.Speed < 0.1)
                    {
                        Rage.Native.NativeFunction.Natives.SET_VEHICLE_BRAKE_LIGHTS(Game.LocalPlayer.Character.CurrentVehicle, true);
                    }
                    else if (lastPlayerVehicle.Exists() && lastPlayerVehicle.Speed < 0.1)
                    {
                        Rage.Native.NativeFunction.Natives.SET_VEHICLE_BRAKE_LIGHTS(Game.LocalPlayer.Character.LastVehicle, true);
                    }
                    GameFiber.Yield();
                }
            });
        }


        public static void TireRetainment()
        {
            float steeringAngle = 0;
            bool saveSteeringLocation = false;

            GameFiber.StartNew(delegate
            {
                while (true)
                {
                    Vehicle playerVehicle = Game.LocalPlayer.Character.CurrentVehicle;
                    Vehicle lastPlayerVehicle = Game.LocalPlayer.Character.LastVehicle;

                    if (playerVehicle.Exists() && playerVehicle.Speed < 1 && !saveSteeringLocation)
                    {
                        if (Game.IsControlPressed(1, GameControl.VehicleMoveLeftOnly) || Game.IsControlPressed(1, GameControl.VehicleMoveRightOnly))
                        {
                            steeringAngle = playerVehicle.SteeringAngle * 1.2f;
                        }
                        else if (!Game.IsControlPressed(1, GameControl.VehicleMoveLeftOnly) && !Game.IsControlPressed(1, GameControl.VehicleMoveRightOnly))
                        {
                            playerVehicle.SteeringAngle = steeringAngle;
                            saveSteeringLocation = true;
                        }
                    }
                    else if (playerVehicle.Exists() && (playerVehicle.Speed > 1 || Game.IsControlPressed(1, GameControl.VehicleMoveLeftOnly) || Game.IsControlPressed(1, GameControl.VehicleMoveRightOnly)))
                    {
                        saveSteeringLocation = false;
                        steeringAngle = 0f;
                    }
                    else if (saveSteeringLocation)
                    {
                        if (playerVehicle.Exists())
                        {
                            playerVehicle.SteeringAngle = steeringAngle;
                        }
                        else if (lastPlayerVehicle.Exists())
                        {
                            lastPlayerVehicle.SteeringAngle = steeringAngle;
                        }
                    }
                    GameFiber.Yield();
                }
            });
        }

        //isSpeedometerEnabled activates/deactivates Speedometer() through in-game Menu
        public static bool isSpeedometerEnabled = false;
        private static void Speedometer()
        {
            GameFiber.StartNew(delegate
            {
                while (true)
                {
                    if (isSpeedometerEnabled && Game.LocalPlayer.Character.IsInAnyVehicle(false))
                    {
                        float vehicleSpeed = Game.LocalPlayer.Character.CurrentVehicle.Speed * 2.23694f;
                        Game.DisplaySubtitle("Speed: " + System.Math.Round(vehicleSpeed).ToString() + " MPH   Gear: " + Game.LocalPlayer.Character.CurrentVehicle.CurrentGear);
                    }
                    GameFiber.Yield();
                }
            });
        }

        public static bool isInteriorLightsEnabled = false;
        public static void ToggleInteriorLights()
        {
            Vehicle vehicle;

            if (Game.LocalPlayer.Character.CurrentVehicle.Exists())
            {
                vehicle = Game.LocalPlayer.Character.CurrentVehicle;
            }
            else if (Game.LocalPlayer.Character.LastVehicle.Exists())
            {
                vehicle = Game.LocalPlayer.Character.LastVehicle;
            }
            else
            {
                Game.LogTrivialDebug("Error: Valid vehicle not detected");
                return;
            }

            if (isInteriorLightsEnabled)
            {
                vehicle.IsInteriorLightOn = true;
            }
            else
            {
                vehicle.IsInteriorLightOn = false;
            }
            Game.LogTrivialDebug("Interior Lights Activated");
        }

        public static int intendedRadio = -1;
        public static void ToggleDefaultRadio()
        {
            Vehicle vehicle;

            if (Game.LocalPlayer.Character.CurrentVehicle.Exists())
            {
                vehicle = Game.LocalPlayer.Character.CurrentVehicle;
            }
            else
            {
                return;
            }

            if (Menu.isRadioStationEnabled)
            {
                vehicle.RadioStation = (RadioStation)intendedRadio;
            }
        }

        public static bool isAlarmEnabled = false;
        public static void ToggleAlarm()
        {
            Vehicle vehicle;

            if (Game.LocalPlayer.Character.CurrentVehicle.Exists())
            {
                vehicle = Game.LocalPlayer.Character.CurrentVehicle;
            }
            else if (Game.LocalPlayer.Character.LastVehicle.Exists())
            {
                vehicle = Game.LocalPlayer.Character.LastVehicle;
            }
            else
            {
                Game.LogTrivialDebug("Error: Valid vehicle not detected");
                return;
            }

            if (isAlarmEnabled)
            {
                vehicle.AlarmTimeLeft = System.TimeSpan.FromMinutes(5);
            }
            else
            {
                vehicle.AlarmTimeLeft = System.TimeSpan.Zero;
            }
            Game.LogTrivialDebug("Alarm Activated");
        }

        public static bool isVehicleLocked = false;
        public static void ToggleVehicleLock()
        {
            Vehicle vehicle;

            if (Game.LocalPlayer.Character.CurrentVehicle.Exists())
            {
                vehicle = Game.LocalPlayer.Character.CurrentVehicle;
            }
            else if (Game.LocalPlayer.Character.LastVehicle.Exists())
            {
                vehicle = Game.LocalPlayer.Character.LastVehicle;
            }
            else
            {
                Game.LogTrivialDebug("Error: Valid vehicle not detected");
                return;
            }

            if (isVehicleLocked)
            {
                vehicle.LockStatus = VehicleLockStatus.Locked;
            }
            else
            {
                vehicle.LockStatus = VehicleLockStatus.Unlocked;
            }
        }

        public static bool areWindowsUp = true;
        public static void ToggleWindows()
        {
            if (Game.LocalPlayer.Character.CurrentVehicle.Exists())
            {
                if (!areWindowsUp)
                {
                    Rage.Native.NativeFunction.Natives.ROLL_DOWN_WINDOWS(Game.LocalPlayer.Character.CurrentVehicle);
                    areWindowsUp = !areWindowsUp;
                }
                else
                {
                    Rage.Native.NativeFunction.Natives.ROLL_UP_WINDOW(Game.LocalPlayer.Character.CurrentVehicle, 0);
                    Rage.Native.NativeFunction.Natives.ROLL_UP_WINDOW(Game.LocalPlayer.Character.CurrentVehicle, 1);
                    Rage.Native.NativeFunction.Natives.ROLL_UP_WINDOW(Game.LocalPlayer.Character.CurrentVehicle, 2);
                    Rage.Native.NativeFunction.Natives.ROLL_UP_WINDOW(Game.LocalPlayer.Character.CurrentVehicle, 3);
                    areWindowsUp = !areWindowsUp;
                }
            }
        }

        //Activates door using index
        public static void ActivateVehicleDoors(int index)
        {
            Vehicle vehicle;

            if (Game.LocalPlayer.Character.CurrentVehicle.Exists())
            {
                vehicle = Game.LocalPlayer.Character.CurrentVehicle;
            }
            else if (Game.LocalPlayer.Character.LastVehicle.Exists())
            {
                vehicle = Game.LocalPlayer.Character.LastVehicle;
            }
            else
            {
                Game.LogTrivialDebug("Error: Valid vehicle not detected");
                return;
            }

            if (vehicle.Doors[index].IsValid())
            {
                if (vehicle.Doors[index].IsOpen)
                {
                    vehicle.Doors[index].Close(false);
                }
                else
                {
                    vehicle.Doors[index].Open(false);
                }
            }
        }
        public static void ActivateVehicleDoors()
        {
            Vehicle vehicle;

            if (Game.LocalPlayer.Character.CurrentVehicle.Exists())
            {
                vehicle = Game.LocalPlayer.Character.CurrentVehicle;
            }
            else if (Game.LocalPlayer.Character.LastVehicle.Exists())
            {
                vehicle = Game.LocalPlayer.Character.LastVehicle;
            }
            else
            {
                return;
            }

            if (CheckAllDoorsOpen(vehicle))
            {
                for (int i = 0; i < 6; i++)
                {
                    if (vehicle.Doors[i].IsValid())
                    {
                        ActivateVehicleDoors(i);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    if (vehicle.Doors[i].IsValid() && !vehicle.Doors[i].IsOpen)
                    {
                        ActivateVehicleDoors(i);
                    }
                }
            }
        }

        //Checks if all doors in a vehicle have the same state (all doors are opened/closed) returns true if yes, false if no
        private static bool CheckAllDoorsOpen(Vehicle vehicle)
        {
            bool areAllDoorsOpen = true;
            for (int i = 0; i < 6; i++)
            {
                if (vehicle.Doors[i].IsValid())
                {
                    if (!vehicle.Doors[i].IsOpen)
                    {
                        areAllDoorsOpen = false;
                    }
                }
            }
            return areAllDoorsOpen;
        }

        static VehicleIndicatorLightsStatus status = VehicleIndicatorLightsStatus.Off;
        static VehicleIndicatorLightsStatus intendedStatus = VehicleIndicatorLightsStatus.Off;
        static float initialHeading = 0f;
        static uint turnOffAt = 0;
        private static void VehicleIndicators()
        {
            GameFiber.StartNew(delegate
            {
                while (true)
                {
                    Vehicle playerVehicle = Game.LocalPlayer.Character.CurrentVehicle;
                    if (playerVehicle.Exists())
                    {
                        if (intendedStatus == VehicleIndicatorLightsStatus.RightOnly && playerVehicle.IsEngineOn) //right blinker activates with close bracket
                        {
                            Game.LogTrivialDebug("Right Blinker activated");
                            turnOffAt = 0;
                            if (status == VehicleIndicatorLightsStatus.RightOnly)
                            {
                                status = VehicleIndicatorLightsStatus.Off;
                            }
                            else
                            {
                                status = VehicleIndicatorLightsStatus.RightOnly;
                                initialHeading = playerVehicle.Heading;
                            }
                            playerVehicle.IndicatorLightsStatus = status;
                        }
                        else if (intendedStatus == VehicleIndicatorLightsStatus.LeftOnly && playerVehicle.IsEngineOn) //left blinker activates with close bracket
                        {
                            Game.LogTrivialDebug("Left Blinker activated");
                            turnOffAt = 0;
                            if (status == VehicleIndicatorLightsStatus.LeftOnly)
                            {
                                status = VehicleIndicatorLightsStatus.Off;
                            }
                            else
                            {
                                status = VehicleIndicatorLightsStatus.LeftOnly;
                                initialHeading = playerVehicle.Heading;
                            }
                            playerVehicle.IndicatorLightsStatus = status;
                        }
                        else if (intendedStatus == VehicleIndicatorLightsStatus.Both) //left blinker activates with close bracket
                        {
                            Game.LogTrivialDebug("Hazards activated");
                            if (status == VehicleIndicatorLightsStatus.Both)
                            {
                                status = VehicleIndicatorLightsStatus.Off;
                            }
                            else
                            {
                                status = VehicleIndicatorLightsStatus.Both;
                            }
                            playerVehicle.IndicatorLightsStatus = status;
                        }

                        if (status != VehicleIndicatorLightsStatus.Both)
                        {
                            if (turnOffAt == 0u)
                            {
                                if (status != VehicleIndicatorLightsStatus.Off)
                                {
                                    float heading = playerVehicle.Heading;
                                    if (System.Math.Abs(heading - initialHeading) > 60.0f)
                                    {
                                        turnOffAt = Game.GameTime + 1500;
                                    }
                                }
                            }
                            else
                            {
                                if (Game.GameTime >= turnOffAt)
                                {
                                    status = VehicleIndicatorLightsStatus.Off;
                                    playerVehicle.IndicatorLightsStatus = status;
                                }
                            }
                        }
                    }
                    intendedStatus = VehicleIndicatorLightsStatus.Off;
                    GameFiber.Yield();
                }
            });
        }
    }
}
