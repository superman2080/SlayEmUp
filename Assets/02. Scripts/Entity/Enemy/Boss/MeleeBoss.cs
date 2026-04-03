using MeleeBossState;
using System;
using System.Collections;
using UnityEngine;

namespace MeleeBossState
{
    public enum MeleeBossStateType
    {
        DEFAULT_STATE,
        DEFAULT_ATTACK,
        CHARGE,
        GROUND_HIT,
    }

    [Serializable]
    public class DefaultStateConfig
    {
        public float chargeDist = 10f;
    }

    [Serializable]
    public class ChargeConfig
    {
        public float chargeTime = 1.5f;
        public float dashDist = 10f;
        public float dashTime = 1f;
        public float castTime = 3f;
    }

    [Serializable]
    public class GroundHitConfig {
        public float castTime = 1f;
        public float attckDist = 4f;
        public float castRadius = 3f;
        public float attackDamage = 30f;
    }

    public class DefaultState: BossState
    {
        public override float PatternCooldown => 0;
        private DefaultStateConfig config;
        private MeleeBoss meleeBoss;

        public DefaultState(DefaultStateConfig config)
        {
            this.config = config;
        }

        public override void OnStart(Boss boss)
        {
            boss.enemyAnimator.SetAttack(false);
            meleeBoss = boss as MeleeBoss;
        }

        public override void OnUpdate(Boss boss)
        {
            if (boss.player == null)
                return;

            boss.enemyAnimator.SetMoving(true);

            Vector2 dir = (boss.player.transform.position - boss.transform.position);
            float dist = dir.magnitude;

            boss.rb2d.MovePosition((Vector2)boss.transform.position + dir.normalized * Time.fixedDeltaTime * boss.defaultStat.Get(EntityStatType.MOVE_SPEED));

            if(dist >= config.chargeDist && boss.StateMachine.GetState(MeleeBossStateType.CHARGE).CanUsePattern)
                boss.StateMachine.ChangeState(MeleeBossStateType.CHARGE);

            else if (dist <= meleeBoss.groundHitConfig.attckDist && boss.StateMachine.GetState(MeleeBossStateType.GROUND_HIT).CanUsePattern)
                boss.StateMachine.ChangeState(MeleeBossStateType.GROUND_HIT);

            else if (dist <= meleeBoss.attackStat.Get(AttackStatType.ATTACK_DISTANCE))
                boss.StateMachine.ChangeState(MeleeBossStateType.DEFAULT_ATTACK);
        }

        public override void OnExit(Boss boss)
        {
            base.OnExit(boss);

            boss.enemyAnimator.SetMoving(false);
        }
    }

    public class DefaultAttack: BossState
    {
        public override float PatternCooldown => 0;

        public override void OnStart(Boss boss)
        {
            base.OnStart(boss);
            boss.enemyAnimator.SetAttack(true);
            boss.rb2d.linearVelocity = Vector2.zero;
            boss.StartCoroutine(BossDefaultAttackCor(boss));
        }

        public IEnumerator BossDefaultAttackCor(Boss boss)
        {
            WaitForSeconds waitTime = new WaitForSeconds(1f / boss.attackStat.Get(AttackStatType.ATTACK_SPEED) / 2f);

            yield return waitTime;
            boss.Attack(null, boss.attackStat.Get(AttackStatType.ATTACK_DAMAGE));
            yield return waitTime;

            boss.StateMachine.ChangeState(MeleeBossStateType.DEFAULT_STATE);
        }

        public override void OnExit(Boss boss)
        {
            base.OnExit(boss);
            boss.enemyAnimator.SetAttack(false);
        }
    }

    public class Charge : BossState
    {
        public override float PatternCooldown => 5f;
        private ChargeConfig config;

        public Charge(ChargeConfig config)
        {
            this.config = config;
        }

