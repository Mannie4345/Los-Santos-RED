﻿using ExtensionsMethods;
using LosSantosRED.lsr.Interface;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;

public class GangDispatcher
{
    private readonly IGangs Gangs;
    private readonly IDispatchable Player;

    private readonly float MinimumDeleteDistance = 150f;//200f
    private readonly uint MinimumExistingTime = 20000;
    private readonly ISettingsProvideable Settings;
    private readonly IStreets Streets;
    private readonly IEntityProvideable World;
    private readonly IZones Zones;
    private uint GameTimeAttemptedDispatch;
    private uint GameTimeAttemptedRecall;
    private bool HasDispatchedThisTick;
    private IWeapons Weapons;
    private INameProvideable Names;
    private IGangTerritories GangTerritories;
    private IPedGroups PedGroups;
    private ICrimes Crimes;
    private IShopMenus ShopMenus;
    private IPlacesOfInterest PlacesOfInterest;
    private GangDen GangDen;
    private bool IsDenSpawn;
    private SpawnLocation SpawnLocation;
    private Gang Gang;
    private DispatchableVehicle VehicleType;
    private DispatchablePerson PersonType;
    private bool ShouldRunAmbientDispatch;
    private IModItems ModItems;
    private bool IsPedestrianOnlySpawn = false;
    private uint TimeBetweenHitSquads;
    private uint GameTimeLastDispatchedHitSquad;

