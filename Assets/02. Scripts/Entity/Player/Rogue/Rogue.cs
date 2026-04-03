using System.Collections;
using UnityEngine;

public class Rogue : Player
{
    private EntityHitEventObject knife;
    private float nowDistance; // orbit
    private Coroutine cor;


    public override ActiveSkillBase ActiveSkill
    {
        get
        {
            if (activeSkill == null)
                activeSkill = new RogueActiveSkill();
            return activeSkill;
        }
    }
    private RogueActiveSkill activeSkill;

    protected override void Start()
    {
        base.Start();
        knife = Instantiate(Resources.Load<EntityHitEventObject>("Prefabs/Sword Object"));
        knife.transform.localScale = Vector3.one;
        knife.OnHitEvent += OnKnifeAttacked;
    }


    protected override void EntityUpdate()
    {
        base.EntityUpdate();

        if (cor == null)
            knife.transform.position = transform.position;
    }

    private void OnKnifeAttacked(Entity target)
    {
        if (target == this)
            return;

        target.TakeDamage(this, attackStat.Get(AttackStatType.ATTACK_DAMAGE));
        target.AddEffect(new Stun(1, 0.25f, this));
    }

    public override void Attack(IDamagable target, float amount)
    {
        if (target == null)
            return;

        Vector2 dir = ((target as MonoBehaviour).transform.position - transform.position).normalized;
        cor = StartCoroutine(SetKnifePos(dir));
    }

    private IEnumerator SetKnifePos(Vector2 dir)
    {
        float speed = 1f / attackStat.Get(AttackStatType.ATTACK_SPEED);
        float dist = attackStat.Get(AttackStatType.ATTACK_DISTANCE);
        float radian = Util.DirectionToAngle(dir) * Mathf.Deg2Rad;

        knife.transform.rotation = Quaternion.AngleAxis(radian * Mathf.Rad2Deg - 90f, Vector3.forward);
        for (float elapsedTime = 0f; elapsedTime < speed; elapsedTime += Time.deltaTime)
        {
            nowDistance = Mathf.Sin(Mathf.Clamp01(elapsedTime / speed) * Mathf.PI) * dist;
            knife.transform.position = (Vector2)transform.position + new Vector2(
                Mathf.Cos(radian),
                Mathf.Sin(radian)
                ) * nowDistance;
            yield return null;
        }

        knife.transform.position = transform.position;
        cor = null;
    }
}
