using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

// 구조체
[System.Serializable]
public struct WaveData
{
    public WaveData(string waveName, int delayBeforeStart, string poolName, int enemyCount, float interval, float growthValue)
    {
        this.WaveName = waveName;
        this.DelayBeforeStart = delayBeforeStart;
        this.PoolName = poolName;
        this.EnemyCount = enemyCount;
        this.Interval = interval;
        this.GrowthValue = growthValue;
    }

    public string WaveName { get; private set; }
    public int DelayBeforeStart { get; private set; }
    public string PoolName { get; private set; }
    public int EnemyCount { get; private set; }
    public float Interval { get; private set; }

    public float GrowthValue { get; private set; }
}

public class SheetReader : MonoBehaviour
{
    public const string csvUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vSqABQJWIpPvxZsUrVr5IWIgmpu4shGrS43sFZDlUeRdHeE7EeTVkAS0_VCX1j92yLekBczaguAG2Rp/pub?output=csv";
    
    public List<WaveData> WaveData { get; private set; } = new List<WaveData>();

    public void Start()
    {
        StartCoroutine(LoadCSV());
    }

    public IEnumerator LoadCSV()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(csvUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError(www.error);
            else
            {
                string csvText = www.downloadHandler.text;
                WaveData = ParseCSVToMonsters(csvText);
                foreach (var wave in WaveData)
                {
                    Debug.Log($"" +
                        $"{wave.WaveName}\n" +
                        $"{wave.DelayBeforeStart}\n" +
                        $"{wave.PoolName}\n" +
                        $"{wave.EnemyCount}\n" +
                        $"{wave.Interval}");
                }
            }
        }
    }

    private List<WaveData> ParseCSVToMonsters(string csvText)
    {
        List<WaveData> monsterList = new List<WaveData>();

        string[] lines = csvText.Split('\n');

        // 첫 줄은 헤더이므로 1부터 시작
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue; 

            string[] values = line.Split(',');


            string waveName = values[0];
            int delayBeforeStart = int.Parse(values[1]);
            string poolName = values[2];
            int enemyCount = int.Parse(values[3]);
            float interval = float.Parse(values[4]);
            float growthVal = float.Parse(values[5]);

            monsterList.Add(new WaveData(waveName, delayBeforeStart, poolName, enemyCount, interval, growthVal));
        }

        return monsterList;
    }

    // Co + routine 
    // 협동<< 유니티 라이프사이클과 협동 루틴 => 반복
}
