using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RogueActiveSkill : ActiveSkillBase
{
    protected override string SkillDataPath => "SkillData/RogueSkillData";

    protected override SkillType SkillType => SkillType.CASTING;

    private IDamagable[] targets;

    private readonly int maxAttackCount = 7;
    private float attackTime;
    private int count;
    private float elapsedTime;

    protected override void Start(Player player)
    {
        base.Start(player);
        targets = player.GetEnemiesWithinRange(player.transform.position, SkillData.castRange);


        if(targets == null || targets.Length == 0 )
        {
            Finish(player);
            return;
        }

        attackTime = SkillData.castTime / maxAttackCount;
        count = 0;
        player.AddEffect(new Invincible(1, SkillData.castTime, player));
        player.canControl = false;
    }

    protected override void Execute(Player player)
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime >= attackTime)
        {
            targets = targets?.Where(t => t != null && (t as MonoBehaviour) != null && t.HP > 0).ToArray();

            if (targets == null || targets.Length == 0 )
            {
                Finish(player);
                return;
            }

            var target = targets[count % targets.Length];
            if(target != null)
            {
                target.TakeDamage(player, player.attackStat.Get(AttackStatType.ATTACK_DAMAGE));
                player.transform.position = (target as MonoBehaviour).transform.position;
            }
            count++;
            elapsedTime = 0;
        }
    }

    protected override void Finish(Player player)
    {
        base.Finish(player);
        player.canControl = true;
    }

}
