using UnityEngine;

public class Unstoppable : StatusEffect
{
    private Animator animator;
    private readonly int debuffHash = Animator.StringToHash("5_Debuff");

    public override StatusEffectType Type => StatusEffectType.BUFF;

    public Unstoppable(int level, float duration, Entity caster = null, params object[] datas) : base(level, duration, caster, datas)
    {

    }

    public override void OnStart(Entity target)
    {
        animator = target.GetComponentInChildren<Animator>();
        animator?.SetBool(debuffHash, false);

        target.canControl = true;
    }

    public override void OnUpdate(Entity target)
    {
        target.canControl = true;
    }

    public override void OnFinish(Entity target)
    {
        target.canControl = true;
    }
}
