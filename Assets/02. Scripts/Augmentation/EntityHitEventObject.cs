using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityHitEventObject : MonoBehaviour
{
    public Action<Entity> OnHitEvent;
    private List<Entity> entities = new List<Entity>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Entity entity) && entities.Contains(entity) == false)
        {
            entities.Add(entity);
            OnHitEvent?.Invoke(entity);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Entity entity)&& entities.Contains(entity))
        {
            entities.Remove(entity);
        }
    }
}
