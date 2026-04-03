using System;
using System.Collections;
using UnityEngine;

[Serializable]
public struct ProjectileData
{
    public Sprite sprite;
    [HideInInspector] public Vector2 dir;
    public float speed;
    public int penetrationCount;

    public ProjectileData(Sprite sprite, Vector2 dir, float speed, int penetrationCount)
    {
        this.sprite = sprite;
        this.dir = dir;
        this.speed = speed;
        this.penetrationCount = penetrationCount;
    }
}

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(SpriteRenderer))]
public class Projectile : MonoBehaviour
{
    private ProjectileData projectileData;
    public ProjectileData ProjectileData
    {
        get => projectileData;
        set
        {
            projectileData = value;
            sR.sprite = projectileData.sprite;
        }
    }


    public IAttackable owner;
    public SpriteRenderer sR;
    public event EventDelegate OnTargetHit;
    private Rigidbody2D rb2d;
    private CircleCollider2D col;
    private const float disableTime = 3f;



    private void OnEnable()
    {
        StartCoroutine(DisableProjectile());
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sR = gameObject.GetComponent<SpriteRenderer>();
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        col = gameObject.GetComponent<CircleCollider2D>();
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        col.isTrigger = true;
    }

    private void FixedUpdate()
    {
        ProjectileMovement();
    }

    protected virtual void ProjectileMovement()
    {
        transform.rotation = Quaternion.AngleAxis(Util.DirectionToAngle(projectileData.dir), Vector3.forward);
        rb2d.MovePosition((Vector2)transform.position + projectileData.dir.normalized * projectileData.speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == owner.AttackLayer)
        {
            // 충돌이벤트호출
            OnTargetHit?.Invoke(owner as Entity, collision.gameObject.GetComponents<IDamagable>());
            projectileData.penetrationCount--;
            if (projectileData.penetrationCount <= 0)
                // 풀에 반환
                ProjectilePool.Instance.Return(this);
        }
    }

    private IEnumerator DisableProjectile()
    {
        float checkTime = 1f;
        var waitTime = new WaitForSeconds(checkTime);
        yield return new WaitForSeconds(disableTime);
        while (true)
        {
            yield return waitTime;
            if (Util.IsVisibleFromCamera(transform, Camera.main) == false)
            {
                ProjectilePool.Instance.Return(this);
                yield break;
            }
        }
    }

    public void ResetProjectileEvent() => OnTargetHit = null;
}
