using Content.Shared.CCVar;
using Content.Shared._Daiquiri.Genetics.Mutations;
using Robust.Client.Audio;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Player;

namespace Content.Client._Daiquiri.Genetics.Mutations;

public sealed partial class DeafnessSystem : EntitySystem
{
    [Dependency] private IPlayerManager _player = default!;
    [Dependency] private IAudioManager _audio = default!;
    [Dependency] private IConfigurationManager _cfg = default!;

    private float _originalVolume;
    private bool _deaf;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DeafComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<DeafComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<DeafComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<DeafComponent, LocalPlayerDetachedEvent>(OnPlayerDetached);
    }

    private void OnStartup(Entity<DeafComponent> ent, ref ComponentStartup args)
    {
        if (_player.LocalEntity == ent.Owner)
            TryDeafen();
    }

    private void OnShutdown(Entity<DeafComponent> ent, ref ComponentShutdown args)
    {
        if (_player.LocalEntity == ent.Owner)
            TryUndeafen();
    }

    private void OnPlayerAttached(Entity<DeafComponent> ent, ref LocalPlayerAttachedEvent args)
    {
        TryDeafen();
    }

    private void OnPlayerDetached(Entity<DeafComponent> ent, ref LocalPlayerDetachedEvent args)
    {
        TryUndeafen();
    }

    private void TryDeafen()
    {
        if (_deaf)
            return;

        _originalVolume = _cfg.GetCVar(CCVars.AudioMasterVolume);
        _audio.SetMasterGain(0);
        _deaf = true;
    }

    private void TryUndeafen()
    {
        if (!_deaf)
            return;

        _audio.SetMasterGain(_originalVolume);
        _deaf = false;
    }
}