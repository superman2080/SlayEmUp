using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Player player;
    private PlayerInputHandler handler;
    private RectTransform modelTr;
    private Animator animator;
    private Vector3 flipX = new Vector3(-1, 1, 1);

    private readonly int moveHash = Animator.StringToHash("1_Move");
    private readonly int castHash = Animator.StringToHash("7_Cast");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        handler = gameObject.GetComponent<PlayerInputHandler>();
        modelTr = gameObject.GetComponentInChildren<RectTransform>();
        animator = gameObject.GetComponentInChildren<Animator>();
        player = gameObject.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        Anim();
        FlipX();
    }

    private void FlipX()
    {
        var mousePosX = Camera.main.ScreenToWorldPoint(handler.MousePosition).x;
        if (mousePosX - transform.position.x > 0)
            modelTr.localScale = flipX;
        else
            modelTr.localScale = Vector3.one;
    }

    private void Anim()
    {
        animator.SetBool(moveHash, handler.MoveInput != Vector2.zero);
        animator.SetBool(castHash, player.isChanneling);
    }
}
