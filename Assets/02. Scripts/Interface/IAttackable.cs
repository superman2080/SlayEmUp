using UnityEngine;

public interface IAttackable
{
    AttackStat attackStat { get; }

    LayerMask AttackLayer { get; }

    void Attack(IDamagable target, float amount);
}
