using System.Collections;
using UnityEngine;

public abstract class Enemy : Entity, IAttackable
{
    public EventChannelSO onDiedEventChannel;

    public AttackStat attackStat { get; protected set; } = new AttackStat();

    public LayerMask AttackLayer => LayerMask.NameToLayer("Player");

    [HideInInspector] public Player player;
    [SerializeField] private float exp;

    [HideInInspector] public EnemyAnimator enemyAnimator;

    private Coroutine attackCor;
    private float lastAttackTime;


    protected override void Start()
    {
        base.Start();
        // 제대로 생성되지 않은 경우
        if(statData == null || player == null)
        {
            Debug.LogError($"Error: Data or player does not exist!\n(data is null: {statData is null} | player is null: {player is null})");
            Destroy(gameObject);
        }

        enemyAnimator = gameObject.GetComponent<EnemyAnimator>();
    }

    protected override void EntityFixedUpdate()
    {
        Vector2 origin = transform.position;
        float dist = (origin - (Vector2)player.transform.position).magnitude;
        if(dist < attackStat.Get(AttackStatType.ATTACK_DISTANCE))
        {
            enemyAnimator.SetAttack(true);
            enemyAnimator.SetMoving(false);
            rb2d.linearVelocity = Vector2.zero;
            if (attackCor == null)
                attackCor = StartCoroutine(AttackCoroutine());
            //공격 로직
        }
        else
        {
            //이동 로직
            enemyAnimator.SetAttack(false);
            enemyAnimator.SetMoving(true);
            Move();
            if(attackCor != null)
            {
                StopCoroutine(attackCor);
                // 코루틴이 끝난다고 자동으로 null이 되지 않기 때문에,
                // 코루틴을 수동으로 null로 만들어줌.
                attackCor = null;
            }
        }

        enemyAnimator.FlipX(player.transform.position.x - transform.position.x > 0);
    }

    protected virtual void Move()
    {
        Vector2 origin = transform.position;
        Vector2 dir = ((Vector2)player.transform.position - origin).normalized;
        rb2d.MovePosition(origin + (dir * defaultStat.Get(EntityStatType.MOVE_SPEED) * Time.fixedDeltaTime));
    }

    public abstract void Attack(IDamagable target, float amount);

    // 스탯 초기화
    public override void SetStat(StatData data)
    {
        base.SetStat(data);

        attackStat.SetDefault(AttackStatType.ATTACK_DAMAGE, data.AttackDamage);
        attackStat.SetDefault(AttackStatType.ATTACK_DISTANCE, data.AttackDist);
        attackStat.SetDefault(AttackStatType.ATTACK_SPEED, data.AttackSpeed);
    }

    private IEnumerator AttackCoroutine()
    {
        while (true)
        {
            if(canControl == false)
            {
                attackCor = null;
                yield break;
            }

            yield return new WaitForSeconds(1f / attackStat.Get(AttackStatType.ATTACK_SPEED));
            lastAttackTime = Time.time;
            Attack(player, attackStat.Get(AttackStatType.ATTACK_DAMAGE));
        }
    }

    public override void OnDied(IAttackable caster)
    {
        (Pool<ExperiencePoint>.Instance as ExpPool).Get(transform.position, exp);
        enemyAnimator.PlayDie();
        player.killCount++;
        onDiedEventChannel.RaiseEvent();
        // 경험치
    }

    public override void TakeDamage(IAttackable caster, float amount)
    {
        player.InvokeEvent(AugmentationType.ON_ENEMY_TAKE_DAMAGE, this, player, amount);
        base.TakeDamage(caster, amount);
    }
}