    public GangDispatcher(IEntityProvideable world, IDispatchable player, IGangs gangs, ISettingsProvideable settings, IStreets streets, IZones zones, IGangTerritories gangTerritories, IWeapons weapons, INameProvideable names, IPedGroups pedGroups, ICrimes crimes, IShopMenus shopMenus, IPlacesOfInterest placesOfInterest, IModItems modItems)
    {
        Player = player;
        World = world;
        Gangs = gangs;
        Settings = settings;
        Streets = streets;
        Zones = zones;
        GangTerritories = gangTerritories;
        Weapons = weapons;
        Names = names;
        PedGroups = pedGroups;
        Crimes = crimes;
        ShopMenus = shopMenus;
        PlacesOfInterest = placesOfInterest;
        ModItems = modItems;
        TimeBetweenHitSquads = RandomItems.GetRandomNumber(Settings.SettingsManager.GangSettings.MinTimeBetweenHitSquads, Settings.SettingsManager.GangSettings.MaxTimeBetweenHitSquads);
        GameTimeLastDispatchedHitSquad = Game.GameTime;
    }
    private float ClosestGangSpawnToPlayerAllowed => 45f;
    private List<GangMember> DeleteableGangMembers => World.Pedestrians.GangMemberList.Where(x => (x.RecentlyUpdated && x.DistanceToPlayer >= MinimumDeleteDistance && x.HasBeenSpawnedFor >= MinimumExistingTime) || x.CanRemove).ToList();
    private float DistanceToDeleteInVehicle => Settings.SettingsManager.GangSettings.MaxDistanceToSpawnInVehicle + 150f;// 300f;
    private float DistanceToDeleteOnFoot => Settings.SettingsManager.GangSettings.MaxDistanceToSpawnOnFoot + 50f;// 200 + 50f grace = 250f;
    private bool IsTimeToAmbientDispatch => Game.GameTime - GameTimeAttemptedDispatch >= TimeBetweenSpawn;//15000;
    private bool IsTimeToRecall => Game.GameTime - GameTimeAttemptedRecall >= 5000;// TimeBetweenSpawn;
    private float MaxDistanceToSpawnOnFoot => Settings.SettingsManager.GangSettings.MaxDistanceToSpawnOnFoot;//150f;
    private float MinDistanceToSpawnOnFoot => Settings.SettingsManager.GangSettings.MinDistanceToSpawnOnFoot;//50f;
    private float MaxDistanceToSpawnInVehicle => Settings.SettingsManager.GangSettings.MaxDistanceToSpawnInVehicle;//150f;
    private float MinDistanceToSpawnInVehicle => Settings.SettingsManager.GangSettings.MinDistanceToSpawnInVehicle;//50f;
    private bool IsTimeToDispatchHitSquad => Game.GameTime - GameTimeLastDispatchedHitSquad >= TimeBetweenHitSquads;
    private bool HasNeedToAmbientDispatch
    {
        get
        {
            if (World.Pedestrians.TotalSpawnedGangMembers >= Settings.SettingsManager.GangSettings.TotalSpawnedMembersLimit)
            {
                return false;
            }
            //if (World.Pedestrians.TotalSpawnedGangMembers > Settings.SettingsManager.GangSettings.TotalSpawnedAmbientMembersLimit)
            //{
            //    return false;
            //}
            if(!Settings.SettingsManager.GangSettings.AllowAmbientSpawningWhenPlayerWanted && Player.IsWanted)
            {
                return false;
            }
            if(Settings.SettingsManager.GangSettings.AllowAmbientSpawningWhenPlayerWanted && Player.WantedLevel > Settings.SettingsManager.GangSettings.AmbientSpawningWhenPlayerWantedMaxWanted)
            {
                return false;
            }
            if(World.Pedestrians.TotalSpawnedAmbientGangMembers > AmbientMemberLimitForZoneType)
            {
                return false;
            }
            return true;
        }
    }  
    private int AmbientMemberLimitForZoneType
    {
        get
        {
            int AmbientMemberLimit = Settings.SettingsManager.GangSettings.TotalSpawnedAmbientMembersLimit;
            if (EntryPoint.FocusZone?.Type == eLocationType.Wilderness)
            {
                AmbientMemberLimit = Settings.SettingsManager.GangSettings.TotalSpawnedAmbientMembersLimit_Wilderness;
            }
            else if (EntryPoint.FocusZone?.Type == eLocationType.Rural)
            {
                AmbientMemberLimit = Settings.SettingsManager.GangSettings.TotalSpawnedAmbientMembersLimit_Rural;
            }
            else if (EntryPoint.FocusZone?.Type == eLocationType.Suburb)
            {
                AmbientMemberLimit = Settings.SettingsManager.GangSettings.TotalSpawnedAmbientMembersLimit_Suburb;
            }
            else if (EntryPoint.FocusZone?.Type == eLocationType.Industrial)
            {
                AmbientMemberLimit = Settings.SettingsManager.GangSettings.TotalSpawnedAmbientMembersLimit_Industrial;
            }
            else if (EntryPoint.FocusZone?.Type == eLocationType.Downtown)
            {
                AmbientMemberLimit = Settings.SettingsManager.GangSettings.TotalSpawnedAmbientMembersLimit_Downtown;
            }
            return AmbientMemberLimit;
        }
    }
    private int TimeBetweenSpawn// => Settings.SettingsManager.GangSettings.TimeBetweenSpawn;//15000;
    {
        get
        {
            int TotalTimeBetweenSpawns = Settings.SettingsManager.GangSettings.TimeBetweenSpawn;
            if (EntryPoint.FocusZone?.Type == eLocationType.Wilderness)
            {
                TotalTimeBetweenSpawns += Settings.SettingsManager.GangSettings.TimeBetweenSpawn_WildernessAdditional;
            }
            else if (EntryPoint.FocusZone?.Type == eLocationType.Rural)
            {
                TotalTimeBetweenSpawns += Settings.SettingsManager.GangSettings.TimeBetweenSpawn_RuralAdditional;
            }
            else if (EntryPoint.FocusZone?.Type == eLocationType.Suburb)
            {
                TotalTimeBetweenSpawns += Settings.SettingsManager.GangSettings.TimeBetweenSpawn_SuburbAdditional;
            }
            else if (EntryPoint.FocusZone?.Type == eLocationType.Industrial)
            {
                TotalTimeBetweenSpawns += Settings.SettingsManager.GangSettings.TimeBetweenSpawn_IndustrialAdditional;
            }
            else if (EntryPoint.FocusZone?.Type == eLocationType.Downtown)
            {
                TotalTimeBetweenSpawns += Settings.SettingsManager.GangSettings.TimeBetweenSpawn;
            }
            return TotalTimeBetweenSpawns;
        }
    }
    private int PercentageOfAmbientSpawn // => Settings.SettingsManager.GangSettings.TimeBetweenSpawn;//15000;
    {
        get
        {
            int ambientSpawnPercent = Settings.SettingsManager.GangSettings.AmbientSpawnPercentage;
            if (EntryPoint.FocusZone?.Type == eLocationType.Wilderness)
            {
                ambientSpawnPercent = Settings.SettingsManager.GangSettings.AmbientSpawnPercentage_Wilderness;
            }
            else if (EntryPoint.FocusZone?.Type == eLocationType.Rural)
            {
                ambientSpawnPercent = Settings.SettingsManager.GangSettings.AmbientSpawnPercentage_Rural;
            }
            else if (EntryPoint.FocusZone?.Type == eLocationType.Suburb)
            {
                ambientSpawnPercent = Settings.SettingsManager.GangSettings.AmbientSpawnPercentage_Suburb;
            }
            else if (EntryPoint.FocusZone?.Type == eLocationType.Industrial)
            {
                ambientSpawnPercent = Settings.SettingsManager.GangSettings.AmbientSpawnPercentage_Industrial;
            }
            else if (EntryPoint.FocusZone?.Type == eLocationType.Downtown)
            {
                ambientSpawnPercent = Settings.SettingsManager.GangSettings.AmbientSpawnPercentage_Downtown;
            }
            return ambientSpawnPercent;
        }
    }
    public int LikelyHoodOfAnySpawn => Settings.SettingsManager.GangSettings.PercentSpawnOutsideTerritory;
    public int LikelyHoodOfDenSpawnWhenNear => Settings.SettingsManager.GangSettings.PercentageSpawnNearDen;

