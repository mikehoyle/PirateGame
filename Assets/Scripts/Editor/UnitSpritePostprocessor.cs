using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using StaticConfig;
using UnityEditor;
using UnityEngine;

// Avoiding Editor namespace because it messes with a lot of things.
namespace EditorInternal {
  public class UnitSpritePostprocessor : AssetPostprocessor {
    private const string UnitAssetPath = "Sprites/Units/Composite";
    private const string SpritesPath = "Sprites/";
    private const string CompositeAssetPath = "Assets/Sprites/Units/Composite/CompositeSprite.asset";
    private const string AssetExtension = ".asset";
    private const int UnitSpritesheetFrameWidth = 160;
    private const int UnitSpritesheetFrameHeight = 128;
    private const int SpritePixelsPerUnit = 64;
    
    private void OnPreprocessTexture() {
      var normalizedPath = assetPath.Replace("\\", "/");
      if (normalizedPath.Contains(SpritesPath)) {
        SetCommonMetadata();
      }
      if (normalizedPath.Contains(UnitAssetPath)) {
        SliceSpritesheets();
      }
    } 

    private void SetCommonMetadata() {
      var textureImporter = (TextureImporter)assetImporter;
      if (textureImporter == null || textureImporter.textureType != TextureImporterType.Sprite) {
        return;
      }
      textureImporter.wrapMode = TextureWrapMode.Clamp;
      textureImporter.filterMode = FilterMode.Point;
      textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
      textureImporter.spritePixelsPerUnit = SpritePixelsPerUnit;
      textureImporter.maxTextureSize = 8192;
    }

    private void SliceSpritesheets() {
      var textureImporter = (TextureImporter)assetImporter;
      if (textureImporter == null || textureImporter.textureType != TextureImporterType.Sprite) {
        return;
      }

      textureImporter.spriteImportMode = SpriteImportMode.Multiple;
      
      // Sucks I have to load the whole file for this, but it's pixel art who cares
      int imageWidth, imageHeight;
      using (var bitmap = new Bitmap(assetPath)) {
        imageWidth = bitmap.Width;
        imageHeight = bitmap.Height;
      }

      if (imageWidth % UnitSpritesheetFrameWidth != 0 || imageHeight % UnitSpritesheetFrameHeight != 0) {
        Debug.LogWarning($"Unable to slice spritesheet {assetPath}, because its dimensions are unexpected: " +
            $"{imageWidth} x {imageHeight}.");
        return;
      }

      int numCols = imageWidth / UnitSpritesheetFrameWidth;
      int numRows = imageHeight / UnitSpritesheetFrameHeight;
      
      List<SpriteMetaData> metadataList = new List<SpriteMetaData>();
      string filename = Path.GetFileNameWithoutExtension(assetPath);
      int spriteCount = 0;
                   
      for (int row = numRows - 1; row >= 0; row--) {
        for (int col = 0; col < numCols; col++) {
          // The sprites are rendered left-to-right, top-to-bottom, but for some ungodly reason
          // unity sprite editor considers up positive for y, so flip the y coord.
          var metadata = new SpriteMetaData() {
              rect = new Rect(
                  x: col * UnitSpritesheetFrameWidth,
                  y: row * UnitSpritesheetFrameHeight,
                  width: UnitSpritesheetFrameWidth,
                  height: UnitSpritesheetFrameHeight),
              name = $"{filename}_{spriteCount}",
              // slice pivot is relative to bounding rectangle, so this is dead center.
              pivot = new Vector2(0.5f, 0.5f),
          };
          metadataList.Add(metadata);
          spriteCount++;
        }
      }
      
      textureImporter.spritesheet = metadataList.ToArray();
    }
    
    

    private static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths) {
      AddAllUnitAssets(importedAssets.AsEnumerable().Concat(movedAssets.AsEnumerable()));
      DeleteRemovedUnitAssets(deletedAssets.AsEnumerable().Concat(movedFromAssetPaths.AsEnumerable()));
    }
    
