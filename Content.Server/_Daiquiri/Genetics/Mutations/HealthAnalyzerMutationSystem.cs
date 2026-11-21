using Content.Server._Starlight.Medical.HealthAnalyzer;
using Content.Shared._Daiquiri.Genetics.Mutations;
using Content.Shared._Starlight.Medical.HealthAnalyzer;
using Robust.Shared.Prototypes;

namespace Content.Server._Daiquiri.Genetics.Mutations;

/// <summary>
/// Shows the active and dormant mutations of the scanned patient in the health analyzer and its printed report.
/// </summary>
public sealed partial class HealthAnalyzerMutationSystem : EntitySystem
{
    [Dependency] private IPrototypeManager _prototypes = default!;

    private const string MutationColor = "#8BE28B";
    private const string DormantMutationColor = "#9AA7B8";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MutatableComponent, CollectHealthAnalyzerExtensionsEvent>(OnCollect);
    }

    private void OnCollect(Entity<MutatableComponent> patient, ref CollectHealthAnalyzerExtensionsEvent args)
    {
        foreach (var id in patient.Comp.Mutations.Keys)
        {
            Add(args.Abnormalities, "genetics-health-analyzer-mutation", id, MutationColor);
        }

        foreach (var id in patient.Comp.Dormant)
        {
            Add(args.Abnormalities, "genetics-health-analyzer-dormant-mutation", id, DormantMutationColor);
        }
    }

    private void Add(List<HealthAnalyzerAbnormalityData> abnormalities, string locId, EntProtoId<MutationComponent> id, string color)
    {
        var name = _prototypes.TryIndex(id, out var proto)
            ? proto.Name
            : id.Id;

        abnormalities.Add(new HealthAnalyzerAbnormalityData
        {
            Description = Loc.GetString(locId, ("mutation", name)),
            Color = Color.FromHex(color),
        });
    }
}
