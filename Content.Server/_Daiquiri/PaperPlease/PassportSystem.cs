using Content.Shared._Daiquiri.PaperPlease;
using Content.Server.Administration.Managers;
using Content.Shared.Clothing.Components;
using Content.Shared.Examine;
using Robust.Server.Player;
using Content.Shared.Humanoid;
using Content.Shared.Mobs.Components;
using Content.Shared.Inventory.Events;
using Robust.Shared.Containers;
using Content.Shared.IdentityManagement;
using Content.Shared.Paper;
using Content.Shared.UserInterface;
using Robust.Shared.Random;

namespace Content.Server._Daiquiri.PaperPlease;

public sealed partial class PassportSystem : EntitySystem
{
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private SharedUserInterfaceSystem _ui = default!;
    [Dependency] private PaperSystem _paper = default!;
    [Dependency] private MetaDataSystem _meta = default!;
    [Dependency] private IAdminManager _admin = default!;
    [Dependency] private IPlayerManager _players = default!;

    private const int CurrentYear = 3036;

    private static readonly string[] FakeNames =
    {
        "Иван Волков", "Пётр Кузнецов", "Мария Соколова", "Ольга Лебедева",
        "Дмитрий Морозов", "Николай Новиков", "Елена Фёдорова", "Артём Егоров",
    };

    private static readonly Dictionary<string, string[]> StateNames = new()
    {
        { "SSSP", new[] { "Иван Волков", "Пётр Кузнецов", "Мария Соколова", "Ольга Лебедева", "Николай Новиков" } },
        { "SolGov", new[] { "Джон Смит", "Мэри Джонсон", "Дэвид Браун", "Эмили Дэвис", "Майкл Уилсон" } },
        { "Confederation", new[] { "Амир Халид", "Лейла Хассан", "Юсуф Наджи", "Самира Фаррух", "Тарик Бен Али" } },
        { "NovoNihon", new[] { "Хиро Танака", "Юки Сато", "Кэн Ватанабэ", "Ая Накамура", "Сора Исикава" } },
        { "Raitoria", new[] { "Кай Меркурий", "Вера Церера", "Андрэ Райт", "Лира Вега", "Сол Ортега" } },
        { "IndependentColonies", new[] { "Джек Стоун", "Роза Флинт", "Билл Карвер", "Нэнси Дрейк", "Том Харт" } },
    };

