using System;
using System.Linq;
using StaticConfig;
using StaticConfig.Builds;
using StaticConfig.RawResources;
using UnityEditor;

namespace EditorInternal {
  public class ResourceAggregatorPostprocessor : AssetPostprocessor {
    private const string ConstructablesPath = "Assets/Config/Constructables";
    private const string ConstructableAggregatePath = "Assets/Config/AllBuildOptions.asset";
    private const string ResourcesPath = "Assets/Config/Resources";
    private const string ResourcesAggregatePath = "Assets/Config/AllResources.asset";
    
    private static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths) {
      var constructableAggregate =
          AssetDatabase.LoadAssetAtPath<AllBuildOptionsScriptableObject>(ConstructableAggregatePath);
      var resourcesAggregate =
          AssetDatabase.LoadAssetAtPath<AllResourcesScriptableObject>(ResourcesAggregatePath);
      foreach (var updatedAssetPath in importedAssets.AsEnumerable().Concat(movedAssets.AsEnumerable())) {
        UpdateAggregates(updatedAssetPath, constructableAggregate, resourcesAggregate);
      }
      RemoveFromAggregates(constructableAggregate, resourcesAggregate);
      EditorUtility.SetDirty(constructableAggregate);
      EditorUtility.SetDirty(resourcesAggregate);
    }
    
    private static void UpdateAggregates(
        string updatedAssetPath,
        AllBuildOptionsScriptableObject constructableAggregate,
        AllResourcesScriptableObject resourcesAggregate) {
      if (Normalize(updatedAssetPath).Contains(ConstructablesPath)) {
        var itemToUpdate = AssetDatabase.LoadAssetAtPath<ConstructableObject>(updatedAssetPath);
        if (itemToUpdate != null) {
          var itemUpdated = false;
          for (int i = 0; i < constructableAggregate.buildOptions.Length; i++) {
            if (constructableAggregate.buildOptions[i].id == itemToUpdate.id) {
              constructableAggregate.buildOptions[i] = itemToUpdate;
              itemUpdated = true;
              break;
            }
          }
          if (!itemUpdated) {
            constructableAggregate.buildOptions = 
                constructableAggregate.buildOptions
                    .ToList()
                    .Append(itemToUpdate)
                    .ToArray();
          }
        }
      }
      
      // Ugly duplicated code, oh well
      if (Normalize(updatedAssetPath).Contains(ResourcesPath)) {
        var itemToUpdate = AssetDatabase.LoadAssetAtPath<RawResource>(updatedAssetPath);
        if (itemToUpdate != null) {
          var itemUpdated = false;
          for (int i = 0; i < resourcesAggregate.resources.Length; i++) {
            if (resourcesAggregate.resources[i] == itemToUpdate) {
              resourcesAggregate.resources[i] = itemToUpdate;
              itemUpdated = true;
              break;
            }
          }
          if (!itemUpdated) {
            resourcesAggregate.resources = 
                resourcesAggregate.resources
                    .ToList()
                    .Append(itemToUpdate)
                    .ToArray();
          }
        }
      }
    }
    
    
    private static void RemoveFromAggregates(
        AllBuildOptionsScriptableObject constructableAggregate,
        AllResourcesScriptableObject resourcesAggregate) {
      // Technically we just remove everything that's null, hopefully that works
      constructableAggregate.buildOptions = constructableAggregate.buildOptions.ToList()
          .Where(item => item != null)
          .ToArray();
      resourcesAggregate.resources = resourcesAggregate.resources.ToList()
          .Where(item => item != null)
          .ToArray();
    }

    private static string Normalize(string assetPath) {
      return assetPath.Replace("\\", "/");
    }
  }
}