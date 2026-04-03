using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private PlayerInputHandler handler;
    private Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        handler = gameObject.GetComponent<PlayerInputHandler>();
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        player = gameObject.GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        if(player.canControl && player.isChanneling == false)
            Move();   
    }

    private void Move()
    {
        rb2d.MovePosition((Vector2)transform.position + handler.MoveInput * Time.fixedDeltaTime * player.defaultStat.Get(EntityStatType.MOVE_SPEED));
    }
}
