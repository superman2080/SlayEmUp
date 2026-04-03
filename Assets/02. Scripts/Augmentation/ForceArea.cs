using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceArea : Augmentation
{
    public float coolTime = 10f;
    public float remainTime = 2.5f;
    public float damage = 50f;
    public float incrementPerLevel = 1.5f;
    public float radius = 2.5f;

    private float tick = 0.25f;
    private float executeTime = 0f;

    public override void AugmentationEffect(Entity sender, params object[] data)
    {
        if(Time.time > executeTime)
        {
            //시전 할 조건
            var player = sender as Player;
            var nearestEnemy = player.GetNearestTarget() as MonoBehaviour;

            Vector2 pos = nearestEnemy != null ? nearestEnemy.transform.position : (Vector2)player.transform.position + Random.insideUnitCircle * player.attackStat.Get(AttackStatType.ATTACK_DISTANCE);

            //
            CoroutineRunner.Start(CreateForceAreaCoroutine(pos, player));
            //
            executeTime = Time.time + coolTime;
        }
    }

    private IEnumerator CreateForceAreaCoroutine(Vector2 pos, Player player)
    {
        var particle = (Pool<ParticleWrapper>.Instance as ParticlePool).Get(ParticleType.FORCE_AREA, pos);
        particle.Size = radius;
        float dps = (damage + (damage * Level * incrementPerLevel)) * 1f / remainTime;
        for (float elapsedTime = 0; elapsedTime < remainTime; elapsedTime += tick) 
        {
            var targets = Physics2D.OverlapCircleAll(pos, radius, 1 << player.AttackLayer);
            foreach (var target in targets)
            {
                if (target.TryGetComponent(out Entity entity))
                    entity.TakeDamage(player, dps * tick);
            }
            yield return new WaitForSeconds(tick);
        }

        particle.Stop();
    }

    protected override AugmentationType GetAugmentationType()
    {
        return AugmentationType.ON_UPDATE;
    }
}
