using UnityEngine;

public class ProjectilePool : Pool<Projectile>
{
    public override GameObject Prefab => prefab;
    [SerializeField] private GameObject prefab;

    public Projectile Get(IAttackable owner, ProjectileData data, EventDelegate OnTargetHit = null)
    {
        var projectile = base.Get();
        projectile.owner = owner;
        projectile.ProjectileData = data;

        projectile.ResetProjectileEvent();
        projectile.OnTargetHit += OnTargetHit;
        return projectile;
    }
}