    private bool HasNeedToSpawnHeli => World.Vehicles.GangHelicoptersCount < Settings.SettingsManager.GangSettings.HeliSpawnLimit_Default;
   // private bool HasNeedToSpawnBoat => (Player.CurrentVehicle?.IsBoat == true || Player.IsSwimming) && World.Vehicles.GangBoatsCount < Settings.SettingsManager.GangSettings.BoatSpawnLimit_Default;

    public bool Dispatch()
    {      
        HasDispatchedThisTick = false;
        if(!Settings.SettingsManager.GangSettings.ManageDispatching)
        {
            return false;
        }
        HandleAmbientSpawns();
        HandleHitSquadSpawns();
       // EntryPoint.WriteToConsole($"GANG DISPATCHER IsTimeToDispatch:{IsTimeToDispatch} GameTimeSinceDispatch:{Game.GameTime - GameTimeAttemptedDispatch} HasNeedToDispatch:{HasNeedToDispatch} TotalGangMembers:{World.Pedestrians.TotalSpawnedGangMembers} AmbientMemberLimitForZoneType:{AmbientMemberLimitForZoneType} TimeBetweenSpawn:{TimeBetweenSpawn} HasNeedToDispatchToDens:{HasNeedToDispatchToDens} PercentageOfAmbientSpawn:{PercentageOfAmbientSpawn}");
        return HasDispatchedThisTick;
    }
    private void HandleHitSquadSpawns()
    {
        if (!Settings.SettingsManager.GangSettings.AllowHitSquads || !IsTimeToDispatchHitSquad)
        {
            return;
        }
        Gang EnemyGang;
        if (Settings.SettingsManager.GangSettings.AllowHitSquadsOnlyEnemy)
        {
            EnemyGang = Player.RelationshipManager.GangRelationships.EnemyGangs?.PickRandom();
        }
        else
        {
            EnemyGang = Player.RelationshipManager.GangRelationships.HitSquadGangs?.PickRandom();
        }
        DispatchHitSquad(EnemyGang);
        TimeBetweenHitSquads = RandomItems.GetRandomNumber(Settings.SettingsManager.GangSettings.MinTimeBetweenHitSquads, Settings.SettingsManager.GangSettings.MaxTimeBetweenHitSquads);
        GameTimeLastDispatchedHitSquad = Game.GameTime;
        HasDispatchedThisTick = true;
    }

