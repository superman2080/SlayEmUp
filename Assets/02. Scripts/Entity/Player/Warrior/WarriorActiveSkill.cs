using System.Collections.Generic;
using UnityEngine;

public class WarriorActiveSkill : ActiveSkillBase
{
    protected override string SkillDataPath => "SkillData/WarriorSkillData";

    protected override SkillType SkillType => SkillType.CHANNELING;

    protected override void Start(Player player)
    {
        base.Start(player);
        player.AddEffect(new Unstoppable(1, SkillData.castTime + 0.1f, player));
        player.AddEffect(new StatBuff<EntityStatType>(100, SkillData.castTime + 0.1f, player, EntityStatType.DEFENSE));
    }

    protected override void Finish(Player player)
    {
        base.Finish(player);
        (Pool<ParticleWrapper>.Instance as ParticlePool).Get(ParticleType.WARRIOR_SKILL, player.transform.position).Size = SkillData.castRange + 5f;
        CameraManager.Instance.CameraShake(2f, 5f, 0.5f, CameraShakeMode.DECREMENT);

        var enemies = GetEnemiesInRange(player);
        if (enemies == null)
            return;

        foreach (var enemy in enemies)
        {
            int dist = Mathf.RoundToInt(((enemy as MonoBehaviour).transform.position - player.transform.position).magnitude);
            int level = Mathf.RoundToInt(SkillData.castRange) - dist;
            (enemy as Entity)?.AddEffect(new Knockback(level, 1f, player, player.transform.position));
            enemy.TakeDamage(player, player.attackStat.Get(AttackStatType.ATTACK_DAMAGE));
        }
    }


    private IDamagable[] GetEnemiesInRange(Player player)
    {
        var enemies = Physics2D.OverlapCircleAll(player.transform.position, SkillData.castRange, 1 << player.AttackLayer);
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
}
