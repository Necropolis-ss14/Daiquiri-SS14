using System.Linq;

namespace Content.Shared._Lavaland;

public static class LavalandRandom
{
    public static T Pick<T>(Dictionary<T, float> weights, System.Random random) where T : notnull
    {
        var total = weights.Values.Sum();
        if (total <= 0f)
            return weights.Keys.First();

        var roll = (float) (random.NextDouble() * total);
        foreach (var (item, weight) in weights)
        {
            roll -= weight;
            if (roll <= 0f)
                return item;
        }

        return weights.Keys.Last();
    }
}