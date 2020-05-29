﻿using ExtensionsMethods;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


internal static class Menus
{
    private static UIMenuListItem menuMainTakeoverRandomPed;
    private static UIMenuItem menuDebugResetCharacter;
    private static UIMenuItem menuMainSuicide;
    private static UIMenuItem menuMainChangeLicensePlate;
    private static UIMenuItem menuMainRemoveLicensePlate;
    private static UIMenuItem menuMainShowPlayerStatus;
    private static UIMenuItem menuMainChangeHelmet;
    private static UIMenuItem menuDebugKillPlayer;
    private static UIMenuListItem menuDebugRandomWeapon;
    private static UIMenuItem menuDebugRandomVariation;
    private static UIMenuListItem menuDebugScreenEffect;

    private static UIMenuItem menuDeathUndie;
    private static UIMenuListItem menuDeathTakeoverRandomPed;
    private static UIMenuItem menuBustedResistArrest;
    private static UIMenuItem menuBustedBribe;
    private static UIMenuItem menuBustedTalk;
    private static UIMenuListItem menuBustedTakeoverRandomPed;
    private static UIMenuListItem menuBustedSurrender;
    private static UIMenuListItem menuDeathHospitalRespawn;
    private static UIMenuItem menuDebugGiveMoney;
    private static UIMenuItem menuDebugHealthAndArmor;
    private static UIMenuItem menuActionSmoking;
    private static UIMenuListItem menuAutoSetRadioStation;
    private static UIMenuItem menuDebugResetMod;
    private static UIMenuItem ReloadSettings;

    private static MenuPool menuPool;
    private static UIMenu mainMenu;
    private static UIMenu deathMenu;
    private static UIMenu debugMenu;
    private static UIMenu bustedMenu;
    private static UIMenu optionsMenu;
    private static UIMenu actionsMenu;
    private static UIMenu scenariosMenu;

    private static int RandomWeaponCategory;

    private static Location CurrentSelectedSurrenderLocation;
    private static Location CurrentSelectedHospitalLocation;
    private static UIMenuItem scenariosMainPrisonEscape;
    private static readonly List<string> strRadioStations = new List<string> { "NONE", "RADIO_01_CLASS_ROCK", "RADIO_02_POP", "RADIO_03_HIPHOP_NEW", "RADIO_04_PUNK", "RADIO_05_TALK_01", "RADIO_06_COUNTRY", "RADIO_07_DANCE_01", "RADIO_08_MEXICAN", "RADIO_09_HIPHOP_OLD", "RADIO_12_REGGAE", "RADIO_13_JAZZ", "RADIO_14_DANCE_02", "RADIO_15_MOTOWN", "RADIO_20_THELAB", "RADIO_16_SILVERLAKE", "RADIO_17_FUNK", "RADIO_18_90S_ROCK", "RADIO_19_USER", "RADIO_11_TALK_02", "HIDDEN_RADIO_AMBIENT_TV_BRIGHT", "OFF" };
    private static readonly new List<string> ScreenEffects = new List<string>  { "SwitchHUDIn",
        "SwitchHUDOut",
        "FocusIn",
        "FocusOut",
        "MinigameEndNeutral",
        "MinigameEndTrevor",
        "MinigameEndFranklin",
        "MinigameEndMichael",
        "MinigameTransitionOut",
        "MinigameTransitionIn",
        "SwitchShortNeutralIn",
        "SwitchShortFranklinIn",
        "SwitchShortTrevorIn",
        "SwitchShortMichaelIn",
        "SwitchOpenMichaelIn",
        "SwitchOpenFranklinIn",
        "SwitchOpenTrevorIn",
        "SwitchHUDMichaelOut",
        "SwitchHUDFranklinOut",
        "SwitchHUDTrevorOut",
        "SwitchShortFranklinMid",
        "SwitchShortMichaelMid",
        "SwitchShortTrevorMid",
        "DeathFailOut",
        "CamPushInNeutral",
        "CamPushInFranklin",
        "CamPushInMichael",
        "CamPushInTrevor",
        "SwitchSceneFranklin",
        "SwitchSceneTrevor",
        "SwitchSceneMichael",
        "SwitchSceneNeutral",
        "MP_Celeb_Win",
        "MP_Celeb_Win_Out",
        "MP_Celeb_Lose",
        "MP_Celeb_Lose_Out",
        "DeathFailNeutralIn",
        "DeathFailMPDark",
        "DeathFailMPIn",
        "MP_Celeb_Preload_Fade",
        "PeyoteEndOut",
        "PeyoteEndIn",
        "PeyoteIn",
        "PeyoteOut",
        "MP_race_crash",
        "SuccessFranklin",
        "SuccessTrevor",
        "SuccessMichael",
        "DrugsMichaelAliensFightIn",
        "DrugsMichaelAliensFight",
        "DrugsMichaelAliensFightOut",
        "DrugsTrevorClownsFightIn",
        "DrugsTrevorClownsFight",
        "DrugsTrevorClownsFightOut",
        "HeistCelebPass",
        "HeistCelebPassBW",
        "HeistCelebEnd",
        "HeistCelebToast",
        "MenuMGHeistIn",
        "MenuMGTournamentIn",
        "MenuMGSelectionIn",
        "ChopVision",
        "DMT_flight_intro",
        "DMT_flight",
        "DrugsDrivingIn",
        "DrugsDrivingOut",
        "SwitchOpenNeutralFIB5",
        "HeistLocate",
        "MP_job_load",
        "RaceTurbo",
        "MP_intro_logo",
        "HeistTripSkipFade",
        "MenuMGHeistOut",
        "MP_corona_switch",
        "MenuMGSelectionTint",
        "SuccessNeutral",
        "ExplosionJosh3",
        "SniperOverlay",
        "RampageOut",
        "Rampage",
        "Dont_tazeme_bro" };

