using System;
using UnityEngine;

public class StatBuff<StatType> : StatusEffect where StatType : Enum
{
    private int type;

    public StatBuff(int level, float duration, Entity caster = null, params object[] datas) : base(level, duration, caster, datas)
    {
        type = Convert.ToInt32(datas[0]);
    }

    public override StatusEffectType Type => StatusEffectType.BUFF;

    public override void OnStart(Entity target)
    {
        ApplyBuff(target);
    }

    public override void OnUpdate(Entity target)
    {
        ApplyBuff(target);
    }

    public override void OnFinish(Entity target)
    {
        ApplyBuff(target);  
    }

    private void ApplyBuff(Entity target)
    {
        if(typeof(StatType) == typeof(EntityStatType))
        {
            target.defaultStat.Add((EntityStatType)type, Level);
        }

        else if(typeof(StatType) == typeof(AttackStatType) && target is IAttackable)
        {
            (target as IAttackable).attackStat.Add((AttackStatType)type, Level);
        }
    }
}

public class StatDebuff<StatType> : StatusEffect where StatType : Enum
{
    private int type;

    public StatDebuff(int level, float duration, Entity caster = null, params object[] datas) : base(level, duration, caster, datas)
    {
        type = Convert.ToInt32(datas[0]);
    }

    public override StatusEffectType Type => StatusEffectType.NORMAL_CC;

    public override void OnStart(Entity target)
    {
        ApplyDebuff(target);
    }

    public override void OnUpdate(Entity target)
    {
        ApplyDebuff(target);
    }

    public override void OnFinish(Entity target)
    {
        ApplyDebuff(target);
    }

    private void ApplyDebuff(Entity target)
    {
        if (typeof(StatType) == typeof(EntityStatType))
        {
            target.defaultStat.Add((EntityStatType)type, -Level);
        }

        else if (typeof(StatType) == typeof(AttackStatType) && target is IAttackable)
        {
            (target as IAttackable).attackStat.Add((AttackStatType)type, -Level);
        }
    }
}