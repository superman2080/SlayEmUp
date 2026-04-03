using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatUI : EventListener
{
    [SerializeField] private Player player;
    [SerializeField] private Slider levelBar;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private Image skillImage;
    [SerializeField] private Image cooldownImage;

    public void OnStatChanged()
    {
        levelBar.value = player.Exp / (float)player.RequireExp(player.Level);
        levelText.text = $"{player.Exp} / {(float)player.RequireExp(player.Level)}";

        if (player.ActiveSkill.CanActivateSkill())
            cooldownImage.fillAmount = 0;
        else
            cooldownImage.fillAmount = 1f - player.ActiveSkill.CooldownProgress;
    }

    private void Start()
    {
        ResetFields();
        skillImage.sprite = player?.ActiveSkill.SkillData.icon;
        cooldownImage.fillAmount = 0;
    }

    private void Reset()
    {
        ResetFields();
    }

    private void ResetFields()
    {
        player ??= FindAnyObjectByType<Player>();
        levelBar = gameObject.GetComponentInChildren<Slider>();
        levelText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        skillImage = transform.Find("SkillImage").GetComponent<Image>();
        cooldownImage = skillImage.transform.Find("CooldownImage").GetComponent<Image>();
    }
}
