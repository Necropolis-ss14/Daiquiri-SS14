using System.Globalization;
using System.Linq;
using Content.IntegrationTests.Fixtures;
using Content.Shared.Dataset;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.IntegrationTests.Tests.Localization;

[TestFixture]
public sealed class LocalizedDatasetPrototypeTest : GameTest
{
    [Test]
    public async Task ValidProtoIdsTest()
    {
        var pair = Pair;

        var server = pair.Server;
        var protoMan = server.ResolveDependency<IPrototypeManager>();
        var localizationMan = server.ResolveDependency<ILocalizationManager>();

        var protos = protoMan.EnumeratePrototypes<LocalizedDatasetPrototype>().OrderBy(p => p.ID);

        Assert.Multiple(() =>
        {
            // Check each prototype
            foreach (var proto in protos)
            {
                // Check each value in the prototype
                foreach (var locId in proto.Values)
                {
                    // Make sure the localization manager has a string for the LocId
                    Assert.That(localizationMan.HasString(locId), $"LocalizedDataset {proto.ID} with prefix \"{proto.Values.Prefix}\" specifies {proto.Values.Count} entries, but no localized string was found matching {locId}!");
                }
            }
        });

        var oldCulture = localizationMan.DefaultCulture;
        localizationMan.SetCulture(new CultureInfo("en-US"));
        try
        {
            Assert.Multiple(() =>
            {
                foreach (var proto in protos)
                {
                    // Check that count isn't set too low
                    var nextId = proto.Values.Prefix + (proto.Values.Count + 1);
                    Assert.That(localizationMan.HasString(nextId), Is.False, $"LocalizedDataset {proto.ID} with prefix \"{proto.Values.Prefix}\" specifies {proto.Values.Count} entries, but a localized string exists with ID {nextId}! Does count need to be raised?");
                }
            });
        }
        finally
        {
            if (oldCulture != null)
                localizationMan.SetCulture(oldCulture);
        }
    }
}
