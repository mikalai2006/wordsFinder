using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class AdManager : Singleton<LevelManager>
{
  [DllImport("__Internal")]
  private static extern void ShowAdvFullScreen();
  [DllImport("__Internal")]
  private static extern void AddHintExtern(string data);
  [DllImport("__Internal")]
  public static extern void AddBonusExtern(string data);
  private string lastTimeShowAdv;
  private GameManager _gameManager => GameManager.Instance;
  private GameSetting _gameSetting => GameManager.Instance.GameSettings;
  private StateManager _stateManager => GameManager.Instance.StateManager;

  protected override void Awake()
  {
    base.Awake();
    SetLastTime();
  }

  public void ShowAdvFullScr()
  {
    var diffDate = DateTime.Now - DateTime.Parse(lastTimeShowAdv);

    if (diffDate.TotalMinutes > (double)_gameSetting.adsPerTime)
    {
#if ysdk
      ShowAdvFullScreen();
#endif
      SetLastTime();
      UnityEngine.Debug.Log("Show auto adv");
    }
  }

  public void SetLastTime()
  {
    lastTimeShowAdv = System.DateTime.Now.ToString();
  }

  public void AddHintByAdv(ShopAdvBuyItem<TypeEntity> data)
  {
    string stringAddedHint = JsonUtility.ToJson(data);

    Debug.Log($"AddHintByAdv {stringAddedHint}");

    AddHintExtern(stringAddedHint);

    lastTimeShowAdv = System.DateTime.Now.ToString();
  }


  public void AddBonusByAdv(ShopAdvBuyItem<TypeBonus> data)
  {
    string stringAddedBonus = JsonUtility.ToJson(data);
    Debug.Log($"AddBonusByAdv {stringAddedBonus}");

    AddBonusExtern(stringAddedBonus);

    lastTimeShowAdv = System.DateTime.Now.ToString();
  }

}
