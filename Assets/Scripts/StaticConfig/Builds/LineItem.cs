using System;
using StaticConfig.RawResources;

namespace StaticConfig.Builds {
  [Serializable]
  public class LineItem {
    public RawResource resource;
    public int cost;
  }
}