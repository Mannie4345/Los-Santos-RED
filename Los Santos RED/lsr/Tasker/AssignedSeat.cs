﻿using LosSantosRED.lsr.Interface;
using LSR.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class AssignedSeat
{
    public AssignedSeat(IComplexTaskable ped, VehicleExt vehicle, int seat)
    {
        Ped = ped;
        Vehicle = vehicle;
        Seat = seat;
    }

    public IComplexTaskable Ped { get; set; }
    public VehicleExt Vehicle { get; set; }
    public int Seat { get; set; }
}

