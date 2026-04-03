using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void EventDelegate(Entity sender, params object[] data);

public abstract class Player : Entity, IAttackable
{
    public int killCount;
    protected PlayerInputHandler handler;

    #region Stat
    public PlayerStat playerStat { get; protected set; } = new PlayerStat();

    public AttackStat attackStat { get; private set; } = new AttackStat();

    public LayerMask AttackLayer => LayerMask.NameToLayer("Enemy");
    #endregion

    #region Augmentation
    public List<Augmentation> augmentationList = new List<Augmentation>();
    public event EventDelegate OnPlayerUpdated;
    public event EventDelegate OnAttacked;
    public event EventDelegate OnTakeDamaged;
    public event EventDelegate OnHealed;
    public event EventDelegate OnEnemyTakeDamaged;
    #endregion

    #region Level
    public const int baseXP = 10;
    public const float exponent = 1.1f;


    public int Level { get; private set; }
    public float Exp { get => exp; }

    public float AddExp
    {
        set
        {
            exp += value;
            if (exp >= RequireExp(Level))
            {
                exp -= RequireExp(Level);
                Level++;
                levelUpEvent.RaiseEvent();
            }
        }
    }

    private float exp;
    #endregion

    #region Event Channel

    [Space]
    [Header("Event Channels")]
    public EventChannelSO levelUpEvent;
    public EventChannelSO hpUIEvent;
    public EventChannelSO statEvent;
    public EventChannelSO onDiedEvent;

    #endregion

    #region Attack
    private Coroutine attackCor;
    private float lastAttackTime;
    public abstract ActiveSkillBase ActiveSkill { get; }

    #endregion

    protected override void OnEnable()
    {
        base.OnEnable();
        if (CameraManager.Instance != null)
            (CameraManager.Instance.GetCameraState(CameraStateEnum.FOLLOW_CAM) as FollowCam)?.RegisterTarget(transform);
    }

    protected virtual void OnDisable()
    {
        if (CameraManager.Instance != null)
            (CameraManager.Instance.GetCameraState(CameraStateEnum.FOLLOW_CAM) as FollowCam)?.UnRegisterTarget(transform);
    }

    protected override void Start()
    {
        base.Start();

        handler = gameObject.GetComponent<PlayerInputHandler>();
        hpUIEvent.RaiseEvent();
    }

    protected override void EntityUpdate()
    {
        if(Time.timeScale == 0)
            return;

        OnPlayerUpdated?.Invoke(this);
        #region Attack
        var targets = GetEnemiesWithinAttackDistance();
        if (targets != null && attackCor == null && lastAttackTime + 1f / attackStat.Get(AttackStatType.ATTACK_SPEED) <= Time.time) 
        {
            attackCor = StartCoroutine(AttackCor());
        }
        else if (targets == null && attackCor != null)
        {
            StopCoroutine(attackCor);
            attackCor = null;
        }

        if(handler.ActiveSkillEnabled && ActiveSkill.CanActivateSkill())
        {
            ActiveSkill?.Activate(this);
        }
        #endregion
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        attackStat.Update();
        playerStat.Update();

        statEvent.RaiseEvent();
    }

    private IEnumerator AttackCor()
    {
        while (true)
        {
            if(canControl == false && isChanneling)
            {
                attackCor = null;
                yield break;
            }
            var target = GetNearestTarget(GetEnemiesWithinAttackDistance());

            OnAttacked?.Invoke(this);
            Attack(target, attackStat.Get(AttackStatType.ATTACK_DAMAGE));
            lastAttackTime = Time.time;
            yield return new WaitForSeconds(1 / attackStat.Get(AttackStatType.ATTACK_SPEED));
        }
    }

    protected IDamagable[] GetEnemiesWithinAttackDistance()
    {
        var enemies = Physics2D.OverlapCircleAll(transform.position, attackStat.Get(AttackStatType.ATTACK_DISTANCE), 1 << AttackLayer);
        if (enemies.Length > 0)
        {
            var result = new List<IDamagable>();
            for (int i = 0; i < enemies.Length; i++)
            {
                result.Add(enemies[i].GetComponent<IDamagable>());
            }
            return result.ToArray();
        }
        else
            return null;
    }

    public IDamagable[] GetEnemiesWithinRange(Vector2 pos, float range)
    {
        var enemies = Physics2D.OverlapCircleAll(pos, range, 1 << AttackLayer);
        if (enemies.Length > 0)
        {
            var result = new List<IDamagable>();
            for (int i = 0; i < enemies.Length; i++)
            {
                result.Add(enemies[i].GetComponent<IDamagable>());
            }
            return result.ToArray();
        }
        else
            return null;
    }