    public static float SelectedTakeoverRadius { get; set; }
    public static int SelectedPlateIndex { get; set; }
    public static string CurrentScreenEffect { get; set; }
    public static bool IsRunning { get; set; }
    public static void Intitialize()
    {
        IsRunning = true;
        RandomWeaponCategory = 0;

        CurrentSelectedSurrenderLocation = null;
        CurrentSelectedHospitalLocation = null;

        SelectedTakeoverRadius = -1f;
        SelectedPlateIndex = 0;

        menuPool = new MenuPool();
        mainMenu = new UIMenu("Los Santos RED", "Select an Option");
        menuPool.Add(mainMenu);
        deathMenu = new UIMenu("Wasted", "Choose Respawn");
        menuPool.Add(deathMenu);
        debugMenu = new UIMenu("Debug", "Debug Settings");
        menuPool.Add(debugMenu);
        bustedMenu = new UIMenu("Busted", "Choose Respawn");
        menuPool.Add(bustedMenu);
        actionsMenu = menuPool.AddSubMenu(mainMenu, "Actions");
        optionsMenu = menuPool.AddSubMenu(mainMenu, "Options");
        scenariosMenu = menuPool.AddSubMenu(mainMenu, "Scenarios");

        mainMenu.OnItemSelect += MainMenuSelect;
        mainMenu.OnListChange += OnListChange;
        mainMenu.OnCheckboxChange += OnCheckboxChange;

        deathMenu.OnItemSelect += DeathMenuSelect;
        deathMenu.OnListChange += OnListChange;

        bustedMenu.OnItemSelect += BustedMenuSelect;
        bustedMenu.OnListChange += OnListChange;

        debugMenu.OnItemSelect += DebugMenuSelect;
        debugMenu.OnListChange += OnListChange;
        debugMenu.OnCheckboxChange += OnCheckboxChange;
    }
    public static void Tick()
    {
        //try
        //{
            if (Game.IsKeyDown(General.MySettings.KeyBinding.MenuKey)) // Our menu on/off switch.
            {
                if (PlayerState.IsDead)
                {
                    if (!deathMenu.Visible)
                        ShowDeathMenu();
                    else
                        deathMenu.Visible = false;
                }
                else if (PlayerState.IsBusted)
                {
                    if (!bustedMenu.Visible)
                        ShowBustedMenu();
                    else
                        bustedMenu.Visible = false;
                }
                else if (optionsMenu.Visible)
                {
                    optionsMenu.Visible = !optionsMenu.Visible;
                }
                else if (actionsMenu.Visible)
                {
                    actionsMenu.Visible = !actionsMenu.Visible;
                }
                else if (scenariosMenu.Visible)
                {
                    scenariosMenu.Visible = !scenariosMenu.Visible;
                }
                else
                {
                    if (!mainMenu.Visible)
                        ShowMainMenu();
                    else
                        mainMenu.Visible = false;
                }
            }
            else if (Game.IsKeyDown(General.MySettings.KeyBinding.DebugMenuKey)) // Our menu on/off switch.
            {
                if (!debugMenu.Visible)
                    ShowDebugMenu();
                else
                    debugMenu.Visible = false;
            }
            menuPool.ProcessMenus();       // Process all our menus: draw the menu and process the key strokes and the mouse.      
        //}
        //catch (Exception e)
        //{
        //    ScriptController.Dispose();
        //    Debugging.WriteToLog("Error", e.Message + " : " + e.StackTrace);
        //}
    }
    public static void Dispose()
    {
        IsRunning = false;
        menuPool = null;
    }
    public static void ShowMainMenu()
    {
        CreateMainMenu();
        mainMenu.Visible = true;
    }
    public static void ShowDeathMenu()
    {
        CreateDeathMenu();
        
        mainMenu.Visible = false;
        debugMenu.Visible = false;
        bustedMenu.Visible = false;
        optionsMenu.Visible = false;
        actionsMenu.Visible = false;
        scenariosMenu.Visible = false;

        deathMenu.Visible = true;
    }
    public static void ShowBustedMenu()
    {
        if (PlayerState.IsDead)
            return;

        CreateBustedMenu();

        mainMenu.Visible = false;
        deathMenu.Visible = false;
        debugMenu.Visible = false;
        optionsMenu.Visible = false;
        actionsMenu.Visible = false;
        scenariosMenu.Visible = false;

        bustedMenu.Visible = true;
    }
    public static void ShowDebugMenu()
    {
        CreateDebugMenu();

        mainMenu.Visible = false;
        deathMenu.Visible = false;
        optionsMenu.Visible = false;
        actionsMenu.Visible = false;
        scenariosMenu.Visible = false;

        debugMenu.Visible = true;
    }
    private static void CreateMainMenu()
    {
        mainMenu.Clear();
        menuMainTakeoverRandomPed = new UIMenuListItem("Takeover Random Pedestrian", "Takes over a random pedestrian around the player.", new List<dynamic> { "Closest", "100 M", "500 M" });
        menuMainShowPlayerStatus = new UIMenuItem("Show Status", "Show the player status with a notification");
        mainMenu.AddItem(menuMainTakeoverRandomPed);
        mainMenu.AddItem(menuMainShowPlayerStatus);
        actionsMenu = menuPool.AddSubMenu(mainMenu, "Actions");
        optionsMenu = menuPool.AddSubMenu(mainMenu, "Options");
        scenariosMenu = menuPool.AddSubMenu(mainMenu, "Scenarios");

        CreateActionsMenu();
        CreateOptionsMenu();
        CreateScenariosMenu();
    }
    private static void CreateDeathMenu()
    {
        deathMenu.Clear();
        menuDeathUndie = new UIMenuItem("Un-Die", "Respawn at this exact spot as yourself.");
        menuDeathHospitalRespawn = new UIMenuListItem("Give Up", "Respawn at the nearest hospital. Lose a hospital fee and your guns.", Locations.GetAllLocationsOfType(Location.LocationType.Hospital));
        menuDeathTakeoverRandomPed = new UIMenuListItem("Takeover Random Pedestrian", "Takes over a random pedestrian around the player.", new List<dynamic> { "Closest", "20 M", "40 M", "60 M", "100 M", "500 M" });

        UpdateClosestHospitalIndex();

        deathMenu.AddItem(menuDeathUndie);
        deathMenu.AddItem(menuDeathHospitalRespawn);
        deathMenu.AddItem(menuDeathTakeoverRandomPed);

        if (General.MySettings.General.UndieLimit == 0 || PlayerState.TimesDied < General.MySettings.General.UndieLimit)
            menuDeathUndie.Enabled = true;
        else
            menuDeathUndie.Enabled = false;
    }
    private static void CreateBustedMenu()
    {
        bustedMenu.Clear();
        menuBustedResistArrest = new UIMenuItem("Resist Arrest", "Better hope you're strapped.");
        menuBustedBribe = new UIMenuItem("Bribe Police", "Bribe the police to let you go. Don't be cheap.");
        menuBustedSurrender = new UIMenuListItem("Surrender", "Surrender and get out on bail. Lose bail money and your guns.", Locations.GetAllLocationsOfType(Location.LocationType.Police));
        menuBustedTakeoverRandomPed = new UIMenuListItem("Takeover Random Pedestrian", "Takes over a random pedestrian around the player.", new List<dynamic> { "Closest", "20 M", "40 M", "60 M", "100 M", "500 M" });
        menuBustedTalk = new UIMenuItem("Talk", "Try to talk your way out of an arrest.");

        UpdateClosestPoliceStationIndex();

        bustedMenu.AddItem(menuBustedResistArrest);
        bustedMenu.AddItem(menuBustedBribe);
        bustedMenu.AddItem(menuBustedSurrender);
        bustedMenu.AddItem(menuBustedTalk);
        bustedMenu.AddItem(menuBustedTakeoverRandomPed);

        if (PlayerState.WantedLevel <= 1)
            menuBustedTalk.Enabled = true;
        else
            menuBustedTalk.Enabled = false;
    }
    private static void CreateOptionsMenu()
    {
        ReloadSettings = new UIMenuItem("Reload Settings", "Reload All settings from XML");
        optionsMenu.AddItem(ReloadSettings);
        foreach (FieldInfo fi in Type.GetType("Settings", false).GetFields())
        {
            if (fi.FieldType == typeof(bool))
            {
                UIMenuCheckboxItem MySetting = new UIMenuCheckboxItem(fi.Name, (bool)fi.GetValue(null));
                optionsMenu.AddItem(MySetting);
            }
            if (fi.FieldType == typeof(int) || fi.FieldType == typeof(string) || fi.FieldType == typeof(float))
            {
                UIMenuItem MySetting = new UIMenuItem(string.Format("{0}: {1}", fi.Name, fi.GetValue(null)));
                optionsMenu.AddItem(MySetting);
            }
        }
        optionsMenu.OnItemSelect += OptionsMenuSelect;
        optionsMenu.OnListChange += OnListChange;
        optionsMenu.OnCheckboxChange += OnCheckboxChange;
    }
    private static void CreateActionsMenu()
    {
        menuMainSuicide = new UIMenuItem("Suicide", "Commit Suicide");
        menuActionSmoking = new UIMenuItem("Smoking", "Start smoking.");
        menuMainChangeLicensePlate = new UIMenuListItem("Change Plate", "Change your license plate if you have spares.", LicensePlateTheft.SpareLicensePlates);//new UIMenuItem("Change Plate", "Change your license plate if you have spares");
        menuMainRemoveLicensePlate = new UIMenuItem("Remove Plate", "Remove the license plate.");
        menuMainChangeHelmet = new UIMenuItem("Toggle Helmet", "Add/Removes your helmet");

        actionsMenu.AddItem(menuMainSuicide);
        actionsMenu.AddItem(menuActionSmoking);

        if (!PlayerState.IsInVehicle)
        {
            actionsMenu.AddItem(menuMainChangeLicensePlate);
            actionsMenu.AddItem(menuMainRemoveLicensePlate);
        }

        actionsMenu.OnItemSelect += ActionsMenuSelect;
        actionsMenu.OnListChange += OnListChange;
        actionsMenu.OnCheckboxChange += OnCheckboxChange;
    }
    private static void CreateScenariosMenu()
    {
        scenariosMainPrisonEscape = new UIMenuItem("Prison Escape", "Escape the prison");

        scenariosMenu.AddItem(scenariosMainPrisonEscape);


        scenariosMenu.OnItemSelect += ScenarioMenuSelect;
        scenariosMenu.OnListChange += OnListChange;
        scenariosMenu.OnCheckboxChange += OnCheckboxChange;
    }
    private static void CreateDebugMenu()
    {
        debugMenu.Clear();
        menuDebugResetCharacter = new UIMenuItem("Reset Character", "Change your character back to the default model.");
        menuDebugKillPlayer = new UIMenuItem("Kill Player", "Immediatly die and ragdoll");
        menuDebugRandomWeapon = new UIMenuListItem("Get Random Weapon", "Gives the Player a random weapon and ammo.", new List<dynamic> { "Melee", "Pistol", "Shotgun", "SMG", "AR", "LMG", "Sniper", "Heavy" });
        menuDebugRandomVariation = new UIMenuItem("Apply Random Variation", "Add some cool stuff to your gun");
        menuDebugScreenEffect = new UIMenuListItem("Play Screen Effect", "Choose Screen Effect To Play", ScreenEffects);
        menuDebugGiveMoney = new UIMenuItem("Get Money", "Give you some cash");
        menuDebugHealthAndArmor = new UIMenuItem("Health and Armor", "Get loaded for bear");
        menuAutoSetRadioStation = new UIMenuListItem("Auto-Set Station", "Will auto set the station any time the radio is on", strRadioStations);
        menuDebugResetMod = new UIMenuItem("Reset Mod", "Reloads Mod and reads from the XML");

        debugMenu.AddItem(menuDebugResetCharacter);
        debugMenu.AddItem(menuDebugKillPlayer);
        debugMenu.AddItem(menuDebugRandomWeapon);
        debugMenu.AddItem(menuDebugRandomVariation);
        debugMenu.AddItem(menuDebugGiveMoney);
        debugMenu.AddItem(menuDebugHealthAndArmor);
        debugMenu.AddItem(menuAutoSetRadioStation);
        debugMenu.AddItem(menuDebugScreenEffect);
        debugMenu.AddItem(menuDebugResetMod);
    }
    private static void UpdateClosestHospitalIndex()
    {
        menuDeathHospitalRespawn.Index = Locations.GetAllLocationsOfType(Location.LocationType.Hospital).IndexOf(Locations.GetClosestLocationByType(Game.LocalPlayer.Character.Position, Location.LocationType.Hospital));
    }
    private static void UpdateClosestPoliceStationIndex()
    {
        menuBustedSurrender.Index = Locations.GetAllLocationsOfType(Location.LocationType.Police).IndexOf(Locations.GetClosestLocationByType(Game.LocalPlayer.Character.Position, Location.LocationType.Police));
    }
    private static void MainMenuSelect(UIMenu sender, UIMenuItem selectedItem, int index)
    {
        if (selectedItem == menuMainTakeoverRandomPed)
        {
            if(PlayerState.WantedLevel > 0)
            {
                Game.DisplayNotification("Lose your wanted level first");
                return;
            }
            if (SelectedTakeoverRadius == -1f)
                PedSwap.TakeoverPed(PedSwap.GetPedestrian(500f, true), false, false,true,false);
            else
                PedSwap.TakeoverPed(PedSwap.GetPedestrian(SelectedTakeoverRadius, false), false, false,true,false);
        }
        else if(selectedItem == menuMainShowPlayerStatus)
        {
            PlayerState.DisplayPlayerNotification();
        }
        mainMenu.Visible = false;
    }
    private static void BustedMenuSelect(UIMenu sender, UIMenuItem selectedItem, int index)
    {
        if (selectedItem == menuBustedResistArrest)
        {
            Respawn.ResistArrest();
        }
        else if (selectedItem == menuBustedBribe)
        {
            if (int.TryParse(GetKeyboardInput(), out int BribeAmount))
            {
                Respawn.BribePolice(BribeAmount);
            }
        }
        if (selectedItem == menuBustedSurrender)
        {
            Respawn.Surrender(CurrentSelectedSurrenderLocation);
        }
        else if (selectedItem == menuBustedTalk)
        {
            Respawn.Talk();
        }
        else if (selectedItem == menuBustedTakeoverRandomPed)
        {
            if (SelectedTakeoverRadius == -1f)
                PedSwap.TakeoverPed(PedSwap.GetPedestrian(500f, true), true, false,true,true);
            else
                PedSwap.TakeoverPed(PedSwap.GetPedestrian(SelectedTakeoverRadius, false), true, false,true,true);
        }
        bustedMenu.Visible = false;
    }
    private static void DeathMenuSelect(UIMenu sender, UIMenuItem selectedItem, int index)
    {
        if (selectedItem == menuDeathUndie)
        {
            Respawn.UnDie();
        }
        if (selectedItem == menuDeathHospitalRespawn)
        {
            Respawn.RespawnAtHospital(CurrentSelectedHospitalLocation);
        }
        else if (selectedItem == menuDeathTakeoverRandomPed)
        {
            if (SelectedTakeoverRadius == -1f)
                PedSwap.TakeoverPed(PedSwap.GetPedestrian(500f, true), true, false,true,true);
            else
                PedSwap.TakeoverPed(PedSwap.GetPedestrian(SelectedTakeoverRadius, false), true, false,true,true);
        }
        deathMenu.Visible = false;
    }
    private static void OptionsMenuSelect(UIMenu sender, UIMenuItem selectedItem, int index)
    {
        if(selectedItem == ReloadSettings)
        {
            General.ReadAllConfigs();
        }
        string mySettingName = selectedItem.Text.Split(':')[0];
        FieldInfo[] MyFields = Type.GetType("Settings", false).GetFields();
        FieldInfo MySetting = MyFields.Where(x => x.Name == mySettingName).FirstOrDefault();

        string Value = GetKeyboardInput();
        if (MySetting.FieldType == typeof(float))
        {
            if (float.TryParse(Value, out float myFloat))
            {
                MySetting.SetValue(null, myFloat);
                selectedItem.Text = string.Format("{0}: {1}", mySettingName, Value);
            }
        }
        else if (MySetting.FieldType == typeof(int))
        {
            if (int.TryParse(Value, out int myInt))
            {
                MySetting.SetValue(null, myInt);
                selectedItem.Text = string.Format("{0}: {1}", mySettingName, Value);
            }
        }
        else if (MySetting.FieldType == typeof(string))
        {
            MySetting.SetValue(null, Value);
            selectedItem.Text = string.Format("{0}: {1}", mySettingName, Value);
        }
    }
    private static void ActionsMenuSelect(UIMenu sender, UIMenuItem selectedItem, int index)
    {
        if (selectedItem == menuMainSuicide)
        {
            Surrender.CommitSuicide(Game.LocalPlayer.Character);
        }
        else if (selectedItem == menuActionSmoking)
        {
            Smoking.StartScenario();
        }
        else if (selectedItem == menuMainChangeLicensePlate)
        {
            LicensePlateTheft.ChangeNearestLicensePlate();
        }
        else if (selectedItem == menuMainRemoveLicensePlate)
        {
            LicensePlateTheft.RemoveNearestLicensePlate();
        }
    }
    private static void ScenarioMenuSelect(UIMenu sender, UIMenuItem selectedItem, int index)
    {
        if (selectedItem == scenariosMainPrisonEscape)
        {
            PedSwap.BecomeScenarioPed();
        }
    }
    private static void DebugMenuSelect(UIMenu sender, UIMenuItem selectedItem, int index)
    {
        if (selectedItem == menuDebugKillPlayer)
        {
            Game.LocalPlayer.Character.Kill();
        }
        if (selectedItem == menuDebugRandomWeapon)
        {
            GTAWeapon myGun = Weapons.GetRandomRegularWeaponByCategory((GTAWeapon.WeaponCategory)RandomWeaponCategory);
            Game.LocalPlayer.Character.Inventory.GiveNewWeapon(myGun.Name, myGun.AmmoAmount, true);
        }
        if (selectedItem == menuDebugGiveMoney)
        {
            Game.LocalPlayer.Character.GiveCash(5000);
        }
        if (selectedItem == menuDebugResetMod)
        {
            //ScriptController.Dispose();
            //ScriptController.Initialize();
        }
        if (selectedItem == menuDebugHealthAndArmor)
        {
            Game.LocalPlayer.Character.Health = Game.LocalPlayer.Character.MaxHealth;
            Game.LocalPlayer.Character.Armor = 100;
        }
        if(selectedItem == menuDebugScreenEffect)
        {
            NativeFunction.Natives.xB4EDDC19532BFB85();//ANIMPOSTFX_STOP_ALL
            Debugging.WriteToLog("Screen Effect: ", CurrentScreenEffect);


            if (CurrentScreenEffect != "")
            {
                NativeFunction.Natives.x2206BF9A37B7F724(CurrentScreenEffect, 0, true);//ANIMPOSTFX_PLAY
            }
        }
        debugMenu.Visible = false;
    }
    private static void OnCheckboxChange(UIMenu sender, UIMenuCheckboxItem checkbox, bool Checked)
    {
        if (sender == optionsMenu)
        {
            FieldInfo[] MyFields = Type.GetType("Settings", false).GetFields();
            FieldInfo MySetting = MyFields.Where(x => x.Name == checkbox.Text).FirstOrDefault();
            MySetting.SetValue(null, Checked);
        }
    }
    private static void OnListChange(UIMenu sender, UIMenuListItem list, int index)
    {
        if (sender == mainMenu)
        {
            if (list == menuMainTakeoverRandomPed)
            {
                if (index == 0)
                    SelectedTakeoverRadius = -1f;
                else if (index == 1)
                    SelectedTakeoverRadius = 100f;
                else if (index == 2)
                    SelectedTakeoverRadius = 500f;
            }
            if (list == menuMainChangeLicensePlate)
            {
                SelectedPlateIndex = index;
            }
        }
        else if (sender == deathMenu)
        {
            if (list == menuDeathHospitalRespawn)
            {
                CurrentSelectedHospitalLocation = Locations.GetAllLocationsOfType(Location.LocationType.Hospital)[index];
                Debugging.WriteToLog("menuDeathHospitalRespawn Changed", String.Format("Location: {0}", CurrentSelectedHospitalLocation));
            }
        }
        else if (sender == bustedMenu)
        {
            if (list == menuBustedSurrender)
            {
                CurrentSelectedSurrenderLocation = Locations.GetAllLocationsOfType(Location.LocationType.Police)[index];
                Debugging.WriteToLog("menuBustedSurrender Changed", String.Format("Location: {0}", CurrentSelectedSurrenderLocation));
            }
        }
        else if (sender == debugMenu)
        {
            if (list == menuDebugRandomWeapon)
                RandomWeaponCategory = list.Index;
            else if (list == menuAutoSetRadioStation)
                VehicleEngine.AutoTuneStation = strRadioStations[index];
            if (list == menuDebugScreenEffect)
                CurrentScreenEffect = ScreenEffects[index];
        }
    }
    private static string GetKeyboardInput()
    {
        NativeFunction.CallByName<bool>("DISPLAY_ONSCREEN_KEYBOARD", true, "FMMC_KEY_TIP8", "", "", "", "", "", 255 + 1);

        while (NativeFunction.CallByName<int>("UPDATE_ONSCREEN_KEYBOARD") == 0)
        {
            GameFiber.Sleep(500);
        }
        string Value;
        unsafe
        {
            IntPtr ptr = Rage.Native.NativeFunction.CallByName<IntPtr>("GET_ONSCREEN_KEYBOARD_RESULT");
            Value = Marshal.PtrToStringAnsi(ptr);
        }
        return Value;
    }
}