    private void DispatchHitSquad(Gang enemyGang)
    {
        if(enemyGang == null)
        {
            EntryPoint.WriteToConsole($"DispatchHitSquad Abort, No Enemy Gang");
            return;
        }
        EntryPoint.WriteToConsole($"DispatchHitSquad Attempting to Dispatch {enemyGang.ShortName}");
        if(GetFarVehicleSpawnLocation() && GetHitSquadSpawnTypes(enemyGang))
        {
            EntryPoint.WriteToConsole($"DispatchHitSquad Disptaching HitSquad from {enemyGang.ShortName}");
            CallSpawnTask(false, true, false, false, TaskRequirements.None, true);
        }
    }

    public void Dispose()
    {

    }
    public void Recall()
    {
        if(!IsTimeToRecall)
        {
            return;
        }
        foreach (GangMember gangMember in DeleteableGangMembers)
        {
            if (ShouldBeRecalled(gangMember))
            {
                Delete(gangMember);
                GameFiber.Yield();
            }
        }
        GameTimeAttemptedRecall = Game.GameTime;
        
    }
    private void HandleAmbientSpawns()
    {
        if(!IsTimeToAmbientDispatch || !HasNeedToAmbientDispatch)
        {
            return;
        }
        HasDispatchedThisTick = true;//up here for now, might be better down low
        if(ShouldRunAmbientDispatch)
        {
            //EntryPoint.WriteToConsole($"AMBIENT GANG RunAmbientDispatch 1 TimeBetweenSpawn{TimeBetweenSpawn}");
            RunAmbientDispatch();
        }
        else
        {
            ShouldRunAmbientDispatch = RandomItems.RandomPercent(PercentageOfAmbientSpawn);
            if(ShouldRunAmbientDispatch)
            {
                //EntryPoint.WriteToConsole($"AMBIENT GANG RunAmbientDispatch 2 TimeBetweenSpawn{TimeBetweenSpawn}");
                RunAmbientDispatch();              
            }
            else
            {
                //EntryPoint.WriteToConsole($"AMBIENT GANG Aborting Spawn for this dispatch TimeBetweenSpawn{TimeBetweenSpawn} PercentageOfAmbientSpawn{PercentageOfAmbientSpawn}");
                GameTimeAttemptedDispatch = Game.GameTime;
            }
        }  
    }
    private void RunAmbientDispatch()
    {
        //EntryPoint.WriteToConsoleTestLong($"AMBIENT GANG SPAWN RunAmbientDispatch ShouldRunAmbientDispatch{ShouldRunAmbientDispatch}: %{PercentageOfAmbientSpawn} TimeBetween:{TimeBetweenSpawn} AmbLimit:{AmbientMemberLimitForZoneType}");

        if(!GetSpawnLocation() || !GetSpawnTypes())
        {
            return;
        }
        GameTimeAttemptedDispatch = Game.GameTime;
        GameFiber.Yield();
        //EntryPoint.WriteToConsoleTestLong($"AMBIENT GANG CALLED SPAWN TASK");
        if (CallSpawnTask(false, true, false, false, TaskRequirements.None, false))
        {
            ShouldRunAmbientDispatch = false;
            //GameTimeAttemptedDispatch = Game.GameTime;
        }    
    }
    private bool GetSpawnLocation()
    {
        int timesTried = 0;
        bool isValidSpawn;
        GangDen = null;
        IsDenSpawn = false;
        IsPedestrianOnlySpawn = false;
        SpawnLocation = new SpawnLocation();
        do
        {
            if (RandomItems.RandomPercent(LikelyHoodOfDenSpawnWhenNear))
            {
                GangDen = PlacesOfInterest.PossibleLocations.GangDens.Where(x => x.IsNearby).PickRandom();
            }
            if(GangDen != null && GangDen.DistanceToPlayer >= 90f)
            {
                Gang = GangDen.AssociatedGang;
                IsPedestrianOnlySpawn = RandomItems.RandomPercent(Gang.PedestrianSpawnPercentageAroundDen);
                IsDenSpawn = true;
                SpawnLocation.InitialPosition = GangDen.EntrancePosition.Around2D(50f);
               // EntryPoint.WriteToConsole($"Gang Dispatcher Den Spawn, Ped Only:{IsPedestrianOnlySpawn}");
            }
            else
            {
                IsPedestrianOnlySpawn = RandomItems.RandomPercent(Settings.SettingsManager.GangSettings.AmbientSpawnPedestrianAttemptPercentage);
                GangDen = null;
                SpawnLocation.InitialPosition = GetPositionAroundPlayer();
               // EntryPoint.WriteToConsole($"Gang Dispatcher Regular Spawn, Ped Only:{IsPedestrianOnlySpawn}");
            }
            SpawnLocation.GetClosestStreet(false);
            SpawnLocation.GetClosestSidewalk();
            if(IsPedestrianOnlySpawn && !IsDenSpawn && !SpawnLocation.HasSidewalk)
            {
                IsPedestrianOnlySpawn = false;
               // EntryPoint.WriteToConsole($"Gang Dispatcher attempted Ped Only spawn without sidewalks, changing to vehicle");
            }
            GameFiber.Yield();
            isValidSpawn = IsValidSpawn(SpawnLocation);
            timesTried++;
        }
        while (!SpawnLocation.HasSpawns && !isValidSpawn && timesTried < 2);//10
        return isValidSpawn && SpawnLocation.HasSpawns;
    }

