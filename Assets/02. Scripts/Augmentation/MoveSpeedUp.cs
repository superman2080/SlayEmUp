using UnityEngine;

public class MoveSpeedUp : Augmentation
{
    public override void AugmentationEffect(Entity sender, params object[] data)
    {
        var increment = sender.defaultStat.Get(EntityStatType.MOVE_SPEED) * 1.1f;
        sender.defaultStat.SetDefault(EntityStatType.MOVE_SPEED, increment);
    }

    protected override AugmentationType GetAugmentationType()
    {
        return AugmentationType.ON_SELECT;
    }
}
