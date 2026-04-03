using System;
using UnityEngine;

public class Lifesteal : Augmentation
{
    public float magnitude = 0.1f;

    /*
     0: enemy
     data[0] : player
     data[1] : amount
     */
    public override void AugmentationEffect(Entity sender, params object[] data)
    {
        Debug.Log(data);
        float val = (Level + 1) * magnitude;
        if(data[0] is Player)
        {
            var player = (data[0] as Player);
            float amount = Convert.ToSingle(data[1]) * magnitude;
            player.Heal(player, amount);
        }
    }

    protected override AugmentationType GetAugmentationType()
    {
        return AugmentationType.ON_ENEMY_TAKE_DAMAGE;
    }
}
