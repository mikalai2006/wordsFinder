using UnityEngine;

public static class HelpersAnimation
{
  public static void Pulse(GameObject gameObject, Vector3 amount)
  {
    iTween.PunchScale(gameObject, iTween.Hash(
        "amount", amount,
        "time", .5f,
        "easetype", iTween.EaseType.easeOutBack
        ));
  }

}