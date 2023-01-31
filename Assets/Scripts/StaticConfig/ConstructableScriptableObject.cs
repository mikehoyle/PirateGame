﻿using System;
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
    
    public string id;
    public string buildDisplayName;
    public LineItem[] buildCost;
    // Indicates the tile can be built next to other foundations, but not atop them.
    public bool isFoundationTile;
    
    public TileBase inGameTile;
    public Sprite inGameSprite;

    // TODO(P1): Actually support builds bigger than one tile
    public Vector3Int dimensions = Vector3Int.one;
  }
}