using Robust.Shared.GameStates;

namespace Content.Shared._Daiquiri.Genetics.Speech;

[RegisterComponent, NetworkedComponent]
public sealed partial class CharactersAccentComponent : Component
{
    [DataField(required: true)]
    public Dictionary<char, List<string>> Chars = new();
}