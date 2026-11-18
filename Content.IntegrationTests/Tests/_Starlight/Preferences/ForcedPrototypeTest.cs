using Content.Client.Lobby;
using Content.IntegrationTests.Fixtures;
using Content.Server.Preferences.Managers;
using Content.Shared.Preferences;
using Robust.Client.State;
using Robust.Shared.Network;

namespace Content.IntegrationTests.Tests._Starlight.Preferences;

[TestFixture]
[TestOf(typeof(ServerPreferencesManager))]
public sealed class ForcedPrototypeTest : GameTest
{
    public override PoolSettings PoolSettings => new() { InLobby = true };

    private const string AdminSet = "MobCorgi";
    private const string ClientSet = "MobCat";

    /// <summary>
    /// A client save can't set ForcedPrototype, and can't change or clear one an admin set.
    /// </summary>
    [Test]
    public async Task ClientCannotSetForcedPrototype()
    {
        var server = Pair.Server;
        var client = Pair.Client;

        var clientNet = client.ResolveDependency<IClientNetManager>();
        var clientState = client.ResolveDependency<IStateManager>();
        var clientPrefs = client.ResolveDependency<IClientPreferencesManager>();
        var serverPrefs = server.ResolveDependency<IServerPreferencesManager>();

        await Pair.RunTicksSync(1);
        await PoolManager.WaitUntil(client, () => clientState.CurrentState is LobbyState, 600);

        Assert.That(clientNet.ServerChannel, Is.Not.Null);
        var user = clientNet.ServerChannel!.UserId;

        // Tampered client, nothing set by an admin.
        await SaveFromClient("Alpha Test", ClientSet);
        await server.WaitAssertion(() => Assert.That(ServerSlot().ForcedPrototype, Is.Empty));
        await WaitForClientCopy(string.Empty);

        // What characterforceprototype does. The client is sent the new value straight away.
        await server.WaitPost(() => _ = serverPrefs.SetProfile(user, 0, ServerSlot().WithForcedPrototype(AdminSet)));
        await WaitForClientCopy(AdminSet);

        // A save sent without the admin's value, e.g. before the resend arrived.
        await SaveFromClient("Bravo Test", string.Empty);
        await server.WaitAssertion(() => Assert.That(ServerSlot().ForcedPrototype, Is.EqualTo(AdminSet)));
        await WaitForClientCopy(AdminSet);

        // Tampered client over an admin value.
        await SaveFromClient("Charlie Test", ClientSet);
        await server.WaitAssertion(() => Assert.That(ServerSlot().ForcedPrototype, Is.EqualTo(AdminSet)));
        await WaitForClientCopy(AdminSet);

        HumanoidCharacterProfile ServerSlot() => serverPrefs.GetPreferences(user).Characters[0];

        // Waits for the server's resend to reach the client.
        async Task WaitForClientCopy(string expected)
        {
            var current = string.Empty;
            for (var i = 0; i < 60; i++)
            {
                await Pair.RunTicksSync(1);
                await client.WaitPost(() => current = clientPrefs.Preferences!.Characters[0].ForcedPrototype);

                if (current == expected)
                    break;
            }

            Assert.That(current, Is.EqualTo(expected), "Client copy after the server's resend");
        }

        async Task SaveFromClient(string name, string forcedPrototype)
        {
            await client.WaitPost(() =>
            {
                var profile = clientPrefs.Preferences!.Characters[0].WithName(name).WithForcedPrototype(forcedPrototype);
                clientPrefs.UpdateCharacter(profile, 0);
            });

            await PoolManager.WaitUntil(server, () => ServerSlot().Name == name, maxTicks: 60);
        }
    }

    [Test]
    public async Task FlavorTextReachesServer()
    {
        var server = Pair.Server;
        var client = Pair.Client;

        var clientNet = client.ResolveDependency<IClientNetManager>();
        var clientState = client.ResolveDependency<IStateManager>();
        var clientPrefs = client.ResolveDependency<IClientPreferencesManager>();
        var serverPrefs = server.ResolveDependency<IServerPreferencesManager>();

        await Pair.RunTicksSync(1);
        await PoolManager.WaitUntil(client, () => clientState.CurrentState is LobbyState, 600);

        var user = clientNet.ServerChannel!.UserId;

        await client.WaitPost(() =>
        {
            var profile = clientPrefs.Preferences!.Characters[0]
                .WithPhysicalDesc("physical-text")
                .WithPersonalityDesc("personality-text")
                .WithSecrets("secrets-text")
                .WithExploitable("exploitable-text")
                .WithPersonalNotes("personal-notes-text")
                .WithOOCNotes("ooc-notes-text");

            clientPrefs.UpdateCharacter(profile, 0);
        });

        await PoolManager.WaitUntil(server,
            () => serverPrefs.GetPreferences(user).Characters[0].OOCNotes == "ooc-notes-text",
            maxTicks: 60);

        await server.WaitAssertion(() =>
        {
            var saved = serverPrefs.GetPreferences(user).Characters[0];
            Assert.Multiple(() =>
            {
                Assert.That(saved.PhysicalDescription, Is.EqualTo("physical-text"));
                Assert.That(saved.PersonalityDescription, Is.EqualTo("personality-text"));
                Assert.That(saved.Secrets, Is.EqualTo("secrets-text"));
                Assert.That(saved.ExploitableInfo, Is.EqualTo("exploitable-text"));
                Assert.That(saved.PersonalNotes, Is.EqualTo("personal-notes-text"));
                Assert.That(saved.OOCNotes, Is.EqualTo("ooc-notes-text"));
            });
        });
    }
}
