using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public enum CameraStateEnum
{
    NONE = 0,
    FOLLOW_CAM,
}

public interface ICameraState
{
    void Enter(CameraManager manager);
    void Update(CameraManager manager);
    void Exit(CameraManager manager);
}

public class FollowCam : ICameraState
{
    private List<Transform> targets = new List<Transform>();
    private CinemachineCamera follow;
    private Transform followTarget;

    public void Enter(CameraManager manager)
    {
        manager.SetCameraPriority(CameraStateEnum.FOLLOW_CAM, 10);
        follow ??= manager.GetCamera(CameraStateEnum.FOLLOW_CAM);
        followTarget ??= new GameObject("Follow Cam Target").transform;
        // CameraTarget 구조체를 생성하여 Transform을 할당
        CameraTarget camTarget = new CameraTarget
        {
            TrackingTarget = followTarget,
            LookAtTarget = null,
            CustomLookAtTarget = false
        };
        follow.Target = camTarget;
    }
    public void Update(CameraManager manager)
    {
        if (followTarget != null && targets != null && targets.Count > 0)
        {
            followTarget.position = GetCentralPosition();
        }
    }

    public void Exit(CameraManager manager)
    {
    }


    public Vector2 GetCentralPosition() => targets == null || targets.Count == 0 ? Vector2.zero : new Vector2(targets.Average(t => t.position.x), targets.Average(t => t.position.y));

    public void RegisterTarget(Transform t)
    {
        targets.Add(t);
    }

    public void UnRegisterTarget(Transform t)
    {
        if (!targets.Remove(t))
            Debug.LogWarning($"Attempted to unregister target but it was not found: {t?.name}");
    }

    public void ResetTargets() => targets.Clear();
}

[Serializable]
public struct CameraConfig
{
    public CameraConfig(CameraStateEnum state, CinemachineCamera vCam, int priority = 10)
    {
        this.state = state;
        this.vCam = vCam;
        this.priority = priority;
    }

    public int priority;
    public CameraStateEnum state;
    public CinemachineCamera vCam;
}
public enum CameraShakeMode
{
    CONSTANT = 0,
    DECREMENT,
    INCREMENT,
}

[DefaultExecutionOrder(-100)]
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance => instance;
    private static CameraManager instance;

    [SerializeField] private List<CameraConfig> cameraConfigs = new List<CameraConfig>();
    [SerializeField] private CameraStateEnum defaultState = CameraStateEnum.FOLLOW_CAM;
    private Dictionary<CameraStateEnum, ICameraState> states = new Dictionary<CameraStateEnum, ICameraState>();
    private Dictionary<CameraStateEnum, CinemachineCamera> cameras = new Dictionary<CameraStateEnum, CinemachineCamera>();
    private Coroutine cameraShakeCor;


    public ICameraState CurrentState { get; private set; }
    public CameraStateEnum CurrentStateEnum { get; private set; }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        InitializeStates();
        InitializeCameras();
    }

    void Start()
    {
        if (defaultState != CameraStateEnum.NONE && states.ContainsKey(defaultState))
            ChangeState(defaultState);
    }

    private void InitializeCameras()
    {
        foreach (var config in cameraConfigs)
        {
            cameras[config.state] = config.vCam;
            config.vCam.Priority = 0; // 모든 카메라를 비활성화
        }
    }

    private void InitializeStates()
    {
        RegisterState(CameraStateEnum.FOLLOW_CAM, new FollowCam());
    }

    public void RegisterState(CameraStateEnum stateEnum, ICameraState state)
    {
        states[stateEnum] = state;
    }

    public void ChangeState(CameraStateEnum stateEnum)
    {
        if (!states.ContainsKey(stateEnum))
        {
            Debug.LogWarning($"Camera state '{stateEnum}' not found!");
            return;
        }

        // 현재 상태 종료
        CurrentState?.Exit(this);

        // 모든 카메라 비활성화
        foreach (var camera in cameras.Values)
        {
            camera.Priority = 0;
        }

        // 새 상태 진입
        CurrentState = states[stateEnum];
        CurrentStateEnum = stateEnum;
        CurrentState.Enter(this);
    }

    public CinemachineCamera GetCamera(CameraStateEnum stateEnum)
    {
        cameras.TryGetValue(stateEnum, out var camera);
        return camera;
    }

    public ICameraState GetCameraState(CameraStateEnum stateEnum)
    {
        if (states.TryGetValue(stateEnum, out var state))
            return state;

        Debug.LogWarning($"Camera state '{stateEnum}' not found in CameraManager.");
        return null;
    }

    public void SetCameraPriority(CameraStateEnum stateEnum, int priority)
    {
        if (cameras.TryGetValue(stateEnum, out var camera))
        {
            camera.Priority = priority;
        }
    }

    private void Update()
    {
        CurrentState?.Update(this);
    }



    public void CameraShake(float amplitude, float frequency, float duration, CameraShakeMode mode)
    {
        if (cameraShakeCor != null)
            StopCoroutine(cameraShakeCor);
        cameraShakeCor = StartCoroutine(LinearCameraShakeCor(amplitude, frequency, duration, mode));
    }

    private IEnumerator LinearCameraShakeCor(float amplitude, float frequency, float duration, CameraShakeMode mode)
    {
        var cam = GetCamera(CurrentStateEnum);
        if (cam == null)
        {
            Debug.LogWarning("No CinemachineCamera found.");
            yield break;
        }

        // Stage.Noise에서 CinemachineComponentBase 가져오기
        var component = cam.GetCinemachineComponent(CinemachineCore.Stage.Noise);

        Debug.Log(component.name);
        // CinemachineBasicMultiChannelPerlin으로 캐스팅
        if (component is CinemachineBasicMultiChannelPerlin noise)
        {
            noise.AmplitudeGain = amplitude;
            noise.FrequencyGain = frequency;
            switch (mode)
            {
                case CameraShakeMode.CONSTANT:
                    yield return new WaitForSeconds(duration);
                    break;
                case CameraShakeMode.DECREMENT:
                    for (float elapsedTime = 0; elapsedTime < duration; elapsedTime += Time.deltaTime)
                    {
                        noise.AmplitudeGain = Mathf.Lerp(amplitude, 0, elapsedTime / duration);
                        yield return null;
                    }
                    break;
                case CameraShakeMode.INCREMENT:
                    for (float elapsedTime = 0; elapsedTime < duration; elapsedTime += Time.deltaTime)
                    {
                        noise.AmplitudeGain = Mathf.Lerp(0, amplitude, elapsedTime / duration);
                        yield return null;
                    }
                    break;
                default:
                    break;
            }
            noise.AmplitudeGain = 0;
            noise.FrequencyGain = 0;
            cameraShakeCor = null;
        }
        else
        {
            Debug.LogWarning("There's no CinemachineBasicMultiChannelPerlin component.");
        }
    }
}

