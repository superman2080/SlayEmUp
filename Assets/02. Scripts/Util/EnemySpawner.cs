using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EventChannelSO stageChangedEventChannel;
    public static int nowWaveIndex = 0;

    public int MaxWaveCnt => maxWaveCnt;
    private int maxWaveCnt = 0;

    public bool IsInfinityStage => isInfinityStage;
    [SerializeField] private bool isInfinityStage;

    // Pool
    // Data
    // Wave가 종료되었는지
    public bool IsWaveDone { get; private set; } = false;
    public SheetReader reader;
    public List<Pool<Enemy>> enemySpawnPoolList = new List<Pool<Enemy>>();
    private List<List<WaveData>> waveList = new List<List<WaveData>>();
    private int runningWaveCnt;         // 현재 돌아가고있는 웨이브 개수

    //생성되는 위치의 마진값
    [Min(0)] public float margin = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartWave());
    }

    private IEnumerator StartWave()
    {
        yield return StartCoroutine(reader.LoadCSV());
        if (reader.WaveData == null)
            yield break;        // return


        maxWaveCnt = int.Parse(reader.WaveData.OrderByDescending((w) => int.Parse(w.WaveName.Split("/")[1])).First().WaveName.Split("/")[1]) + 1;
        // SQL

        for (int i = 0; i < maxWaveCnt; i++)
        {
            waveList.Add(reader.WaveData.FindAll((w) => int.Parse(w.WaveName.Split("/")[1]) == i));
        }

        for (int i = 0; i < waveList.Count; i++)
        {
            do
            {
                // 웨이브 시작
                nowWaveIndex = i;
                stageChangedEventChannel.RaiseEvent();
                runningWaveCnt = waveList[i].Count;
                foreach (var wave in waveList[i])
                {
                    // 웨이브 시작 코루틴
                    StartCoroutine(WaveCoroutine(wave));
                }
                yield return new WaitUntil(() => runningWaveCnt <= 0);
            } while (isInfinityStage && i == waveList.Count - 1);
        }
        IsWaveDone = true;
    }

    private IEnumerator WaveCoroutine(WaveData data)
    {
        yield return new WaitForSeconds(data.DelayBeforeStart);
        var pool = enemySpawnPoolList.Find((p) => p.gameObject.name == data.PoolName);

        for (int i = 0; i < data.EnemyCount; i++)
        {
            var enemy = pool.Get();
            enemy.SetStat(enemy.statData * data.GrowthValue);
            Debug.Log($"{enemy.name} + {enemy.HP}");
            enemy.transform.position = GetRandomPointOutsideView(Camera.main, margin);
            yield return new WaitForSeconds(data.Interval);
        }
        runningWaveCnt--;
    }

    private Vector2 GetRandomPointOutsideView(Camera cam, float margin = 0.1f)
    {
        float randX = 0f;
        float randY = 0f;

        if(Random.value < 0.5f)
        {
            // X축을 화면 밖으로 
            randX = Random.value < 0.5f ? -margin : 1f + margin;
            randY = Random.Range(0, 1f);
        }
        else
        {
            // Y축이 화면 밖
            randX = Random.Range(0f, 1f);
            randY = Random.value < 0.5f ? -margin : 1f + margin;
        }

        return cam.ViewportToWorldPoint(new Vector2(randX, randY));
    }

    private void OnDrawGizmos()
    {
        var cam = Camera.main;
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(-margin, -margin, cam.nearClipPlane));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1f + margin, 1 + margin, cam.nearClipPlane));

        // 2D 게임용 Vector2 경계
        Vector2 min = new Vector2(bottomLeft.x, bottomLeft.y);
        Vector2 max = new Vector2(topRight.x, topRight.y);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(cam.transform.position, max - min);
    }
}
