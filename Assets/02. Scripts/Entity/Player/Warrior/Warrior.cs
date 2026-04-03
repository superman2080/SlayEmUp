using UnityEngine;

public class Warrior : Player
{
    public override ActiveSkillBase ActiveSkill { 
        get {
            if (activeSkill == null)
                activeSkill = new WarriorActiveSkill();
            return activeSkill;
        } 
    }
    private WarriorActiveSkill activeSkill;

    public override void Attack(IDamagable target, float amount)
    {
        // 임시로 파티클 생성
        var particle = (Pool<ParticleWrapper>.Instance as ParticlePool).Get(ParticleType.WARRIOR_ATTACK, transform.position);
        particle.Size = attackStat.Get(AttackStatType.ATTACK_DISTANCE);
#nullable enable
        IDamagable[]? enemies = GetEnemiesWithinAttackDistance();
#nullable disable
        foreach (var enemy in enemies)
        {
            enemy.TakeDamage(this, amount);
        }
    }
}
