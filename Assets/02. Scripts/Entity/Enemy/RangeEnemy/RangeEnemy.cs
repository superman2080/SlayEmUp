using UnityEngine;

public class RangeEnemy : Enemy
{
    public ProjectileData projectileData;

    public override void Attack(IDamagable target, float amount)
    {
        // 시야 밖일 때
        if (Util.IsVisibleFromCamera(transform, Camera.main) == false)
            return;

        projectileData.dir = (player.transform.position - transform.position).normalized;

        var projectile = (Pool<Projectile>.Instance as ProjectilePool).Get(this, projectileData, (sender, data) => {
            var c = sender as IAttackable;
            target.TakeDamage(c, amount);
        });
        projectile.transform.position = transform.position;
    }

    private void OnDisable()
    {
        RangeEnemyPool.Instance.Return(this);
    }
}
