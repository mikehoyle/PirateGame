﻿using Common.Events;
using RuntimeVars.Encounters.Events;
using UnityEngine;

namespace RuntimeVars.ShipBuilder.Events {
  [CreateAssetMenu(menuName = "Events/ShipBuilder/ShipBuilderEvents")]
  public class ShipBuilderEvents : ScriptableObject {
    public BuildSelectedEvent buildSelected;
    public EmptyGameEvent enterConstructionMode;
    public EmptyGameEvent exitConstructionMode;
    public UnitSelectedEvent unitSelected;
    public ObjectClickedEvent objectClicked;
  }
}