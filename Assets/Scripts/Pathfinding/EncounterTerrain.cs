﻿using System.Collections.Generic;
using System.Linq;
using Common;
using Roy_T.AStar.Paths;
using RuntimeVars;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using Units;
using UnityEngine;

namespace Pathfinding {
  public class EncounterTerrain : MonoBehaviour {
    [SerializeField] private UnitCollection unitsInEncounter;
    [SerializeField] private CurrentSelection currentSelection;
    [SerializeField] private UnitSelectedEvent unitSelectedEvent;
    
    private const int MaxEncounterWidth = 300;
    private const int MaxEncounterHeight = 300;
    
    private EncounterNode[,] _nodes;
    private int _width;
    private int _height;
    private int _gridOffsetX;
    private int _gridOffsetY;
    private PathFinder _pathfinder;

    private void Awake() {
      _width = MaxEncounterWidth;
      _height = MaxEncounterHeight;
      _gridOffsetX = MaxEncounterWidth / 2;
      _gridOffsetY = MaxEncounterHeight / 2;
      _nodes = new EncounterNode[MaxEncounterWidth, MaxEncounterHeight];
      _pathfinder = new PathFinder();
    }

    private void OnEnable() {
      unitSelectedEvent.RegisterListener(OnUnitSelected);
    }

    private void OnDisable() {
      unitSelectedEvent.UnregisterListener(OnUnitSelected);
    }

    private void Update() {
      // OPTIMIZE this is terribly inefficient any may need to change if there is lag
      RefreshUnitObstacles();
    }

    private void OnUnitSelected(UnitController _) {
      RefreshUnitObstacles();
    }

    private void RefreshUnitObstacles() {
      foreach (var unit in unitsInEncounter) {
        SetEnabled(unit.State.encounterState.position, false);
      }

      var coords = new Vector2Int(0, 0);
      for (int i = 0; i < _nodes.GetLength(0); i++) {
        for (int j = 0; j < _nodes.GetLength(1); j++) {
          coords.x = i;
          coords.y = j;
          if (!IsGridPositionDefined(coords)) {
            continue;
          }
          
          var node = GetNode(coords);
          if (unitsInEncounter.Any(unit => unit.State.encounterState.position == node.GridPosition)) {
            if (currentSelection.TryGet(out _, out var unit)) {
              if (unit.State.encounterState.position == node.GridPosition) {
                node.Enabled = true;
                continue;
              }
            }
            node.Enabled = false;
            continue;
          }
          node.Enabled = true;
        }
      }
    }

    private EncounterNode GetNode(Vector2Int coords) => _nodes[coords.x, coords.y];

    public void SetEnabled(Vector3Int coords, bool enabled) {
      if (!IsGridPositionDefined(PositionForCoords(coords))) {
        return;
      }
      GetNode(PositionForCoords(coords)).Enabled = enabled;
    }
    
    /// <summary>
    /// Returns proper elevation, but ignores elevation for pathing purposes.
    /// </summary>
    public TravelPath GetPath(Vector3Int origin, Vector3Int destination) {
      var originGridPosition = PositionForCoords(origin);
      var destinationGridPosition = PositionForCoords(destination);
      if (!IsGridPositionDefined(originGridPosition) || !IsGridPositionDefined(destinationGridPosition)) {
        return new TravelPath(null);
      }
      
      var path = _pathfinder.FindPath(
          GetNode(originGridPosition), GetNode(destinationGridPosition), EncounterNode.TraversalVelocity);
      if (path.Type != PathType.Complete) {
        return new TravelPath(null);
      }

      var result = new LinkedList<Vector3Int>();
      if (path.Edges.Count > 0) {
        result.AddLast(((EncounterNode)path.Edges[0].Start).GridPosition);
      }
      foreach (var edge in path.Edges) {
        result.AddLast(((EncounterNode)edge.End).GridPosition);
      }
      return new TravelPath(result);
    }

    public void MarkCellTraversable(Vector3Int cell) {
      var gridPosition = PositionForCoords(cell);

      if (!IsGridPositionInBounds(gridPosition)) {
        Debug.LogWarning("Attempt to access coordinate outside grid, ignoring.");
        return;
      }

      var node = GetNode(gridPosition);
      if (node == null) {
        node = new EncounterNode(cell);
        _nodes[gridPosition.x, gridPosition.y] = node;
      }
      
      IsometricGridUtils.ForEachAdjacentTile(gridPosition, adjacentPosition => {
        var connectedNode = GetNode(adjacentPosition);
        if (connectedNode != null) {
          node.Connect(connectedNode);
        }
      });
    }

    private Vector2Int PositionForCoords(Vector3Int coords) {
      // Just ignore Z value for now
      return new Vector2Int(coords.x + _gridOffsetX, coords.y + _gridOffsetY);
    }
    
    private bool IsGridPositionDefined(Vector2Int gridPosition) {
      if (!IsGridPositionInBounds(gridPosition) || GetNode(gridPosition) == null) {
        return false;
      }
      
      return true;
    }

    private bool IsGridPositionInBounds(Vector2Int gridPosition) {
      return gridPosition.x >= 0
          && gridPosition.y >= 0
          && gridPosition.x <= _width
          && gridPosition.y <= _height;
    }

    public static EncounterTerrain Get() {
      return GameObject.FindWithTag(Tags.Grid).GetComponent<EncounterTerrain>();
    }
  }
}