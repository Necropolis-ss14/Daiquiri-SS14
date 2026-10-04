using System;
using Content.Shared._Starlight.Playtime;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._Starlight.Abstract.Conditions;

public sealed partial class ServerPlaytimeRequirement : BaseRequirement
{
    [Dependency] public IServerPlaytimeManager _playtime = default!;

    [DataField(required: true)]
    public TimeSpan Time = TimeSpan.Zero;

    public override string GetRequirementDescription()
    {
        base.GetRequirementDescription();

        return Loc.GetString("requirements-server-playtime", ("time", Time.ToString(@"hh\:mm\:ss")));
    }

    public override bool Handle(ICommonSession user)
    {
        base.Handle(user);

        return _playtime.GetOverallPlaytime(user) >= Time;
    }
}
