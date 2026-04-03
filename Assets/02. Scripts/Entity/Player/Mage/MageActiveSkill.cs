using UnityEngine;

public class MageActiveSkill : ActiveSkillBase
{
    protected override string SkillDataPath => "SkillData/MageSkillData";

    protected override SkillType SkillType => SkillType.ONE_SHOT;

    private ProjectileData projectileData;

    public MageActiveSkill()
    {
        projectileData = new ProjectileData(Resources.Load<Sprite>("Sprites/Fire bolt purple1"), Vector2.zero, 5f, 1);
    }

    protected override void Execute(Player player)
    {
        var target = player.GetNearestTarget(player.GetEnemiesWithinRange(player.transform.position, SkillData.castRange));
        if(target == null)
        {
            Finish(player);
            return;
        }    

        projectileData.dir = ((target as MonoBehaviour).transform.position - player.transform.position).normalized;
        var projectile = (Pool<Projectile>.Instance as ProjectilePool).Get(player, projectileData, (caster, data) =>
        {
            var targets = Physics2D.OverlapCircleAll((target as MonoBehaviour).transform.position, SkillData.castRange, 1 << player.AttackLayer);
            CameraManager.Instance.CameraShake(1.5f, 1.5f, 2f, CameraShakeMode.DECREMENT);
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].TryGetComponent(out Enemy enemy))
                {
                    enemy.TakeDamage(player, player.attackStat.Get(AttackStatType.ATTACK_DAMAGE) * 1.5f);
                    enemy.AddEffect(new Stun(1, 2f, player));
                }
            }
        });

        projectile.transform.position = player.transform.position;
        projectile.transform.localScale = Vector2.one * 2f;
    }
}
