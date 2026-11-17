using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server._Daiquiri.DropPod;

public sealed partial class DropPodSystem : EntitySystem
{
    [Dependency] private SharedAudioSystem _audio = default!;

    private static readonly EntProtoId MarkerPing = "DaiquiriDropPodMarkerPing";
    private static readonly EntProtoId Marker = "DaiquiriDropPodMarker";
    private static readonly EntProtoId Landing = "DaiquiriDropPodLanding";
    private static readonly EntProtoId Landed = "DaiquiriDropPodLanded";
    private static readonly EntProtoId Opened = "DaiquiriDropPodOpened";

    private static readonly SoundSpecifier LandingSound = new SoundPathSpecifier("/Audio/_Daiquiri/DropPod/supplypodlandig.ogg");
    private static readonly SoundSpecifier OpenSound = new SoundPathSpecifier("/Audio/_Daiquiri/DropPod/closet_open.ogg");
    private static readonly SoundSpecifier WooshSound = new SoundPathSpecifier("/Audio/_Daiquiri/DropPod/podwoosh.ogg");

    public void Drop(EntityCoordinates coordinates, EntProtoId payload, int amount = 1)
    {
        if (!coordinates.IsValid(EntityManager))
            return;

        SpawnAtPosition(MarkerPing.Id, coordinates);

        Timer.Spawn(TimeSpan.FromSeconds(1.75), () => SpawnAtPosition(Marker.Id, coordinates));

        Timer.Spawn(TimeSpan.FromSeconds(3.75), () =>
        {
            SpawnAtPosition(Landing.Id, coordinates);
            _audio.PlayPvs(LandingSound, coordinates);
        });

        Timer.Spawn(TimeSpan.FromSeconds(5.55), () => SpawnAtPosition(Landed.Id, coordinates));

        Timer.Spawn(TimeSpan.FromSeconds(11.55), () =>
        {
            SpawnAtPosition(Opened.Id, coordinates);
            _audio.PlayPvs(OpenSound, coordinates);

            for (var i = 0; i < amount; i++)
                SpawnAtPosition(payload.Id, coordinates);

            _audio.PlayPvs(WooshSound, coordinates);
        });
    }
}
