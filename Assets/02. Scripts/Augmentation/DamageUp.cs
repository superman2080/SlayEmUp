using UnityEngine;

public class DamageUp : Augmentation
{
    public override void AugmentationEffect(Entity sender, params object[] data)
    {
        if (sender is not IAttackable)
            return;

        var target = sender as IAttackable;
        var increment = target.attackStat.Get(AttackStatType.ATTACK_DAMAGE) * 1.1f;
        target.attackStat.SetDefault(AttackStatType.ATTACK_DAMAGE, increment);
    }

    protected override AugmentationType GetAugmentationType()
    {
        return AugmentationType.ON_SELECT;
    }
}