    private bool GetFarVehicleSpawnLocation()
    {
        int timesTried = 0;
        bool isValidSpawn;
        GangDen = null;
        IsDenSpawn = false;
        IsPedestrianOnlySpawn = false;
        SpawnLocation = new SpawnLocation();
        do
        {
            IsPedestrianOnlySpawn = false;
            SpawnLocation.InitialPosition = GetPositionAroundPlayer();
            SpawnLocation.GetClosestStreet(false);
            GameFiber.Yield();
            isValidSpawn = IsValidSpawn(SpawnLocation);
            timesTried++;
        }
        while (!SpawnLocation.HasSpawns && !isValidSpawn && timesTried < 2);//10
        return isValidSpawn && SpawnLocation.HasSpawns;
    }

    private bool GetSpawnTypes()
    {
        Gang = null;
        VehicleType = null;
        PersonType = null;
        if (IsDenSpawn && GangDen != null)
        {
            Gang = GangDen.AssociatedGang;
        }
        else
        {
            Gang = GetRandomGang(SpawnLocation);
        }
        GameFiber.Yield();
        if (Gang == null)
        {
            return false;
        }    
        if (World.Pedestrians.GangMemberList.Count(x => x.Gang?.ID == Gang.ID) >= Gang.SpawnLimit)
        {
            return false;
        }
        if(!IsPedestrianOnlySpawn)
        {
            VehicleType = Gang.GetRandomVehicle(Player.WantedLevel, HasNeedToSpawnHeli, false, true, "", Settings);
        }
        GameFiber.Yield();
        string RequiredGroup = "";
        if (VehicleType != null)
        {
            RequiredGroup = VehicleType.RequiredPedGroup;
        }
        PersonType = Gang.GetRandomPed(Player.WantedLevel, RequiredGroup);
        GameFiber.Yield();
        if (PersonType != null)
        {
            return true;
        }         
        return false;
    }

    private bool GetHitSquadSpawnTypes(Gang gang)
    {
        Gang = gang;
        VehicleType = null;
        PersonType = null;
        if (Gang == null)
        {
            return false;
        }
        if (World.Pedestrians.GangMemberList.Count(x => x.Gang?.ID == Gang.ID) >= Gang.SpawnLimit)
        {
            return false;
        }
        VehicleType = Gang.GetRandomVehicle(Player.WantedLevel, HasNeedToSpawnHeli, false, true, "", Settings);   
        GameFiber.Yield();
        string RequiredGroup = "";
        if (VehicleType != null)
        {
            RequiredGroup = VehicleType.RequiredPedGroup;
        }
        PersonType = Gang.GetRandomPed(Player.WantedLevel, RequiredGroup);
        GameFiber.Yield();
        if (PersonType != null)
        {
            return true;
        }
        return false;
    }

