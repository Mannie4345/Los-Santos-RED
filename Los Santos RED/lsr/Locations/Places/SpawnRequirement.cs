﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Flags]
public enum SpawnRequirement
{
    None = 0,
    Guard = 1 << 0,
    Patrol = 1 << 1,
    AnyScenario = 1 << 2,
    LocalScenario = 1 << 3,
    StandardScenario = 1 << 4,
    //ForceGu = 1 << 5,
    //Chinese = 1 << 6,
    //Japanese = 1 << 7,
    //Snack = 1 << 8,
    //Donut = 1 << 9,
    //Sandwiches = 1 << 10,
    //Seafood = 1 << 11,
    //Burger = 1 << 12,
    //Pizza = 1 << 13,
    //FastFood = 1 << 14,
    //Bagels = 1 << 15,
    //Coffee = 1 << 16,
    //Smoothies = 1 << 17,
    //Chicken = 1 << 18,
    //Dessert = 1 << 19,
}