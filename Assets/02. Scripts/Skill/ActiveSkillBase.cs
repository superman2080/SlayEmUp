using System.Collections;
using UnityEngine;

public enum SkillType
{
    ONE_SHOT,
    CASTING,
    CHANNELING,
}

public abstract class ActiveSkillBase
{
    public SkillData SkillData { 
        get 
        { 
            if(skillData == null)
            {
                skillData = Resources.Load<SkillData>(SkillDataPath);
                if(skillData == null)
                {
                    Debug.LogError($"SkillData not found at path: {SkillDataPath}");
                    skillData = ScriptableObject.CreateInstance<SkillData>();
                }
            }
            return skillData;
        } 
    }
    private SkillData skillData;
    protected abstract string SkillDataPath { get; }

    protected abstract SkillType SkillType { get; }

    // 0 ~ 1
    public float CooldownProgress { get; private set; } = 0f;
    private int skillCorId;

    public virtual void Activate(Player player)
    {
        switch (SkillType)
        {
            case SkillType.ONE_SHOT:
                Execute(player);
                CoroutineRunner.Start(CooldownCoroutine(SkillData.cooldownTime));
                break;
            case SkillType.CASTING:
                CoroutineRunner.Start(Casting(player, SkillData.castTime));
                break;
            case SkillType.CHANNELING:
                CoroutineRunner.Start(Channeling(player, SkillData.castTime));
                break;
            default:
                break;
        }
    }

    protected virtual void Start(Player player)
    {
        if(SkillType == SkillType.CHANNELING)
            player.isChanneling = true;
    }

    protected virtual void Execute(Player player)
    {
    }

    protected virtual void Finish(Player player)
    {
        if (SkillType == SkillType.CHANNELING)
            player.isChanneling = false;

        skillCorId = CoroutineRunner.Start(CooldownCoroutine(SkillData.cooldownTime));
    }

    private IEnumerator Casting(Player player, float castingTime)
    {
        Start(player);
        for (float elapsedTime = 0; elapsedTime < castingTime; elapsedTime += Time.deltaTime)
        {
            Execute(player);
            yield return null;  
        }
        Finish(player);
    }

    private IEnumerator Channeling(Player player, float channelingTime)
    {
        Start(player);
        for (float elapsedTime = 0; elapsedTime < channelingTime; elapsedTime += Time.deltaTime)
        {
            if(player.HasEffect(StatusEffectType.HARD_CC))    // CC¸ÔÀ¸¸é
            {
                Finish(player);
                yield break;
            }
            Execute(player);
            yield return null;
        }
        Finish(player);
    }

    private IEnumerator CooldownCoroutine(float cooldownTime)
    {
        CooldownProgress = 0;
        for (float elapsedTime = 0; elapsedTime < cooldownTime; elapsedTime += Time.deltaTime)
        {
            CooldownProgress = Mathf.Clamp01(elapsedTime / cooldownTime);
            yield return null;
        }
        CooldownProgress = 1;
    }

    public bool CanActivateSkill() => CoroutineRunner.Instance.IsCoroutineRunning(skillCorId) == false;
}
