using UnityEngine;

public class TestAugmentation : Augmentation
{
    public override void AugmentationEffect(Entity sender, params object[] data)
    {
        Debug.Log(sender.name + ", " + Level);
    }

    protected override AugmentationType GetAugmentationType()
    {
        return AugmentationType.ON_UPDATE;
    }
}
