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

        var hours = (int) Time.TotalHours;
        var lastTwo = hours % 100;
        var lastOne = hours % 10;
        var form = lastOne == 1 && lastTwo != 11
            ? "час"
            : lastOne >= 2 && lastOne <= 4 && (lastTwo < 12 || lastTwo > 14)
                ? "часа"
                : "часов";
        return Loc.GetString("requirements-server-playtime", ("hours", hours), ("form", form));
    }

    public override bool Handle(ICommonSession user)
    {
        base.Handle(user);

        return _playtime.GetOverallPlaytime(user) >= Time;
    }
}
