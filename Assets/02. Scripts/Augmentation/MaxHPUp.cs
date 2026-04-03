using UnityEngine;

public class MaxHPUp : Augmentation
{
    public override void AugmentationEffect(Entity sender, params object[] data)
    {
        float increment = sender.defaultStat.Get(EntityStatType.MAX_HP) * 0.1f;
        float nowHP = sender.defaultStat.Get(EntityStatType.MAX_HP);

        sender.defaultStat.SetDefault(EntityStatType.MAX_HP, nowHP + increment);
        sender.Heal(sender, increment);
    }

    protected override AugmentationType GetAugmentationType()
    {
        return AugmentationType.ON_SELECT;
    }
}
