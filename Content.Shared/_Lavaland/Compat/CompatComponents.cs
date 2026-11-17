// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Dataset;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._Lavaland.Compat;

[RegisterComponent, NetworkedComponent]
public sealed partial class TileMovementComponent : Component;

[RegisterComponent, NetworkedComponent]
public sealed partial class FlipOnHitComponent : Component;

[RegisterComponent, NetworkedComponent]
public sealed partial class UndetectableContrabandComponent : Component;

[RegisterComponent, NetworkedComponent]
public sealed partial class SharpComponent : Component;

[RegisterComponent, NetworkedComponent]
public sealed partial class MansusGraspBlockTriggerComponent : Component;

[RegisterComponent, NetworkedComponent]
public sealed partial class AlmanacBladeChestSpawnerComponent : Component;

[RegisterComponent, NetworkedComponent]
public sealed partial class RustRequiresPathStageComponent : Component
{
    [DataField]
    public int PathStage = 2;
}

[RegisterComponent, NetworkedComponent]
public sealed partial class TriggerCounterComponent : Component
{
    [DataField]
    public int Count;
}

[RegisterComponent, NetworkedComponent]
public sealed partial class TriggerCounterLimitComponent : Component
{
    [DataField]
    public int MaxCount = 1;
}

[RegisterComponent, NetworkedComponent]
public sealed partial class UseDelayBlockMeleeComponent : Component
{
    [DataField]
    public List<string> Delays = new() { "default" };
}

[RegisterComponent, ComponentProtoName("HeldGrantComponent"), NetworkedComponent]
public sealed partial class HeldGrantComponent : Component
{
    [DataField(required: true)]
    [AlwaysPushInheritance]
    public ComponentRegistry Components { get; private set; } = new();

    [ViewVariables(VVAccess.ReadWrite)]
    public Dictionary<string, bool> Active = new();
}

[RegisterComponent, NetworkedComponent]
public sealed partial class MeleeDashComponent : Component
{
    [DataField]
    public string? EmoteOnDash = "Flip";

    [DataField]
    public SoundSpecifier? DashSound = new SoundPathSpecifier("/Audio/_Goobstation/Weapons/Effects/throwhard.ogg");

    [DataField("force")]
    public float DashForce = 15f;

    [DataField("length")]
    public float MaxDashLength = 4f;
}

[RegisterComponent, NetworkedComponent]
public sealed partial class LogComponent : Component
{
    [DataField]
    public string SpawnedPrototype = "MaterialWoodPlank1";

    [DataField]
    public int SpawnCount = 2;
}

[RegisterComponent, NetworkedComponent]
public sealed partial class ChangeTemperatureOnHitComponent : Component
{
    [DataField]
    public float Heat;

    [DataField]
    public bool IgnoreResistances = true;
}

[RegisterComponent, NetworkedComponent]
public sealed partial class AdvertiseComponent : Component
{
    [DataField]
    public int MinimumWait = 8 * 60;

    [DataField]
    public int MaximumWait = 10 * 60;

    [DataField]
    public bool Prewarm = true;

    [DataField(required: true)]
    public ProtoId<LocalizedDatasetPrototype> Pack;

    [DataField]
    public TimeSpan NextAdvertisementTime = TimeSpan.Zero;
}

[RegisterComponent, NetworkedComponent]
public sealed partial class SetSelectorComponent : Component
{
    [DataField]
    public List<ProtoId<SelectableSetPrototype>> PossibleSets = new();

    [DataField]
    public List<ProtoId<SelectableSetPrototype>> AvailableSets = new();

    [DataField]
    public List<int> SelectedSets = new();

    [DataField]
    public int MaxSelectedSets = 1;

    [DataField]
    public int SetsToSelect = -1;

    [DataField]
    public EntProtoId? SpawnedStoragePrototype;

    [DataField]
    public string? SpawnedStorageContainer;

    [DataField]
    public bool OpenSpawnedStorage;

    [DataField]
    public SoundSpecifier? ApproveSound;

    [DataField]
    public bool PlaySparksEffect = true;
}
