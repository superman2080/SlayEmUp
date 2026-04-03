using UnityEngine;

[CreateAssetMenu(fileName = "StatData", menuName = "DataContainer/StatData")]
public class StatData : ScriptableObject
{
    public float HP => hp;
    [SerializeField] private float hp;

    public float MoveSpeed => moveSpeed;
    [SerializeField] private float moveSpeed;

    public float Defense => defense;
    [SerializeField] private float defense;

    public float AttackDamage => attackDamage;
    [SerializeField] float attackDamage;

    public float AttackDist => attackDist;
    [SerializeField] float attackDist;

    public float AttackSpeed => attackSpeed;
    [SerializeField] float attackSpeed;

    public StatData(float hp, float moveSpeed, float defense, float attackDamage, float attackDist, float attackSpeed)
    {
        this.hp = hp;
        this.moveSpeed = moveSpeed;
        this.defense = defense;
        this.attackDamage = attackDamage;
        this.attackDist = attackDist;
        this.attackSpeed = attackSpeed;
    }

    public static StatData operator *(StatData data, float value)
    {
        return new StatData(
            data.hp * value,
            data.moveSpeed * value,
            data.defense * value,
            data.attackDamage * value,
            data.attackDist * value,
            data.AttackSpeed * value);
    }
}
