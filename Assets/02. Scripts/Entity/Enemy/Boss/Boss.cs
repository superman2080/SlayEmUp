using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossState
{
    public virtual float PatternCooldown { get; } = 1f;
    public float PatternCooldownProgress { get; private set; }
    private int cooldownID = -1;
    public bool CanUsePattern { get => CoroutineRunner.Instance.IsCoroutineRunning(cooldownID) == false; }



    public virtual void OnStart(Boss boss) { }

    public virtual void OnUpdate(Boss boss) { }

    public virtual void OnExit(Boss boss) {
        if(PatternCooldown > 0)
        {
            cooldownID = CoroutineRunner.Start(CooldownCoroutine(PatternCooldown));
        }
    }
    private IEnumerator CooldownCoroutine(float cooldownTime)
    {
        PatternCooldownProgress = 0;
        for (float elapsedTime = 0; elapsedTime < cooldownTime; elapsedTime += Time.deltaTime)
        {
            PatternCooldownProgress = Mathf.Clamp01(elapsedTime / cooldownTime);
            yield return null;
        }
        PatternCooldownProgress = 1;
    }

}

public class BossStateMachine
{
    public BossState CurState => curState;
    private BossState curState;

    private Dictionary<Enum, BossState> states;
    private Boss owner;

    public BossStateMachine(Boss owner, BossState entryState)
    {
        this.owner = owner;
        curState = entryState;
        curState.OnStart(owner);
        states = new Dictionary<Enum, BossState>();

    }

    public void RegisterState(Enum type, BossState state)
    {
        states[type] = state;
    }

    public void UnRegisterState(Enum type)
    {
        states[type] = null;
    }

    public BossState GetState(Enum type) => states[type];

    public void ChangeState(Enum type) {
        curState?.OnExit(owner);
        curState = states[type];
        curState?.OnStart(owner);
    }

    public void Update()
    {
        curState?.OnUpdate(owner);
    }
}


public abstract class Boss : Enemy, IAttackable
{
    public BossStateMachine StateMachine { get; private set; }
    protected abstract BossState EntryState { get; }

    public virtual void Awake()
    {
        enemyAnimator = gameObject.GetComponent<EnemyAnimator>();
        StateMachine = new BossStateMachine(this, EntryState);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (CameraManager.Instance != null)
            (CameraManager.Instance.GetCameraState(CameraStateEnum.FOLLOW_CAM) as FollowCam)?.RegisterTarget(transform);

    }

    protected virtual void OnDisable()
    {
        if (CameraManager.Instance != null)
            (CameraManager.Instance.GetCameraState(CameraStateEnum.FOLLOW_CAM) as FollowCam)?.UnRegisterTarget(transform);
    }

    protected override void Start()
    {
#if UNITY_EDITOR
        player ??= FindAnyObjectByType<Player>();
#endif
        base.Start();
    }

    protected override void EntityFixedUpdate()
    {
        StateMachine?.Update();
        enemyAnimator.FlipX(player.transform.position.x - transform.position.x > 0);
    }

}
