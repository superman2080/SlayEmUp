using System.Linq;
using UnityEngine;

public class ParticleWrapper : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private ParticleType particleType;
    [SerializeField] private bool enableStart = true;
    [SerializeField] [Min(0.1f)] private float size = 1f;

    private float maxParticleSize;
    public float Size
    {
        get => size;
        set
        {
            size = value;
            SetStartSize(size);
        }
    }
    public ParticleSystem ParticleSystem
    {
        get
        {
            if (particleSystem == null)
                particleSystem = gameObject.GetComponent<ParticleSystem>();
            return particleSystem;
        }
    }

    public ParticleType ParticleType { get => particleType; set => particleType = value; }

    public bool IsPlaying => ParticleSystem != null && ParticleSystem.isPlaying;
    public bool IsAlive => ParticleSystem != null && ParticleSystem.IsAlive();



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (particleSystem == null)
            particleSystem = gameObject.GetComponent<ParticleSystem>();
        InitParticleSize();
    }

    private void OnEnable()
    {
        SetStartSize(size);

        if (enableStart == true)
            Play();
    }

    public void Play() => particleSystem?.Play();

    public void Stop() => particleSystem?.Stop();

    public void Clear() => particleSystem?.Clear();

    // Update is called once per frame
    void Update()
    {
        if(IsPlaying == false)
        {
            ParticlePool.Instance.Return(this);
            // 풀에 다시 반환
        }
    }

    private void Reset()
    {
        InitParticleSize();
        particleSystem ??= gameObject.GetComponent<ParticleSystem>();
    }

    public void SetStartSize(float size)
    {
        transform.localScale = Vector3.one * (size / maxParticleSize);
    }

    private void InitParticleSize()
    {
        maxParticleSize = gameObject.GetComponentsInChildren<ParticleSystem>().Max((p) => p.main.startSize.constant);
    }
}
