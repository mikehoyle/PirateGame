namespace HUD.Encounter.HoverDetails {
  /// <summary>
  /// An encounter object which can provide details about itself when hovered.
  /// </summary>
  public interface IDisplayDetailsProvider {
    DisplayDetails GetDisplayDetails();
  }
}