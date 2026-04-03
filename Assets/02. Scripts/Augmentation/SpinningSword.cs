using System.Collections.Generic;
using UnityEngine;

public class SpinningSword : Augmentation
{
    public float damage = 20f;
    public float orbitRadius = 3f;
    public float rotationSpeed = 90f;

    private EntityHitEventObject swordObjectPrefab;
    private Player player;

    private float nowRotationAngle = 0f;
    private List<EntityHitEventObject> swordObjects = new List<EntityHitEventObject>();

    public override void OnSelected(Player target)
    {
        swordObjectPrefab ??= Resources.Load<EntityHitEventObject>("Prefabs/Sword Object");
        player ??= target;
        var sword = Object.Instantiate(swordObjectPrefab, player.transform.position, Quaternion.identity);
        sword.transform.localScale = Vector3.one * 3f;
        sword.OnHitEvent += OnHitEvent;
        swordObjects.Add(sword);
    }

    public override void AugmentationEffect(Entity sender, params object[] data)
    {
        nowRotationAngle += rotationSpeed * Time.deltaTime;
        nowRotationAngle %= 360f;
        UpdateSwordPosition(player.transform.position);
    }

    private void UpdateSwordPosition(Vector2 center)
    {
        float angleStep = 360f / swordObjects.Count;

        for (int i = 0; i < swordObjects.Count; i++)
        {
            float angle = nowRotationAngle + (angleStep * i);
            Vector2 offset = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad) * orbitRadius,
                Mathf.Sin(angle * Mathf.Deg2Rad) * orbitRadius
                );

            swordObjects[i].transform.position = center + offset;
            swordObjects[i].transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        }
    }



    protected override AugmentationType GetAugmentationType()
    {
        return AugmentationType.ON_UPDATE;
    }

    private void OnHitEvent(Entity entity)
    {
        if(entity.gameObject.layer == player.AttackLayer)
        {
            entity.TakeDamage(player, damage);
        }
    }
}
