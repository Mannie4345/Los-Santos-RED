using ExtensionsMethods;
using LosSantosRED.lsr.Interface;
using Mod;
using Rage;
using Rage.Native;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public class GangMember : PedExt, IWeaponIssuable
{
    public GangMember(Ped _Pedestrian, ISettingsProvideable settings, Gang gang, bool wasModSpawned, string _Name, ICrimes crimes, IWeapons weapons, IEntityProvideable world) : base(_Pedestrian, settings, crimes, weapons, _Name, gang.MemberName, world)
    {
        Gang = gang;
        WasModSpawned = wasModSpawned;
        WeaponInventory = new WeaponInventory(this, settings);

        ReputationReport = new ReputationReport(this);
        PedBrain = new GangBrain(this, Settings, world, weapons);
        Voice = new GangVoice(this, Settings);
    }

    public List<ReputationReport> WitnessedReports { get; private set; } = new List<ReputationReport>();
    public ReputationReport ReputationReport { get; private set; }
    public override int ShootRate { get; set; } = 400;
    public override int Accuracy { get; set; } = 5;
    public override int CombatAbility { get; set; } = 0;
    public override int TaserAccuracy { get; set; } = 10;
    public override int TaserShootRate { get; set; } = 100;
    public override int VehicleAccuracy { get; set; } = 10;
    public override int VehicleShootRate { get; set; } = 100;
    public override int TurretAccuracy { get; set; } = 10;
    public override int TurretShootRate { get; set; } = 1000;
    public override int InsultLimit => 2;
    public override int CollideWithPlayerLimit => 0;
    public override int PlayerStandTooCloseLimit => 1;
    public bool IsUsingMountedWeapon { get; set; } = false;
    public WeaponInventory WeaponInventory { get; private set; }
    public GangVoice Voice { get; private set; }
    public IssuableWeapon GetRandomMeleeWeapon(IWeapons weapons) => Gang.GetRandomMeleeWeapon(weapons);
    public IssuableWeapon GetRandomWeapon(bool v, IWeapons weapons) => Gang.GetRandomWeapon(v, weapons);
    public Gang Gang { get; set; } = new Gang();
    public override Color BlipColor => Gang != null ? Gang.Color : base.BlipColor;
    public override float BlipSize => 0.3f;
    public bool HasTaser { get; set; } = false;
    public override string BlipName => "Gang Member";
    public bool IsHitSquad { get; set; } = false;
    public bool IsBackupSquad { get; set; } = false;
    public bool IsAddedToPlayerGroup { get; set; } = false;
    public new string FormattedName => (PlayerKnownsName ? Name : GroupName);
    public override bool KnowsDrugAreas => true;
    public override bool KnowsGangAreas => true;
    public override bool IsGangMember { get; set; } = true;
    public bool IsGeneralBackup { get; internal set; }
    public override bool HasWeapon => WeaponInventory.HasPistol || WeaponInventory.HasLongGun;

    /// <summary>
    /// Equips the specified weapon to the gang member and ensures its ammo does not exceed the specified limit.
    /// </summary>
    /// <param name="weapon">The weapon to equip.</param>
    /// <param name="ammoLimit">The maximum ammo allowed for the weapon.</param>
    public void EquipAndLimitAmmo(IssuableWeapon weapon, int ammoLimit)
    {
        if (weapon != null && Pedestrian.Exists() && !Pedestrian.IsDead)
        {
            WeaponInventory.EquipWeapon(weapon);
            WeaponInventory.EnsureAmmoLimit(weapon, ammoLimit);
        }
    }

    public override void Update(IPerceptable perceptable, IPoliceRespondable policeRespondable, Vector3 placeLastSeen, IEntityProvideable world)
    {
        PlayerToCheck = policeRespondable;
        if (!Pedestrian.Exists())
        {
            return;
        }
        if (Pedestrian.IsAlive)
        {
            if (NeedsFullUpdate)
            {
                IsInWrithe = Pedestrian.IsInWrithe;
                UpdatePositionData();
                PlayerPerception.Update(perceptable, placeLastSeen);
                UpdateVehicleState();
                if (!IsUnconscious && PlayerPerception.DistanceToTarget <= 200f)
                {
                    if (!PlayerPerception.RanSightThisUpdate && !Settings.SettingsManager.PerformanceSettings.EnableIncreasedUpdateMode)
                    {
                        GameFiber.Yield();
                    }
                    PedViolations.Update(policeRespondable);
                    PedPerception.Update();
                    if (policeRespondable.CanBustPeds)
                    {
                        CheckPlayerBusted();
                    }
                }
                GameTimeLastUpdated = Game.GameTime;
            }
        }
        ReputationReport.Update(perceptable, world, Settings);
        CurrentHealthState.Update(policeRespondable);
    }

    // Additional methods omitted for brevity...
}

