using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu]
public class GameEntity : ScriptableObject
{
  public string idEntity;
  public TypeEntity typeEntity;
  public TextLocalize text;
  public Sprite sprite;
  [SerializeField] public AssetReferenceGameObject prefab;
  public ParticleSystem activateEffect;
  public ParticleSystem MoveEffect;
}


[System.Serializable]
public enum TypeEntity
{
  None = 0,
  Bomb = 1,
  Lighting = 2,
  Star = 3,
  Coin = 4,
  OpenWord = 5,
  Hint = 6
}
