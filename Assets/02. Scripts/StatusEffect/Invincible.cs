using UnityEngine;

public class Invincible : StatusEffect
{
    public Invincible(int level, float duration, Entity caster = null, params object[] datas) : base(level, duration, caster, datas)
    {
    }

    public override StatusEffectType Type => StatusEffectType.BUFF;

    public override void OnFinish(Entity target)
    {
    }

    public override void OnStart(Entity target)
    {
    }

    public override void OnUpdate(Entity target)
    {
    }
}
