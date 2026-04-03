using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    public string SelectedCharacterName { get; set; }

    private Player selectedCharacter;

    private void Start()
    {
        SceneLoader.Instance.OnLoadStarted += CreateCharacter;
        SceneLoader.Instance.OnSceneLoaded += EnableCharacter;
    }

    private void OnDestroy()
    {
        SceneLoader.Instance.OnLoadStarted -= CreateCharacter;
        SceneLoader.Instance.OnSceneLoaded -= EnableCharacter;
    }



    public void CreateCharacter(string sceneName)
    {
        if(sceneName == "GameScene")
        {
            selectedCharacter = Instantiate(Resources.Load<Player>($"Player/{SelectedCharacterName}"));
            selectedCharacter.enabled = false;
            DontDestroyOnLoad(selectedCharacter);
        }
        else
        {
            if(selectedCharacter != null)
                Destroy(selectedCharacter.gameObject);
        }
    }

    public void EnableCharacter(string sceneName)
    {
        if(sceneName == "GameScene" && selectedCharacter != null)
        {
            selectedCharacter.enabled = true;
        }
    }
}
