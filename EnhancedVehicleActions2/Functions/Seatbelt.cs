using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;

namespace EnhancedVehicleActions2
{
    class Seatbelt
    {
        private static int seatbeltMode = 0; //seatbelt mode 0: auto, seatbelt mode 1: always off, sealtbelt mode 2: always on

        public static void MainLogic()
        {
            GameFiber.StartNew(delegate
            {

                /*
                if (vehicle.Exists() && Game.LocalPlayer.Character.IsGettingIntoVehicle)
                {
                    if (seatbeltMode == 0)
                    {

                    }
                    else if (seatbeltMode == 1)
                    {
                        
                    }
                    else if (seatbeltMode == 2)
                    {
                        Game.LocalPlayer.Character.Tasks.Pause(3);
                        Game.LocalPlayer.Character.CanFlyThroughWindshields = false;
                    }
                }
                else if (Game.IsControlPressed(0, GameControl.VehicleExit) && Game.LocalPlayer.Character.IsInAnyVehicle(false))
                {
                    if (!Game.LocalPlayer.Character.CanFlyThroughWindshields) //seatbelt is on
                    {
                        Game.LocalPlayer.Character.Tasks.Pause(3);
                    }
                }
                */


                while (true)
                {
                    if (Game.LocalPlayer.Character.IsGettingIntoVehicle) //Entering vehicle
                    {
                        if (seatbeltMode == 0)
                        {
                            WearSeatbelt();
                        }
                    }
                    else if (Game.IsControlJustPressed(0, GameControl.VehicleExit)) //Exiting vehicle
                    {
                        if (!Game.LocalPlayer.Character.CanFlyThroughWindshields) //Seatbelt is on
                        {
                            Game.LocalPlayer.Character.Tasks.Pause(2);
                            Game.LocalPlayer.Character.Tasks.PlayAnimation(seatbeltAnimation, "std_hand_off_ps_passenger", 2f, AnimationFlags.UpperBodyOnly).WaitForCompletion(2000); Game.LocalPlayer.Character.CanFlyThroughWindshields = true;
                        }
                    }
                    /*
                    else if (Game.LocalPlayer.Character.IsInAnyVehicle(false)) //In vehicle
                    {
                        if (seatbeltMode == 0) //Seatbelt mode auto
                        {
                            
                        }
                    }*/

                    GameFiber.Yield();
                }     
            });
        }

        private static readonly AnimationDictionary seatbeltAnimation = new AnimationDictionary("oddjobs@taxi@cyi");
        private static void WearSeatbelt()
        {
            if (Game.LocalPlayer.Character.IsInAnyVehicle(false) && Game.LocalPlayer.Character.CurrentVehicle.IsCar)
            {
                Game.LocalPlayer.Character.Tasks.PlayAnimation(seatbeltAnimation, "std_hand_off_ps_passenger", 2f, AnimationFlags.SecondaryTask).WaitForCompletion(2000);
                Game.LocalPlayer.Character.CanFlyThroughWindshields = false;
            }
        }

        public static void ChangeMode(int mode)
        {
            if (mode == 1)
            {
                Game.LocalPlayer.Character.CanFlyThroughWindshields = true;
            }
            seatbeltMode = mode;
        }
    }
}
