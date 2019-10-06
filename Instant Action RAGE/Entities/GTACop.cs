﻿
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class GTACop
{
    static GTACop()
    {
        rnd = new Random();
    }
    public GTACop()
    {

    }
    public GTACop(Ped _Cop,bool _canSeePlayer)
    {
        CopPed = _Cop;
        canSeePlayer = _canSeePlayer;
    }
    public GTACop(Ped _Cop, bool _canSeePlayer, uint _gameTimeLastSeenPlayer,Vector3 _positionLastSeenPlayer)
    {
        CopPed = _Cop;
        canSeePlayer = _canSeePlayer;
        GameTimeLastSeenPlayer = _gameTimeLastSeenPlayer;
        PositionLastSeenPlayer = _positionLastSeenPlayer;
    }

    private static Random rnd;
    public Ped CopPed { get; set; }
    public string SimpleTaskName { get; set; }
    public bool canSeePlayer { get; set; }
    public bool isTasked { get; set; } = false;
    public uint GameTimeLastSeenPlayer { get; set; }
    public Vector3 PositionLastSeenPlayer { get; set; }
    public bool isPursuitPrimary { get; set; } = false;
    public bool HurtByPlayer { get; set; } = false;
    public GameFiber TaskFiber { get; set; }
    public bool SetTazer { get; set; } = false;
    public bool SetUnarmed { get; set; } = false;
    public bool SetDeadly { get; set; } = false;
    public bool WasInVehicle { get; set; } = false;
    public bool TaskIsQueued { get; set; } = false;
    public uint GameTimeLastWeaponCheck { get; set; }
    public uint GameTimeLastTask { get; set; }
    public uint GameTimeLastSpoke { get; set; }
    public bool isDriveTasked { get; set; } = false;
    public string SubTaskName { get; set; }
    public bool isInVehicle { get; set; } = false;
    public bool isInHelicopter { get; set; } = false;
    public bool isOnBike { get; set; } = false;
    public float DistanceToPlayer { get; set; }
    public bool NeedsWeaponCheck
    {
        get
        {
            if (GameTimeLastWeaponCheck == 0)
                return true;
            else if (Game.GameTime > GameTimeLastWeaponCheck + 500)
                return true;
            else
                return false;
        }       
    }
    public bool CanSpeak
    {
        get
        {
            if (GameTimeLastSpoke == 0)
                return true;
            else if (Game.GameTime > GameTimeLastSpoke + 15000)
                return true;
            else
                return false;
        }
    }
    public bool RecentlySeenPlayer()
    {
        if (canSeePlayer)
            return true;
        else if (Game.GameTime - GameTimeLastSeenPlayer <= 10000)//Seen in last 10 seconds?
            return true;
        else
            return false;
    }
    public bool SeenPlayerSince(int _Duration)
    {
        if (canSeePlayer)
            return true;
        else if (Game.GameTime - GameTimeLastSeenPlayer <= _Duration)
            return true;
        else
            return false;
    }
}

