using Unity.VisualScripting;
using UnityEngine;

public class Stun : StatusEffect
{
    private Animator animator;
    private readonly int debuffHash = Animator.StringToHash("5_Debuff");

    public Stun(int level, float duration, Entity caster = null, params object[] datas) : base(level, duration, caster, datas)
    {
    }

    public override StatusEffectType Type => StatusEffectType.HARD_CC;

    public override void OnStart(Entity target)
    {
        target.rb2d.linearVelocity = Vector2.zero;
        target.canControl = false;

        animator = target.GetComponentInChildren<Animator>();
        animator?.SetBool(debuffHash, true);
    }

    public override void OnUpdate(Entity target)
    {
        target.canControl = false;
        animator?.SetBool(debuffHash, true);
    }

    public override void OnFinish(Entity target)
    {
        target.canControl = true;
        animator?.SetBool(debuffHash, false);
    }
}