    private static readonly Dictionary<string, (string Name, Color Color, string[] Cities, string Series)> States = new()
    {
        { "SSSP", ("СССП", Color.FromHex("#c62828"), new[] { "Москва", "Ленинград", "Новосибирск" }, "СССП") },
        { "SolGov", ("СолГов", Color.FromHex("#1565c0"), new[] { "Новый Вашингтон", "Новый Пекин", "Новый Лондон" }, "СГ") },
        { "Confederation", ("Конфедерация", Color.FromHex("#6a1b9a"), new[] { "Аль-Мадина", "Свободный порт", "Тарсис" }, "КФ") },
        { "NovoNihon", ("Ново-нихоновец", Color.FromHex("#d81b60"), new[] { "Сиодзири", "Хакодате", "Киото-2" }, "НН") },
        { "Raitoria", ("Райтория", Color.FromHex("#4a148c"), new[] { "Райта", "Меркурий", "Церера-Сити" }, "РТ") },
        { "IndependentColonies", ("Союз независимых колоний", Color.FromHex("#f9a825"), new[] { "Колониальный узел", "Фронтир", "Дикий Порт" }, "СНК") },
    };

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PassportComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<NTEntryPermitComponent, MapInitEvent>(OnPermitInit);
        SubscribeLocalEvent<NTEntryPermitComponent, GotEquippedEvent>(OnPermitEquipped);
        SubscribeLocalEvent<NTEntryPermitComponent, EntInsertedIntoContainerMessage>(OnPermitInserted);
        SubscribeLocalEvent<NTEntryPermitComponent, BoundUIOpenedEvent>(OnPermitOpened);
        SubscribeLocalEvent<PassportComponent, GotEquippedEvent>(OnPassportEquipped);
        SubscribeLocalEvent<PassportComponent, BoundUIOpenedEvent>(OnPassportOpened);
        SubscribeLocalEvent<PassportComponent, ExaminedEvent>(OnPassportExamined);
        SubscribeLocalEvent<PassportComponent, PaperStampedEvent>(OnPassportStamped);
        SubscribeLocalEvent<PassportComponent, EntInsertedIntoContainerMessage>(OnPassportInserted);
    }

    private void OnMapInit(Entity<PassportComponent> ent, ref MapInitEvent args)
    {
        if (ent.Comp.RandomState)
        {
            var keys = new List<string>(States.Keys);
            ent.Comp.State = _random.Pick(keys);
        }

        Generate(ent);

        var holder = Transform(ent).ParentUid;
        if (holder.IsValid())
            BindOwner(ent, holder);
    }

    private void OnPassportEquipped(Entity<PassportComponent> ent, ref GotEquippedEvent args)
    {
        BindOwner(ent, args.EquipTarget);
    }

    private void BindOwner(Entity<PassportComponent> ent, EntityUid holder)
    {
        if (ent.Comp.Bound || !HasComp<MobStateComponent>(holder))
            return;

        var name = Identity.Name(holder, EntityManager);
        if (string.IsNullOrWhiteSpace(name) || name == "identity")
            name = MetaData(holder).EntityName;

        if (string.IsNullOrWhiteSpace(name))
            return;

        ent.Comp.Bound = true;
        ent.Comp.Owner = GetNetEntity(holder);
        ent.Comp.OwnerName = name;
        Dirty(ent);
        UpdateUi(ent);
    }

    private void OnPassportInserted(Entity<PassportComponent> ent, ref EntInsertedIntoContainerMessage args)
    {
        BindOwner(ent, args.Container.Owner);
    }

    private void OnPassportStamped(Entity<PassportComponent> ent, ref PaperStampedEvent args)
    {
        UpdateUi(ent);
    }

    private void OnPassportExamined(Entity<PassportComponent> ent, ref ExaminedEvent args)
    {
        if (!_players.TryGetSessionByEntity(args.Examiner, out var session) || !_admin.IsAdmin(session))
            return;

        var valid = string.IsNullOrEmpty(ent.Comp.CorruptionReason);
        var color = valid ? "#5abc55" : "#c62828";
        args.PushMarkup(valid
            ? $"[color={color}]АДМИН: документ валиден[/color]"
            : $"[color={color}]АДМИН: документ невалиден ({ent.Comp.CorruptionReason})[/color]");
    }

    private void OnPassportOpened(Entity<PassportComponent> ent, ref BoundUIOpenedEvent args)
    {
        if (!ent.Comp.Generated)
            Generate(ent);

        if (!ent.Comp.NameLocked && ent.Comp.Owner is { } ownerNet
            && TryGetEntity(ownerNet, out var ownerMaybe) && ownerMaybe is { } ownerUid)
        {
            var live = Identity.Name(ownerUid, EntityManager);
            if (string.IsNullOrWhiteSpace(live) || live == "identity")
                live = MetaData(ownerUid).EntityName;

            if (!string.IsNullOrWhiteSpace(live) && live != ent.Comp.OwnerName)
            {
                ent.Comp.OwnerName = live;
                Dirty(ent);
            }
        }

        UpdateUi(ent);
    }

    private void OnPermitInit(Entity<NTEntryPermitComponent> ent, ref MapInitEvent args)
    {
        if (!TryComp<PaperComponent>(ent, out var paper))
            return;

        var holder = Transform(ent).ParentUid;
        var name = holder.IsValid() && HasComp<HumanoidAppearanceComponent>(holder)
            ? Identity.Name(holder, EntityManager)
            : _random.Pick(FakeNames);

        if (ent.Comp.LocId == "nt-entry-permit-invalid" && _random.Prob(0.5f))
        {
            name = _random.Pick(FakeNames);
            ent.Comp.NameLocked = true;
        }

        _paper.SetContent((ent.Owner, paper), Loc.GetString(ent.Comp.LocId, ("name", name)));
    }

    private void OnPermitEquipped(Entity<NTEntryPermitComponent> ent, ref GotEquippedEvent args)
    {
        BindPermitOwner(ent, args.EquipTarget);
    }

    private void OnPermitInserted(Entity<NTEntryPermitComponent> ent, ref EntInsertedIntoContainerMessage args)
    {
        BindPermitOwner(ent, args.Container.Owner);
    }

    private void OnPermitOpened(Entity<NTEntryPermitComponent> ent, ref BoundUIOpenedEvent args)
    {
        if (ent.Comp.Owner is { } ownerNet && TryGetEntity(ownerNet, out var ownerMaybe) && ownerMaybe is { } ownerUid
            && !ent.Comp.NameLocked)
        {
            SetPermitName(ent, NameOf(ownerUid));
        }
    }

    private string NameOf(EntityUid uid)
    {
        var name = Identity.Name(uid, EntityManager);
        if (string.IsNullOrWhiteSpace(name) || name == "identity")
            name = MetaData(uid).EntityName;
        return name;
    }

    private void BindPermitOwner(Entity<NTEntryPermitComponent> ent, EntityUid holder)
    {
        if (ent.Comp.Bound || !HasComp<MobStateComponent>(holder))
            return;

        ent.Comp.Bound = true;
        ent.Comp.Owner = GetNetEntity(holder);

        if (!ent.Comp.NameLocked)
            SetPermitName(ent, NameOf(holder));
    }

    private void SetPermitName(Entity<NTEntryPermitComponent> ent, string name)
    {
        if (!TryComp<PaperComponent>(ent, out var paper) || string.IsNullOrWhiteSpace(name))
            return;

        _paper.SetContent((ent.Owner, paper), Loc.GetString(ent.Comp.LocId, ("name", name)));
    }

    public void Generate(Entity<PassportComponent> ent)
    {
        var comp = ent.Comp;
        if (!States.TryGetValue(comp.State, out var info))
            return;

        comp.StateName = info.Name;
        comp.StateColor = info.Color;
        comp.IssueCity = _random.Pick(info.Cities);
        comp.IssueYear = CurrentYear - _random.Next(0, 3);
        comp.ExpiryYear = comp.IssueYear + _random.Next(5, 11);
        comp.Series = info.Series;
        comp.Number = _random.Next(100000, 999999).ToString();
        comp.HasStamp = true;
        comp.HasSignature = true;

        ResolveOwner(ent);

        if (comp.ForcedInvalid || (!comp.ForcedValid && _random.Prob(0.5f)))
            Corrupt(comp, info);

        comp.Generated = true;
        Dirty(ent);
        UpdateUi(ent);
    }

    private void ResolveOwner(Entity<PassportComponent> ent)
    {
        var holder = Transform(ent).ParentUid;
        if (holder.IsValid() && HasComp<HumanoidAppearanceComponent>(holder))
        {
            ent.Comp.Owner = GetNetEntity(holder);
            ent.Comp.OwnerName = Identity.Name(holder, EntityManager);
            return;
        }

        ent.Comp.Owner = null;
        ent.Comp.OwnerName = RandomNameFor(ent.Comp.State);
    }

    private string RandomNumber(string series) => _random.Next(100000, 999999).ToString();

    private string RandomNameFor(string state)
        => StateNames.TryGetValue(state, out var names) ? _random.Pick(names) : _random.Pick(FakeNames);

    private void Corrupt(PassportComponent comp, (string Name, Color Color, string[] Cities, string Series) info)
    {
        var others = new List<string[]>();
        foreach (var state in States.Values)
        {
            if (state.Name != info.Name)
                others.Add(state.Cities);
        }

        var othersSeries = new List<string>();
        foreach (var state in States.Values)
        {
            if (state.Name != info.Name)
                othersSeries.Add(state.Series);
        }

        comp.CorruptionReason = string.Empty;
        switch (_random.Next(10))
        {
            case 0:
                comp.IssueCity = _random.Pick(_random.Pick(others));
                comp.CorruptionReason = "город выдачи чужого государства";
                break;
            case 1:
                comp.IssueYear = CurrentYear - _random.Next(9, 15);
                comp.ExpiryYear = comp.IssueYear + _random.Next(5, 8);
                comp.CorruptionReason = "срок действия истёк";
                break;
            case 2:
                comp.OwnerName = comp.OwnerName + " *";
                comp.NameLocked = true;
                comp.CorruptionReason = "ФИО подправлено";
                break;
            case 3:
                comp.Series = _random.Pick(othersSeries);
                comp.Number = RandomNumber(comp.Series);
                break;
            case 4:
                comp.IssueYear = CurrentYear + _random.Next(1, 4);
                comp.ExpiryYear = comp.IssueYear + 5;
                comp.CorruptionReason = "выдан в будущем";
                break;
            case 5:
                comp.Number = _random.Next(1000, 9999).ToString();
                comp.CorruptionReason = "номер слишком короткий";
                break;
            case 6:
                comp.ExpiryYear = comp.IssueYear - _random.Next(1, 4);
                comp.CorruptionReason = "срок раньше даты выдачи";
                break;
            case 7:
                comp.OwnerName = _random.Pick(FakeNames);
                comp.NameLocked = true;
                comp.CorruptionReason = "ФИО не совпадает с владельцем";
                break;
            case 8:
                comp.HasStamp = false;
                comp.CorruptionReason = "нет печати";
                break;
            default:
                comp.HasSignature = false;
                comp.CorruptionReason = "нет росписи";
                break;
        }
    }

    private void UpdateUi(Entity<PassportComponent> ent)
    {
        if (TryComp<PaperComponent>(ent, out var paper))
        {
            var stamps = new List<(string, Color)>();
            foreach (var stamp in paper.StampedBy)
                stamps.Add((Loc.GetString(stamp.StampedName), stamp.StampedColor));

            if (stamps.Count > 0 || true)
            {
                var state0 = new PassportUiState(ent.Comp, ent.Comp.Owner) { Stamps = stamps };
                if (TryComp<UserInterfaceComponent>(ent, out _))
                    _ui.SetUiState(ent.Owner, PassportUiKey.Key, state0);
                return;
            }
        }

        if (TryComp<UserInterfaceComponent>(ent, out var ui))
            _ui.SetUiState(ent.Owner, PassportUiKey.Key, new PassportUiState(ent.Comp, ent.Comp.Owner));
    }
}
