﻿namespace BossMod.Components;

// generic 'twister' component: a set of aoes that appear under players, but can't be accurately predicted until it's too late
// normally you'd predict them at the end (or slightly before the end) of some cast, or on component creation
public class GenericTwister(BossModule module, float radius, uint oid, ActionID aid = default) : GenericAOEs(module, aid, "GTFO from twister!")
{
    private AOEShapeCircle _shape = new(radius);
    private uint _twisterOID = oid;
    protected IReadOnlyList<Actor> Twisters = module.Enemies(oid);
    protected DateTime PredictedActivation;
    protected List<WPos> PredictedPositions = new();

    public IEnumerable<Actor> ActiveTwisters => Twisters.Where(v => v.EventState != 7);
    public bool Active => ActiveTwisters.Count() > 0;

    public void AddPredicted(float activationDelay)
    {
        PredictedPositions.Clear();
        PredictedPositions.AddRange(Raid.WithoutSlot().Select(a => a.Position));
        PredictedActivation = WorldState.FutureTime(activationDelay);
    }

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        foreach (var p in PredictedPositions)
            yield return new(_shape, p, default, PredictedActivation);
        foreach (var p in ActiveTwisters)
            yield return new(_shape, p.Position);
    }

    public override void OnActorCreated(Actor actor)
    {
        if (actor.OID == _twisterOID)
            PredictedPositions.Clear();
    }
}

// twister that activates immediately on init
public class ImmediateTwister : GenericTwister
{
    public ImmediateTwister(BossModule module, float radius, uint oid, float activationDelay) : base(module, radius, oid)
    {
        AddPredicted(activationDelay);
    }
}

// twister that activates on cast end, or slightly before
public class CastTwister(BossModule module, float radius, uint oid, ActionID aid, float activationDelay, float predictBeforeCastEnd = 0) : GenericTwister(module, radius, oid, aid)
{
    private float _activationDelay = activationDelay; // from cast-end to twister spawn
    private float _predictBeforeCastEnd = predictBeforeCastEnd;
    private DateTime _predictStart = DateTime.MaxValue;

    public override void Update()
    {
        if (PredictedPositions.Count == 0 && Twisters.Count == 0 && WorldState.CurrentTime >= _predictStart)
        {
            AddPredicted(_predictBeforeCastEnd + _activationDelay);
            _predictStart = DateTime.MaxValue;
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action == WatchedAction && _predictStart == DateTime.MaxValue)
        {
            _predictStart = spell.NPCFinishAt.AddSeconds(-_predictBeforeCastEnd);
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action == WatchedAction && _predictStart < DateTime.MaxValue)
        {
            // cast finished earlier than expected, just activate things now
            AddPredicted(_activationDelay);
            _predictStart = DateTime.MaxValue;
        }
    }
}
