using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ExperiencePoint : MonoBehaviour
{
    [SerializeField] [Min(0.1f)] private float forcePerFrame = 0.1f;
    [SerializeField] private float maxForce = 3f;
    private Rigidbody2D rb2d;
    [HideInInspector] public Player player;
    [HideInInspector] public float amount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 0f;
        rb2d.mass = 0.1f;
    }

    // 50

    private void FixedUpdate()
    {
        Vector2 dir = (player.transform.position - transform.position);
        float dist = dir.magnitude;
        if(dist <= player.playerStat.Get(PlayerStatType.EXP_DISTANCE))
        {
            if (rb2d.linearVelocity.magnitude <= maxForce)
                rb2d.AddForce(dir.normalized * forcePerFrame, ForceMode2D.Force);
            else
                rb2d.linearVelocity = rb2d.linearVelocity.normalized * maxForce;
        }
        else
        {
            rb2d.linearVelocity = Vector2.zero;
        }

        if(dist <= 0.5f)
        {
            player.AddExp = amount;
            ExpPool.Instance.Return(this);
            // ExpPool ¿¡ ¹ÝÈ¯
        }
    }
}