    private static void AddAllUnitAssets(IEnumerable<string> importedAssets) {
      var updatedAssetPaths = new List<string>();
      foreach (var importedAssetPath in importedAssets) {
        if (!IsUnitPath(importedAssetPath)) {
          continue;
        }

        var spriteList = new List<Sprite>();
        foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(importedAssetPath)) {
          // Anecdotally, the sprites always seem to be in order.
          if (asset is Sprite sprite) {
            spriteList.Add(sprite);
          }
        }

        if (spriteList.Count > 0) {
          var targetDir = Path.GetDirectoryName(importedAssetPath);
          var targetFilename = Path.GetFileNameWithoutExtension(importedAssetPath) + AssetExtension;
          var targetPath = Path.Join(targetDir, targetFilename);

          UpdateOrCreateComponentAsset(targetPath, spriteList);
          updatedAssetPaths.Add(targetPath);
        }
      }

      if (updatedAssetPaths.Count > 0) {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        UpdateOrCreateCompositeAsset(updatedAssetPaths, removeAssets: false);
        AssetDatabase.SaveAssets();
      }
    }

    private static void DeleteRemovedUnitAssets(IEnumerable<string> deletedPaths) {
      var updatedAssetPaths = new List<string>();
      foreach (var deletedPath in deletedPaths) {
        if (!IsUnitPath(deletedPath)) {
          continue;
        }

        var assetPath = Path.ChangeExtension(deletedPath, AssetExtension);
        if (AssetDatabase.DeleteAsset(assetPath)) {
          updatedAssetPaths.Add(assetPath);
        }
      }

      if (updatedAssetPaths.Count > 0) {
        UpdateOrCreateCompositeAsset(updatedAssetPaths, removeAssets: true);
      }
    }

    private static void UpdateOrCreateComponentAsset(
        string targetPath, List<Sprite> spriteList) {
      var spriteComponent = AssetDatabase.LoadAssetAtPath<CompositeSpriteComponentScriptableObject>(targetPath);
      if (spriteComponent == null) {
        spriteComponent = ScriptableObject.CreateInstance<CompositeSpriteComponentScriptableObject>();
        AssetDatabase.CreateAsset(spriteComponent, targetPath);
      }
      
      spriteComponent.frames = spriteList.ToArray();
      EditorUtility.SetDirty(spriteComponent);
    }

    private static void UpdateOrCreateCompositeAsset(List<string> updatedAssetPaths, bool removeAssets = false) {
      var composite = AssetDatabase.LoadAssetAtPath<CompositeUnitSpriteScriptableObject>(CompositeAssetPath);
      if (composite == null) {
        composite = ScriptableObject.CreateInstance<CompositeUnitSpriteScriptableObject>();
        AssetDatabase.CreateAsset(composite, CompositeAssetPath);
      }
      composite.EnsureFieldsInitialized();

      foreach (var updatedAssetPath in updatedAssetPaths) {
        var assetLayer = CompositeUnitSpriteScriptableObject.Layer.None;
        foreach (var layer in CompositeUnitSpriteScriptableObject.LayerToString) {
          if (updatedAssetPath.Contains(layer.Value)) {
            assetLayer = layer.Key;
            break;
          }
        }

        if (assetLayer == CompositeUnitSpriteScriptableObject.Layer.None) {
          Debug.LogWarning($"Asset {updatedAssetPath} not found to be in any known composite layer");
          continue;
        }

        var componentDictionary = composite.layerOptions[(int)assetLayer];
        if (removeAssets) {
          componentDictionary.Remove(Path.GetFileNameWithoutExtension(updatedAssetPath));
        } else {
          var assetToLink = AssetDatabase.LoadAssetAtPath<CompositeSpriteComponentScriptableObject>(updatedAssetPath);
          if (assetToLink == null) {
            Debug.LogWarning($"Asset {updatedAssetPath} not found! This should not happen," +
                $"as it should've been just created. Look for creation errors.");
            continue;
          }
          
          componentDictionary[Path.GetFileNameWithoutExtension(updatedAssetPath)] = assetToLink;
        }
        
      }
      
      EditorUtility.SetDirty(composite);
    }

    private static bool IsUnitPath(string assetPath) {
      var normalizedPath = assetPath.Replace("\\", "/");
      return normalizedPath.Contains(UnitAssetPath);
    }
  }
}