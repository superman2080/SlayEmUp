using TMPro;
using UnityEngine;

public class StageDataUI : EventListener
{
    public TextMeshProUGUI waveIndexText;
    [SerializeField] private EnemySpawner spawner;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetData();
    }

    private void Reset()
    {
        ResetData();
    }

    void ResetData()
    {
        spawner ??= FindAnyObjectByType<EnemySpawner>();
        waveIndexText ??= gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void OnWaveChanged()
    {
        if (spawner.IsInfinityStage && EnemySpawner.nowWaveIndex == spawner.MaxWaveCnt - 1)
            waveIndexText.text = "Infinity Wave";
        else
            waveIndexText.text = $"Wave {EnemySpawner.nowWaveIndex + 1}";
    }
}
