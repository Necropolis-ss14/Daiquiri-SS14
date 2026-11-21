// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;

using Content.Shared.Forensics.Components;
using Content.Shared.Humanoid;
using Content.Shared._Daiquiri.Genetics.Mutations;

namespace Content.Shared._Daiquiri.Genetics;

/// <summary>
/// Simple API for getting and changing <see cref="UniqueEnzymes"/> for mobs.
/// </summary>
public sealed partial class UniqueEnzymesSystem : EntitySystem
{
    [Dependency] private MetaDataSystem _meta = default!;
    [Dependency] private MutationSystem _mutation = default!;
    [Dependency] private SharedHumanoidAppearanceSystem _humanoid = default!;

    [Dependency] private EntityQuery<FingerprintComponent> _printsQuery = default!;
    [Dependency] private EntityQuery<HumanoidAppearanceComponent> _humanoidQuery = default!;

    /// <summary>
    /// Change a mob's unique enzymes, if it is mutatable (i.e. no renaming mice and shit).
    /// </summary>
    public void ChangeEnzymes(EntityUid mob, UniqueEnzymes enzymes)
    {
        if (!_mutation.CanMutate(mob))
            return;

        _meta.SetEntityName(mob, enzymes.Name);
        if (enzymes.Prints is {} print && _printsQuery.TryComp(mob, out var prints))
        {
            prints.Fingerprint = print;
            Dirty(mob, prints);
        }

        if (enzymes.EyeColor is {} eyeColor)
            _humanoid.SetEyeColor(mob, eyeColor);
        if (enzymes.SkinColor is {} skinColor)
            _humanoid.SetSkinColor(mob, skinColor);

        if (!_humanoidQuery.TryComp(mob, out var humanoid))
            return;

        if (enzymes.Sex is {} sex)
            _humanoid.SetSex(mob, sex);
        if (enzymes.Gender is {} gender)
            _humanoid.SetGender((mob, humanoid), gender);
    }

    /// <summary>
    /// Get the unique enzymes for a mob.
    /// </summary>
    public UniqueEnzymes GetEnzymes(EntityUid mob)
    {
        var humanoid = _humanoidQuery.CompOrNull(mob);
        return new UniqueEnzymes(
            Name(mob),
            _printsQuery.CompOrNull(mob)?.Fingerprint,
            humanoid?.Sex,
            humanoid?.Gender,
            humanoid?.EyeColor,
            humanoid?.SkinColor
        );
    }
}
