using UnityEngine;

public class ExpPool : Pool<ExperiencePoint>
{
    public override GameObject Prefab => prefab;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Player player;

    private void Start()
    {
        if (player == null)
            player = FindAnyObjectByType<Player>();
    }

    public ExperiencePoint Get(Vector2 pos, float amount)
    {
        var exp = base.Get();
        exp.amount = amount;
        exp.transform.position = pos;
        exp.player = player;
        return exp;
    }
}
