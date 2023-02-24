using System.Collections.Generic;
using Common;
using StaticConfig.Equipment;
using Terrain;
using UnityEngine;

namespace StaticConfig.Builds {
  /// <summary>
  /// Represents an object that can be constructed on the ship.
  /// </summary>
  [CreateAssetMenu(menuName = "ShipManagement/ConstructableObject")]
  public class ConstructableObject : EnumScriptableObject {
    public string buildDisplayName;
    public LineItem[] buildCost;
    // Indicates the tile can be built next to other foundations, but not atop them.
    public bool isFoundationTile;
    // Indicates the tile can be walked through.
    public bool isTraversable;
    public Sprite inGameSprite;
    // TODO(P1): Actually support builds bigger than one tile
    public Vector3Int dimensions = Vector3Int.one;
    public ProvidedAbility providedAbility;
    public List<CraftingRecipe> providedCraftables;

    public Vector3 WorldPosition(Vector3Int gridPosition) {
      // All these if/elses are begging for better code design... but also the game is begging
      // to be actually made, so leaving it for now.
      return isFoundationTile
          ? SceneTerrain.CellAnchorWorldStatic(gridPosition)
          : SceneTerrain.CellBaseWorldStatic(gridPosition);
    }

    public string SortingLayer() {
      return isFoundationTile ? SortingLayers.Terrain : SortingLayers.Default;
    }
  }
}