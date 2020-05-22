using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;

namespace EnhancedVehicleActions2.Functions
{
    class Engine
    {
        //mode 0-auto, 1-always on, 2-always off
        public static int mode = 0;

        public Engine()
        {
            MainLogic();
        }

        public static void MainLogic()
        {
            GameFiber.StartNew(delegate
            {
                while (true)
                {
                    if (mode == 1)
                    {
                        if (Game.LocalPlayer.Character.CurrentVehicle.Exists())
                        {
                            if (!Game.LocalPlayer.Character.CurrentVehicle.IsEngineOn)
                            {
                                Game.LocalPlayer.Character.CurrentVehicle.IsEngineOn = true;
                            }
                        }
                        else if (Game.LocalPlayer.Character.LastVehicle.Exists())
                        {
                            if (!Game.LocalPlayer.Character.LastVehicle.IsEngineOn)
                            {
                                Game.LocalPlayer.Character.LastVehicle.IsEngineOn = true;
                            }
                        }
                        else
                        {
                            Game.LogTrivialDebug("Error: Valid vehicle not detected");
                        }
                    }
                    else if (mode == 2)
                    {
                        if (Game.LocalPlayer.Character.CurrentVehicle.Exists())
                        {
                            if (Game.LocalPlayer.Character.CurrentVehicle.IsEngineOn)
                            {
                                Game.LocalPlayer.Character.CurrentVehicle.IsEngineOn = false;
                            }
                        }
                        else if (Game.LocalPlayer.Character.LastVehicle.Exists())
                        {
                            if (Game.LocalPlayer.Character.LastVehicle.IsEngineOn)
                            {
                                Game.LocalPlayer.Character.LastVehicle.IsEngineOn = false;
                            }
                        }
                        else
                        {
                            Game.LogTrivialDebug("Error: Valid vehicle not detected");
                            return;
                        }
                    }
                    GameFiber.Yield();
                }
            });
        }
    }
}
