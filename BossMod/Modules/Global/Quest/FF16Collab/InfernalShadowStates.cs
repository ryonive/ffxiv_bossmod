// CONTRIB: made by malediktus, not checked
namespace BossMod.Global.Quest.FF16Collab.InfernalShadow;

class InfernalShadowStates : StateMachineBuilder
{
    public InfernalShadowStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<VulcanBurst>()
            .ActivateOnEnter<Incinerate>()
            .ActivateOnEnter<SpreadingFire>()
            .ActivateOnEnter<SmolderingClaw>()
            .ActivateOnEnter<TailStrike>()
            .ActivateOnEnter<CrimsonRush>()
            .ActivateOnEnter<Fireball>()
            .ActivateOnEnter<CrimsonStreak>()
            .ActivateOnEnter<Hellfire>()
            .ActivateOnEnter<FireRampageCleave>()
            .ActivateOnEnter<FieryRampageCircle>()
            .ActivateOnEnter<FieryRampageRaidwide>()
            .ActivateOnEnter<Eruption>()
            .ActivateOnEnter<Eruption2>()
            .ActivateOnEnter<BurningStrike>()
            .ActivateOnEnter<SearingStomp>()
            .Raw.Update = () => module.PrimaryActor.HP.Cur == 1;
    }
}