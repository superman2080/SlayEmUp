using UnityEngine;

public class StartSceneUI : MonoBehaviour
{
    public void SelectCharacter(string characterName) => GameManager.Instance.SelectedCharacterName = characterName;

    public void GameSceneLoad()
    {
        SceneLoader.Instance.SceneLoadAsync("GameScene");
    }
}
