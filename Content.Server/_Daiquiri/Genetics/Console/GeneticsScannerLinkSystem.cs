// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;

using Content.Server.Medical.Components;
using Content.Shared.DeviceLinking;
using Content.Shared.DeviceLinking.Events;
using Content.Shared._Daiquiri.Genetics.Console;
using Robust.Shared.Containers;

namespace Content.Server._Daiquiri.Genetics.Console;

/// <summary>
/// Relays medical scanner link and container events to a connected genetics console.
/// </summary>
public sealed partial class GeneticsScannerLinkSystem : EntitySystem
{
    private const string ScannerSenderPort = "MedicalScannerSender";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MedicalScannerComponent, NewLinkEvent>(OnNewLink);
        SubscribeLocalEvent<MedicalScannerComponent, EntInsertedIntoContainerMessage>(OnInserted);
        SubscribeLocalEvent<MedicalScannerComponent, EntRemovedFromContainerMessage>(OnRemoved);

        SubscribeLocalEvent<GeneticsConsoleComponent, PortDisconnectedEvent>(OnPortDisconnected);
        SubscribeLocalEvent<DeviceLinkSinkComponent, AnchorStateChangedEvent>(OnAnchorChanged);
    }

    private void OnNewLink(Entity<MedicalScannerComponent> ent, ref NewLinkEvent args)
    {
        if (args.Sink != ent.Owner ||
            args.SinkPort != MedicalScannerComponent.ScannerPort ||
            args.SourcePort != ScannerSenderPort)
            return;

        ent.Comp.ConnectedConsole ??= args.Source;
        var ev = new ScannerConnectedEvent(ent.Owner);
        RaiseLocalEvent(args.Source, ref ev);
        SyncOccupant(args.Source, ent);
    }

    private void SyncOccupant(EntityUid console, Entity<MedicalScannerComponent> scanner)
    {
        if (scanner.Comp.BodyContainer.ContainedEntities.Count == 0)
            return;

        var ev = new ScannerInsertedEvent(scanner.Owner, scanner.Comp.BodyContainer.ContainedEntities[0]);
        RaiseLocalEvent(console, ref ev);
    }

    private void OnPortDisconnected(Entity<GeneticsConsoleComponent> ent, ref PortDisconnectedEvent args)
    {
        if (args.Port != ScannerSenderPort ||
            !TryComp<MedicalScannerComponent>(args.Sink, out var scanner) ||
            scanner.ConnectedConsole != ent.Owner)
            return;

        scanner.ConnectedConsole = null;
        var ev = new ScannerDisconnectedEvent(args.Sink);
        RaiseLocalEvent(ent.Owner, ref ev);
    }

    private void OnAnchorChanged(Entity<DeviceLinkSinkComponent> ent, ref AnchorStateChangedEvent args)
    {
        if (!TryComp<MedicalScannerComponent>(ent, out var scanner) ||
            scanner.ConnectedConsole is not {} console)
            return;

        if (args.Anchored)
        {
            var ev = new ScannerConnectedEvent(ent.Owner);
            RaiseLocalEvent(console, ref ev);
            SyncOccupant(console, (ent.Owner, scanner));
        }
        else
        {
            var ev = new ScannerDisconnectedEvent(ent.Owner);
            RaiseLocalEvent(console, ref ev);
        }
    }

    private void OnInserted(Entity<MedicalScannerComponent> ent, ref EntInsertedIntoContainerMessage args)
    {
        if (ent.Comp.ConnectedConsole is not {} console || args.Container != ent.Comp.BodyContainer)
            return;

        var ev = new ScannerInsertedEvent(ent.Owner, args.Entity);
        RaiseLocalEvent(console, ref ev);
    }

    private void OnRemoved(Entity<MedicalScannerComponent> ent, ref EntRemovedFromContainerMessage args)
    {
        if (ent.Comp.ConnectedConsole is not {} console || args.Container != ent.Comp.BodyContainer)
            return;

        var ev = new ScannerEjectedEvent(ent.Owner, args.Entity);
        RaiseLocalEvent(console, ref ev);
    }
}
