using TMPro;
using UnityEngine;

public class KillCountUI : EventListener
{
    public Player player;
    private TextMeshProUGUI killCountText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        killCountText = gameObject.GetComponent<TextMeshProUGUI>();
        player = FindAnyObjectByType<Player>();
        killCountText.text = $"Kill Count {player.killCount}";
    }

    public void OnEnemyDied()
    {
        killCountText.text = $"Kill Count {player.killCount}";
    }
}
