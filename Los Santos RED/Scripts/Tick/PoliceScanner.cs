﻿using ExtensionsMethods;
using NAudio.Wave;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using static DispatchScannerFiles;
using Rage.Native;
using System.Runtime.InteropServices;

public static class PoliceScanner
{
    private static WaveOutEvent outputDevice;
    private static AudioFileReader audioFile;
    private static List<uint> NotificationHandles = new List<uint>();
    private static DispatchEvent CurrentlyPlaying;
    private static int HighestCivilianReportedPriority = 99;
    private static int HighestOfficerReportedPriority = 99;
    private static uint GameTimeLastDisplayedSubtitle;
    private static uint GameTimeLastAnnouncedDispatch;
    private static bool ReportedLethalForceAuthorized = false;

    private static Dispatch OfficerDown;
    private static Dispatch ShotsFiredAtAnOfficer;
    private static Dispatch AssaultingOfficer;
    private static Dispatch ThreateningOfficerWithFirearm;
    private static Dispatch TrespassingOnGovernmentProperty;
    private static Dispatch StealingAirVehicle;
    private static Dispatch ShotsFired;
    private static Dispatch CarryingWeapon;
    private static Dispatch CivilianDown;
    private static Dispatch CivilianShot;
    private static Dispatch CivilianInjury;
    private static Dispatch GrandTheftAuto;
    private static Dispatch SuspiciousActivity;
    private static Dispatch CriminalActivity;
    private static Dispatch Mugging;
    private static Dispatch TerroristActivity;
    private static Dispatch SuspiciousVehicle;
    private static Dispatch DrivingAtStolenVehicle;
    private static Dispatch ResistingArrest;
    private static Dispatch AttemptingSuicide;
    private static Dispatch FelonySpeeding;
    private static Dispatch PedHitAndRun;
    private static Dispatch VehicleHitAndRun;
    private static Dispatch RecklessDriving;
    private static Dispatch AnnounceStolenVehicle;
    private static Dispatch RequestAirSupport;
    private static Dispatch RequestMilitaryUnits;
    private static Dispatch SuspectSpotted;
    private static Dispatch WantedSuspectSpotted;
    private static Dispatch SuspectEvaded;
    private static Dispatch LostVisual;
    private static Dispatch ResumePatrol;
    private static Dispatch SuspectLost;
    private static Dispatch NoFurtherUnitsNeeded;
    private static Dispatch SuspectArrested;
    private static Dispatch SuspectWasted;
    private static Dispatch ChangedVehicles;
    private static Dispatch RequestBackup;
    private static Dispatch WeaponsFree;
    private static Dispatch LethalForceAuthorized;
    private static Dispatch RunningARedLight;

