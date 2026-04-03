using UnityEngine;
using System;

[Flags]
public enum AugmentationType
{
    NONE                    = 1 << 0,
    ON_SELECT               = 1 << 1,
    ON_UPDATE               = 1 << 2,
    ON_ATTACK               = 1 << 3,
    ON_TAKE_DAMAGE          = 1 << 4,
    ON_HEAL                 = 1 << 5,
    ON_ENEMY_TAKE_DAMAGE    = 1 << 6,
}

public abstract class Augmentation
{
    public AugmentationType AugType => GetAugmentationType();
    protected abstract AugmentationType GetAugmentationType();

    public int Level { get; set; }

    public void Select(Player target)
    {
        target.AddAugmetation(this);
    }

    public abstract void AugmentationEffect(Entity sender, params object[] data);

    public virtual void OnSelected(Player target) { }
}
