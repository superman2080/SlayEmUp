using UnityEngine;
using System;

[Serializable]
public struct AugmentationData
{
    // 실제 생성될 인스턴스 클래스의 이름
    public string className;

    // UI에서 보여줄 텍스트 이름
    public string augmentationName;

    // 증강체 설명
    [TextArea(3, 10)] public string description;

    public Sprite icon;
}

[CreateAssetMenu(fileName = "AugmentationListSO", menuName = "Scriptable Objects/AugmentationListSO")]
public class AugmentationListSO : ScriptableObject
{
    public AugmentationData[] AugmentationDatas => augmentationDatas;
    [SerializeField] private AugmentationData[] augmentationDatas;

    public Augmentation GetAugmentation(string className)
    {
        Type type = Type.GetType(className + ", Assembly-CSharp");
        if (type != null && typeof(Augmentation).IsAssignableFrom(type))
        {
            return Activator.CreateInstance(type) as Augmentation;
        }
        else
            return null;
    }
}
