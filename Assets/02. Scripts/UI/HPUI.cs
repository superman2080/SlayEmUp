using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(-100)]
public class HPUI : EventListener
{
    [SerializeField] private Player player;
    [SerializeField] private Slider hpBar;
    [SerializeField] private TextMeshProUGUI hpText;

    public void OnStatChanged()
    {
        hpBar.value = player.HP / player.defaultStat.Get(EntityStatType.MAX_HP);
        hpText.text = $"{Mathf.RoundToInt(player.HP)} / {Mathf.RoundToInt(player.defaultStat.Get(EntityStatType.MAX_HP))}";
    }

    private void Start()
    {
        player ??= FindAnyObjectByType<Player>();
        OnStatChanged();
    }

    private void Reset()
    {
        player ??= FindAnyObjectByType<Player>();
    }
}
