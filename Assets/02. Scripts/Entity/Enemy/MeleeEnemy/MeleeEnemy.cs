using UnityEngine;

public class MeleeEnemy : Enemy
{
    public override void Attack(IDamagable target, float amount)
    {
        var p = Physics2D.OverlapCircle(transform.position, attackStat.Get(AttackStatType.ATTACK_DISTANCE), 1 << AttackLayer);
        if(p != null && p.TryGetComponent(out Player player))
        {
            player.TakeDamage(this, attackStat.Get(AttackStatType.ATTACK_DAMAGE));
            player.AddEffect(new Knockback(1, 0.2f, this, transform.position));
        }
    }

    private void OnDisable()
    {
        MeleeEnemyPool.Instance.Return(this);
    }
}
