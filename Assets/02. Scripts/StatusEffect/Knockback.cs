using UnityEngine;

public class Knockback : StatusEffect
{
    private Vector2 origin;

    public Knockback(int level, float duration, Entity caster = null, params object[] datas) : base(level, duration, caster, datas)
    {
        origin = datas[0] is Vector2 v ? v : caster.transform.position;
    }

    public override StatusEffectType Type => StatusEffectType.HARD_CC;

    public override void OnStart(Entity target)
    {
        // ≥ÀπÈ πÊ«‚
        Vector2 dir = ((Vector2)target.transform.position - origin).normalized;
        target.AddEffect(new Stun(Level, Duration, Caster));
        target.rb2d.AddForce(dir * Level, ForceMode2D.Impulse);
    }

    public override void OnUpdate(Entity target)
    {
    }

    public override void OnFinish(Entity target)
    {
        target.rb2d.linearVelocity = Vector2.zero;
    }
}
