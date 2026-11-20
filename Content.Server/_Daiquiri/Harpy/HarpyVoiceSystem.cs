using Content.Shared.Chat;
using Content.Shared.VoiceMask;

namespace Content.Server._Daiquiri.Harpy;

/// <summary>
/// Applies a harpy's chosen voice override when they speak.
/// </summary>
public sealed partial class HarpyVoiceSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VoiceMaskComponent, TransformSpeakerNameEvent>(OnTransformSpeakerName);
    }

    private void OnTransformSpeakerName(Entity<VoiceMaskComponent> ent, ref TransformSpeakerNameEvent args)
    {
        if (!ent.Comp.Active)
            return;

        if (ent.Comp.VoiceMaskName is { } name)
            args.VoiceName = name;

        if (ent.Comp.VoiceMaskSpeechVerb != null)
            args.SpeechVerb = ent.Comp.VoiceMaskSpeechVerb;
    }
}
