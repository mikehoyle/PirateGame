using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace StaticConfig {
  /// <summary>
  /// Represents an object that can be constructed on the ship.
  /// </summary>
  [CreateAssetMenu(fileName = "Constructable", menuName = "ScriptableObjects/Constructable", order = 0)]
  public class ConstructableScriptableObject : ScriptableObject {
    [Serializable]
    public class LineItem {
      public RawResourceScriptableObject resource;
      public int cost;
    }

    [InspectorName("Id (DO NOT CHANGE)")]
    public string id;
    public string buildDisplayName;
    public LineItem[] buildCost;
    
    public bool isTile;
    // If tile
    public TileBase inGameTile;
    // If sprite
    public Sprite inGameSprite;
  }
}