﻿using System;
using System.Collections.Generic;


public class PossibleInteriors
{


    public PossibleInteriors()
    {

    }
    public List<Interior> GeneralInteriors { get; private set; } = new List<Interior>();
    public List<ResidenceInterior> ResidenceInteriors { get; private set; } = new List<ResidenceInterior>();

    public List<Interior> AllInteriors()
    {
        List<Interior> Allinteriors = new List<Interior>();
        Allinteriors.AddRange(GeneralInteriors);
        Allinteriors.AddRange(ResidenceInteriors);
        return Allinteriors;
    }
}

