using UnityEngine;

public class Mage : Player
{
    public override ActiveSkillBase ActiveSkill
    {
        get
        {
            if (activeSkill == null)
                activeSkill = new MageActiveSkill();
            return activeSkill;
        }
    }
    private MageActiveSkill activeSkill;

    public ProjectileData projectileData;

    public override void Attack(IDamagable target, float amount)
    {
        projectileData.dir = ((target as MonoBehaviour).transform.position - transform.position).normalized;
        var projectile = (Pool<Projectile>.Instance as ProjectilePool).Get(this, projectileData, (caster, data) =>
        {
            target.TakeDamage(this, amount);
        });
        projectile.transform.position = transform.position;
    }
}
