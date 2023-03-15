﻿using System;
using Common.Grid;
using UnityEngine;
using Zen.Hexagons;

namespace State.World {
  [Serializable]
  public abstract class WorldTile {
    private static readonly float DifficultyBaseline = 1;
    private static readonly float DifficultyScalingByDistance = 0.1f;

    public float difficulty;
    public TileState state = TileState.Obscured;
    public HexOffsetCoordinates coordinates;
    public bool isTraversable;
    public bool connectsToBoundaries;

    protected WorldTile(HexOffsetCoordinates coordinates) {
      this.coordinates = coordinates;
      difficulty = DifficultyBaseline +
          (int)(HexGridUtils.HexLibrary.GetDistance(HexOffsetCoordinates.Origin, coordinates)
          * DifficultyScalingByDistance);
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