﻿using ExtensionsMethods;
using LosSantosRED.lsr;
using LosSantosRED.lsr.Interface;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public class Pedestrians
{
    //private int MaxCivilianHealth = 100;
    //private int MaxCopArmor = 50;
    //private int MaxCopHealth = 125;
    //private int MinCivilianHealth = 70;
    //private int MinCopArmor = 0;
    //private int MinCopHealth = 85;
    private IAgencies Agencies;
    private IJurisdictions Jurisdictions;
    private ISettingsProvideable Settings;
    private IZones Zones;
    private INameProvideable Names;
    private IPedGroups RelationshipGroups;
    private List<Entity> WorldPeds = new List<Entity>();
    private IWeapons Weapons;
    private ICrimes Crimes;
    private IShopMenus ShopMenus;
    public Pedestrians(IAgencies agencies, IZones zones, IJurisdictions jurisdictions, ISettingsProvideable settings, INameProvideable names, IPedGroups relationshipGroups, IWeapons weapons, ICrimes crimes, IShopMenus shopMenus)
    {
        Agencies = agencies;
        Zones = zones;
        Jurisdictions = jurisdictions;
        Settings = settings;
        Names = names;
        RelationshipGroups = relationshipGroups;
        Weapons = weapons;
        Crimes = crimes;
        ShopMenus = shopMenus;
    }
    public List<PedExt> Civilians { get; private set; } = new List<PedExt>();
    public List<Cop> Police { get; private set; } = new List<Cop>();
    public List<EMT> EMTs { get; private set; } = new List<EMT>();
    public List<Firefighter> Firefighters { get; private set; } = new List<Firefighter>();
    public List<Merchant> Merchants { get; private set; } = new List<Merchant>();
    public string DebugString { get; set; } = "";
    public bool AnyArmyUnitsSpawned
    {
        get
        {
            return Police.Any(x => x.AssignedAgency.ID == "ARMY" && x.WasModSpawned);
        }
    }
    public bool AnyHelicopterUnitsSpawned
    {
        get
        {
            return Police.Any(x => x.IsInHelicopter && x.WasModSpawned);
        }
    }
    public bool AnyNooseUnitsSpawned
    {
        get
        {
            return Police.Any(x => x.AssignedAgency.ID == "NOOSE" && x.WasModSpawned);
        }
    }
    public int TotalSpawnedPolice
    {
        get
        {
            return Police.Where(x => x.WasModSpawned && x.Pedestrian.Exists() && x.Pedestrian.IsAlive).Count();
        }
    }
    public int TotalSpawnedEMTs
    {
        get
        {
            return EMTs.Where(x => x.WasModSpawned && x.Pedestrian.Exists() && x.Pedestrian.IsAlive).Count();
        }
    }
    public int TotalSpawnedFirefighters
    {
        get
        {
            return Firefighters.Where(x => x.WasModSpawned && x.Pedestrian.Exists() && x.Pedestrian.IsAlive).Count();
        }
    }
    public bool AnyOtherTargetsTasked => Police.Any(x => x.CurrentTask?.OtherTarget != null);
    public bool AnyCopsNearPosition(Vector3 Position, float Distance)
    {
        if (Position != Vector3.Zero && Police.Any(x => x.Pedestrian.Exists() && x.Pedestrian.DistanceTo2D(Position) <= Distance))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void ClearSpawned()
    {
        foreach (Cop Cop in Police)
        {
            if (Cop.Pedestrian.Exists() && Cop.Pedestrian.Handle != Game.LocalPlayer.Character.Handle)
            {
                Cop.Pedestrian.Delete();
                EntryPoint.PersistentPedsDeleted++;
            }
        }
        Police.Clear();
        foreach (EMT EMT in EMTs)
        {
            if (EMT.Pedestrian.Exists() && EMT.Pedestrian.Handle != Game.LocalPlayer.Character.Handle)
            {
                EMT.Pedestrian.Delete();
                EntryPoint.PersistentPedsDeleted++;
            }
        }
        EMTs.Clear();
        foreach (Firefighter Firefighter in Firefighters)
        {
            if (Firefighter.Pedestrian.Exists() && Firefighter.Pedestrian.Handle != Game.LocalPlayer.Character.Handle)
            {
                Firefighter.Pedestrian.Delete();
                EntryPoint.PersistentPedsDeleted++;
            }
        }
        Firefighters.Clear();
        foreach (Merchant merchant in Merchants)
        {
            if (merchant.Pedestrian.Exists() && merchant.Pedestrian.Handle != Game.LocalPlayer.Character.Handle)
            {
                merchant.Pedestrian.Delete();
            }
        }
        Merchants.Clear();
    }
    public PedExt GetPedExt(uint Handle)
    {
        PedExt pedExt = Police.FirstOrDefault(x => x.Handle == Handle);
        if (pedExt != null)
        {
            return pedExt;
       }
        pedExt = EMTs.FirstOrDefault(x => x.Handle == Handle);
        if (pedExt != null)
        {
            return pedExt;
        }
        pedExt = Firefighters.FirstOrDefault(x => x.Handle == Handle);
        if (pedExt != null)
        {
            return pedExt;
        }
        pedExt = Merchants.FirstOrDefault(x => x.Handle == Handle);
        if (pedExt != null)
        {
            return pedExt;
        }
        return Civilians.FirstOrDefault(x => x.Handle == Handle);

    }
    public void Prune()
    {
        //Police.RemoveAll(x => x.CanRemove);
        //EMTs.RemoveAll(x => x.CanRemove);
        //Firefighters.RemoveAll(x => x.CanRemove);
        //Merchants.RemoveAll(x => x.CanRemove);
        //Civilians.RemoveAll(x => x.CanRemove);
        foreach (Cop Cop in Police.Where(x => x.Pedestrian.Exists() && x.CanRemove && x.Pedestrian.IsDead))// && x.Pedestrian.DistanceTo2D(Game.LocalPlayer.Character) >= 200))
        {
            bool hasBlip = false;
            Blip myblip = Cop.Pedestrian.GetAttachedBlip();
            if (myblip.Exists())
            {
                hasBlip = true;
                myblip.Delete();
            }
            Cop.Pedestrian.IsPersistent = false;
            EntryPoint.PersistentPedsNonPersistent++;
            EntryPoint.WriteToConsole($"Pedestrians: Cop {Cop.Pedestrian.Handle} Removed Blip Set Non Persistent hasBlip {hasBlip}", 5);

        }
        foreach (EMT EMT in EMTs.Where(x => x.Pedestrian.Exists() && x.CanRemove && x.Pedestrian.IsDead))// && x.Pedestrian.DistanceTo2D(Game.LocalPlayer.Character) >= 200))
        {
            bool hasBlip = false;
            Blip myblip = EMT.Pedestrian.GetAttachedBlip();
            if (myblip.Exists())
            {
                hasBlip = true;
                myblip.Delete();
            }
            EMT.Pedestrian.IsPersistent = false;
            EntryPoint.PersistentPedsNonPersistent++;
            EntryPoint.WriteToConsole($"Pedestrians: Cop {EMT.Pedestrian.Handle} Removed Blip Set Non Persistent hasBlip {hasBlip}", 5);
        }
        foreach (Firefighter Firefighter in Firefighters.Where(x => x.Pedestrian.Exists() && x.CanRemove && x.Pedestrian.IsDead))// && x.Pedestrian.DistanceTo2D(Game.LocalPlayer.Character) >= 200))
        {
            bool hasBlip = false;
            Blip myblip = Firefighter.Pedestrian.GetAttachedBlip();
            if (myblip.Exists())
            {
                hasBlip = true;
                myblip.Delete();
            }
            Firefighter.Pedestrian.IsPersistent = false;
            EntryPoint.PersistentPedsNonPersistent++;
            EntryPoint.WriteToConsole($"Pedestrians: Cop {Firefighter.Pedestrian.Handle} Removed Blip Set Non Persistent hasBlip {hasBlip}", 5);
        }
        foreach (PedExt Civilian in Civilians.Where(x => !x.Pedestrian.Exists()))// && x.Pedestrian.DistanceTo2D(Game.LocalPlayer.Character) >= 200))
        {
            if(Civilian.HasSeenPlayerCommitCrime && Civilian.WillCallPolice && !Civilian.HasLoggedDeath)
            {
                Civilian.CurrentTask?.Update();
            }
        }
        // Police.RemoveAll(x => x.Pedestrian.Exists() && x.Pedestrian.Handle == Game.LocalPlayer.Character.Handle);
        Police.RemoveAll(x => x.CanRemove);
        EMTs.RemoveAll(x => x.CanRemove);
        Firefighters.RemoveAll(x => x.CanRemove);
        Merchants.RemoveAll(x => x.CanRemove);
        Civilians.RemoveAll(x => x.CanRemove);
    }
    public void Scan()
    {
        WorldPeds = Rage.World.GetEntities(GetEntitiesFlags.ConsiderHumanPeds | GetEntitiesFlags.ExcludePlayerPed).ToList();
        //DebugString = $"Cop#: {Police.Count()} EMT#: {EMTs.Count()} Fire#: {Firefighters.Count()} Merch# {Merchants.Count()} Civ#: {Civilians.Count()}"; 
    }
    public void CreateNew()
    {
        foreach (Ped Pedestrian in WorldPeds.Where(s => s.Exists() && !s.IsDead && s.MaxHealth != 1))//take 20 is new
        {
            //if(Settings.SettingsManager.WorldSettings.RemoveMPShopKeepPed && Pedestrian.Model.Name.ToLower() == "mp_m_shopkeep_01")
            //{
            //    Pedestrian.Delete();
            //    continue;
            //}
            uint localHandle = Pedestrian.Handle;
            if (Pedestrian.IsPoliceArmy())
            {
                if (!Police.Any(x => x.Handle == localHandle))
                {
                    AddCop(Pedestrian);
                }
            }
            else
            {
                if (!Civilians.Any(x => x.Handle == localHandle) && !Merchants.Any(x=> x.Handle == localHandle))
                {
                    AddCivilian(Pedestrian);
                }
            }
        }
    }
    private void AddCivilian(Ped Pedestrian)
    {
        SetCivilianStats(Pedestrian);
        bool WillFight = RandomItems.RandomPercent(Settings.SettingsManager.CivilianSettings.FightPercentage);
        bool WillCallPolice = RandomItems.RandomPercent(Settings.SettingsManager.CivilianSettings.CallPolicePercentage);
        bool IsGangMember = false;
        bool canBeAmbientTasked = true;
        if (Pedestrian.Exists())
        {
            if (RandomItems.RandomPercent(Settings.SettingsManager.CivilianSettings.GangFightPercentage) && Pedestrian.IsGangMember())
            {
                IsGangMember = true;
                WillFight = true;
                WillCallPolice = false;
            }
            else if (RandomItems.RandomPercent(Settings.SettingsManager.CivilianSettings.SecurityFightPercentage) && Pedestrian.IsSecurity())
            {
                WillFight = true;
                WillCallPolice = false;
            }
            else if (!Settings.SettingsManager.CivilianSettings.TaskMissionPeds && Pedestrian.IsPersistent)//must have been spawned by another mod?
            {
                WillFight = false;
                WillCallPolice = false;
                canBeAmbientTasked = false;
            }
        }
        PedGroup myGroup = RelationshipGroups.GetPedGroup(Pedestrian.RelationshipGroup.Name);
        if(myGroup == null)
        {
            myGroup = new PedGroup(Pedestrian.RelationshipGroup.Name, Pedestrian.RelationshipGroup.Name, Pedestrian.RelationshipGroup.Name, false);
        }
        ShopMenu toAdd = null;
        if(IsGangMember)
        {
            toAdd = ShopMenus.GetRanomdDrugMenu();
        }
        Civilians.Add(new PedExt(Pedestrian, Settings, WillFight, WillCallPolice, IsGangMember, Names.GetRandomName(Pedestrian.IsMale), myGroup, Crimes, Weapons) { CanBeAmbientTasked = canBeAmbientTasked, TransactionMenu = toAdd?.Items });
    }
    private void AddCop(Ped Pedestrian)
    {
        Agency AssignedAgency = GetAgency(Pedestrian, 0);//maybe need the actual wanted level here?
        if (AssignedAgency != null && Pedestrian.Exists())
        {
            Cop myCop = new Cop(Pedestrian, Settings, Pedestrian.Health, AssignedAgency, false, Crimes, Weapons, Names.GetRandomName(Pedestrian.IsMale));
            myCop.IssueWeapons(Weapons);
            if (Settings.SettingsManager.PoliceSettings.ShowSpawnedBlips && Pedestrian.Exists())
            {
                Blip myBlip = Pedestrian.AttachBlip();
                myBlip.Color = AssignedAgency.Color;
                myBlip.Scale = 0.6f;
                string CopName = AssignedAgency.ID;
                myBlip.Name = CopName;
                NativeFunction.Natives.BEGIN_TEXT_COMMAND_SET_BLIP_NAME("STRING");
                NativeFunction.Natives.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(CopName);
                NativeFunction.Natives.END_TEXT_COMMAND_SET_BLIP_NAME(myBlip);
            }
            SetCopStats(Pedestrian);
            Police.Add(myCop);
            EntryPoint.WriteToConsole($"PEDESTRIANS: Add COP {Pedestrian.Handle}", 2);
        }
        else
        {
            EntryPoint.WriteToConsole($"PEDESTRIANS: Add COP FAIL", 2);
        }
    }
    public Agency GetAgency(Ped Cop, int WantedLevel)
    {
        if (!Cop.IsPoliceArmy())
        {
            return null;
        }
        if (Cop.IsArmy())
        {
            return Agencies.GetRandomMilitaryAgency();//return AgenciesList.Where(x => x.AgencyClassification == Classification.Military).FirstOrDefault();
        }
        else if (Cop.IsPolice())
        {
            Agency ToReturn;
            List<Agency> ModelMatchAgencies = Agencies.GetAgencies(Cop);
            if (ModelMatchAgencies.Count > 1)
            {
                string ZoneName = GetInternalZoneString(Cop.Position);
                if (ZoneName != "")
                {
                    //EntryPoint.WriteToConsole(string.Format("GetAgencyFromPed! ZoneName {0}", ZoneName));
                    if(Jurisdictions == null)
                    {
                        //EntryPoint.WriteToConsole("GetAgencyFromPed! ZoneJurisdictions is null!!!!!");
                    }
                    List<Agency> ZoneAgencies = Jurisdictions.GetAgencies(ZoneName, WantedLevel,ResponseType.LawEnforcement);
                    if (ZoneAgencies != null)
                    {
                        foreach (Agency ZoneAgency in ZoneAgencies)
                        {
                            if (ModelMatchAgencies.Any(x => x.ID == ZoneAgency.ID))
                            {
                                return ZoneAgency;
                            }
                        }
                    }
                }
            }
            ToReturn = ModelMatchAgencies.FirstOrDefault();
            if (ToReturn == null)
            {
                //EntryPoint.WriteToConsole(string.Format("GetAgencyFromPed! Couldnt get agency from {0} ped deleting", Cop.Model.Name));
                if(Cop.IsPersistent)
                {
                    EntryPoint.PersistentPedsDeleted++;
                }
                Cop.Delete();
            }
            return ToReturn;
        }
        else
        {
            return null;
        }
    }
    private void SetCivilianStats(Ped Pedestrian)
    {
        if (Pedestrian.Exists() && !Pedestrian.IsPersistent)
        {
            if (Settings.SettingsManager.CivilianSettings.OverrideAccuracy)
            {
                Pedestrian.Accuracy = Settings.SettingsManager.CivilianSettings.GeneralAccuracy;
            }
            if (Settings.SettingsManager.CivilianSettings.OverrideHealth)
            {
                int DesiredHealth = RandomItems.MyRand.Next(Settings.SettingsManager.CivilianSettings.MinHealth, Settings.SettingsManager.CivilianSettings.MaxHealth) + 100;
                Pedestrian.MaxHealth = DesiredHealth;
                Pedestrian.Health = DesiredHealth;
                Pedestrian.Armor = 0;
            }
            NativeFunction.CallByName<bool>("SET_PED_CONFIG_FLAG", Pedestrian, 281, true);//Can Writhe
            NativeFunction.CallByName<bool>("SET_PED_DIES_WHEN_INJURED", Pedestrian, false);
           // NativeFunction.Natives.SET_DRIVER_ABILITY(Pedestrian, 100f);
        }
    }
    private void SetCopStats(Ped Pedestrian)
    {
        if (Settings.SettingsManager.PoliceSettings.OverrideAccuracy)
        {
            Pedestrian.Accuracy = Settings.SettingsManager.PoliceSettings.GeneralAccuracy;
        }
        if(Settings.SettingsManager.PoliceSettings.OverrideHealth)
        {
            int DesiredHealth = RandomItems.MyRand.Next(Settings.SettingsManager.PoliceSettings.MinHealth, Settings.SettingsManager.PoliceSettings.MaxHealth) + 100;
            Pedestrian.MaxHealth = DesiredHealth;
            Pedestrian.Health = DesiredHealth;
        }
        if (Settings.SettingsManager.PoliceSettings.OverrideArmor)
        {
            int DesiredArmor = RandomItems.MyRand.Next(Settings.SettingsManager.PoliceSettings.MinArmor, Settings.SettingsManager.PoliceSettings.MaxArmor);
            Pedestrian.Armor = DesiredArmor;
        }
        //NativeFunction.CallByName<bool>("SET_PED_CONFIG_FLAG", Pedestrian, 281, true);//Can Writhe
        //NativeFunction.CallByName<bool>("SET_PED_DIES_WHEN_INJURED", Pedestrian, false);
    }
    private string GetInternalZoneString(Vector3 ZonePosition)
    {
        string zoneName;
        unsafe
        {
            IntPtr ptr = Rage.Native.NativeFunction.CallByName<IntPtr>("GET_NAME_OF_ZONE", ZonePosition.X, ZonePosition.Y, ZonePosition.Z);

            zoneName = Marshal.PtrToStringAnsi(ptr);
        }
        return zoneName;
    }
}