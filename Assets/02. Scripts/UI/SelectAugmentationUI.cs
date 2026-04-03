using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;

[Serializable]
public struct AugmentationSelectButtonData
{
    public Button button;
    public TextMeshProUGUI nameText;
    public Image icon;
    public TextMeshProUGUI desciptionText;
}


public class SelectAugmentationUI : EventListener
{
    [SerializeField] private Player player;

    [Header("UI")]
    public RectTransform panel;
    [SerializeField] private AugmentationSelectButtonData[] buttonData;

    [Header("Data")]
    [SerializeField] private AugmentationListSO listSO;
    [SerializeField] private int maxChoiceCount = 3;
    private List<Augmentation> augList = new List<Augmentation>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player ??= FindAnyObjectByType<Player>();
        panel.gameObject.SetActive(false);
    }

    public void OnLevelUpEvent()
    {
        Time.timeScale = 0;
        panel.gameObject.SetActive(true);
        GetRandomAugmentation();
    }

    private void GetRandomAugmentation()
    {
        var randList = Util.FisherYatesShuffle(0, listSO.AugmentationDatas.Length, maxChoiceCount);
        for (int i = 0; i < maxChoiceCount; i++)
        {
            int rand = randList[i];
            var data = listSO.AugmentationDatas[rand];

            augList.Add(listSO.GetAugmentation(data.className));
            buttonData[i].nameText.text = data.augmentationName;
            buttonData[i].icon.sprite = data.icon;
            buttonData[i].desciptionText.text = data.description;
        }
    }

    public void SelectAugmentation(int idx)
    {
        augList[idx].Select(player);
        Time.timeScale = 1;
        panel.gameObject.SetActive(false);
        augList.Clear();
    }
}
