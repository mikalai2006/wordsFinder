using UnityEngine;

public static class HelpersAnimation
{
  public static void Pulse(GameObject gameObject, Vector3 amount, float time)
  {
    iTween.PunchScale(gameObject, iTween.Hash(
        "amount", amount,
        "time", time,
        "easetype", iTween.EaseType.easeOutBack
        ));
  }

}