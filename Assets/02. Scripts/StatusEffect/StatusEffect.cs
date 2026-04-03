using System;
using UnityEngine;

[Flags]
public enum StatusEffectType
{
    NONE            = 0,
    NORMAL_CC       = 1 << 0,
    HARD_CC         = 1 << 1,
    BUFF            = 1 << 2,
}

public abstract class StatusEffect
{
    public int Level { get; protected set; }

    // 지속시간 (초)
    public float Duration { get; set; }

    // 처음 지속시간
    public float MaxDuration { get; protected set; }

    // 시전자
    public Entity Caster { get; protected set; }

    public abstract StatusEffectType Type { get; }

    public StatusEffect(int level, float duration, Entity caster = null, params object[] datas)
    {
        Level = level;
        Duration = MaxDuration = duration;
        Caster = caster;
    }

    public abstract void OnStart(Entity target);

    public abstract void OnUpdate(Entity target);

    public abstract void OnFinish(Entity target);
}
