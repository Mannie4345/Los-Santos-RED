﻿using Rage;
using System.Collections.Generic;

namespace LosSantosRED.lsr.Interface
{
    public interface ICrimes
    {
        List<Crime> CrimeList { get; }

        Crime GetCrime(string v);
        void SerializeAllSettings();
        void SetEasy();
        void SetDefault();
        void SetHard();
        void SetPreferred();
    }
}