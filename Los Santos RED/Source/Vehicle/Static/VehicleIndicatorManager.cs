﻿using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public static class VehicleIndicatorManager
{
    private static bool LeftBlinkerStartedTurn;
    private static bool RightBlinkerStartedTurn;
    private static int TimeWheelsTurnedRight;
    private static int TimeWheelsTurnedLeft;
    private static int TimeWheelsStraight;
    private static Vehicle CurrentVehicle;

    private static uint GameTimeStartedTurnWheelRight;
    private static uint GameTimeStartedTurnWheelLeft;
    private static uint GameTimeStartedTurnWheelStraight;

    public static bool IsRunning { get; set; }
    public static bool LeftBlinkerOn { get; private set; }
    public static bool RightBlinkerOn { get; private set; }
    public static bool HazardsOn { get; private set; }
    public static string DebugStatus
    {
        get
        {
            return string.Format("R {0},STR {1},RBST {2},L {3},STL {4},LBST {5},GTS {6}"
                            , RightBlinkerOn, GameTimeStartedTurnWheelRight, RightBlinkerStartedTurn, LeftBlinkerOn, GameTimeStartedTurnWheelLeft, LeftBlinkerStartedTurn, GameTimeStartedTurnWheelStraight);
        }
    }
    public static string Status
    {
        get
        {
            if (LeftBlinkerOn)
                return "(LI)";
            else if (RightBlinkerOn)
                return "(RI)";
            else if (HazardsOn)
                return "(HAZ)";
            else
                return "";
        }
    }
    public static void Initialize()
    {
        IsRunning = true;
    }
    public static void Dispose()
    {
        IsRunning = false;
    }
    public static void Tick()
    {
        if (IsRunning)
        {
            bool PlayerInVehicle = Game.LocalPlayer.Character.IsInAnyVehicle(false);
            if (PlayerInVehicle)
            {
                IndicatorsTick();
            }
        }
    }
    public static void ToggleHazards()
    {
        Vehicle MyCar = Game.LocalPlayer.Character.CurrentVehicle;
        if (MyCar == null || !MyCar.Exists())
            return;

        if (HazardsOn)
        {
            MyCar.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Off;
            HazardsOn = false;
        }
        else
        {
            MyCar.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Both;
            HazardsOn = true;
            LeftBlinkerOn = false;
            RightBlinkerOn = false;
            return;
        }
    }
    public static void ToggleLeftIndicator()
    {
        Vehicle MyCar = Game.LocalPlayer.Character.CurrentVehicle;
        if (MyCar == null || !MyCar.Exists())
            return;

        if (LeftBlinkerOn)
        {
            MyCar.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Off;
            LeftBlinkerOn = false;
        }
        else
        {
            MyCar.IndicatorLightsStatus = VehicleIndicatorLightsStatus.LeftOnly;
            LeftBlinkerOn = true;
            RightBlinkerOn = false;
            HazardsOn = false;
        }
    }
    public static void ToggleRightIndicator()
    {
        Vehicle MyCar = Game.LocalPlayer.Character.CurrentVehicle;
        if (MyCar == null || !MyCar.Exists())
            return;

        if (RightBlinkerOn)
        {
            MyCar.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Off;
            RightBlinkerOn = false;
        }
        else
        {
            MyCar.IndicatorLightsStatus = VehicleIndicatorLightsStatus.RightOnly;
            RightBlinkerOn = true;
            LeftBlinkerOn = false;
            HazardsOn = false;
        }
        
    }
    private static void IndicatorsTick()
    {
        CurrentVehicle = Game.LocalPlayer.Character.CurrentVehicle;
        if (CurrentVehicle == null || !CurrentVehicle.Exists())
            return;

        RightBlinkerTick();
        LeftBlinkerTick();
    }
    private static void RightBlinkerTick()
    {
        if (RightBlinkerOn)
        {
            if (CurrentVehicle.SteeringAngle <= -25f)
            {
                if (GameTimeStartedTurnWheelRight == 0)
                {
                    GameTimeStartedTurnWheelRight = Game.GameTime;
                }
            }
            else
            {
                GameTimeStartedTurnWheelRight = 0;
            }

            if (GameTimeStartedTurnWheelRight != 0 && Game.GameTime - GameTimeStartedTurnWheelRight >= 750)
            {
                RightBlinkerStartedTurn = true;
            }

        }
        if (RightBlinkerOn && RightBlinkerStartedTurn)
        {
            if (CurrentVehicle.SteeringAngle > -10f)
            {
                if (GameTimeStartedTurnWheelStraight == 0)
                {
                    GameTimeStartedTurnWheelStraight = Game.GameTime;
                }
            }
            else
            {
                GameTimeStartedTurnWheelStraight = 0;
            }
        }
        if (RightBlinkerOn && GameTimeStartedTurnWheelStraight != 0 && Game.GameTime - GameTimeStartedTurnWheelStraight >= 750)
        {
            GameTimeStartedTurnWheelRight = 0;
            GameTimeStartedTurnWheelStraight = 0;
            RightBlinkerStartedTurn = false;
            CurrentVehicle.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Off;
            RightBlinkerOn = false;
        }
    }
    private static void LeftBlinkerTick()
    {
        if (LeftBlinkerOn)
        {
            if (CurrentVehicle.SteeringAngle >= 25f)
            {
                if(GameTimeStartedTurnWheelLeft == 0)
                {
                    GameTimeStartedTurnWheelLeft = Game.GameTime;
                }
            }
            else
            {
                GameTimeStartedTurnWheelLeft = 0;
            }

            if (GameTimeStartedTurnWheelLeft != 0 && Game.GameTime - GameTimeStartedTurnWheelLeft >= 750)
            {
                LeftBlinkerStartedTurn = true;
            }

        }
        if (LeftBlinkerOn && LeftBlinkerStartedTurn)
        {
            if (CurrentVehicle.SteeringAngle < 10f)
            {
                if (GameTimeStartedTurnWheelStraight == 0)
                {
                    GameTimeStartedTurnWheelStraight = Game.GameTime;
                }
            }
            else
            {
                GameTimeStartedTurnWheelStraight = 0;
            }
        }
        if (LeftBlinkerOn && GameTimeStartedTurnWheelStraight != 0 && Game.GameTime - GameTimeStartedTurnWheelStraight >= 750)
        {
            GameTimeStartedTurnWheelLeft = 0;
            GameTimeStartedTurnWheelStraight = 0;
            LeftBlinkerStartedTurn = false;
            CurrentVehicle.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Off;
            LeftBlinkerOn = false;
        }
    }
    private static void RightBlinkerTickOld()
    {
        if (RightBlinkerOn)
        {
            if (CurrentVehicle.SteeringAngle <= -25f)
                TimeWheelsTurnedRight++;
            else
                TimeWheelsTurnedRight = 0;

            if (TimeWheelsTurnedRight >= 20)
            {
                RightBlinkerStartedTurn = true;
            }

        }
        if (RightBlinkerOn && RightBlinkerStartedTurn)
        {
            if (CurrentVehicle.SteeringAngle > -10f)
                TimeWheelsStraight++;
            else
                TimeWheelsStraight = 0;
        }
        if (RightBlinkerOn && TimeWheelsStraight >= 20)
        {
            TimeWheelsTurnedRight = 0;
            TimeWheelsStraight = 0;
            RightBlinkerStartedTurn = false;
            CurrentVehicle.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Off;
            RightBlinkerOn = false;
        }
    }
    private static void LeftBlinkerTickOld()
    {
        if (LeftBlinkerOn)
        {
            if (CurrentVehicle.SteeringAngle >= 25f)
                TimeWheelsTurnedLeft++;
            else
                TimeWheelsTurnedLeft = 0;

            if (TimeWheelsTurnedLeft >= 20)
            {
                LeftBlinkerStartedTurn = true;
            }

        }
        if (LeftBlinkerOn && LeftBlinkerStartedTurn)
        {
            if (CurrentVehicle.SteeringAngle < 10f)
                TimeWheelsStraight++;
            else
                TimeWheelsStraight = 0;
        }
        if (LeftBlinkerOn && TimeWheelsStraight >= 20)
        {
            TimeWheelsTurnedLeft = 0;
            TimeWheelsStraight = 0;
            LeftBlinkerStartedTurn = false;
            CurrentVehicle.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Off;
            LeftBlinkerOn = false;
        }
    }
}

