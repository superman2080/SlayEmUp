using UnityEngine;
using PixelBattleText;
using System.Collections.Generic;
using System.Linq;
using Unity.Android.Gradle.Manifest;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour, IDamagable
{
    public Rigidbody2D rb2d { get; private set; }
    public Collider2D col { get; private set; }
    public float HP => hp;
    private float hp;
    public DefaultStat defaultStat { get; private set; } = new DefaultStat();

    #region Status Effect
    [HideInInspector] public bool canControl = true;
    public List<StatusEffect> statusEffects = new List<StatusEffect>();

    #endregion
    [HideInInspector] public bool isChanneling = false;

    public StatData statData;


    protected virtual void OnEnable()
    {
        hp = defaultStat.Get(EntityStatType.MAX_HP);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        if (statData != null)
        {
            SetStat(statData);
        }

        rb2d = gameObject.GetComponent<Rigidbody2D>();
        col = gameObject.GetComponent<Collider2D>();
    }

    protected void Update()
    {
        if (canControl && isChanneling == false)
            EntityUpdate();
    }

    protected void FixedUpdate()
    {
        if (canControl && isChanneling == false)
            EntityFixedUpdate();
    }

    protected virtual void EntityUpdate() { }
    protected virtual void EntityFixedUpdate() { }



    protected virtual void LateUpdate()
    {
        UpdateEffect();
        defaultStat.Update();
    }

    public virtual void TakeDamage(IAttackable caster, float amount)
    {
        if (HasEffect<Invincible>())
            return;

        float defense = defaultStat.Get(EntityStatType.DEFENSE);
        float val = defense > 0 ? amount * (1f / (1 + defense * 0.01f)) : amount * (1f / (1 - defense * 0.01f));
        hp = Mathf.Clamp(hp - val, 0, defaultStat.Get(EntityStatType.MAX_HP));
        PixelBattleTextController.DisplayText($"{Mathf.RoundToInt(val)}", PixelBattleTextController.singleton.GetTextAnimation(TextType.textAnim_damage), Camera.main.WorldToViewportPoint(transform.position));
        if (hp <= 0)
        {
            // 1. 적이 죽었을 때, 재생성하는게 아니라, 재활용할 것 이기 때문에
            // 2. 플레이어가 죽었을 때, 에러를 발생시키지 않게 하기 위해서
            OnDied(caster);
            gameObject.SetActive(false);
        }
    }

    public virtual void Heal(Entity caster, float amount)
    {
        hp = Mathf.Clamp(hp + amount, 0, defaultStat.Get(EntityStatType.MAX_HP));
    }

    public abstract void OnDied(IAttackable caster);

    public virtual void SetStat(StatData data)
    {
        defaultStat.SetDefault(EntityStatType.MAX_HP, data.HP);
        defaultStat.SetDefault(EntityStatType.MOVE_SPEED, data.MoveSpeed);
        defaultStat.SetDefault(EntityStatType.DEFENSE, data.Defense);

        hp = defaultStat.Get(EntityStatType.MAX_HP);
        Heal(null, data.HP);
    }

    protected void UpdateEffect()
    {
        if(statusEffects.Count <= 0) return;

        statusEffects = statusEffects.OrderBy(effect => effect.Type).
            ThenBy(effect => effect.Duration).ToList();

        List<StatusEffect> delEffects = new List<StatusEffect>();
        foreach (var effect in statusEffects)
        {
            if (!HasEffect<Unstoppable>()
                || (HasEffect<Unstoppable>() && effect.Type == StatusEffectType.BUFF))
                effect.OnUpdate(this);

            effect.Duration -= Time.deltaTime;
            if(effect.Duration <= 0)
            {
                effect.OnFinish(this);
                delEffects.Add(effect);
            }
        }

        statusEffects.RemoveAll(effect => delEffects.Contains(effect));
    }

    public void AddEffect(StatusEffect effect)
    {
        if (HasEffect<Unstoppable>() 
            && ((effect.Type == StatusEffectType.HARD_CC) 
            || (effect.Type == StatusEffectType.NORMAL_CC)))
            return;
        statusEffects.Add(effect);
        effect.OnStart(this);
    }

    public bool HasEffect<T>() where T : StatusEffect
    {
        return !(statusEffects.Find(effect => effect is T) == null);    
    }

    public bool HasEffect(StatusEffectType type)
    {
        return !(statusEffects.Find(effect => effect.Type == type) == null);
    }
}
