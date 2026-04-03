using UnityEngine;

public class EnemyPool : Pool<Enemy>
{
    public override GameObject Prefab => prefab;
    [SerializeField] private GameObject prefab;
    private Player player;

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
    }

    public override Enemy Get()
    {
        var result = base.Get();
        result.player = player;
        return result;
    }
}

public class MeleeEnemyPool : EnemyPool { }
public class RangeEnemyPool : EnemyPool { }
public class MeleeBossPool : EnemyPool { }