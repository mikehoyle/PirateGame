using System.Collections.Generic;
using Units.Abilities;
using UnityEngine;

namespace StaticConfig.Equipment {
  [CreateAssetMenu(menuName = "Equipment/Equipment")]
  public class Equipment : ScriptableObject {
    public Sprite hudSprite;
    public EquipmentSlot applicableSlot;
    public List<UnitAbility> abilitiesProvided;
  }
}