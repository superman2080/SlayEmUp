using UnityEngine;

public class Berserker : Augmentation
{
    public float maxIncrement = 1.5f;
    public float incrementPerLevel = 0.5f;
    public float threshold = 0.3f;

    public override void AugmentationEffect(Entity sender, params object[] data)
    {
        float ratio = Mathf.Clamp01((1 - (sender.HP / sender.defaultStat.Get(EntityStatType.MAX_HP))) / (1f - threshold));
        float val = Mathf.Lerp(1f, maxIncrement + (incrementPerLevel * Level), ratio);

        var player = sender as Player;
        player.attackStat.Multiply(AttackStatType.ATTACK_DAMAGE, val);
        player.attackStat.Multiply(AttackStatType.ATTACK_SPEED, val);
        player.defaultStat.Multiply(EntityStatType.MOVE_SPEED, val);
    }

    protected override AugmentationType GetAugmentationType()
    {
        return AugmentationType.ON_UPDATE;
    }
}
