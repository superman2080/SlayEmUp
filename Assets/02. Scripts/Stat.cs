using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Stat<T> where T : Enum
{
    protected Dictionary<T, float> defaultStat = new Dictionary<T, float>();
    protected Dictionary<T, float> addValue = new Dictionary<T, float>();
    protected Dictionary<T, float> multipleValue = new Dictionary<T, float>();
    protected Dictionary<T, float> currentValue = new Dictionary<T, float>();

    public void Update()
    {
        UpdateStat();
        InitStat();
    }

    protected void InitStat()
    {
        T[] statTypes = (T[])Enum.GetValues(typeof(T));
        foreach (var type in statTypes)
        {
            addValue[type] = 0;
            multipleValue[type] = 1;
        }
    }

    protected void UpdateStat()
    {
        T[] statTypes = (T[])Enum.GetValues(typeof(T));
        foreach (var type in statTypes)
        {
            currentValue[type] = (defaultStat[type] + addValue[type]) * multipleValue[type];
        }
    }

    public void SetDefault(T type, float value)
    {
        defaultStat[type] = value;
        UpdateStat();
    }
    
    public void Add(T type, float value)
    {
        addValue[type] += value;
    }

    public void Multiply(T type, float value)
    {
        multipleValue[type] *= value;
    }

    public float Get(T type) => currentValue[type];
}

public enum EntityStatType
{
    MAX_HP,
    MOVE_SPEED,
    DEFENSE,
}

public class DefaultStat : Stat<EntityStatType>
{
    public DefaultStat(float hp, float moveSpeed, float defense)
    {
        defaultStat = new Dictionary<EntityStatType, float>()
        {
            [EntityStatType.MAX_HP] = hp,
            [EntityStatType.MOVE_SPEED] = moveSpeed,
            [EntityStatType.DEFENSE] = defense,
        };

        InitStat();
        UpdateStat();
    }

    public DefaultStat()
    {
        defaultStat = new Dictionary<EntityStatType, float>()
        {
            [EntityStatType.MAX_HP] = 100,
            [EntityStatType.MOVE_SPEED] = 2,
            [EntityStatType.DEFENSE] = 0,
        };

        InitStat();
        UpdateStat();
    }
}

public enum AttackStatType
{
    ATTACK_DAMAGE,
    ATTACK_DISTANCE,
    ATTACK_SPEED,
}

public class AttackStat: Stat<AttackStatType>
{
    public AttackStat(float damage, float dist, float speed)
    {
        defaultStat = new Dictionary<AttackStatType, float>()
        {
            [AttackStatType.ATTACK_DAMAGE] = damage,
            [AttackStatType.ATTACK_DISTANCE] = dist,
            [AttackStatType.ATTACK_SPEED] = speed
        };

        InitStat();
        UpdateStat();
    }

    public AttackStat()
    {
        defaultStat = new Dictionary<AttackStatType, float>()
        {
            [AttackStatType.ATTACK_DAMAGE] = 10,
            [AttackStatType.ATTACK_DISTANCE] = 1,
            [AttackStatType.ATTACK_SPEED] = 1
        };

        InitStat();
        UpdateStat();
    }



}

public enum PlayerStatType
{
    EXP_DISTANCE,
    SKILL_FORCE_MAG,
}

public class PlayerStat : Stat<PlayerStatType>
{
    public PlayerStat(float dist, float mag)
    {
        defaultStat = new Dictionary<PlayerStatType, float>()
        {
            [PlayerStatType.EXP_DISTANCE] = dist,
            [PlayerStatType.SKILL_FORCE_MAG] = mag,
        };

        InitStat();
        UpdateStat();
    }

    public PlayerStat()
    {
        defaultStat = new Dictionary<PlayerStatType, float>()
        {
            [PlayerStatType.EXP_DISTANCE] = 4,
            [PlayerStatType.SKILL_FORCE_MAG] = 1,
        };

        InitStat();
        UpdateStat();
    }
}
