using UnityEngine;

public class ResetState : MonoBehaviour
{
  private GameManager _gameManager => GameManager.Instance;
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  private StateManager _stateManager => GameManager.Instance.StateManager;
  bool ctrl = false;

  private async void Update()
  {
    if (Input.GetKeyDown(KeyCode.LeftControl))
    {
      ctrl = true;
      Debug.Log("Reset state");
    }

    if (Input.GetKeyDown(KeyCode.Print) && ctrl && _gameManager.LevelManager == null)
    {
      await _gameManager.StateManager.Reset();

      _gameManager.DataManager.Save();
      Debug.Log("Reset state");
      ctrl = false;
    }
  }
}
