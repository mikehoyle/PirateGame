using UnityEngine;

namespace StaticConfig.Builds {
  [CreateAssetMenu(menuName = "ShipManagement/CraftsCollection")]
  public class CraftsCollection : ScriptableObject {
    public CraftingRecipe[] craftingRecipes;
  }
}