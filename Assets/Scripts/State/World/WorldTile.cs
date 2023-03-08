using System;
using UnityEngine;
using Zen.Hexagons;

namespace State.World {
  [Serializable]
  public abstract class WorldTile {
    private static readonly int DifficultyBaseline = 2;
    private static readonly float DifficultyScaling = 1f;

    public int difficulty;
    public TileState state = TileState.Obscured;
    public HexOffsetCoordinates coordinates;
    public bool isTraversable;

    protected WorldTile(HexOffsetCoordinates coordinates) {
      var hexLibrary = new HexLibrary(HexType.FlatTopped, OffsetCoordinatesType.Odd, 1);
      this.coordinates = coordinates;
      difficulty = DifficultyBaseline +
          (int)(hexLibrary.GetDistance(HexOffsetCoordinates.Origin, coordinates) * DifficultyScaling);
    }

    public T DownCast<T>() where T : WorldTile {
      if (this is T downRef) {
        return downRef;
      }
      
      Debug.LogWarning($"Cannot cast tile as {nameof(T)}");
      return null;
    }

    public void Reveal() {
      if (state == TileState.Obscured) {
        state = TileState.Visible;
      }
    }
    
    public virtual void OnVisit() { }
  }
}