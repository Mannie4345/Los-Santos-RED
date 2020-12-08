﻿using ExtensionsMethods;
using LosSantosRED.lsr;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class RespawnManager
{
    private static int BailFee;
    private static uint GameTimeLastUndied;
    private static uint GameTimeLastRespawned;
    private static uint GameTimeLastSurrenderedToPolice;
    private static uint GameTimeLastBribedPolice;
    private static uint GameTimeLastDischargedFromHospital;
    private static uint GameTimeLastResistedArrest;
    private static uint GameTimeLastTalkedToPolice;
    private static int HospitalBillPastDue;
    private static int BailFeePastDue;
    public static bool RecentlyUndied
    {
        get
        {
            if (GameTimeLastUndied == 0)
                return false;
            else if (Game.GameTime - GameTimeLastUndied <= 5000)
                return true;
            else
                return false;
        }
    }
    public static bool RecentlyRespawned
    {
        get
        {
            if (GameTimeLastRespawned == 0)
                return false;
            else if (Game.GameTime - GameTimeLastRespawned <= 5000)
                return true;
            else
                return false;
        }
    }
    public static bool RecentlySurrenderedToPolice
    {
        get
        {
            if (GameTimeLastSurrenderedToPolice == 0)
                return false;
            else if (Game.GameTime - GameTimeLastSurrenderedToPolice <= 5000)
                return true;
            else
                return false;
        }
    }
    public static bool RecentlyBribedPolice
    {
        get
        {
            if (GameTimeLastBribedPolice == 0)
                return false;
            else if (Game.GameTime - GameTimeLastBribedPolice <= 10000)
                return true;
            else
                return false;
        }
    }
    public static bool RecentlyDischargedFromHospital
    {
        get
        {
            if (GameTimeLastDischargedFromHospital == 0)
                return false;
            else if (Game.GameTime - GameTimeLastDischargedFromHospital <= 5000)
                return true;
            else
                return false;
        }
    }
    public static bool RecentlyResistedArrest
    {
        get
        {
            if (GameTimeLastResistedArrest == 0)
                return false;
            else if (Game.GameTime - GameTimeLastResistedArrest <= 5000)
                return true;
            else
                return false;
        }
    }
    public static bool RecentlyTalkedtoPolice
    {
        get
        {
            if (GameTimeLastTalkedToPolice == 0)
                return false;
            else if (Game.GameTime - GameTimeLastTalkedToPolice <= 5000)
                return true;
            else
                return false;
        }
    }
    public static void UnDie()
    {
        GameTimeLastUndied = Game.GameTime;
        RespawnInPlace(true);
        ScannerManager.AbortAudio();
        Game.LocalPlayer.Character.IsInvincible = true;
        GameFiber.StartNew(delegate
        {
            GameFiber.Sleep(5000);
            Game.LocalPlayer.Character.IsInvincible = false;
        });
    }
    public static void RespawnInPlace(bool AsOldCharacter)
    {
        try
        {
            Mod.Player.ResetState(false);
            Game.LocalPlayer.Character.Health = Game.LocalPlayer.Character.MaxHealth;
            NativeFunction.Natives.xB69317BF5E782347(Game.LocalPlayer.Character);//"NETWORK_REQUEST_CONTROL_OF_ENTITY" 
            if (Mod.Player.DiedInVehicle)
            {
                NativeFunction.Natives.xEA23C49EAA83ACFB(Game.LocalPlayer.Character.Position.X + 10f, Game.LocalPlayer.Character.Position.Y, Game.LocalPlayer.Character.Position.Z, 0, false, false);//"NETWORK_RESURRECT_LOCAL_PLAYER"
                if (Game.LocalPlayer.Character.LastVehicle.Exists() && Game.LocalPlayer.Character.LastVehicle.IsDriveable)
                {
                    Game.LocalPlayer.Character.WarpIntoVehicle(Game.LocalPlayer.Character.LastVehicle, -1);
                }
            }
            else
            {
                NativeFunction.Natives.xEA23C49EAA83ACFB(Game.LocalPlayer.Character.Position.X, Game.LocalPlayer.Character.Position.Y, Game.LocalPlayer.Character.Position.Z, 0, false, false);//"NETWORK_RESURRECT_LOCAL_PLAYER"
            }
            NativeFunction.Natives.xC0AA53F866B3134D();//_RESET_LOCALPLAYER_STATE
            if (AsOldCharacter)
            {
                ResetPlayer(false, false);
                WantedLevelManager.SetWantedLevel(Mod.Player.MaxWantedLastLife, "Resetting to max wanted last life after respawn in place", true);
                ++Mod.Player.TimesDied;
            }
            else
            {
                ResetPlayer(true, true);
                Game.LocalPlayer.Character.Inventory.Weapons.Clear();
                Mod.Player.LastWeaponHash = 0;
                Mod.PolicePerception.PreviousWantedLevel = 0;
                Mod.Player.TimesDied = 0;
                Mod.Player.MaxWantedLastLife = 0;
            }
            GameTimeLastRespawned = Game.GameTime;
            Game.HandleRespawn();
            ScannerManager.AbortAudio();
            Mod.ClockManager.UnpauseTime();
        }
        catch (Exception e)
        {
            Debugging.WriteToLog("RespawnInPlace", e.Message);
        }
    }
    public static void SurrenderToPolice(GameLocation PoliceStation)
    {
        FadeOut();
        CheckWeapons();
        BailFee = Mod.Player.MaxWantedLastLife * SettingsManager.MySettings.Police.PoliceBailWantedLevelScale;//max wanted last life wil get reset when calling resetplayer
        Mod.Player.ResetState(true);
        SurrenderManager.RaiseHands();
        ResetPlayer(true, true);
        if (PoliceStation == null)
            PoliceStation = LocationManager.GetClosestLocation(Game.LocalPlayer.Character.Position, LocationType.Police);
        SetPlayerAtLocation(PoliceStation);
        Game.LocalPlayer.Character.Tasks.ClearImmediately();
        Mod.PedManager.ClearPolice();
        VehicleManager.ClearPolice();
        FadeIn();
        SetPoliceFee(PoliceStation.Name, BailFee);
        GameTimeLastSurrenderedToPolice = Game.GameTime;
    }
    public static void BribePolice(int Amount)
    {
        if (Game.LocalPlayer.Character.IsRagdoll || Game.LocalPlayer.Character.IsSwimming)
            return;

        if (Game.LocalPlayer.Character.GetCash() < Amount)
        {
            Game.DisplayNotification("CHAR_BANK_FLEECA", "CHAR_BANK_FLEECA", "FLEECA Bank", "Overdrawn Notice", string.Format("Current transaction would overdraw account. Denied.", Amount));
            return;
        }

        if (Amount < (Mod.PolicePerception.PreviousWantedLevel * SettingsManager.MySettings.Police.PoliceBribeWantedLevelScale))
        {
            Game.DisplayNotification("CHAR_BLANK_ENTRY", "CHAR_BLANK_ENTRY", "Officer Friendly", "Expedited Service Fee", string.Format("Thats it? ${0}?", Amount));
            Game.LocalPlayer.Character.GiveCash(-1 * Amount);
            return;
        }
        else
        {
            GameTimeLastBribedPolice = Game.GameTime;
            Game.DisplayNotification("CHAR_BLANK_ENTRY", "CHAR_BLANK_ENTRY", "Officer Friendly", "Expedited Service Fee", "Thanks for the cash, now beat it.");
            Game.LocalPlayer.Character.GiveCash(-1 * Amount);
            SurrenderManager.UnSetArrestedAnimation(Game.LocalPlayer.Character);
            ResetPlayer(true, false);

            //Animation goes here if you want to add it somehow
        }
    }
    public static void RespawnAtHospital(GameLocation Hospital)
    {
        FadeOut();
        Mod.Player.ResetState(true);
        RespawnInPlace(false);
        if (Hospital == null)
            Hospital = LocationManager.GetClosestLocation(Game.LocalPlayer.Character.Position, LocationType.Hospital);
        SetPlayerAtLocation(Hospital);
        GameTimeLastDischargedFromHospital = Game.GameTime;
        Mod.PedManager.ClearPolice();
        VehicleManager.ClearPolice();
        SetHospitalFee(Hospital.Name);
        FadeIn();   
    }
    public static void ResistArrest()
    {
        Mod.Player.ResetState(false);//maxwanted last life maybe wont work?
        WantedLevelManager.RefreshPoliceState();
        WantedLevelManager.SetWantedLevel(Mod.Player.WantedLevel, "Resisting Arrest", true);
        SurrenderManager.UnSetArrestedAnimation(Game.LocalPlayer.Character);
        NativeFunction.CallByName<uint>("RESET_PLAYER_ARREST_STATE", Game.LocalPlayer);
        ResetPlayer(false, false);
        GameTimeLastResistedArrest = Game.GameTime;
    }
    public static void Talk()
    {
        //GTACop ClosestCop = PoliceScanning.CopPeds.Where(x => x.Pedestrian.Exists() && x.Pedestrian.IsAlive).OrderBy(x => x.DistanceToPlayer).FirstOrDefault();
        //MovePlayerToCop(ClosestCop,false,0);
        GameTimeLastTalkedToPolice = Game.GameTime;
        Game.DisplayHelp("~INPUT_SELECT_WEAPON_UNARMED~ \"Hello Officer, what seems to be the problem?\",~INPUT_SELECT_WEAPON_MELEE~ \"Am I being Detained?\"", 8000);
    }
    private static bool BribePoliceAnimation(int Amount)//temp public
    {
        GameFiber.StartNew(delegate
        {
            Cop CopToBribe = Mod.PedManager.Cops.Where(x => x.Pedestrian.Exists() && x.Pedestrian.IsAlive).OrderBy(x => x.DistanceToPlayer).FirstOrDefault();
            NativeFunction.Natives.xB4EDDC19532BFB85(); //_STOP_ALL_SCREEN_EFFECTS;
            Game.TimeScale = 1.0f;
            //CopToBribe.SetUnarmed();

            SurrenderManager.UnSetArrestedAnimation(Game.LocalPlayer.Character);

            while (NativeFunction.CallByName<bool>("IS_ENTITY_PLAYING_ANIM", Game.LocalPlayer.Character, "random@arrests", "kneeling_arrest_escape", 1))
                GameFiber.Wait(250);


            GameFiber.Wait(2000);

            if (!CopToBribe.Pedestrian.Exists())
                return;

            CopToBribe.Pedestrian.BlockPermanentEvents = true;
            CopToBribe.Pedestrian.IsPositionFrozen = true;


            Ped PedToMove = Game.LocalPlayer.Character;
            Ped PedToMoveTo = CopToBribe.Pedestrian;
            Vector3 OriginalPosition = PedToMoveTo.Position;

            bool Continue = true;
            Vector3 PositionToMoveTo = PedToMoveTo.GetOffsetPositionFront(1f);
            float DesiredHeading = PedToMoveTo.Heading - 180;
            NativeFunction.CallByName<uint>("TASK_PED_SLIDE_TO_COORD", PedToMove, PositionToMoveTo.X, PositionToMoveTo.Y, PositionToMoveTo.Z, DesiredHeading);
            uint GameTimeStarted = Game.GameTime;
            while (Game.GameTime - GameTimeStarted <= 15000 && !(PedToMove.DistanceTo2D(PositionToMoveTo) <= 0.15f && PedToMove.FacingOppositeDirection(PedToMoveTo)))// PedToMove.Heading.IsWithin(DesiredHeading - 15f, DesiredHeading + 15f)))
            {
                GameFiber.Yield();
                if (Extensions.IsMoveControlPressed() || PedToMoveTo.DistanceTo2D(OriginalPosition) >= 0.1f)
                {
                    Continue = false;
                    break;
                }
            }
            if (!Continue)
            {
                CopToBribe.Pedestrian.BlockPermanentEvents = false;
                CopToBribe.Pedestrian.IsPositionFrozen = false;
                PedToMove.Tasks.Clear();
                return;
            }

            AnimationDictionary AnimDictionary = new AnimationDictionary("mp_common");

            NativeFunction.CallByName<bool>("TASK_PLAY_ANIM", Game.LocalPlayer.Character, "mp_common", "givetake1_a", 8.0f, -8.0f, -1, 2, 0, false, false, false);
            NativeFunction.CallByName<uint>("TASK_PLAY_ANIM", PedToMoveTo, "mp_common", "givetake1_b", 8.0f, -8.0f, -1, 2, 0, false, false, false);

            Rage.Object MoneyPile = AttachMoneyToPed(PedToMove);

            GameFiber.Wait(1500);
            if (MoneyPile.Exists())
                MoneyPile.Delete();

            MoneyPile = AttachMoneyToPed(PedToMoveTo);
            GameFiber.Wait(1500);
            if (MoneyPile.Exists())
                MoneyPile.Delete();

            Game.LocalPlayer.Character.Tasks.Clear();
            PedToMoveTo.Tasks.Clear();
            CopToBribe.Pedestrian.BlockPermanentEvents = false;
            CopToBribe.Pedestrian.IsPositionFrozen = false;

            Game.LocalPlayer.Character.GiveCash(-1 * Amount);
            CopToBribe.Pedestrian.PlayAmbientSpeech("GENERIC_THANKS");
            //DispatchAudio.AddDispatchToQueue(new DispatchAudio.DispatchQueueItem(DispatchAudio.AvailableDispatch.ResumePatrol, 3));

            ResetPlayer(true, false);
        });


        return true;
    }
    private static Rage.Object AttachMoneyToPed(Ped Pedestrian)
    {
        Rage.Object Money = new Rage.Object("xs_prop_arena_cash_pile_m", Pedestrian.GetOffsetPositionUp(50f));
        if (!Money.Exists())
            return null;
        int BoneIndexRightHand = NativeFunction.CallByName<int>("GET_PED_BONE_INDEX", Pedestrian, 57005);
        Money.AttachTo(Pedestrian, BoneIndexRightHand, new Vector3(0.12f, 0.03f, -0.01f), new Rotator(0f, -45f, 90f));
        return Money;
    }
    private static void RemoveIllegalWeapons()
    {
        //Needed cuz for some reason the other weapon list just forgets your last gun in in there and it isnt applied, so until I can find it i can only remove all
        //Make a list of my old guns
        List<DroppedWeapon> MyOldGuns = new List<DroppedWeapon>();
        WeaponDescriptorCollection CurrentWeapons = Game.LocalPlayer.Character.Inventory.Weapons;
        foreach (WeaponDescriptor Weapon in CurrentWeapons)
        {
            WeaponVariation DroppedGunVariation = WeaponManager.GetWeaponVariation(Game.LocalPlayer.Character, (uint)Weapon.Hash);
            DroppedWeapon MyGun = new DroppedWeapon(Weapon, Vector3.Zero, DroppedGunVariation,Weapon.Ammo);
            MyOldGuns.Add(MyGun);
        }
        //Totally clear our guns
        Game.LocalPlayer.Character.Inventory.Weapons.Clear();
        //Add out guns back with variations
        foreach (DroppedWeapon MyNewGun in MyOldGuns)
        {
            WeaponInformation MyGTANewGun = WeaponManager.GetWeapon((ulong)MyNewGun.Weapon.Hash);
            if (MyGTANewGun == null || MyGTANewGun.IsLegal)//or its an addon gun
            {
                Game.LocalPlayer.Character.Inventory.GiveNewWeapon(MyNewGun.Weapon.Hash, (short)MyNewGun.Ammo, false);
                MyNewGun.Variation.ApplyWeaponVariation(Game.LocalPlayer.Character, (uint)MyNewGun.Weapon.Hash);
                NativeFunction.CallByName<bool>("ADD_AMMO_TO_PED", Game.LocalPlayer.Character, (uint)MyNewGun.Weapon.Hash, MyNewGun.Ammo + 1);
            }
        }
    }
    private static void ResetPlayer(bool ClearWanted, bool ResetHealth)
    {
        Mod.Player.ResetState(false);

        NativeFunction.CallByName<bool>("NETWORK_REQUEST_CONTROL_OF_ENTITY", Game.LocalPlayer.Character);
        NativeFunction.Natives.xC0AA53F866B3134D();
        Game.TimeScale = 1f;
        if (ClearWanted)
        {
            PersonOfInterestManager.Reset();
            WantedLevelManager.Reset();
            WantedLevelManager.SetWantedLevel(0, "Reset player with Clear Wanted", false);
            Mod.Player.MaxWantedLastLife = 0;
            NativeFunction.CallByName<bool>("RESET_PLAYER_ARREST_STATE", Game.LocalPlayer);
            PedDamageManager.Reset();
            InvestigationManager.Reset();
        }

        NativeFunction.Natives.xB4EDDC19532BFB85(); //_STOP_ALL_SCREEN_EFFECTS;
        NativeFunction.Natives.x80C8B1846639BB19(0);

        if (ResetHealth)
            Game.LocalPlayer.Character.Health = Game.LocalPlayer.Character.MaxHealth;

        NativeFunction.CallByName<bool>("RESET_HUD_COMPONENT_VALUES", 0);

        NativeFunction.Natives.xB9EFD5C25018725A("DISPLAY_HUD", true);
        NativeFunction.Natives.xC0AA53F866B3134D();//_RESET_LOCALPLAYER_STATE

        NativeFunction.CallByName<bool>("SET_PLAYER_HEALTH_RECHARGE_MULTIPLIER", Game.LocalPlayer, 0f);
    }
    private static void CheckWeapons()
    {
        if (!PedDamageManager.KilledAnyCops)
        {
            RemoveIllegalWeapons();
        }
        else
        {
            Game.LocalPlayer.Character.Inventory.Weapons.Clear();
        }
    }
    private static void SetHospitalFee(string HospitalName)
    {
        int HospitalFee = SettingsManager.MySettings.Police.HospitalFee * (1 + Mod.Player.MaxWantedLastLife);
        int CurrentCash = Game.LocalPlayer.Character.GetCash();
        int TodaysPayment = 0;

        int TotalNeededPayment = HospitalFee + HospitalBillPastDue;

        if (TotalNeededPayment > CurrentCash)
        {
            HospitalBillPastDue = TotalNeededPayment - CurrentCash;
            TodaysPayment = CurrentCash;
        }
        else
        {
            HospitalBillPastDue = 0;
            TodaysPayment = TotalNeededPayment;
        }

        Game.DisplayNotification("CHAR_BANK_FLEECA", "CHAR_BANK_FLEECA", HospitalName, "Hospital Fees", string.Format("Todays Bill: ~r~${0}~s~~n~Payment Today: ~g~${1}~s~~n~Outstanding: ~r~${2}", HospitalFee, TodaysPayment, HospitalBillPastDue));

        Game.LocalPlayer.Character.GiveCash(-1 * TodaysPayment);
    }
    private static void SetPoliceFee(string PoliceStationName, int BailFee)
    {
        int CurrentCash = Game.LocalPlayer.Character.GetCash();
        int TodaysPayment = 0;

        int TotalNeededPayment = BailFee + BailFeePastDue;

        if (TotalNeededPayment > CurrentCash)
        {
            BailFeePastDue = TotalNeededPayment - CurrentCash;
            TodaysPayment = CurrentCash;
        }
        else
        {
            BailFeePastDue = 0;
            TodaysPayment = TotalNeededPayment;
        }

        bool LesterHelp = RandomItems.RandomPercent(20);
        if (!LesterHelp)
        {
            Game.DisplayNotification("CHAR_BANK_FLEECA", "CHAR_BANK_FLEECA", PoliceStationName, "Bail Fees", string.Format("Todays Bill: ~r~${0}~s~~n~Payment Today: ~g~${1}~s~~n~Outstanding: ~r~${2}", BailFee, TodaysPayment, BailFeePastDue));
            Game.LocalPlayer.Character.GiveCash(-1 * TodaysPayment);
        }
        else
        {
            Game.DisplayNotification("CHAR_LESTER", "CHAR_LESTER", PoliceStationName, "Bail Fees", string.Format("~g~${0} ~s~", 0));
        }
    }
    private static void SetPlayerAtLocation(GameLocation ToSet)
    {
        Game.LocalPlayer.Character.Position = ToSet.LocationPosition;
        Game.LocalPlayer.Character.Heading = ToSet.Heading;
    }
    private static void FadeOut()
    {
        Game.FadeScreenOut(1500);
        GameFiber.Wait(1500);
    }
    private static void FadeIn()
    {
        GameFiber.Wait(1500);
        Game.FadeScreenIn(1500);
    }

    private class PoliceBribeAnimation
    {
        private Cop CopToBribe;
        public bool IsFinished { get; private set; }
        public bool TransactionOccured { get; private set; }
        private void Setup()
        {
            CopToBribe = Mod.PedManager.Cops.Where(x => x.Pedestrian.Exists() && x.Pedestrian.IsAlive).OrderBy(x => x.DistanceToPlayer).FirstOrDefault();
            if(CopToBribe == null)
            {
                IsFinished = true;
                return;
            }
            CopToBribe.ShouldAutoSetWeaponState = false;
            //CopToBribe.SetUnarmed();//need to change this

            NativeFunction.Natives.xB4EDDC19532BFB85(); //_STOP_ALL_SCREEN_EFFECTS;
            Game.TimeScale = 1.0f;
        }
        private bool BribePoliceAnimation(int Amount)//temp public
        {
            GameFiber.StartNew(delegate
            {
                CopToBribe = Mod.PedManager.Cops.Where(x => x.Pedestrian.Exists() && x.Pedestrian.IsAlive).OrderBy(x => x.DistanceToPlayer).FirstOrDefault();
                NativeFunction.Natives.xB4EDDC19532BFB85(); //_STOP_ALL_SCREEN_EFFECTS;
                Game.TimeScale = 1.0f;
                CopToBribe.ShouldAutoSetWeaponState = false;
               // CopToBribe.SetUnarmed();//need to change this

                SurrenderManager.UnSetArrestedAnimation(Game.LocalPlayer.Character);

                while (NativeFunction.CallByName<bool>("IS_ENTITY_PLAYING_ANIM", Game.LocalPlayer.Character, "random@arrests", "kneeling_arrest_escape", 1))
                    GameFiber.Wait(250);


                GameFiber.Wait(2000);

                if (!CopToBribe.Pedestrian.Exists())
                    return;

                CopToBribe.Pedestrian.BlockPermanentEvents = true;
                CopToBribe.Pedestrian.IsPositionFrozen = true;


                Ped PedToMove = Game.LocalPlayer.Character;
                Ped PedToMoveTo = CopToBribe.Pedestrian;
                Vector3 OriginalPosition = PedToMoveTo.Position;

                bool Continue = true;
                Vector3 PositionToMoveTo = PedToMoveTo.GetOffsetPositionFront(1f);
                float DesiredHeading = PedToMoveTo.Heading - 180;
                NativeFunction.CallByName<uint>("TASK_PED_SLIDE_TO_COORD", PedToMove, PositionToMoveTo.X, PositionToMoveTo.Y, PositionToMoveTo.Z, DesiredHeading);
                uint GameTimeStarted = Game.GameTime;
                while (Game.GameTime - GameTimeStarted <= 15000 && !(PedToMove.DistanceTo2D(PositionToMoveTo) <= 0.15f && PedToMove.FacingOppositeDirection(PedToMoveTo)))// PedToMove.Heading.IsWithin(DesiredHeading - 15f, DesiredHeading + 15f)))
                {
                    GameFiber.Yield();
                    if (Extensions.IsMoveControlPressed() || PedToMoveTo.DistanceTo2D(OriginalPosition) >= 0.1f)
                    {
                        Continue = false;
                        break;
                    }
                }
                if (!Continue)
                {
                    CopToBribe.Pedestrian.BlockPermanentEvents = false;
                    CopToBribe.Pedestrian.IsPositionFrozen = false;
                    PedToMove.Tasks.Clear();
                    return;
                }

                AnimationDictionary AnimDictionary = new AnimationDictionary("mp_common");

                NativeFunction.CallByName<bool>("TASK_PLAY_ANIM", Game.LocalPlayer.Character, "mp_common", "givetake1_a", 8.0f, -8.0f, -1, 2, 0, false, false, false);
                NativeFunction.CallByName<uint>("TASK_PLAY_ANIM", PedToMoveTo, "mp_common", "givetake1_b", 8.0f, -8.0f, -1, 2, 0, false, false, false);

                Rage.Object MoneyPile = AttachMoneyToPed(PedToMove);

                GameFiber.Wait(1500);
                if (MoneyPile.Exists())
                    MoneyPile.Delete();

                MoneyPile = AttachMoneyToPed(PedToMoveTo);
                GameFiber.Wait(1500);
                if (MoneyPile.Exists())
                    MoneyPile.Delete();

                Game.LocalPlayer.Character.Tasks.Clear();
                PedToMoveTo.Tasks.Clear();
                CopToBribe.Pedestrian.BlockPermanentEvents = false;
                CopToBribe.Pedestrian.IsPositionFrozen = false;

                Game.LocalPlayer.Character.GiveCash(-1 * Amount);
                CopToBribe.Pedestrian.PlayAmbientSpeech("GENERIC_THANKS");
                //DispatchAudio.AddDispatchToQueue(new DispatchAudio.DispatchQueueItem(DispatchAudio.AvailableDispatch.ResumePatrol, 3));

                ResetPlayer(true, false);
            });


            return true;
        }
    }
}