    public IDamagable GetNearestTarget(IDamagable[] targets)
    {
        if (targets != null && targets.Length > 0)      // 감지한 적이 최소 하나 이상일 때
        {
            var result = targets[0];
            float resDist = ((result as MonoBehaviour).transform.position - transform.position).sqrMagnitude;
            foreach (var enemy in targets)
            {
                float nowDist = ((enemy as MonoBehaviour).transform.position - transform.position).sqrMagnitude;
                if (nowDist < resDist)
                {
                    result = enemy;
                    resDist = nowDist;
                }
            }
            return result;
        }
        else
        {
            return null;
        }
    }

    public IDamagable GetNearestTarget()
    {
        return GetNearestTarget(GetEnemiesWithinAttackDistance());
    }

    public abstract void Attack(IDamagable target, float amount);

    public override void TakeDamage(IAttackable caster, float amount)
    {
        OnTakeDamaged?.Invoke(caster as Entity, this);
        base.TakeDamage(caster, amount);
        hpUIEvent.RaiseEvent();
    }

    public override void Heal(Entity caster, float amount)
    {
        OnHealed?.Invoke(caster);
        base.Heal(caster, amount);
        hpUIEvent.RaiseEvent();
    }

    public void AddAugmetation(Augmentation aug)
    {
        var sameAug = augmentationList.FirstOrDefault((a) => a.GetType().Name == aug.GetType().Name);
        // 증강이 존재하는 경우
        if(sameAug != default)
        {
            augmentationList.Find((a) => a.GetType() == aug.GetType()).Level++;
            sameAug.OnSelected(this);
        }
        else    //없는 경우
        {
            aug.OnSelected(this);
            if (aug.AugType == AugmentationType.ON_SELECT)
            {
                aug.AugmentationEffect(this);
            }
            if(aug.AugType == AugmentationType.ON_UPDATE)
            {
                OnPlayerUpdated += aug.AugmentationEffect;
            }
            if(aug.AugType == AugmentationType.ON_ATTACK)
            {
                OnAttacked += aug.AugmentationEffect;
            }
            if(aug.AugType == AugmentationType.ON_TAKE_DAMAGE)
            {
                OnTakeDamaged += aug.AugmentationEffect;
            }
            if(aug.AugType == AugmentationType.ON_HEAL)
            {
                OnHealed += aug.AugmentationEffect;
            }
            if(aug.AugType == AugmentationType.ON_ENEMY_TAKE_DAMAGE)
            {
                OnEnemyTakeDamaged += aug.AugmentationEffect;
            }

            augmentationList.Add(aug);
        }
    }
    public void DeductAugmentation(Augmentation aug)
    {
        var target = augmentationList.Find((a) => a.GetType() == aug.GetType());
        if (target == null)
            return;
        else if (target != null && target.Level > 0)
            target.Level--;
        else
        {
            if (aug.AugType == AugmentationType.ON_UPDATE)
            {
                OnPlayerUpdated -= aug.AugmentationEffect;
            }
            if (aug.AugType == AugmentationType.ON_ATTACK)
            {
                OnAttacked -= aug.AugmentationEffect;
            }
            if (aug.AugType == AugmentationType.ON_TAKE_DAMAGE)
            {
                OnTakeDamaged -= aug.AugmentationEffect;
            }
            if (aug.AugType == AugmentationType.ON_HEAL)
            {
                OnHealed -= aug.AugmentationEffect;
            }
            if (aug.AugType == AugmentationType.ON_ENEMY_TAKE_DAMAGE)
            {
                OnEnemyTakeDamaged -= aug.AugmentationEffect;
            }

            augmentationList.Remove(aug);
        }
    }

    public void DeductAugmentation<T>() where T : Augmentation
    {
        var aug = augmentationList.Find((a) => a.GetType() == typeof(T));
        if (aug != null)
            DeductAugmentation(aug);
    }

    public void InvokeEvent(AugmentationType type, Entity sender, params object[] datas)
    {
        if (type == AugmentationType.ON_UPDATE)
            OnPlayerUpdated?.Invoke(sender, datas);

        if(type == AugmentationType.ON_ATTACK)
            OnAttacked?.Invoke(sender, datas);

        if(type == AugmentationType.ON_HEAL)
            OnHealed?.Invoke(sender, datas);

        if(type == AugmentationType.ON_TAKE_DAMAGE)
            OnTakeDamaged?.Invoke(sender, datas);

        if(type == AugmentationType.ON_ENEMY_TAKE_DAMAGE)
            OnEnemyTakeDamaged?.Invoke(sender, datas);
    }

    public override void OnDied(IAttackable caster)
    {
        onDiedEvent.RaiseEvent();
    }

    public override void SetStat(StatData data)
    {
        base.SetStat(data);
        attackStat.SetDefault(AttackStatType.ATTACK_DAMAGE, data.AttackDamage);
        attackStat.SetDefault(AttackStatType.ATTACK_DISTANCE, data.AttackDist);
        attackStat.SetDefault(AttackStatType.ATTACK_SPEED, data.AttackSpeed);
    }

    public int RequireExp(int level)
    {
        int result = Mathf.FloorToInt(baseXP * Mathf.Pow(level, exponent));
        return level == 0 ? baseXP : result;
    }
}
