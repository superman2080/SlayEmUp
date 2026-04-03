using UnityEngine;

public class DefenseUp : Augmentation
{
    public float increment = 30;

    public override void AugmentationEffect(Entity sender, params object[] data)
    {
        sender.defaultStat.SetDefault(EntityStatType.DEFENSE, sender.defaultStat.Get(EntityStatType.DEFENSE) + increment);
    }

    protected override AugmentationType GetAugmentationType()
    {
        return AugmentationType.ON_SELECT;
    }
}
