using System;
using System.Diagnostics;
using UnityEngine;

namespace Zen.Hexagons
{
  /// <summary>
  /// Minorly edited to be Unity-serializable.
  /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [Serializable]
    public struct HexOffsetCoordinates {
        [NonSerialized]
        public static readonly HexOffsetCoordinates Origin = new(0, 0);

        [SerializeField] private int col;
        [SerializeField] private int row;
        public int Col => col;
        public int Row => row;

        public HexOffsetCoordinates(int col, int row) {
            this.col = col;
            this.row = row;
        }

        public Vector3Int AsVector3Int() {
          // Row as X (swapped), because bafflingly, Unity uses X as the vertical axis
          // in its grids.
          return new Vector3Int(Row, Col, 0);
        }

        public static HexOffsetCoordinates From(Vector3Int vec) {
          // Once again, swapped to accomodate Unity weirdness.
          return new HexOffsetCoordinates(vec.y, vec.x);
        }
        
        public static HexOffsetCoordinates From(int x, int y) {
          // Once again, swapped to accomodate Unity weirdness.
          return new HexOffsetCoordinates(y, x);
        }

        #region Overrides and Overloads

        public override bool Equals(object obj)
        {
            return obj is HexOffsetCoordinates offsetCoordinates && this == offsetCoordinates;
        }

        public static bool operator == (HexOffsetCoordinates a, HexOffsetCoordinates b)
        {
            return a.Col == b.Col && a.Row == b.Row;
        }

        public static bool operator != (HexOffsetCoordinates a, HexOffsetCoordinates b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Col.GetHashCode() ^ Row.GetHashCode();
        }

        public override string ToString()
        {
            return DebuggerDisplay;
        }

        private string DebuggerDisplay => $"{{Col={Col},Row={Row}}}";

        #endregion
    }
}