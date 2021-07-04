﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosSantosRED.lsr.Interface
{
    public interface ISaveable
    {
        string PlayerName { get; }
        string ModelName { get; }
        PedVariation CurrentModelVariation { get; }
        bool IsMale { get; }
        int Money { get; }
    }
}
