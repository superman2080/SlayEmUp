using UnityEngine;

[DefaultExecutionOrder(-10)]
public class EnemyAnimator : MonoBehaviour
{
    // 내가 애니메이션 다뤄줄 적
    private Enemy enemy;
    private RectTransform moderTr;
    private Animator animator;
    private Vector3 flipX = new Vector3(-1, 1, 1);
    private Vector3 modelScale = Vector3.one;

    private static readonly int isMoving = Animator.StringToHash("1_Move");
    private static readonly int attack = Animator.StringToHash("2_Attack");
    private static readonly int damaged = Animator.StringToHash("3_Damage");
    private static readonly int die = Animator.StringToHash("4_Death");
    private static readonly int cast = Animator.StringToHash("7_Cast");

    private static readonly int attackSpeed = Animator.StringToHash("AttackSpeed");

    private void Awake()
    {
        enemy = gameObject.GetComponent<Enemy>();
        moderTr = gameObject.GetComponentInChildren<RectTransform>();
        modelScale = moderTr.localScale;
        animator = gameObject.GetComponentInChildren<Animator>();
    }

    public void SetMoving(bool moving)
    {
        animator.SetBool(isMoving, moving);
    }

    public void SetAttack(bool attacking)
    {
        if(enemy.attackStat != null)
        {
            animator.SetFloat(attackSpeed, enemy.attackStat.Get(AttackStatType.ATTACK_SPEED));
            animator.SetBool(attack, attacking);
        }
    }

    public void PlayDie()
    {
        animator.SetTrigger(die);
    }

    public void SetCast(bool isCasting)
    {
        animator.SetBool(cast, isCasting);
    }

    public void FlipX(bool flipping)
    {
        moderTr.localScale = flipping ? new Vector3(
            flipX.x * modelScale.x,
            modelScale.y,
            modelScale.z) : modelScale;
    }
}
