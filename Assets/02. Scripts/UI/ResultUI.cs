using TMPro;
using UnityEngine;
using UnityEngine.UI;



[DefaultExecutionOrder(200)]
public class ResultUI : EventListener
{
    public Player player;
    public RectTransform resultUI;

    public TextMeshProUGUI killCountText;
    public Button returnToStartScene;

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        resultUI.gameObject.SetActive(false);
    }

    public void OnPlayedDied()
    {
        resultUI.gameObject.SetActive(true);
        killCountText.text = $"Kill Count {player.killCount}";
        returnToStartScene.onClick.AddListener(() => SceneLoader.Instance.SceneLoadAsync("StartScene"));
    }
}