        public override void OnStart(Boss boss)
        {
            boss.rb2d.linearVelocity = Vector2.zero;
            boss.enemyAnimator.SetCast(true);
            boss.StartCoroutine(ChargeCor(boss));
        }

        public override void OnExit(Boss boss)
        {
            base.OnExit(boss);
            boss.enemyAnimator.SetMoving(false);
        }

        public IEnumerator ChargeCor(Boss boss)
        {
            Vector2 dir = (boss.player.transform.position - boss.transform.position).normalized;
            yield return new WaitForSeconds(config.chargeTime);

            Vector2 origin = boss.transform.position;

            boss.enemyAnimator.SetCast(false);
            boss.enemyAnimator.SetMoving(true);

            for (float elapsedTime = 0; elapsedTime < config.dashTime; elapsedTime += Time.fixedDeltaTime)
            {
                boss.rb2d.position = Vector2.Lerp(origin, origin + (dir * config.dashDist), elapsedTime / config.dashTime);
                yield return new WaitForFixedUpdate();
            }

            boss.StateMachine.ChangeState(MeleeBossStateType.DEFAULT_STATE);
        }
    }

    public class GroundHit: BossState
    {
        public override float PatternCooldown => 10f;
        private GroundHitConfig config;
        private Vector2 originPos;

        public GroundHit(GroundHitConfig config)
        {
            this.config = config;
        }

        public override void OnStart(Boss boss)
        {
            boss.rb2d.linearVelocity = Vector2.zero;
            boss.enemyAnimator.SetCast(true);
            originPos = boss.transform.position;

            boss.StartCoroutine(GroundHitCor(boss));    
        }

        public override void OnUpdate(Boss boss)
        {
            boss.transform.position = originPos;
        }

        public IEnumerator GroundHitCor(Boss boss)
        {
            yield return new WaitForSeconds(config.castTime);
            Vector2 origin = boss.transform.position;
            (Pool<ParticleWrapper>.Instance as ParticlePool).Get(ParticleType.MELEE_BOSS_GROUND_HIT, origin).Size = config.attckDist;
            var targets = Physics2D.OverlapCircleAll(origin, config.castRadius, 1 << boss.AttackLayer);

            if(targets.Length > 0)
            {
                foreach (var target in targets)
                {
                    target.GetComponent<IDamagable>().TakeDamage(boss, config.attackDamage);
                    target.GetComponent<Entity>().AddEffect(new Knockback(4, 1, boss, origin));
                }
            }

            boss.StateMachine.ChangeState(MeleeBossStateType.DEFAULT_STATE);
        }

        public override void OnExit(Boss boss)
        {
            base.OnExit(boss);
            boss.enemyAnimator.SetCast(false);
        }
    }
}

public class MeleeBoss : Boss
{
    public DefaultStateConfig defaultConfig;
    public ChargeConfig chargeConfig;
    public GroundHitConfig groundHitConfig;

    protected override BossState EntryState => new DefaultState(defaultConfig);

    protected override void Start()
    {
        base.Start();

        StateMachine.RegisterState(MeleeBossStateType.DEFAULT_STATE, new DefaultState(defaultConfig));
        StateMachine.RegisterState(MeleeBossStateType.DEFAULT_ATTACK, new DefaultAttack());
        StateMachine.RegisterState(MeleeBossStateType.CHARGE, new Charge(chargeConfig));
        StateMachine.RegisterState(MeleeBossStateType.GROUND_HIT, new GroundHit(groundHitConfig));
    }

    public override void Attack(IDamagable target, float amount)
    {
        var targets = Physics2D.OverlapCircleAll(transform.position, attackStat.Get(AttackStatType.ATTACK_DISTANCE), 1 << AttackLayer);
        if(targets.Length > 0)
        {
            foreach (var item in targets)
            {
                if(item.TryGetComponent(out IDamagable t))
                {
                    t.TakeDamage(this, amount);
                }
            }
        }
    }
}