    private bool CallSpawnTask(bool allowAny, bool allowBuddy, bool isLocationSpawn, bool clearArea, TaskRequirements spawnRequirement, bool isHitSquad)
    {
        try
        {
            GangSpawnTask gangSpawnTask = new GangSpawnTask(Gang, SpawnLocation, VehicleType, PersonType, Settings.SettingsManager.GangSettings.ShowSpawnedBlip, Settings, Weapons, Names, true, Crimes, PedGroups, ShopMenus, World, ModItems, false, false, false);// Settings.SettingsManager.Police.SpawnedAmbientPoliceHaveBlip);
            gangSpawnTask.AllowAnySpawn = allowAny;
            gangSpawnTask.AllowBuddySpawn = allowBuddy;
            gangSpawnTask.SpawnRequirement = spawnRequirement;
            gangSpawnTask.ClearArea = clearArea;
            gangSpawnTask.PlacePedOnGround = VehicleType == null;
            gangSpawnTask.IsHitSquad = isHitSquad;
            gangSpawnTask.AttemptSpawn();
            foreach (PedExt created in gangSpawnTask.CreatedPeople)
            {
                World.Pedestrians.AddEntity(created);
            }
            gangSpawnTask.CreatedPeople.ForEach(x => { World.Pedestrians.AddEntity(x); x.IsLocationSpawned = isLocationSpawn; });
            gangSpawnTask.CreatedVehicles.ForEach(x => World.Vehicles.AddEntity(x, ResponseType.None));
            HasDispatchedThisTick = true;
            return gangSpawnTask.CreatedPeople.Any(x => x.Pedestrian.Exists());
        }
        catch (Exception ex)
        {
            EntryPoint.WriteToConsole($"Gang Dispatcher Spawn Error: {ex.Message} : {ex.StackTrace}", 0);
            return false;
        }
    }
    private bool ShouldBeRecalled(GangMember gangMember)
    {
        if(!gangMember.RecentlyUpdated)
        {
            return false;
        }
        if (gangMember.IsInVehicle)
        {
            return gangMember.DistanceToPlayer >= DistanceToDeleteInVehicle;
        }
        else
        {
            return gangMember.DistanceToPlayer >= DistanceToDeleteOnFoot;
        }
    }
    private void Delete(PedExt emt)
    {
        if (emt != null && emt.Pedestrian.Exists())
        {
            //EntryPoint.WriteToConsole($"Attempting to Delete {Cop.Pedestrian.Handle}");
            if (emt.Pedestrian.IsInAnyVehicle(false))
            {
                if (emt.Pedestrian.CurrentVehicle.HasPassengers)
                {
                    foreach (Ped Passenger in emt.Pedestrian.CurrentVehicle.Passengers)
                    {
                        if (Passenger.Handle != Game.LocalPlayer.Character.Handle)
                        {
                            RemoveBlip(Passenger);
                            Passenger.Delete();
                            EntryPoint.PersistentPedsDeleted++;
                        }
                    }
                }
                if (emt.Pedestrian.Exists() && emt.Pedestrian.CurrentVehicle.Exists() && emt.Pedestrian.CurrentVehicle != null)
                {
                    emt.Pedestrian.CurrentVehicle.Delete();
                    EntryPoint.PersistentVehiclesDeleted++;
                }
            }
            RemoveBlip(emt.Pedestrian);
            if (emt.Pedestrian.Exists())
            {
                //EntryPoint.WriteToConsole(string.Format("Delete Cop Handle: {0}, {1}, {2}", Cop.Pedestrian.Handle, Cop.DistanceToPlayer, Cop.AssignedAgency.Initials));
                emt.Pedestrian.Delete();
                EntryPoint.PersistentPedsDeleted++;
            }
        }
    }
    private void RemoveBlip(Ped emt)
    {
        if (!emt.Exists())
        {
            return;
        }
        Blip MyBlip = emt.GetAttachedBlip();
        if (MyBlip.Exists())
        {
            MyBlip.Delete();
        }
    }
    private List<Gang> GetGangs(Vector3 Position, int WantedLevel)
    {
        List<Gang> ToReturn = new List<Gang>();
        Zone CurrentZone = Zones.GetZone(Position);
        Gang ZoneAgency = GangTerritories.GetRandomGang(CurrentZone.InternalGameName, WantedLevel);
        if (ZoneAgency != null)
        {
            ToReturn.Add(ZoneAgency);
        }
        if (!ToReturn.Any() || RandomItems.RandomPercent(LikelyHoodOfAnySpawn))//fall back to anybody
        {
            ToReturn.Clear();
            ToReturn.AddRange(Gangs.GetSpawnableGangs(WantedLevel));
        }
        return ToReturn;
    }
    private Vector3 GetPositionAroundPlayer()
    {
        Vector3 Position;
        Position = Player.Position;
        Position = Position.Around2D(IsPedestrianOnlySpawn ? MinDistanceToSpawnOnFoot : MinDistanceToSpawnInVehicle, IsPedestrianOnlySpawn ? MaxDistanceToSpawnOnFoot : MaxDistanceToSpawnInVehicle);
        return Position;
    }
    private Gang GetRandomGang(SpawnLocation spawnLocation)
    {
        Gang Gang;
        List<Gang> PossibleGangs = GetGangs(spawnLocation.StreetPosition, Player.WantedLevel);
        Gang = PossibleGangs.PickRandom();
        if (Gang == null)
        {
            Gang = GetGangs(spawnLocation.InitialPosition, Player.WantedLevel).PickRandom();
        }
        if (Gang == null)
        {
            //EntryPoint.WriteToConsole("Dispatcher could not find Agency To Spawn");
        }
        return Gang;
    }
    private Gang GetRandomGang(Vector3 spawnLocation)
    {
        Gang agency;
        List<Gang> PossibleAgencies = GetGangs(spawnLocation, Player.WantedLevel);
        agency = PossibleAgencies.PickRandom();
        if (agency == null)
        {
            agency = GetGangs(spawnLocation, Player.WantedLevel).PickRandom();
        }
        if (agency == null)
        {
            //EntryPoint.WriteToConsole("Dispatcher could not find Agency To Spawn");
        }
        return agency;
    }
    private bool IsValidSpawn(SpawnLocation spawnLocation)
    {
        if (spawnLocation.StreetPosition.DistanceTo2D(Player.Position) < ClosestGangSpawnToPlayerAllowed)
        {
            return false;
        }
        else if (spawnLocation.InitialPosition.DistanceTo2D(Player.Position) < ClosestGangSpawnToPlayerAllowed)
        {
            return false;
        }
        return true;
    }
    public void DebugSpawnGangMember(string gangID, bool onFoot, bool isEmpty)
    {
        VehicleType = null;
        PersonType = null;
        Gang = null;
        SpawnLocation = new SpawnLocation();
        SpawnLocation.InitialPosition = Game.LocalPlayer.Character.GetOffsetPositionFront(10f);
        SpawnLocation.StreetPosition = SpawnLocation.InitialPosition;
        SpawnLocation.Heading = Game.LocalPlayer.Character.Heading;
        if (gangID == "")
        {
            Gang = GetRandomGang(SpawnLocation);
        }
        else
        {
            Gang = Gangs.GetGang(gangID);
        }
        if (Gang == null)
        {
            return;
        }
           
        if (!onFoot)
        {
            VehicleType = Gang.GetRandomVehicle(Player.WantedLevel, true, true, true, "", Settings);
        }
        if (VehicleType != null || onFoot)
        {
            string RequiredGroup = "";
            if (VehicleType != null)
            {
                RequiredGroup = VehicleType.RequiredPedGroup;
            }
            PersonType = Gang.GetRandomPed(Player.WantedLevel, RequiredGroup);
        }

        if(isEmpty)
        {
            PersonType = null;
        }

        CallSpawnTask(true, true, false, false, TaskRequirements.None, false);
    }
    public void DebugSpawnHitSquad()
    {
        GameTimeLastDispatchedHitSquad = 0;
        TimeBetweenHitSquads = 0;
    }
    
}