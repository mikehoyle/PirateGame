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

        // In conversions, all Y coords are negated because unity considers Y up, while
        // the library considers Y down.
        public Vector3Int AsVector3Int() {
          return new Vector3Int(Col, -Row, 0);
        }

        public static HexOffsetCoordinates From(Vector3Int vec) {
          return new HexOffsetCoordinates(vec.x, -vec.y);
        }
        
        public static HexOffsetCoordinates From(int x, int y) {
          return new HexOffsetCoordinates(x, -y);
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