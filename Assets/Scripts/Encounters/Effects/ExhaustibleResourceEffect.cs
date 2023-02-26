using System;
using StaticConfig.Units;
using Units.Abilities.Formulas.Values;
using UnityEngine;

namespace Encounters.Effects {
  [Serializable]
  public class ExhaustibleResourceEffect {
    public ExhaustibleResource resource;
    [SerializeReference, SerializeReferenceButton]
    public IDerivedValue value;
  }
}