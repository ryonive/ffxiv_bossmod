namespace BossMod.Endwalker.Trial.T02Hydaelyn;

class MousasScorn(BossModule module) : Components.CastSharedTankbuster(module, ActionID.MakeSpell(AID.MousasScorn), 4);
class MousasScornHint(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.MousasScorn), "Shared Tankbuster");
class HerossSundering(BossModule module) : Components.BaitAwayCast(module, ActionID.MakeSpell(AID.HerossSundering), new AOEShapeCone(40, 45.Degrees()));
class HerossSunderingHint(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.HerossSundering), "Tankbuster cleave");
class HerossRadiance(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.HerossRadiance));
class MagossRadiance(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.MagossRadiance));
class RadiantHalo(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.RadiantHalo));
class CrystallineStoneIII(BossModule module) : Components.StackWithCastTargets(module, ActionID.MakeSpell(AID.CrystallineStoneIII2), 6, 5);
class CrystallineBlizzardIII(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.CrystallineBlizzardIII2), 5);
class Beacon(BossModule module) : Components.ChargeAOEs(module, ActionID.MakeSpell(AID.Beacon), 3);
class Beacon2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Beacon2), new AOEShapeRect(45, 3), 10);
class HydaelynsRay(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.HydaelynsRay), new AOEShapeRect(45, 15));

class T02HydaelynStates : StateMachineBuilder
{
    public T02HydaelynStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<ParhelicCircle>()
            .ActivateOnEnter<MousasScorn>()
            .ActivateOnEnter<Echoes>()
            .ActivateOnEnter<Beacon>()
            .ActivateOnEnter<Beacon2>()
            .ActivateOnEnter<CrystallineStoneIII>()
            .ActivateOnEnter<CrystallineBlizzardIII>()
            .ActivateOnEnter<MousasScornHint>()
            .ActivateOnEnter<HerossSundering>()
            .ActivateOnEnter<HerossSunderingHint>()
            .ActivateOnEnter<HerossRadiance>()
            .ActivateOnEnter<MagossRadiance>()
            .ActivateOnEnter<HydaelynsRay>()
            .ActivateOnEnter<RadiantHalo>()
            .ActivateOnEnter<Lightwave>()
            .ActivateOnEnter<WeaponTracker>()
            .ActivateOnEnter<Exodus>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Contributed, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 790, NameID = 10453)]
public class T02Hydaelyn(WorldState ws, Actor primary) : BossModule(ws, primary, new ArenaBoundsCircle(new(100, 100), 20))
{
    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor, ArenaColor.Enemy);
        foreach (var s in Enemies(OID.CrystalOfLight))
            Arena.Actor(s, ArenaColor.Object);
    }
}
