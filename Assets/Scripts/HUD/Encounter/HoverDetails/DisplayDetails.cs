using System.Collections.Generic;

namespace HUD.Encounter.HoverDetails {
  /// <summary>
  /// Encapsulates details to be displayed in HUD.
  /// </summary>
  public class DisplayDetails {
    public string Name { get; set; }
    // TODO(P2): Include image and more formatting.
    public List<string> AdditionalDetails { get; set; }
  }
}