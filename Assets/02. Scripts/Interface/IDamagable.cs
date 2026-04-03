using UnityEngine;

public interface IDamagable
{
    DefaultStat defaultStat { get; }
    float HP { get; }
    void TakeDamage(IAttackable caster, float amount);
    void Heal(Entity caster, float amount);

    void OnDied(IAttackable caster);
}
