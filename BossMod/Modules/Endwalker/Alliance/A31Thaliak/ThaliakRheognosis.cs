namespace BossMod.Endwalker.Alliance.A31Thaliak;

class Rheognosis(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.RheognosisKnockback), "Raidwide + Knockback");

class RheognosisKnockback(BossModule module) : Components.Knockback(module)
{
    private Source? _knockback;

    public override IEnumerable<Source> Sources(int slot, Actor actor) => Utils.ZeroOrOne(_knockback);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.Rheognosis or AID.RheognosisPetrine)
            _knockback = new(Module.Bounds.Center, 25, WorldState.FutureTime(20.2f), Direction: spell.Rotation + 180.Degrees(), Kind: Kind.DirForward);
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.RheognosisKnockback)
            _knockback = null;
    }
}

public class RheognosisCrash(BossModule module) : Components.Exaflare(module, new AOEShapeRect(24.01f, 12)) //actually the rect is only 24, but there seem to be a few thousandth of variation in the location
{
    private static readonly Angle _rot1 = 90.Degrees();
    private static readonly Angle _rot2 = -90.Degrees();

    public override void OnEventEnvControl(byte index, uint state)
    {
        var _activation = WorldState.FutureTime(0.15f);
        if (state == 0x01000001)
        {
            if (index == 0x00)
                Lines.Add(new() { Next = new(-969, 957), Advance = 10 * _rot1.ToDirection(), NextExplosion = _activation, TimeToMove = 0.15f, ExplosionsLeft = 5, MaxShownExplosions = 5, Rotation = _rot1 });
            if (index == 0x01)
                Lines.Add(new() { Next = new(-921, 933), Advance = 10 * _rot2.ToDirection(), NextExplosion = _activation, TimeToMove = 0.15f, ExplosionsLeft = 5, MaxShownExplosions = 5, Rotation = _rot2 });
        }
        if (state == 0x02000001)
        {
            if (index == 0x00)
                Lines.Add(new() { Next = new(-969, 933), Advance = 10 * _rot1.ToDirection(), NextExplosion = _activation, TimeToMove = 0.15f, ExplosionsLeft = 5, MaxShownExplosions = 5, Rotation = _rot1 });
            if (index == 0x01)
                Lines.Add(new() { Next = new(-921, 957), Advance = 10 * _rot2.ToDirection(), NextExplosion = _activation, TimeToMove = 0.15f, ExplosionsLeft = 5, MaxShownExplosions = 5, Rotation = _rot2 });
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (Lines.Count > 0 && (AID)spell.Action.ID == AID.RheognosisCrashExaflare)
        {
            ++NumCasts;
            int index = Lines.FindIndex(item => item.Next.AlmostEqual(caster.Position, 1));
            AdvanceLine(Lines[index], caster.Position);
            if (Lines[index].ExplosionsLeft == 0)
                Lines.RemoveAt(index);
        }
    }
}
