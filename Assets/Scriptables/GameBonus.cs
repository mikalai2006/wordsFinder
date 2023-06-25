using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu]
public class GameBonus : ScriptableObject
{
  public string idEntity;
  public TypeBonus typeBonus;
  public TextLocalize text;
  public Sprite sprite;
  [SerializeField] public AssetReferenceGameObject prefab;
  public int value;
  // public ParticleSystem activateEffect;
  // public ParticleSystem MoveEffect;
}


[System.Serializable]
public enum TypeBonus
{
  None = 0,
  OpenNeighbours = 1,
  Index = 2,
}