    private static List<Dispatch> DispatchList = new List<Dispatch>();
    private static List<Dispatch> DispatchQueue = new List<Dispatch>();
    private static List<string> RadioStart;
    private static List<string> RadioEnd;
    private static List<Dispatch.AudioSet> AttentionAllUnits;
    private static List<Dispatch.AudioSet> OfficersReport;
    private static List<Dispatch.AudioSet> CiviliansReport;
    private static List<Dispatch.AudioSet> LethalForce;
    private static List<Dispatch.AudioSet> LicensePlateSet;
    private static List<CrimeDispatch> DispatchLookup;
    private static bool ExecutingQueue;
    public static bool CancelAudio { get; set; }
    public static bool IsRunning { get; set; } = true;
    public static bool IsAudioPlaying
    {
        get
        {
            return outputDevice != null;
        }
    }
    public static bool RecentlyAnnouncedDispatch
    {
        get
        {
            if (GameTimeLastAnnouncedDispatch == 0)
                return false;
            if (Game.GameTime - GameTimeLastAnnouncedDispatch <= 25000)
                return true;
            else
                return false;
        }
    }
    private enum LocationSpecificity
    {
        Nothing = 0,
        Zone = 1,
        HeadingAndStreet = 3,
        StreetAndZone = 5,
        Street = 6,
    }
    public static void Initialize()
    {
        SetupLists();
        IsRunning = true;
    }
    public static void Dispose()
    {
        IsRunning = false;
    }
    public static void Tick()
    {
        if (IsRunning && General.MySettings.Police.DispatchAudio)
        {
            CheckDispatch();
            if (DispatchQueue.Count > 0 && !ExecutingQueue)
            {
                ExecutingQueue = true;
                GameFiber PlayDispatchQueue = GameFiber.StartNew(delegate
                {
                    GameFiber.Sleep(General.MyRand.Next(2500, 4500));//Next(1500, 2500)
                    if (DispatchQueue.Any(x => x.LatestInformation.SeenByOfficers))
                    {
                        DispatchQueue.RemoveAll(x => !x.LatestInformation.SeenByOfficers);
                    }
                    if (DispatchQueue.Count() > 1)
                    {
                        Dispatch HighestItem = DispatchQueue.OrderBy(x => x.Priority).FirstOrDefault();
                        DispatchQueue.Clear();
                        if (HighestItem != null)
                        {
                            DispatchQueue.Add(HighestItem);
                        }
                    }
                    while (DispatchQueue.Count > 0)
                    {
                        Dispatch Item = DispatchQueue.OrderBy(x => x.Priority).ToList()[0];
                        BuildDispatch(Item);
                        if (DispatchQueue.Contains(Item))
                            DispatchQueue.Remove(Item);
                    }
                    ExecutingQueue = false;
                }, "PlayDispatchQueue");
                Debugging.GameFibers.Add(PlayDispatchQueue);
            }
        }
    }
    public static void AnnounceCrime(Crime crimeAssociated, DispatchCallIn reportInformation)
    {
        Dispatch ToAnnounce = DetermineDispatchFromCrime(crimeAssociated);
        if(ToAnnounce != null)
        {
            if (!ToAnnounce.HasRecentlyBeenPlayed)
            {
                if (reportInformation.SeenByOfficers)
                {
                    if (ToAnnounce.Priority < HighestOfficerReportedPriority)
                        AddToQueue(ToAnnounce, reportInformation);
                }
                else
                {
                   if (ToAnnounce.Priority < HighestCivilianReportedPriority)
                        AddToQueue(ToAnnounce, reportInformation);
                }
            }
        }
    }
    private static void CheckDispatch()
    {
        if (IsRunning)
        {
            if (PlayerState.IsWanted && Police.AnySeenPlayerCurrentWanted)
            {
                if (!RequestBackup.HasRecentlyBeenPlayed && WantedLevelScript.RecentlyRequestedBackup)
                {
                    AddToQueue(RequestBackup, new DispatchCallIn(!PlayerState.IsInVehicle, true, Police.PlaceLastSeenPlayer));
                }
                if (!RequestMilitaryUnits.HasBeenPlayedThisWanted && WantedLevelScript.IsMilitaryDeployed)
                {
                    AddToQueue(RequestMilitaryUnits);
                }
                if (!WeaponsFree.HasBeenPlayedThisWanted && WantedLevelScript.IsWeaponsFree)
                {
                    AddToQueue(WeaponsFree);
                }
                if (!RequestAirSupport.HasBeenPlayedThisWanted && PedList.CopPeds.Any(x => x.IsInHelicopter && x.WasModSpawned))
                {
                    AddToQueue(RequestAirSupport);
                }
                if (!ReportedLethalForceAuthorized && WantedLevelScript.IsDeadlyChase)
                {
                    AddToQueue(LethalForceAuthorized);
                }
                if (!SuspectArrested.HasRecentlyBeenPlayed && PlayerState.RecentlyBusted && Police.AnyCanSeePlayer)
                {
                    AddToQueue(SuspectArrested);
                }
                if (!ChangedVehicles.HasRecentlyBeenPlayed && PlayerState.PoliceRecentlyNoticedVehicleChange && !PlayerState.CurrentVehicle.HasBeenDescribedByDispatch)
                {
                    AddToQueue(ChangedVehicles, new DispatchCallIn(!PlayerState.IsInVehicle, true, Police.PlaceLastSeenPlayer) { VehicleSeen = PlayerState.CurrentVehicle });
                }
                if (!WantedSuspectSpotted.HasRecentlyBeenPlayed && PersonOfInterest.RecentlyAppliedWantedStats)
                {
                    AddToQueue(WantedSuspectSpotted, new DispatchCallIn(!PlayerState.IsInVehicle, true, Police.PlaceLastSeenPlayer) { VehicleSeen = PlayerState.CurrentVehicle });
                }
                if (!RecentlyAnnouncedDispatch)
                {
                    if (!LostVisual.HasRecentlyBeenPlayed && PlayerState.StarsRecentlyGreyedOut && WantedLevelScript.HasBeenWantedFor > 45000 && !PedList.AnyCopsNearPlayer)
                    {
                        AddToQueue(LostVisual, new DispatchCallIn(!PlayerState.IsInVehicle, true, Police.PlaceLastSeenPlayer));
                    }
                    else if (!SuspectSpotted.HasRecentlyBeenPlayed && Police.AnyCanSeePlayer && WantedLevelScript.HasBeenWantedFor > 25000)
                    {
                        AddToQueue(SuspectSpotted, new DispatchCallIn(!PlayerState.IsInVehicle, true, Game.LocalPlayer.Character.Position));
                    }
                }

            }
            else
            {
                if (!ResumePatrol.HasRecentlyBeenPlayed && Respawn.RecentlyBribedPolice)
                {
                    AddToQueue(ResumePatrol);
                }
                if (!SuspectLost.HasRecentlyBeenPlayed && WantedLevelScript.RecentlyLostWanted && !Respawn.RecentlyRespawned && !Respawn.RecentlyBribedPolice)
                {
                    AddToQueue(SuspectLost, new DispatchCallIn(!PlayerState.IsInVehicle, true, Police.PlaceLastSeenPlayer));
                }
                if (!SuspectWasted.HasRecentlyBeenPlayed && PlayerState.RecentlyDied && Police.AnyRecentlySeenPlayer && Police.PreviousWantedLevel > 0)
                {
                    AddToQueue(SuspectWasted);
                }
                if (!NoFurtherUnitsNeeded.HasRecentlyBeenPlayed && Investigation.LastInvestigationRecentlyExpired && Investigation.DistanceToInvestigationPosition <= 1000f)
                {
                    AddToQueue(NoFurtherUnitsNeeded);
                }
                foreach (VehicleExt StolenCar in PlayerState.ReportedStolenVehicles)
                {
                    AddToQueue(AnnounceStolenVehicle, new DispatchCallIn(!PlayerState.IsInVehicle, true, Police.PlaceLastSeenPlayer) { VehicleSeen = StolenCar });
                }
            }
        }
    }
    private static void AddToQueue(Dispatch ToAdd,DispatchCallIn ToCallIn)
    {
        Dispatch Existing = DispatchQueue.FirstOrDefault(x => x.Name == ToAdd.Name);
        if (Existing != null)
        {
            Existing.LatestInformation = ToCallIn;
        }
        else
        {
            ToAdd.LatestInformation = ToCallIn;
            Debugging.WriteToLog("ScannerScript", ToAdd.Name);
            DispatchQueue.Add(ToAdd);
        }
    }
    private static void AddToQueue(Dispatch ToAdd)
    {
        Dispatch Existing = DispatchQueue.FirstOrDefault(x => x.Name == ToAdd.Name);
        if (Existing == null)
        {
            DispatchQueue.Add(ToAdd);
            Debugging.WriteToLog("ScannerScript", ToAdd.Name);
        }
    }
    private static Dispatch DetermineDispatchFromCrime(Crime crimeAssociated)
    {
        CrimeDispatch ToLookup = DispatchLookup.FirstOrDefault(x => x.CrimeIdentified == crimeAssociated);
        if(ToLookup != null && ToLookup.DispatchToPlay != null)
        {
            ToLookup.DispatchToPlay.Priority = crimeAssociated.Priority;
            return ToLookup.DispatchToPlay;
        }
        return null;
    }
    public static void ResetReportedItems()
    {
        ReportedLethalForceAuthorized = false;
        HighestCivilianReportedPriority = 99;
        HighestOfficerReportedPriority = 99;
        foreach (Dispatch ToReset in DispatchList)
        {
            ToReset.HasBeenPlayedThisWanted = false;
            ToReset.LatestInformation = new DispatchCallIn();
        }
    }   
    private static void BuildDispatch(Dispatch DispatchToPlay)
    {
        DispatchEvent EventToPlay = new DispatchEvent();
        EventToPlay.SoundsToPlay.Add(RadioStart.PickRandom());

        EventToPlay.NotificationTitle = DispatchToPlay.NotificationTitle;

        if(DispatchToPlay.IsStatus)
            EventToPlay.NotificationSubtitle = "~g~Status";
        else if(DispatchToPlay.LatestInformation.SeenByOfficers)
            EventToPlay.NotificationSubtitle = "~r~Crime Observed";
        else
            EventToPlay.NotificationSubtitle = "~o~Crime Reported";

        EventToPlay.NotificationText = DispatchToPlay.NotificationText;

        if (DispatchToPlay.IncludeAttentionAllUnits)
            AddAudioSet(EventToPlay, AttentionAllUnits.PickRandom());

        if (DispatchToPlay.IncludeReportedBy)
        {
            if (DispatchToPlay.LatestInformation.SeenByOfficers)
                AddAudioSet(EventToPlay, OfficersReport.PickRandom());
            else
                AddAudioSet(EventToPlay, CiviliansReport.PickRandom());
        }

        AddAudioSet(EventToPlay, DispatchToPlay.MainAudioSet.PickRandom());
        AddAudioSet(EventToPlay, DispatchToPlay.SecondaryAudioSet.PickRandom());

        bool AddLicense = false;

        if (!DispatchToPlay.LatestInformation.SeenByOfficers && DispatchToPlay.IncludeLicensePlate)
            AddLicense = true;

        if (DispatchToPlay.IncludeDrivingVehicle)
            AddVehicleDescription(EventToPlay, DispatchToPlay.LatestInformation.VehicleSeen, AddLicense);

        if (DispatchToPlay.IncludeRapSheet)
            AddRapSheet(EventToPlay);

        if(DispatchToPlay.MarkVehicleAsStolen && DispatchToPlay.LatestInformation != null && DispatchToPlay.LatestInformation.VehicleSeen != null)
        {
            DispatchToPlay.LatestInformation.VehicleSeen.WasReportedStolen = true;
            
            Debugging.WriteToLog("ScannerScript", "MarkedAsStolen");
        }

        if (DispatchToPlay.IncludeCarryingWeapon)
            AddWeaponDescription(EventToPlay,DispatchToPlay.LatestInformation.WeaponSeen);

        if (DispatchToPlay.ResultsInLethalForce)
            AddLethalForce(EventToPlay);

        if (DispatchToPlay.IncludeDrivingSpeed)
        {
            if (PlayerState.CurrentVehicle != null && PlayerState.CurrentVehicle.VehicleEnt.Exists())
            {
                AddSpeed(EventToPlay,PlayerState.CurrentVehicle.VehicleEnt.Speed);
            }
        }

        AddLocationDescription(EventToPlay, DispatchToPlay.LocationDescription);

        EventToPlay.SoundsToPlay.Add(RadioEnd.PickRandom());

        EventToPlay.Subtitles = FirstCharToUpper(EventToPlay.Subtitles);
        EventToPlay.Priority = DispatchToPlay.Priority;

        DispatchToPlay.SetPlayed();


        if (DispatchToPlay.LatestInformation.SeenByOfficers && DispatchToPlay.Priority < HighestOfficerReportedPriority)
            HighestOfficerReportedPriority = DispatchToPlay.Priority;
        else if (!DispatchToPlay.LatestInformation.SeenByOfficers && !DispatchToPlay.IsStatus && DispatchToPlay.Priority < HighestCivilianReportedPriority)
            HighestCivilianReportedPriority = DispatchToPlay.Priority;

        PlayDispatch(EventToPlay,DispatchToPlay.LatestInformation);
    }
    private static void PlayDispatch(DispatchEvent MyAudioEvent,DispatchCallIn MyDispatch)
    {
        /////////Maybe?
        bool AbortedAudio = false;
        if (MyAudioEvent.CanInterrupt && CurrentlyPlaying != null && CurrentlyPlaying.CanBeInterrupted && MyAudioEvent.Priority < CurrentlyPlaying.Priority)
        {
            Debugging.WriteToLog("ScannerScript", string.Format("Incoming: {0}, Playing: {1}",MyAudioEvent.NotificationText,CurrentlyPlaying.NotificationText));
            AbortAllAudio();
            AbortedAudio = true;
        }
        GameFiber PlayAudioList = GameFiber.StartNew(delegate
        {
            if (AbortedAudio)
            {
                PlayAudioFile(RadioEnd.PickRandom());
                GameFiber.Sleep(1000);
            }

            while (IsAudioPlaying)
            {
                GameFiber.Yield();
            }

            if (MyAudioEvent.NotificationTitle != "" && General.MySettings.Police.DispatchNotifications)
            {
                RemoveAllNotifications();
                NotificationHandles.Add(Game.DisplayNotification("CHAR_CALL911", "CHAR_CALL911", MyAudioEvent.NotificationTitle, MyAudioEvent.NotificationSubtitle, MyAudioEvent.NotificationText));
            }

            Debugging.WriteToLog("ScannerScript", string.Format("Name: {0}, MyAudioEvent.Priority: {1}", MyAudioEvent.NotificationText, MyAudioEvent.Priority));
            CurrentlyPlaying = MyAudioEvent;
            if (MyDispatch.VehicleSeen != null)
                MyDispatch.VehicleSeen.HasBeenDescribedByDispatch = true;

            foreach (string audioname in MyAudioEvent.SoundsToPlay)
            {
                PlayAudioFile(audioname);

                while (IsAudioPlaying)
                {
                    if (MyAudioEvent.Subtitles != "" && General.MySettings.Police.DispatchSubtitles && Game.GameTime - GameTimeLastDisplayedSubtitle >= 1500)
                    {
                        Game.DisplaySubtitle(MyAudioEvent.Subtitles, 2000);
                        GameTimeLastDisplayedSubtitle = Game.GameTime;
                    }
                    GameTimeLastAnnouncedDispatch = Game.GameTime;
                    GameFiber.Yield();
                }
                if (CancelAudio)
                {
                    CancelAudio = false;
                    Debugging.WriteToLog("ScannerScript", "CancelAudio Set to False");
                    break;
                }
            }
            CurrentlyPlaying = null;
        }, "PlayAudioList");
        Debugging.GameFibers.Add(PlayAudioList);
    }
    private static void PlayAudioFile(string _Audio)
    {
        try
        {
            if (_Audio == "")
                return;
            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped += OnPlaybackStopped;
            }
            if (audioFile == null)
            {
                audioFile = new AudioFileReader(string.Format("Plugins\\LosSantosRED\\audio\\{0}", _Audio))
                {
                    Volume = General.MySettings.Police.DispatchAudioVolume
                };
                outputDevice.Init(audioFile);
            }
            else
            {
                outputDevice.Init(audioFile);
            }
            outputDevice.Play();
        }
        catch (Exception e)
        {
            Debugging.WriteToLog("ScannerScript", e.Message);
        }
    }
    private static void AddAudioSet(DispatchEvent dispatchEvent, Dispatch.AudioSet audioSet)
    {
        if (audioSet != null)
        {
            Debugging.WriteToLog("ScannerScript", string.Format("{0}", string.Join(",", audioSet.Sounds)));
            dispatchEvent.SoundsToPlay.AddRange(audioSet.Sounds);
            dispatchEvent.Subtitles += " " + audioSet.Subtitles;
        }
    }
    private static void OnPlaybackStopped(object sender, StoppedEventArgs args)
    {
        outputDevice.Dispose();
        outputDevice = null;
        if (audioFile != null)
        {
            audioFile.Dispose();
        }
        audioFile = null;
    }
    private static void AddLocationDescription(DispatchEvent dispatchEvent, LocationSpecificity locationSpecificity)
    {
        if (locationSpecificity == LocationSpecificity.HeadingAndStreet)
            AddHeading(dispatchEvent);

        if (locationSpecificity == LocationSpecificity.Street || locationSpecificity == LocationSpecificity.HeadingAndStreet || locationSpecificity == LocationSpecificity.StreetAndZone)
            AddStreet(dispatchEvent);

        if (locationSpecificity == LocationSpecificity.Zone || locationSpecificity == LocationSpecificity.StreetAndZone)
            AddZone(dispatchEvent);
    }
    private static void AddHeading(DispatchEvent dispatchEvent)
    {
            dispatchEvent.SoundsToPlay.Add((new List<string>() { suspect_heading.TargetLastSeenHeading.FileName, suspect_heading.TargetReportedHeading.FileName, suspect_heading.TargetSeenHeading.FileName, suspect_heading.TargetSpottedHeading.FileName }).PickRandom());
            dispatchEvent.Subtitles += " ~s~suspect heading~s~";
            string heading = General.GetSimpleCompassHeading(Game.LocalPlayer.Character.Heading);
            if (heading == "N")
            {
                dispatchEvent.SoundsToPlay.Add(direction_heading.North.FileName);
                dispatchEvent.Subtitles += " ~g~North~s~";
            }
            else if (heading == "S")
            {
                dispatchEvent.SoundsToPlay.Add(direction_heading.South.FileName);
                dispatchEvent.Subtitles += " ~g~South~s~";
            }
            else if (heading == "E")
            {
                dispatchEvent.SoundsToPlay.Add(direction_heading.East.FileName);
                dispatchEvent.Subtitles += " ~g~East~s~";
            }
            else if (heading == "W")
            {
                dispatchEvent.SoundsToPlay.Add(direction_heading.West.FileName);
                dispatchEvent.Subtitles += " ~g~West~s~";
            }
    }
    private static void AddStreet(DispatchEvent dispatchEvent)
    {
        Street MyStreet = PlayerLocation.PlayerCurrentStreet;
        if (MyStreet != null && MyStreet.DispatchFile != "")
        {
            dispatchEvent.SoundsToPlay.Add((new List<string>() { conjunctives.On.FileName, conjunctives.On1.FileName, conjunctives.On2.FileName, conjunctives.On3.FileName, conjunctives.On4.FileName }).PickRandom());
            dispatchEvent.SoundsToPlay.Add(MyStreet.DispatchFile);
            dispatchEvent.Subtitles += " ~s~on ~HUD_COLOUR_YELLOWLIGHT~" + MyStreet.Name + "~s~";
            dispatchEvent.NotificationText += "~n~~HUD_COLOUR_YELLOWLIGHT~" + MyStreet.Name + "~s~";

            if (PlayerLocation.PlayerCurrentCrossStreet != null)
            {
                Street MyCrossStreet = PlayerLocation.PlayerCurrentCrossStreet;
                if (MyCrossStreet != null && MyCrossStreet.DispatchFile != "")
                {
                    dispatchEvent.SoundsToPlay.Add((new List<string>() { conjunctives.AT01.FileName,conjunctives.AT02.FileName }).PickRandom());
                    dispatchEvent.SoundsToPlay.Add(MyCrossStreet.DispatchFile);
                    dispatchEvent.NotificationText += " ~s~at ~HUD_COLOUR_YELLOWLIGHT~" + MyCrossStreet.Name + "~s~";
                    dispatchEvent.Subtitles += " ~s~at ~HUD_COLOUR_YELLOWLIGHT~" + MyCrossStreet.Name + "~s~";
                }
            }
        }
    }
    private static void AddZone(DispatchEvent dispatchEvent)
    {
        Zone MyZone = Zones.GetZoneAtLocation(dispatchEvent.PositionToReport);
        if (MyZone != null && MyZone.ScannerValue != "")
        {
            dispatchEvent.SoundsToPlay.Add(new List<string> { conjunctives.Nearumm.FileName, conjunctives.Closetoum.FileName, conjunctives.Closetouhh.FileName }.PickRandom());
            dispatchEvent.SoundsToPlay.Add(MyZone.ScannerValue);
            dispatchEvent.Subtitles += " ~s~near ~p~" + MyZone.DisplayName + "~s~";
            dispatchEvent.NotificationText += "~n~~p~" + MyZone.DisplayName + "~s~";
        }
    }
    private static void AddVehicleDescription(DispatchEvent dispatchEvent, VehicleExt VehicleToDescribe, bool IncludeLicensePlate)
    {
        if (VehicleToDescribe == null)
            return;
        if (VehicleToDescribe.HasBeenDescribedByDispatch)
            return;
        //else
        //    VehicleToDescribe.HasBeenDescribedByDispatch = true;

        if (VehicleToDescribe != null && VehicleToDescribe.VehicleEnt.Exists())
        {
            dispatchEvent.NotificationText += "~n~Vehicle:~s~";
            dispatchEvent.SoundsToPlay.Add(suspect_is.SuspectIs.FileName);
            dispatchEvent.SoundsToPlay.Add(conjunctives.Drivinga.FileName);
            dispatchEvent.Subtitles += " suspect is driving a ~s~";


            Color CarColor = Vehicles.VehicleColor(VehicleToDescribe);
            string MakeName = Vehicles.MakeName(VehicleToDescribe);
            int ClassInt = Vehicles.ClassInt(VehicleToDescribe);
            string ClassName = VehicleScanner.ClassName(ClassInt);
            string ModelName = Vehicles.ModelName(VehicleToDescribe);

            string ColorAudio = VehicleScanner.ColorAudio(CarColor);
            string MakeAudio = VehicleScanner.MakeAudio(MakeName);
            string ClassAudio = VehicleScanner.ClassAudio(ClassInt);
            string ModelAudio = VehicleScanner.ModelAudio(VehicleToDescribe.VehicleEnt.Model.Hash);

            if(ColorAudio != "")
            {
                dispatchEvent.SoundsToPlay.Add(ColorAudio);
                dispatchEvent.Subtitles += " ~s~" + CarColor.Name + "~s~";
                dispatchEvent.NotificationText += " ~s~" + CarColor.Name + "~s~";
            }
            if (MakeAudio != "")
            {
                dispatchEvent.SoundsToPlay.Add(MakeAudio);
                dispatchEvent.Subtitles += " ~s~" + MakeName + "~s~";
                dispatchEvent.NotificationText += " ~s~" + MakeName + "~s~";
            }

            if (ModelAudio != "")
            {
                dispatchEvent.SoundsToPlay.Add(ModelAudio);
                dispatchEvent.Subtitles += " ~s~" + ModelName + "~s~";
                dispatchEvent.NotificationText += " ~s~" + ModelName + "~s~";
            }
            else if (ClassAudio != "")
            {
                dispatchEvent.SoundsToPlay.Add(ClassAudio);
                dispatchEvent.Subtitles += " ~s~" + ClassName + "~s~";
                dispatchEvent.NotificationText += " ~s~" + ClassName + "~s~";
            }

            if(IncludeLicensePlate)
            {
                AddAudioSet(dispatchEvent, LicensePlateSet.PickRandom());
                string LicensePlateText = VehicleToDescribe.VehicleEnt.LicensePlate;
                dispatchEvent.SoundsToPlay.AddRange(VehicleScanner.LicensePlateAudio(LicensePlateText));
                dispatchEvent.Subtitles += " ~s~" + LicensePlateText + "~s~";
                dispatchEvent.NotificationText += " ~s~Plate: " + LicensePlateText + "~s~";
            }
            
            Debugging.WriteToLog("ScannerScript", string.Format("ScannerScript Color {0}, Make {1}, Class {2}, Model {3}, RawModel {4}", CarColor.Name,MakeName,ClassName,ModelName, VehicleToDescribe.VehicleEnt.Model.Name));
        }

    }
    private static void AddWeaponDescription(DispatchEvent dispatchEvent, GTAWeapon WeaponToDescribe)
    {

        dispatchEvent.NotificationText += "~n~Weapon:~s~";
        dispatchEvent.SoundsToPlay.Add(suspect_is.SuspectIs.FileName);
        if (WeaponToDescribe == null)
        {
            dispatchEvent.SoundsToPlay.Add(carrying_weapon.Carryingaweapon.FileName);
            dispatchEvent.Subtitles += " suspect is carrying a ~r~weapon~s~";
            dispatchEvent.NotificationText += " Unknown";
        }
        else if (WeaponToDescribe.Name == "weapon_rpg")
        {
            dispatchEvent.SoundsToPlay.Add(carrying_weapon.ArmedwithanRPG.FileName);
            dispatchEvent.Subtitles += " suspect is armed with an ~r~RPG~s~";
            dispatchEvent.NotificationText += " RPG";
        }
        else if (WeaponToDescribe.Name == "weapon_bat")
        {
            dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithabat.FileName);
            dispatchEvent.Subtitles += " suspect is armed with a ~r~bat~s~";
            dispatchEvent.NotificationText += " Bat";
        }
        else if (WeaponToDescribe.Name == "weapon_grenadelauncher" || WeaponToDescribe.Name == "weapon_grenadelauncher_smoke" || WeaponToDescribe.Name == "weapon_compactlauncher")
        {
            dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithagrenadelauncher.FileName);
            dispatchEvent.Subtitles += " suspect is armed with a ~r~grenade launcher~s~";
            dispatchEvent.NotificationText += " Grenade Launcher";
        }
        else if (WeaponToDescribe.Category ==  GTAWeapon.WeaponCategory.Throwable || WeaponToDescribe.Name == "weapon_grenadelauncher_smoke" || WeaponToDescribe.Name == "weapon_compactlauncher")
        {
            dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithexplosives.FileName);
            dispatchEvent.Subtitles += " suspect is armed with a ~r~explosives~s~";
            dispatchEvent.NotificationText += " Explosives";
        }
        else if (WeaponToDescribe.Name == "weapon_dagger" || WeaponToDescribe.Name == "weapon_knife" || WeaponToDescribe.Name == "weapon_switchblade")
        {
            dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithaknife.FileName);
            dispatchEvent.Subtitles += " suspect is armed with a ~r~knife~s~";
            dispatchEvent.NotificationText += " Knife";
        }
        else if (WeaponToDescribe.Name == "weapon_minigun")
        {
            dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithaminigun.FileName);
            dispatchEvent.Subtitles += " suspect is armed with a ~r~minigun~s~";
            dispatchEvent.NotificationText += " Minigun";
        }
        else if (WeaponToDescribe.Name == "weapon_sawnoffshotgun")
        {
            dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithasawedoffshotgun.FileName);
            dispatchEvent.Subtitles += " suspect is armed with a ~r~sawed off shotgun~s~";
            dispatchEvent.NotificationText += " Sawed Off Shotgun";
        }
        else if (WeaponToDescribe.Category == GTAWeapon.WeaponCategory.LMG)
        {
            dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithamachinegun.FileName);
            dispatchEvent.Subtitles += " suspect is armed with a ~r~machine gun~s~";
            dispatchEvent.NotificationText += " Machine Gun";
        }
        else if (WeaponToDescribe.Category == GTAWeapon.WeaponCategory.Pistol)
        {
            dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithafirearm.FileName);
            dispatchEvent.Subtitles += " suspect is armed with a ~r~pistol~s~";
            dispatchEvent.NotificationText += " Pistol";
        }
        else if (WeaponToDescribe.Category == GTAWeapon.WeaponCategory.Shotgun)
        {
            dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithashotgun.FileName);
            dispatchEvent.Subtitles += " suspect is armed with a ~r~shotgun~s~";
            dispatchEvent.NotificationText += " Shotgun";
        }
        else if (WeaponToDescribe.Category == GTAWeapon.WeaponCategory.SMG)
        {
            dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithasubmachinegun.FileName);
            dispatchEvent.Subtitles += " suspect is armed with a ~r~submachine gun~s~";
            dispatchEvent.NotificationText += " Submachine Gun";
        }
        else if (WeaponToDescribe.Category == GTAWeapon.WeaponCategory.AR)
        {
            dispatchEvent.SoundsToPlay.Add(carrying_weapon.Carryinganassaultrifle.FileName);
            dispatchEvent.Subtitles += " suspect is carrying an ~r~assault rifle~s~";
            dispatchEvent.NotificationText += " Assault Rifle";
        }
        else if (WeaponToDescribe.Category == GTAWeapon.WeaponCategory.Sniper)
        {
            dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithasniperrifle.FileName);
            dispatchEvent.Subtitles += " suspect is armed with a ~r~sniper rifle~s~";
            dispatchEvent.NotificationText += " Sniper Rifle";
        }
        else if (WeaponToDescribe.Category == GTAWeapon.WeaponCategory.Heavy)
        {
            dispatchEvent.SoundsToPlay.Add(status_message.HeavilyArmed.FileName);
            dispatchEvent.Subtitles += " suspect is ~r~heaviy armed~s~";
            dispatchEvent.NotificationText += " Heavy Weapon";
        }
        else if (WeaponToDescribe.Category == GTAWeapon.WeaponCategory.Melee)
        {
            dispatchEvent.SoundsToPlay.Add(carrying_weapon.Carryingaweapon.FileName);
            dispatchEvent.Subtitles += " suspect is carrying a ~r~weapon~s~";
            dispatchEvent.NotificationText += " melee weapon";
        }
        else
        {
            int Num = General.MyRand.Next(1, 5);
            if (Num == 1)
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithafirearm.FileName);
                dispatchEvent.Subtitles += " suspect is armed with a ~r~firearm~s~";
            }
            else if (Num == 2)
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithagat.FileName);
                dispatchEvent.Subtitles += " suspect is armed with a ~r~gat~s~";
            }
            else if (Num == 3)
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.Carryingafirearm.FileName);
                dispatchEvent.Subtitles += " suspect is carrying a ~r~firearm~s~";
            }
            else
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.Carryingagat.FileName);
                dispatchEvent.Subtitles += " suspect is carrying a ~r~gat~s~";
            }
            dispatchEvent.NotificationText += " Gat";
        }      
    }
    private static void AddRapSheet(DispatchEvent dispatchEvent)
    {
        dispatchEvent.NotificationText = "Wanted For:" + WantedLevelScript.CurrentCrimes.PrintCrimes();
    }
    private static void AddSpeed(DispatchEvent dispatchEvent,float Speed)
    {
        if (Speed >= 40f)
        {
            dispatchEvent.SoundsToPlay.Add(suspect_last_seen.TargetLastReported.FileName);
            dispatchEvent.Subtitles += " ~s~target last reported~s~";
            if (Speed >= 40f && Speed < 50f)
            {
                dispatchEvent.SoundsToPlay.Add(doing_speed.Doing40mph.FileName);
                dispatchEvent.Subtitles += " ~s~doing ~o~40 mph~s~";
                dispatchEvent.NotificationText += "~n~Speed Exceeding: ~o~40 mph~s~";
            }
            else if (Speed >= 50f && Speed < 60f)
            {
                dispatchEvent.SoundsToPlay.Add(doing_speed.Doing50mph.FileName);
                dispatchEvent.Subtitles += " ~s~doing ~o~50 mph~s~";
                dispatchEvent.NotificationText += "~n~Speed Exceeding: ~o~50 mph~s~";
            }
            else if (Speed >= 60f && Speed < 70f)
            {
                dispatchEvent.SoundsToPlay.Add(doing_speed.Doing60mph.FileName);
                dispatchEvent.Subtitles += " ~s~doing ~o~60 mph~s~";
                dispatchEvent.NotificationText += "~n~Speed Exceeding: ~o~60 mph~s~";
            }
            else if (Speed >= 70f && Speed < 80f)
            {
                dispatchEvent.SoundsToPlay.Add(doing_speed.Doing70mph.FileName);
                dispatchEvent.Subtitles += " ~s~doing ~o~70 mph~s~";
                dispatchEvent.NotificationText += "~n~Speed Exceeding: ~o~70 mph~s~";
            }
            else if (Speed >= 80f && Speed < 90f)
            {
                dispatchEvent.SoundsToPlay.Add(doing_speed.Doing80mph.FileName);
                dispatchEvent.Subtitles += " ~s~doing ~o~80 mph~s~";
                dispatchEvent.NotificationText += "~n~Speed Exceeding: ~o~80 mph~s~";
            }
            else if (Speed >= 90f && Speed < 100f)
            {
                dispatchEvent.SoundsToPlay.Add(doing_speed.Doing90mph.FileName);
                dispatchEvent.Subtitles += " ~s~doing ~o~90 mph~s~";
                dispatchEvent.NotificationText += "~n~Speed Exceeding: ~o~90 mph~s~";
            }
            else if (Speed >= 100f && Speed < 104f)
            {
                dispatchEvent.SoundsToPlay.Add(doing_speed.Doing100mph.FileName);
                dispatchEvent.Subtitles += " ~s~doing ~o~100 mph~s~";
                dispatchEvent.NotificationText += "~n~Speed Exceeding: ~o~100 mph~s~";
            }
            else if (Speed >= 105f)
            {
                dispatchEvent.SoundsToPlay.Add(doing_speed.Doingover100mph.FileName);
                dispatchEvent.Subtitles += " ~s~doing ~o~over 100 mph~s~";
                dispatchEvent.NotificationText += "~n~Speed Exceeding: ~o~105 mph~s~";
            }
        }
    }
    private static void AddLethalForce(DispatchEvent dispatchEvent)
    {
        if(!ReportedLethalForceAuthorized)
        {
            AddAudioSet(dispatchEvent, LethalForce.PickRandom());
            ReportedLethalForceAuthorized = true;
        }
    }
    private static void RemoveAllNotifications()
    {
        foreach (uint handles in NotificationHandles)
        {
            Game.RemoveNotification(handles);
        }
        NotificationHandles.Clear();
    }
    public static void PlayTestAudio()
    {
        SetupLists();
        BuildDispatch(OfficerDown);
    }
    private static string FirstCharToUpper(this string input)
    {
        switch (input)
        {
            case null: throw new ArgumentNullException(nameof(input));
            case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
            default: return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
    private static void SetupLists()
    {
        SetupDispatches();
        DispatchLookup = new List<CrimeDispatch>
        {
            new CrimeDispatch(Crimes.AttemptingSuicide,AttemptingSuicide),
            new CrimeDispatch(Crimes.BrandishingWeapon,CarryingWeapon),
            new CrimeDispatch(Crimes.ChangingPlates,SuspiciousActivity),
            new CrimeDispatch(Crimes.DrivingAgainstTraffic,RecklessDriving),
            new CrimeDispatch(Crimes.DrivingOnPavement,RecklessDriving),
            new CrimeDispatch(Crimes.FelonySpeeding,FelonySpeeding),
            new CrimeDispatch(Crimes.FiringWeapon,ShotsFired),
            new CrimeDispatch(Crimes.FiringWeaponNearPolice,ShotsFiredAtAnOfficer),
            new CrimeDispatch(Crimes.GotInAirVehicleDuringChase,StealingAirVehicle),
            new CrimeDispatch(Crimes.GrandTheftAuto,GrandTheftAuto),
            new CrimeDispatch(Crimes.HitCarWithCar,VehicleHitAndRun),
            new CrimeDispatch(Crimes.HitPedWithCar,PedHitAndRun),
            new CrimeDispatch(Crimes.RunningARedLight,RunningARedLight),
            new CrimeDispatch(Crimes.HurtingCivilians,CivilianInjury),
            new CrimeDispatch(Crimes.HurtingPolice,AssaultingOfficer),
            new CrimeDispatch(Crimes.KillingCivilians,CivilianDown),
            new CrimeDispatch(Crimes.KillingPolice,OfficerDown),
            new CrimeDispatch(Crimes.Mugging,Mugging),
            new CrimeDispatch(Crimes.NonRoadworthyVehicle,SuspiciousVehicle),
            new CrimeDispatch(Crimes.ResistingArrest,ResistingArrest),
            new CrimeDispatch(Crimes.TrespessingOnGovtProperty,TrespassingOnGovernmentProperty),
            new CrimeDispatch(Crimes.DrivingStolenVehicle,DrivingAtStolenVehicle),
            new CrimeDispatch(Crimes.TerroristActivity,TerroristActivity),
            new CrimeDispatch(Crimes.BrandishingCloseCombatWeapon,CarryingWeapon),
            
        };
        DispatchList = new List<Dispatch>
        {
            OfficerDown
            ,ShotsFiredAtAnOfficer
            ,AssaultingOfficer
            ,ThreateningOfficerWithFirearm
            ,TrespassingOnGovernmentProperty
            ,StealingAirVehicle
            ,ShotsFired
            ,CarryingWeapon
            ,CivilianDown
            ,CivilianShot
            ,CivilianInjury
            ,GrandTheftAuto
            ,SuspiciousActivity
            ,CriminalActivity
            ,Mugging
            ,TerroristActivity
            ,SuspiciousVehicle
            ,DrivingAtStolenVehicle
            ,ResistingArrest
            ,AttemptingSuicide
            ,FelonySpeeding
            ,PedHitAndRun
            ,VehicleHitAndRun
            ,RecklessDriving
            ,AnnounceStolenVehicle
            ,RequestAirSupport
            ,RequestMilitaryUnits
            ,SuspectSpotted
            ,SuspectEvaded
            ,LostVisual
            ,ResumePatrol
            ,SuspectLost
            ,NoFurtherUnitsNeeded
            ,SuspectArrested
            ,SuspectWasted
            ,ChangedVehicles
            ,RequestBackup
            ,WeaponsFree
            ,LethalForceAuthorized
            ,RunningARedLight
        };

    }    
    private static void SetupDispatches()
    {

        RadioStart = new List<string>() { AudioBeeps.Radio_Start_1.FileName };
        RadioEnd = new List<string>() { AudioBeeps.Radio_End_1.FileName };
        AttentionAllUnits = new List<Dispatch.AudioSet>()
        {
            new Dispatch.AudioSet(new List<string>() { attention_all_units_gen.Attentionallunits.FileName},"attention all units"),
            new Dispatch.AudioSet(new List<string>() { attention_all_units_gen.Attentionallunits1.FileName },"attention all units"),
            new Dispatch.AudioSet(new List<string>() { attention_all_units_gen.Attentionallunits3.FileName },"attention all units"),
        };
        OfficersReport = new List<Dispatch.AudioSet>()
        {
            new Dispatch.AudioSet(new List<string>() { we_have.OfficersReport_1.FileName},"officers report"),
            new Dispatch.AudioSet(new List<string>() { we_have.OfficersReport_2.FileName },"officers report"),
        };
        CiviliansReport = new List<Dispatch.AudioSet>()
        {
            new Dispatch.AudioSet(new List<string>() { we_have.CitizensReport_1.FileName },"attention all units"),
            new Dispatch.AudioSet(new List<string>() { we_have.CitizensReport_2.FileName },"attention all units"),
            new Dispatch.AudioSet(new List<string>() { we_have.CitizensReport_3.FileName },"attention all units"),
            new Dispatch.AudioSet(new List<string>() { we_have.CitizensReport_4.FileName },"attention all units"),
        };
        LethalForce = new List<Dispatch.AudioSet>()
        {
            new Dispatch.AudioSet(new List<string>() { lethal_force.Useofdeadlyforceauthorized.FileName},"use of deadly force authorized"),
            new Dispatch.AudioSet(new List<string>() { lethal_force.Useofdeadlyforceisauthorized.FileName },"use of deadly force is authorized"),
            new Dispatch.AudioSet(new List<string>() { lethal_force.Useofdeadlyforceisauthorized1.FileName },"use of deadly force is authorized"),
            new Dispatch.AudioSet(new List<string>() { lethal_force.Useoflethalforceisauthorized.FileName },"use of lethal force is authorized"),
            new Dispatch.AudioSet(new List<string>() { lethal_force.Useofdeadlyforcepermitted1.FileName },"use of deadly force permitted"),
        };

        LicensePlateSet = new List<Dispatch.AudioSet>()
        {
            new Dispatch.AudioSet(new List<string>() { suspect_license_plate.SuspectLicensePlate.FileName},"suspect license plate"),
            new Dispatch.AudioSet(new List<string>() { suspect_license_plate.SuspectsLicensePlate01.FileName },"suspects license plate"),
            new Dispatch.AudioSet(new List<string>() { suspect_license_plate.SuspectsLicensePlate02.FileName },"suspects license plate"),
            new Dispatch.AudioSet(new List<string>() { suspect_license_plate.TargetLicensePlate.FileName },"target license plate"),
            new Dispatch.AudioSet(new List<string>() { suspect_license_plate.TargetsLicensePlate.FileName },"targets license plate"),
            new Dispatch.AudioSet(new List<string>() { suspect_license_plate.TargetVehicleLicensePlate.FileName },"target vehicle license plate"),
        };


        OfficerDown = new Dispatch()
        {
            Name = "Officer Down",
            IncludeAttentionAllUnits = true,
            ResultsInLethalForce = true,
            LocationDescription = LocationSpecificity.StreetAndZone,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { we_have.We_Have_1.FileName, crime_officer_down.AcriticalsituationOfficerdown.FileName },"we have a critical situation, officer down"),
                new Dispatch.AudioSet(new List<string>() { we_have.We_Have_1.FileName, crime_officer_down.AnofferdownpossiblyKIA.FileName },"we have an officer down, possibly KIA"),
                new Dispatch.AudioSet(new List<string>() { we_have.We_Have_1.FileName, crime_officer_down.Anofficerdown.FileName },"we have an officer down"),
                new Dispatch.AudioSet(new List<string>() { we_have.We_Have_2.FileName, crime_officer_down.Anofficerdownconditionunknown.FileName },"we have an officer down, condition unknown"),
            },
            SecondaryAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { dispatch_respond_code.AllunitsrespondCode99.FileName },"all units repond code-99"),
                new Dispatch.AudioSet(new List<string>() { dispatch_respond_code.AllunitsrespondCode99emergency.FileName },"all units repond code-99 emergency"),
                new Dispatch.AudioSet(new List<string>() { dispatch_respond_code.Code99allunitsrespond.FileName },"code-99 all units repond"),
                new Dispatch.AudioSet(new List<string>() { custom_wanted_level_line.Code99allavailableunitsconvergeonsuspect.FileName },"code-99 all available units converge on suspect"),
                new Dispatch.AudioSet(new List<string>() { custom_wanted_level_line.Wehavea1099allavailableunitsrespond.FileName },"we have a 10-99  all available units repond"),
                new Dispatch.AudioSet(new List<string>() { dispatch_respond_code.Code99allunitsrespond.FileName },"code-99 all units respond"),
                new Dispatch.AudioSet(new List<string>() { dispatch_respond_code.EmergencyallunitsrespondCode99.FileName },"emergency all units respond code-99"),
                new Dispatch.AudioSet(new List<string>() { escort_boss.Immediateassistancerequired.FileName },"immediate assistance required"),

                

            }
        };
        ShotsFiredAtAnOfficer = new Dispatch()
        {
            Name = "Shots Fired at an Officer",
            IncludeAttentionAllUnits = true,
            ResultsInLethalForce = true,
            LocationDescription = LocationSpecificity.StreetAndZone,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_shots_fired_at_an_officer.Shotsfiredatanofficer.FileName },"shots fired at an officer"),
                new Dispatch.AudioSet(new List<string>() { crime_shots_fired_at_officer.Afirearmattackonanofficer.FileName },"a firearm attack on an officer"),
                new Dispatch.AudioSet(new List<string>() { crime_shots_fired_at_officer.Anofficershot.FileName },"a officer shot"),
                new Dispatch.AudioSet(new List<string>() { crime_shots_fired_at_officer.Anofficerunderfire.FileName },"a officer under fire"),
                new Dispatch.AudioSet(new List<string>() { crime_shots_fired_at_officer.Shotsfiredatanofficer.FileName },"a shots fired at an officer"),
            },
        };
        AssaultingOfficer = new Dispatch()
        {
            Name = "Assault on an Officer",
            LocationDescription = LocationSpecificity.Street,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_assault_on_an_officer.Anassaultonanofficer.FileName },"an assault on an officer"),
                new Dispatch.AudioSet(new List<string>() { crime_assault_on_an_officer.Anofficerassault.FileName },"an officer assault"),
            },
        };
        ThreateningOfficerWithFirearm = new Dispatch()
        {
            Name = "Threatening an Officer with a Firearm",
            LocationDescription = LocationSpecificity.StreetAndZone,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_suspect_threatening_an_officer_with_a_firearm.Asuspectthreateninganofficerwithafirearm.FileName },"a suspect threatening an officer with a firearm"),
            },
        };
        TrespassingOnGovernmentProperty = new Dispatch()
        {
            Name = "Trespassing on Government Property",
            ResultsInLethalForce = true,
            LocationDescription = LocationSpecificity.Zone,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_trespassing_on_government_property.Trespassingongovernmentproperty.FileName },"trespassing on government property"),
            },
        };
        StealingAirVehicle = new Dispatch()
        {
            Name = "Stolen Air Vehicle",
            ResultsInLethalForce = true,
            IncludeDrivingVehicle = true,
            MarkVehicleAsStolen = true,
            LocationDescription = LocationSpecificity.Zone,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_stolen_aircraft.Astolenaircraft.FileName},"a stolen aircraft"),
                new Dispatch.AudioSet(new List<string>() { crime_hijacked_aircraft.Ahijackedaircraft.FileName },"a hijacked aircraft"),
                new Dispatch.AudioSet(new List<string>() { crime_theft_of_an_aircraft.Theftofanaircraft.FileName },"theft of an aircraft"),
            },
        };
        ShotsFired = new Dispatch()
        {
            Name = "Shots Fired",
            LocationDescription = LocationSpecificity.StreetAndZone,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_shooting.Afirearmssituationseveralshotsfired.FileName },"a firearms situation, several shots fired"),
                new Dispatch.AudioSet(new List<string>() { crime_shooting.Aweaponsincidentshotsfired.FileName },"a weapons incdient, shots fired"),
                new Dispatch.AudioSet(new List<string>() { crime_shoot_out.Ashootout.FileName },"a shoot-out"),
                new Dispatch.AudioSet(new List<string>() { crime_firearms_incident.AfirearmsincidentShotsfired.FileName },"a firearms incident, shots fired"),
                new Dispatch.AudioSet(new List<string>() { crime_firearms_incident.Anincidentinvolvingshotsfired.FileName },"an incident involving shots fired"),
                new Dispatch.AudioSet(new List<string>() { crime_firearms_incident.AweaponsincidentShotsfired.FileName },"a weapons incident, shots fired"),
            },
        };
        CarryingWeapon = new Dispatch()
        {
            Name = "Carrying Weapon",
            LocationDescription = LocationSpecificity.StreetAndZone,
            IncludeCarryingWeapon = true,
        };
        TerroristActivity = new Dispatch()
        {
            Name = "Terrorist Activity",
            LocationDescription = LocationSpecificity.StreetAndZone,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() {  crime_terrorist_activity.Possibleterroristactivity.FileName },"possible terrorist activity"),
                new Dispatch.AudioSet(new List<string>() {  crime_terrorist_activity.Possibleterroristactivity1.FileName},"possible terrorist activity"),
                new Dispatch.AudioSet(new List<string>() {  crime_terrorist_activity.Possibleterroristactivity2.FileName },"possible terrorist activity"),
                new Dispatch.AudioSet(new List<string>() {  crime_terrorist_activity.Terroristactivity.FileName },"terrorist activity"),
            },
        };
        CivilianDown = new Dispatch()
        {
            Name = "Civilian Down",
            LocationDescription = LocationSpecificity.StreetAndZone,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_civilian_fatality.Acivilianfatality.FileName },"civilian fatality"),
                new Dispatch.AudioSet(new List<string>() { crime_civilian_down.Aciviliandown.FileName },"civilian down"),

                new Dispatch.AudioSet(new List<string>() { crime_1_87.A187.FileName },"a 1-87"),
                new Dispatch.AudioSet(new List<string>() { crime_1_87.Ahomicide.FileName },"a homicide"),
            },
        };
        CivilianShot = new Dispatch()
        {
            Name = "Civilian Shot",
            LocationDescription = LocationSpecificity.Street,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_civillian_gsw.AcivilianGSW.FileName },"a civilian GSW"),
                new Dispatch.AudioSet(new List<string>() { crime_civillian_gsw.Acivilianshot.FileName },"a civilian shot"),
                new Dispatch.AudioSet(new List<string>() { crime_civillian_gsw.Agunshotwound.FileName },"a gunshot wound"),
            },
        };
        CivilianInjury = new Dispatch()
        {
            Name = "Civilian Injury",
            LocationDescription = LocationSpecificity.StreetAndZone,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_injured_civilian.Aninjuredcivilian.FileName },"an injured civilian"),
                new Dispatch.AudioSet(new List<string>() { crime_civilian_needing_assistance.Acivilianinneedofassistance.FileName },"a civilian in need of assistance"),
                new Dispatch.AudioSet(new List<string>() { crime_civilian_needing_assistance.Acivilianrequiringassistance.FileName },"a civilian requiring assistance"),
                new Dispatch.AudioSet(new List<string>() { crime_assault_on_a_civilian.Anassaultonacivilian.FileName },"an assault on a civilian"),
            },
        };

        GrandTheftAuto = new Dispatch()
        {
            Name = "Grand Theft Auto",
            IncludeDrivingVehicle = true,
            MarkVehicleAsStolen = true,
            IncludeLicensePlate = true,
            LocationDescription = LocationSpecificity.HeadingAndStreet,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_grand_theft_auto.Agrandtheftauto.FileName },"a grand theft auto"),
                new Dispatch.AudioSet(new List<string>() { crime_grand_theft_auto.Agrandtheftautoinprogress.FileName },"a grand theft auto in progress"),
                new Dispatch.AudioSet(new List<string>() { crime_grand_theft_auto.AGTAinprogress.FileName },"a GTA in progress"),
                new Dispatch.AudioSet(new List<string>() { crime_grand_theft_auto.AGTAinprogress1.FileName },"a GTA in progress"),
            },
        };
        SuspiciousActivity = new Dispatch()
        {
            Name = "Suspicious Activity",
            LocationDescription = LocationSpecificity.StreetAndZone,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_suspicious_activity.Suspiciousactivity.FileName },"suspicious activity"),
                new Dispatch.AudioSet(new List<string>() { crime_theft.Apossibletheft.FileName },"a possible theft"),
            },
        };
        CriminalActivity = new Dispatch()
        {
            Name = "Criminal Activity",
            LocationDescription = LocationSpecificity.Street,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_criminal_activity.Criminalactivity.FileName },"criminal activity"),
                new Dispatch.AudioSet(new List<string>() { crime_criminal_activity.Illegalactivity.FileName },"illegal activity"),
                new Dispatch.AudioSet(new List<string>() { crime_criminal_activity.Prohibitedactivity.FileName },"prohibited activity"),
            },
        };
        Mugging = new Dispatch()
        {
            Name = "Mugging",
            LocationDescription = LocationSpecificity.Street,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_mugging.Apossiblemugging.FileName },"a possible mugging"),
            },
        };
        TerroristActivity = new Dispatch()
        {
            Name = "Terrorist Activity",
            LocationDescription = LocationSpecificity.Street,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_terrorist_activity.Possibleterroristactivity.FileName },"possible terrorist activity in progress"),
                new Dispatch.AudioSet(new List<string>() { crime_terrorist_activity.Possibleterroristactivity1.FileName },"possible terrorist activity in progress"),
                new Dispatch.AudioSet(new List<string>() { crime_terrorist_activity.Possibleterroristactivity2.FileName },"possible terrorist activity in progress"),
                new Dispatch.AudioSet(new List<string>() { crime_terrorist_activity.Terroristactivity.FileName },"terrorist activity"),
            },
        };
        SuspiciousVehicle = new Dispatch()
        {
            Name = "Suspicious Vehicle",
            IncludeDrivingVehicle = true,
            LocationDescription = LocationSpecificity.StreetAndZone,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_suspicious_vehicle.Asuspiciousvehicle.FileName },"a suspicious vehicle"),
            },
        };

        DrivingAtStolenVehicle = new Dispatch()
        {
            Name = "Driving a Stolen Vehicle",
            IncludeDrivingVehicle = true,
            LocationDescription = LocationSpecificity.HeadingAndStreet,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_person_in_a_stolen_car.Apersoninastolencar.FileName},"a person in a stolen car"),
                new Dispatch.AudioSet(new List<string>() { crime_person_in_a_stolen_vehicle.Apersoninastolenvehicle.FileName },"a person in a stolen vehicle"),
            },
        };
        ResistingArrest = new Dispatch()
        {
            Name = "Resisting Arrest",
            LocationDescription = LocationSpecificity.Zone,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_person_resisting_arrest.Apersonresistingarrest.FileName },"a person resisting arrest"),
                new Dispatch.AudioSet(new List<string>() { crime_suspect_resisting_arrest.Asuspectresistingarrest.FileName },"a suspect resisiting arrest"),

                new Dispatch.AudioSet(new List<string>() { crime_1_48_resist_arrest.Acriminalresistingarrest.FileName },"a criminal resisiting arrest"),
                new Dispatch.AudioSet(new List<string>() { crime_1_48_resist_arrest.Acriminalresistingarrest1.FileName },"a criminal resisiting arrest"),
                new Dispatch.AudioSet(new List<string>() { crime_1_48_resist_arrest.Asuspectfleeingacrimescene.FileName },"a suspect fleeing a crime scene"),
                new Dispatch.AudioSet(new List<string>() { crime_1_48_resist_arrest.Asuspectontherun.FileName },"a suspect on the run"),
            }
        };
        AttemptingSuicide = new Dispatch()
        {
            Name = "Suicide Attempt",
            LocationDescription = LocationSpecificity.Street,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_9_14a_attempted_suicide.Apossibleattemptedsuicide.FileName },"a possible attempted suicide"),
                new Dispatch.AudioSet(new List<string>() { crime_9_14a_attempted_suicide.Anattemptedsuicide.FileName },"an attempted suicide")
            }
        };
        FelonySpeeding = new Dispatch()
        {
            Name = "Felony Speeding",
            IncludeDrivingVehicle = true,
            VehicleIncludesIn = true,
            IncludeDrivingSpeed = true,
            LocationDescription = LocationSpecificity.Street,
            CanAlwaysBeInterrupted = true,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_speeding_felony.Aspeedingfelony.FileName },"a speeding felony"),
                new Dispatch.AudioSet(new List<string>() { crime_5_10.A510.FileName,crime_5_10.Speedingvehicle.FileName },"a 5-10, speeding vehicle"),
            },
        };
        PedHitAndRun = new Dispatch()
        {
            Name = "Pedestrian Hit-and-Run",
            LocationDescription = LocationSpecificity.Street,
            CanAlwaysBeInterrupted = true,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_ped_struck_by_veh.Apedestrianstruck.FileName},"a pedestrian struck"),
                new Dispatch.AudioSet(new List<string>() { crime_ped_struck_by_veh.Apedestrianstruck1.FileName },"a pedestrian struck"),
                new Dispatch.AudioSet(new List<string>() { crime_ped_struck_by_veh.Apedestrianstruckbyavehicle.FileName },"a pedestrian struck by a vehicle"),
                new Dispatch.AudioSet(new List<string>() { crime_ped_struck_by_veh.Apedestrianstruckbyavehicle1.FileName },"a pedestrian struck by a vehicle"),
            },
        };
        VehicleHitAndRun = new Dispatch()
        {
            Name = "Motor Vehicle Accident",
            LocationDescription = LocationSpecificity.Street,
            CanAlwaysBeInterrupted = true,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_motor_vehicle_accident.Amotorvehicleaccident.FileName},"a motor vehicle accident"),
                new Dispatch.AudioSet(new List<string>() { crime_motor_vehicle_accident.AnAEincident.FileName },"an A&E incident"),
                new Dispatch.AudioSet(new List<string>() { crime_motor_vehicle_accident.AseriousMVA.FileName },"a serious MVA"),
            },
        };
        RunningARedLight = new Dispatch()
        {
            Name = "Running a Red Light",
            LocationDescription = LocationSpecificity.Street,
            CanAlwaysBeInterrupted = true,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_person_running_a_red_light.Apersonrunningaredlight.FileName},"a person running a red light"),
            },
        };
        RecklessDriving = new Dispatch()
        {
            Name = "Reckless Driving",
            LocationDescription = LocationSpecificity.Street,
            CanAlwaysBeInterrupted = true,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_reckless_driver.Arecklessdriver.FileName},"a reckless driver"),
                new Dispatch.AudioSet(new List<string>() { crime_5_05.A505.FileName,crime_5_05.Adriveroutofcontrol.FileName },"a 505, a driver out of control"),
            },
        };

        AnnounceStolenVehicle = new Dispatch()
        {
            Name = "Stolen Vehicle Reported",
            IsStatus = true,
            IncludeDrivingVehicle = true,
            CanAlwaysBeInterrupted = true,
            MarkVehicleAsStolen = true,
            IncludeLicensePlate = true,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() {crime_stolen_vehicle.Apossiblestolenvehicle.FileName},"a possible stolen vehicle"),
            },
        };
        RequestAirSupport = new Dispatch()
        {
            Name = "Air Support Requested",
            IsStatus = true,
            IncludeReportedBy = false,
            LocationDescription = LocationSpecificity.Zone,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { officer_requests_air_support.Officersrequestinghelicoptersupport.FileName },"officers requesting helicopter support"),
                new Dispatch.AudioSet(new List<string>() { officer_requests_air_support.Code99unitsrequestimmediateairsupport.FileName },"code-99 units request immediate air support"),
                new Dispatch.AudioSet(new List<string>() { officer_requests_air_support.Officersrequireaerialsupport.FileName },"officers require aerial support"),
                new Dispatch.AudioSet(new List<string>() { officer_requests_air_support.Officersrequireaerialsupport1.FileName },"officers require aerial support"),
                new Dispatch.AudioSet(new List<string>() { officer_requests_air_support.Officersrequireairsupport.FileName },"officers require air support"),
                new Dispatch.AudioSet(new List<string>() { officer_requests_air_support.Unitsrequestaerialsupport.FileName },"units request aerial support"),
                new Dispatch.AudioSet(new List<string>() { officer_requests_air_support.Unitsrequestingairsupport.FileName },"units requesting air support"),
                new Dispatch.AudioSet(new List<string>() { officer_requests_air_support.Unitsrequestinghelicoptersupport.FileName },"units requesting helicopter support"),
            },
        };
        RequestMilitaryUnits = new Dispatch()
        {
            IncludeAttentionAllUnits = true,
            Name = "Military Units Requested",
            IsStatus = true,
            IncludeReportedBy = false,
            LocationDescription = LocationSpecificity.Zone,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { custom_wanted_level_line.Code13militaryunitsrequested.FileName },"code-13 military units requested"),
            },
        };
        SuspectSpotted = new Dispatch()
        {
            Name = "Suspect Spotted",
            IsStatus = true,
            IncludeReportedBy = false,
            LocationDescription = LocationSpecificity.HeadingAndStreet,
            IncludeDrivingVehicle = true,
        };
        WantedSuspectSpotted = new Dispatch()
        {
            Name = "Wanted Suspect Spotted",
            IsStatus = true,
            IncludeReportedBy = false,
            IncludeRapSheet = true,
            Priority = 10,
            LocationDescription = LocationSpecificity.HeadingAndStreet,
            IncludeDrivingVehicle = true,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crime_wanted_felon_on_the_loose.Awantedfelonontheloose.FileName },"a wanted felon on the loose"),
            },
        };
        SuspectEvaded = new Dispatch()
        {
            Name = "Suspect Evaded",
            IsStatus = true,
            IncludeReportedBy = false,
            LocationDescription = LocationSpecificity.Zone,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { suspect_eluded_pt_1.SuspectEvadedPursuingOfficiers.FileName },"suspect evaded pursuing officers"),
                new Dispatch.AudioSet(new List<string>() { suspect_eluded_pt_1.OfficiersHaveLostVisualOnSuspect.FileName },"officers have lost visual on suspect"),
            },
        };
        LostVisual = new Dispatch()
        {
            Name = "Lost Visual",
            IsStatus = true,
            IncludeReportedBy = false,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { suspect_eluded_pt_2.AllUnitsStayInTheArea.FileName },"all units stay in the area"),
                new Dispatch.AudioSet(new List<string>() { suspect_eluded_pt_2.AllUnitsRemainOnAlert.FileName },"all units remain on alert"),

                new Dispatch.AudioSet(new List<string>() { suspect_eluded_pt_2.AllUnitsStandby.FileName },"all units standby"),
                new Dispatch.AudioSet(new List<string>() { suspect_eluded_pt_2.AllUnitsStayInTheArea.FileName },"all units stay in the area"),
                new Dispatch.AudioSet(new List<string>() { suspect_eluded_pt_2.AllUnitsRemainOnAlert.FileName },"all un its remain on alert"),
            },
        };
        ResumePatrol = new Dispatch()
        {
            Name = "Resume Patrol",
            IsStatus = true,
            IncludeReportedBy = false,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { officer_begin_patrol.Beginpatrol.FileName },"begin patrol"),
                new Dispatch.AudioSet(new List<string>() { officer_begin_patrol.Beginbeat.FileName },"begin beat"),

                new Dispatch.AudioSet(new List<string>() { officer_begin_patrol.Assigntopatrol.FileName },"assign to patrol"),
                new Dispatch.AudioSet(new List<string>() { officer_begin_patrol.Proceedtopatrolarea.FileName },"proceed to patrol area"),
                new Dispatch.AudioSet(new List<string>() { officer_begin_patrol.Proceedwithpatrol.FileName },"proceed with patrol"),
            },
        };
        SuspectLost = new Dispatch()
        {
            Name = "Suspect Lost",
            IsStatus = true,
            IncludeReportedBy = false,
            LocationDescription = LocationSpecificity.Zone,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { attempt_to_find.AllunitsATonsuspects20.FileName },"all units ATL on suspects 20"),
                new Dispatch.AudioSet(new List<string>() { attempt_to_find.Allunitsattempttoreacquire.FileName },"all units attempt to reacquire"),
                new Dispatch.AudioSet(new List<string>() { attempt_to_find.Allunitsattempttoreacquirevisual.FileName },"all units attempt to reacquire visual"),
                new Dispatch.AudioSet(new List<string>() { attempt_to_find.RemainintheareaATL20onsuspect.FileName },"remain in the area, ATL-20 on suspect"),
                new Dispatch.AudioSet(new List<string>() { attempt_to_find.RemainintheareaATL20onsuspect1.FileName },"remain in the area, ATL-20 on suspect"),
            },
        };
        NoFurtherUnitsNeeded = new Dispatch()
        {
            Name = "Officers On-Site, Code 4-ADAM",
            IsStatus = true,
            IncludeReportedBy = false,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { officers_on_scene.Officersareatthescene.FileName },"officers are at the scene"),
                new Dispatch.AudioSet(new List<string>() { officers_on_scene.Officersarrivedonscene.FileName },"offices have arrived on scene"),
                new Dispatch.AudioSet(new List<string>() { officers_on_scene.Officershavearrived.FileName },"officers have arrived"),
                new Dispatch.AudioSet(new List<string>() { officers_on_scene.Officersonscene.FileName },"officers on scene"),
                new Dispatch.AudioSet(new List<string>() { officers_on_scene.Officersonsite.FileName },"officers on site"),
            },
            SecondaryAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { no_further_units.Noadditionalofficersneeded.FileName },"no additional officers needed"),
                new Dispatch.AudioSet(new List<string>() { no_further_units.Noadditionalofficersneeded1.FileName },"no additional officers needed"),
                new Dispatch.AudioSet(new List<string>() { no_further_units.Nofurtherunitsrequired.FileName },"no further units required"),
                new Dispatch.AudioSet(new List<string>() { no_further_units.WereCode4Adam.FileName },"we're code-4 adam"),
                new Dispatch.AudioSet(new List<string>() { no_further_units.Code4Adamnoadditionalsupportneeded.FileName },"code-4 adam no additional support needed"),
            },
        };
        SuspectArrested = new Dispatch()
        {
            Name = "Suspect Arrested",
            IsStatus = true,
            IncludeReportedBy = false,
            CanAlwaysInterrupt = true,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crook_arrested.Officershaveapprehendedsuspect.FileName },"officers have apprehended suspect"),
                new Dispatch.AudioSet(new List<string>() { crook_arrested.Officershaveapprehendedsuspect1.FileName },"officers have apprehended suspect"),
            },
        };
        SuspectWasted = new Dispatch()
        {
            Name = "Suspect Wasted",
            IsStatus = true,
            IncludeReportedBy = false,
            CanAlwaysInterrupt = true,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { crook_killed.Criminaldown.FileName },"criminal down"),
                new Dispatch.AudioSet(new List<string>() { crook_killed.Suspectdown.FileName },"suspect down"),
                new Dispatch.AudioSet(new List<string>() { crook_killed.Suspectneutralized.FileName },"suspect neutralized"),
                new Dispatch.AudioSet(new List<string>() { crook_killed.Suspectdownmedicalexaminerenroute.FileName },"suspect down, medical examiner in route"),
                new Dispatch.AudioSet(new List<string>() { crook_killed.Suspectdowncoronerenroute.FileName },"suspect down, coroner in route"),
                new Dispatch.AudioSet(new List<string>() { crook_killed.Officershavepacifiedsuspect.FileName },"officers have pacified suspect"),
             },
        };
        ChangedVehicles = new Dispatch()
        {
            Name = "Suspect Changed Vehicle",
            IsStatus = true,
            IncludeDrivingVehicle = true,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { "" },""),
             },
        };
        RequestBackup = new Dispatch()
        {
            IncludeAttentionAllUnits = true,
            Name = "Backup Required",
            IsStatus = true,
            IncludeReportedBy = false,
            CanAlwaysInterrupt = true,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { assistance_required.Assistanceneeded.FileName },"assistance needed"),
                new Dispatch.AudioSet(new List<string>() { assistance_required.Assistancerequired.FileName },"Assistance required"),
                new Dispatch.AudioSet(new List<string>() { assistance_required.Backupneeded.FileName },"backup needed"),
                new Dispatch.AudioSet(new List<string>() { assistance_required.Backuprequired.FileName },"backup required"),
                new Dispatch.AudioSet(new List<string>() { assistance_required.Officersneeded.FileName },"officers needed"),
                new Dispatch.AudioSet(new List<string>() { assistance_required.Officersrequired.FileName },"officers required"),
             },
            SecondaryAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { dispatch_respond_code.UnitsrespondCode3.FileName },"units respond code-3"),
             },
        };
        WeaponsFree = new Dispatch()
        {
            IncludeAttentionAllUnits = true,
            Name = "Weapons Free",
            IsStatus = true,
            IncludeReportedBy = false,
            MainAudioSet = new List<Dispatch.AudioSet>()
            {
                new Dispatch.AudioSet(new List<string>() { custom_wanted_level_line.Suspectisarmedanddangerousweaponsfree.FileName },"suspect is armed and dangerous, weapons free"),
             },
        };
        LethalForceAuthorized = new Dispatch()
        {
            IncludeAttentionAllUnits = true,
            Name = "Lethal Force Authorized",
            IsStatus = true,
            IncludeReportedBy = false,
            ResultsInLethalForce = true,
        };

    }
    public static void AbortAllAudio()
    {
        DispatchQueue.Clear();
        if (IsAudioPlaying)
        {
            CancelAudio = true;
            outputDevice.Stop();
        }
        DispatchQueue.Clear();
        if (IsAudioPlaying)
        {
            CancelAudio = true;
            outputDevice.Stop();
        }
        DispatchQueue.Clear();

        RemoveAllNotifications();
    }
    private class Dispatch
    {
        private uint GameTimeLastPlayed;
        public string Name { get; set; } = "Unknown";
        public bool IncludeAttentionAllUnits { get; set; } = false;
        public bool IsStatus { get; set; } = false;
        public bool IncludeReportedBy { get; set; } = true;
        public string NotificationSubtitle
        {
            get
            {
                return Name;
            }
        }
        public string NotificationTitle { get; set; } = "Police Scanner";
        public string NotificationText
        {
            get
            {
                return Name;
            }
        }
        public List<AudioSet> MainAudioSet { get; set; } = new List<AudioSet>();
        public List<AudioSet> SecondaryAudioSet { get; set; } = new List<AudioSet>();
        public bool MarkVehicleAsStolen { get; set; } = false;
        public bool IncludeDrivingVehicle { get; set; } = false;
        public bool VehicleIncludesIn { get; set; } = false;
        public bool IncludeCarryingWeapon { get; set; } = false;
        public bool IncludeDrivingSpeed { get; set; } = false;
        public bool IncludeLicensePlate { get; set; } = false;
        public bool IncludeRapSheet { get; set; } = false;
        public bool ReportCharctersPosition { get; set; } = true;
        public int Priority { get; set; } = 99;
        public bool ResultsInLethalForce { get; set; } = false;
        public bool ResultsInStolenCarSpotted { get; set; } = false;
        public bool IsTrafficViolation { get; set; } = false;
        public bool IsAmbient { get; set; } = false;
        public int ResultingWantedLevel { get; set; }
        public bool CanAlwaysBeInterrupted { get; set; } = false;
        public bool CanAlwaysInterrupt { get; set; } = false;

        public DispatchCallIn LatestInformation { get; set; } = new DispatchCallIn();
        public bool HasBeenPlayedThisWanted { get; set; } = false;
        public bool HasRecentlyBeenPlayed
        {
            get
            {
                if (Game.GameTime - GameTimeLastPlayed <= 25000)
                    return true;
                else
                    return false;
            }
        }
        public LocationSpecificity LocationDescription { get; set; } = LocationSpecificity.Nothing;
        public Dispatch()
        {

        }
        public class AudioSet
        {
            public AudioSet(List<string> sounds, string subtitles)
            {
                Sounds = sounds;
                Subtitles = subtitles;
            }
            public List<string> Sounds { get; set; }
            public string Subtitles { get; set; }

        }
        public void SetPlayed()
        {
            GameTimeLastPlayed = Game.GameTime;
            HasBeenPlayedThisWanted = true;
        }
    }
    private class DispatchEvent
    {
        public DispatchEvent()
        {

        }
        public List<string> SoundsToPlay { get; set; } = new List<string>();
        public string Subtitles { get; set; }
        public bool CanBeInterrupted { get; set; } = true;
        public bool CanInterrupt { get; set; } = true;
        public Vector3 PositionToReport { get; set; }
        public string NotificationTitle { get; set; } = "Police Scanner";
        public string NotificationSubtitle { get; set; } = "Status";
        public string NotificationText { get; set; } = "~b~Scanner Audio";
        public int Priority { get; set; } = 99;
    }
    private class CrimeDispatch
    {
        public CrimeDispatch()
        {

        }
        public CrimeDispatch(Crime crimeIdentified, Dispatch dispatchToPlay)
        {
            CrimeIdentified = crimeIdentified;
            DispatchToPlay = dispatchToPlay;
        }
        public Crime CrimeIdentified { get; set; }
        public Dispatch DispatchToPlay { get; set; }
    }
  
}
public class DispatchCallIn
{
    public DispatchCallIn()
    {

    }

    public DispatchCallIn(bool seenOnFoot, bool seenByOfficers, Vector3 placeSeen)
    {
        SeenOnFoot = seenOnFoot;
        SeenByOfficers = seenByOfficers;
        PlaceSeen = placeSeen;
    }
    public float Speed { get; set; }
    public GTAWeapon WeaponSeen { get; set; }
    public VehicleExt VehicleSeen { get; set; }
    public bool SeenOnFoot { get; set; } = true;
    public bool SeenByOfficers { get; set; } = false;
    public Vector3 PlaceSeen { get; set; }
}

