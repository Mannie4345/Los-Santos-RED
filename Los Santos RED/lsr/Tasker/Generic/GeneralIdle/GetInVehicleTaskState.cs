﻿using LosSantosRED.lsr.Interface;
using LSR.Vehicles;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class GetInVehicleTaskState : TaskState
{
    private PedExt PedGeneral;
    private IEntityProvideable World;
    private SeatAssigner SeatAssigner;

    private VehicleExt TaskedVehicle;
    private int TaskedSeat;
    private ISettingsProvideable Settings;
    private ITargetable Player;
    private bool BlockPermanentEvents = false;
    public GetInVehicleTaskState(PedExt pedGeneral, ITargetable player, IEntityProvideable world, SeatAssigner seatAssigner, ISettingsProvideable settings, bool blockPermanentEvents)
    {
        PedGeneral = pedGeneral;
        Player = player;
        World = world;
        SeatAssigner = seatAssigner;
        Settings = settings;
        BlockPermanentEvents = blockPermanentEvents;
    }
    public bool IsGang { get; set; } = false;
    public bool IsValid => PedGeneral != null && !PedGeneral.IsInVehicle && (!IsGang || Player.IsInVehicle) && PedGeneral.Pedestrian.Exists() && SeatAssigner != null && SeatAssigner.IsAssignmentValid();
    public string DebugName => $"GetInVehicleTaskState Vehicle {SeatAssigner?.VehicleAssigned?.Handle} Seat {SeatAssigner?.SeatAssigned}";
    public void Dispose()
    {
        Stop();
    }
    public void Start()
    {
        PedGeneral.ClearTasks(true);
        TaskEntry();
    }
    public void Stop()
    {
        PedGeneral.ClearTasks(true);
    }
    public void Update()
    {

    }
    private void TaskEntry()
    {
        if(!PedGeneral.Pedestrian.Exists())
        {
            return;
        }
        if (BlockPermanentEvents)
        {
            PedGeneral.Pedestrian.BlockPermanentEvents = true;
            PedGeneral.Pedestrian.KeepTasks = true;
        }
        if (SeatAssigner.VehicleAssigned != null && SeatAssigner.VehicleAssigned.Vehicle.Exists())
        {
            TaskedVehicle = SeatAssigner.VehicleAssigned;
            TaskedSeat = SeatAssigner.SeatAssigned;
            unsafe
            {
                int lol = 0;
                NativeFunction.CallByName<bool>("OPEN_SEQUENCE_TASK", &lol);
                NativeFunction.CallByName<bool>("TASK_ENTER_VEHICLE", 0, SeatAssigner.VehicleAssigned.Vehicle, -1, SeatAssigner.SeatAssigned, 1f, 9);
                //NativeFunction.CallByName<bool>("TASK_PAUSE", 0, RandomItems.MyRand.Next(8000, 16000));
                NativeFunction.CallByName<bool>("SET_SEQUENCE_TO_REPEAT", lol, true);
                NativeFunction.CallByName<bool>("CLOSE_SEQUENCE_TASK", lol);
                NativeFunction.CallByName<bool>("TASK_PERFORM_SEQUENCE", PedGeneral.Pedestrian, lol);
                NativeFunction.CallByName<bool>("CLEAR_SEQUENCE_TASK", &lol);
            }
        }
    }
}

