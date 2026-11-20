using Content.Shared.Humanoid;
using Content.Shared.Inventory.Events;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;

namespace Content.Shared._Daiquiri.Harpy;

/// <summary>
/// Hides a harpy's wings and tail while wearing outer clothing that covers them.
/// </summary>
public sealed partial class HarpyVisualsSystem : EntitySystem
{
    [Dependency] private TagSystem _tagSystem = default!;
    [Dependency] private SharedHumanoidAppearanceSystem _humanoidSystem = default!;

    private static readonly ProtoId<TagPrototype> HarpyWingsTag = "HidesHarpyWings";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HarpySingerComponent, DidEquipEvent>(OnDidEquipEvent);
        SubscribeLocalEvent<HarpySingerComponent, DidUnequipEvent>(OnDidUnequipEvent);
    }

    private void OnDidEquipEvent(EntityUid uid, HarpySingerComponent component, DidEquipEvent args)
    {
        if (args.Slot == "outerClothing" && _tagSystem.HasTag(args.Equipment, HarpyWingsTag))
        {
            _humanoidSystem.SetLayerVisibility(uid, HumanoidVisualLayers.RArm, false);
            _humanoidSystem.SetLayerVisibility(uid, HumanoidVisualLayers.Tail, false);
        }
    }

    private void OnDidUnequipEvent(EntityUid uid, HarpySingerComponent component, DidUnequipEvent args)
    {
        if (args.Slot == "outerClothing" && _tagSystem.HasTag(args.Equipment, HarpyWingsTag))
        {
            _humanoidSystem.SetLayerVisibility(uid, HumanoidVisualLayers.RArm, true);
            _humanoidSystem.SetLayerVisibility(uid, HumanoidVisualLayers.Tail, true);
        }
    }
}
