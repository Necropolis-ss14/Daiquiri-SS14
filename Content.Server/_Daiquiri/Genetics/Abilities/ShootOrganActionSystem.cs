// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Content.Server.Polymorph.Systems;
using Content.Shared._Daiquiri.Genetics.Abilities;
using Content.Shared.Body.Systems;
using Content.Shared.Popups;
using Content.Shared.Throwing;

namespace Content.Server._Daiquiri.Genetics.Abilities;

public sealed partial class ShootOrganActionSystem : EntitySystem
{
    [Dependency] private SharedBodySystem _body = default!;
    [Dependency] private ChemSpikeMutationSystem _chemSpike = default!;
    [Dependency] private PolymorphSystem _polymorph = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private ThrowingSystem _throwing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ShootOrganActionComponent, ShootOrganActionEvent>(OnShootOrganAction);
    }

    private void OnShootOrganAction(Entity<ShootOrganActionComponent> ent, ref ShootOrganActionEvent args)
    {
        args.Handled = true;

        var user = args.Performer;
        if (RemoveOrgan(ent, user) is not { } organ)
        {
            _popup.PopupEntity(Loc.GetString("MutationTongueSpike-popup-no-organ"), user, user);
            return;
        }

        if (_polymorph.PolymorphEntity(organ, ent.Comp.Polymorph) is not { } projectile)
            return;

        _chemSpike.SetActionProjectile(projectile, args.Action.Comp.Container);

        _throwing.TryThrow(projectile, coordinates: args.Target, user: user);
    }

    private EntityUid? RemoveOrgan(Entity<ShootOrganActionComponent> ent, EntityUid user)
    {
        if (ent.Comp.Organ.Count != 1)
            return null;

        var expected = ent.Comp.Organ.Values.First().Component.GetType();

        foreach (var (id, _) in _body.GetBodyOrgans(user))
        {
            if (HasComp(id, expected) && _body.RemoveOrgan(id))
                return id;
        }

        return null;
    }
}
