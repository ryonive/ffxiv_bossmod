﻿namespace BossMod.Endwalker.Savage.P4S2Hesperos;

// state related to act 1 wreath of thorns
// note: there should be two tethered helpers for aoes on activation
class WreathOfThorns1(BossModule module) : BossComponent(module)
{
    public enum State { FirstAOEs, Towers, LastAOEs, Done }

    public State CurState { get; private set; } = State.FirstAOEs;
    private List<Actor> _relevantHelpers = new(); // 2 aoes -> 8 towers -> 2 aoes

    private IEnumerable<Actor> _firstAOEs => _relevantHelpers.Take(2);
    private IEnumerable<Actor> _towers => _relevantHelpers.Skip(2).Take(8);
    private IEnumerable<Actor> _lastAOEs => _relevantHelpers.Skip(10);

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        switch (CurState)
        {
            case State.FirstAOEs:
                if (_firstAOEs.InRadius(actor.Position, P4S2.WreathAOERadius).Any())
                {
                    hints.Add("GTFO from AOE!");
                }
                break;
            case State.Towers:
                {
                    var soakedTower = _towers.InRadius(actor.Position, P4S2.WreathTowerRadius).FirstOrDefault();
                    if (soakedTower == null)
                    {
                        hints.Add("Soak the tower!");
                    }
                    else if (Raid.WithoutSlot().Exclude(actor).InRadius(soakedTower.Position, P4S2.WreathTowerRadius).Any())
                    {
                        hints.Add("Multiple soakers for the tower!");
                    }
                }
                break;
            case State.LastAOEs:
                if (_lastAOEs.InRadius(actor.Position, P4S2.WreathAOERadius).Any())
                {
                    hints.Add("GTFO from AOE!");
                }
                break;
        }
    }

    public override void DrawArenaBackground(int pcSlot, Actor pc)
    {
        if (CurState == State.FirstAOEs || CurState == State.LastAOEs)
            foreach (var aoe in CurState == State.FirstAOEs ? _firstAOEs : _lastAOEs)
                Arena.ZoneCircle(aoe.Position, P4S2.WreathAOERadius, ArenaColor.AOE);
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        if (CurState == State.Towers)
        {
            foreach (var tower in _towers)
                Arena.AddCircle(tower.Position, P4S2.WreathTowerRadius, ArenaColor.Safe);
            foreach (var player in Raid.WithoutSlot())
                Arena.Actor(player, ArenaColor.PlayerGeneric);
        }
    }

    public override void OnTethered(Actor source, ActorTetherInfo tether)
    {
        if (source.OID == (uint)OID.Helper && tether.ID == (uint)TetherID.WreathOfThorns)
            _relevantHelpers.Add(source);
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (CurState == State.FirstAOEs && (AID)spell.Action.ID == AID.AkanthaiExplodeAOE)
            CurState = State.Towers;
        else if (CurState == State.Towers && (AID)spell.Action.ID == AID.AkanthaiExplodeTower)
            CurState = State.LastAOEs;
        else if (CurState == State.LastAOEs && (AID)spell.Action.ID == AID.AkanthaiExplodeAOE)
            CurState = State.Done;
    }
}
