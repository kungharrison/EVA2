using Rage;

namespace EnhancedVehicleActions2
{
    class DoorSystem
    {
        public DoorSystem()
        {
            MainLogic();
        }

        enum CurrentWeapon
        {
            None,
            Primary,
            Secondary
        }

        public static void MainLogic()
        {
            Game.LogTrivialDebug("Door System Enabled");

            //grabs options from iniFile
            bool isAnimationEnabled = EntryPoint.iniFile.ReadBoolean("Other", "felonyStopAnimations", true);

            //grabs player's preferred primary and secondary weapons for felony stop exit animations
            System.Type weaponType = typeof(WeaponHash);
            WeaponHash primaryWeapon = (WeaponHash)EntryPoint.iniFile.ReadEnum(weaponType, "Other", "primaryWeapon", WeaponHash.PumpShotgun);
            WeaponHash secondaryWeapon = (WeaponHash)EntryPoint.iniFile.ReadEnum(weaponType, "Other", "secondaryWeapon", WeaponHash.CombatPistol);

            AnimationDictionary jeepExit1h = new AnimationDictionary("veh@jeep@mesa@ds@exit_to_aim_1h"); //get_out_north
            AnimationDictionary jeepExit2h = new AnimationDictionary("veh@jeep@mesa@ds@exit_to_aim_2h");
            //AnimationDictionary vanExit1h = new AnimationDictionary("veh@van@policet@ds@exit_to_aim"); //get_out_north
            //AnimationDictionary vanExit2h = new AnimationDictionary("veh@van@riot@ds@exit_to_aim_2h");
            AnimationDictionary lowExit1h = new AnimationDictionary("veh@low@front_ds@exit_to_aim_1h"); //ds_get_out_north
            AnimationDictionary lowExit2h = new AnimationDictionary("veh@low@front_ds@exit_to_aim_2h");
            AnimationDictionary stdExit1h = new AnimationDictionary("veh@std@ds@exit_to_aim_1h"); //ds_get_out_north
            AnimationDictionary stdExit2h = new AnimationDictionary("veh@std@ds@exit_to_aim_2h");

            CurrentWeapon playerWeapon = CurrentWeapon.None;

            GameFiber.StartNew(delegate
            {
                while (true)
                {
                    Ped player = Game.LocalPlayer.Character;

                    if (Game.IsControlJustPressed(0, GameControl.VehicleExit) && player.IsInAnyVehicle(false))
                    {
                        if (Game.IsControlPressed(0, GameControl.Aim)) //Use felony stop animations
                        {
                            player.Tasks.Pause(300);
                            GameFiber.Wait(299);

                            if (!Game.IsControlPressed(0, GameControl.VehicleExit) && player.IsInAnyVehicle(false) && player.Inventory.Weapons.Contains(secondaryWeapon)) //Short press will activate
                            {
                                player.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                                player.Inventory.EquippedWeapon = secondaryWeapon;

                                playerWeapon = CurrentWeapon.Secondary;
                            }
                            else if (Game.IsControlPressed(0, GameControl.VehicleExit) && player.IsInAnyVehicle(false) && player.Inventory.Weapons.Contains(primaryWeapon)) //Long press will activate
                            {
                                player.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                                player.Inventory.EquippedWeapon = primaryWeapon;

                                playerWeapon = CurrentWeapon.Primary;
                            }
                            else
                            {
                                playerWeapon = CurrentWeapon.None;
                            }

                            VehicleClass vehicleClass = player.CurrentVehicle.Class;
                            Model vehicleModel = player.CurrentVehicle.Model;
                            if (isAnimationEnabled && vehicleClass == VehicleClass.Emergency)
                            {
                                if (vehicleModel == new Model("sheriff2") || vehicleModel == new Model("policeold1") || vehicleModel == new Model("riot") || vehicleModel == new Model("policet"))
                                {
                                    if (playerWeapon == CurrentWeapon.Primary)
                                    {
                                        player.Tasks.PlayAnimation(jeepExit2h, "get_out_north", 2f, AnimationFlags.SecondaryTask).WaitForCompletion(2000);
                                    }
                                    else if (playerWeapon == CurrentWeapon.Secondary)
                                    {
                                        player.Tasks.PlayAnimation(jeepExit1h, "get_out_north", 2f, AnimationFlags.SecondaryTask).WaitForCompletion(2000);
                                    }
                                }                  
                                else
                                {
                                    if (playerWeapon == CurrentWeapon.Primary)
                                    {
                                        player.Tasks.PlayAnimation(lowExit2h, "ds_get_out_north", 2f, AnimationFlags.SecondaryTask).WaitForCompletion(2000);
                                    }
                                    else if (playerWeapon == CurrentWeapon.Secondary)
                                    {
                                        player.Tasks.PlayAnimation(stdExit1h, "ds_get_out_north", 2f, AnimationFlags.SecondaryTask).WaitForCompletion(2000);
                                    }
                                }
                            }
                        }
                        else //Felony stop animations will not be used
                        {
                            player.Tasks.Pause(300);
                            GameFiber.Wait(299);

                            if (!Game.IsControlPressed(0, GameControl.VehicleExit) && player.IsInAnyVehicle(false)) //Short press will activate
                            {
                                player.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                            }
                        }
                    }
                    GameFiber.Yield();
                }
            });
        }
    }
}
