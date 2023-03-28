using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Animation;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace EditorInternal {
  public class AsepriteSpritesheetPostprocessor : AssetPostprocessor {
    private const string SpritesPath = "Sprites/";
    private const int SpritePixelsPerUnit = 64;
    private static readonly List<string> ValidDirections = new() {
        "sw",
        "nw",
        "ne",
        "se",
    };
        
    private void OnPreprocessTexture() {
      if (assetPath.Contains(SpritesPath)) {
        SetCommonMetadata();
      }
      var jsonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(Path.ChangeExtension(assetPath, "json"));
      if (jsonAsset != null) {
        ProcessSpritesheet(jsonAsset);
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

    private void ProcessSpritesheet(TextAsset jsonAsset) {
      SliceSpritesheet(jsonAsset);
    }

    private void SliceSpritesheet(TextAsset jsonAsset) {
      dynamic jsonContent = JsonConvert.DeserializeObject(jsonAsset.text);
      
      var textureImporter = (TextureImporter)assetImporter;
      if (textureImporter == null || textureImporter.textureType != TextureImporterType.Sprite) {
        return;
      }
      textureImporter.spriteImportMode = SpriteImportMode.Multiple;
      Vector2Int? pixelPivot = null;
      if (jsonContent.meta.slices != null) {
        foreach (var slice in jsonContent.meta.slices) {
          if (((string)slice.name).ToLower() == "pivot") {
            pixelPivot = new Vector2Int((int)slice.keys[0].bounds["x"], (int)slice.keys[0].bounds["y"]);
          }
        }        
      }

      var textureSize = jsonContent.meta.size;
      var metadataList = new List<SpriteMetaData>();
      foreach (var frame in jsonContent.frames) {
        var pivot = pixelPivot.HasValue ?
            new Vector2(
                pixelPivot.Value.x / (float)frame.frame.w,
                ((float)frame.frame.h - pixelPivot.Value.y) / (float)frame.frame.h) :
            // Default to dead center
            new Vector2(0.5f, 0.5f);
        // We have to adjust Y value, because Aseprite uses Y=0 as top, while Unity uses Y=0 as bottom
        var metadata = new SpriteMetaData {
            rect = new Rect(
                x: (float)frame.frame.x,
                y: (float)textureSize.h - (float)frame.frame.y - (float)frame.frame.h,
                width: (float)frame.frame.w,
                height: (float)frame.frame.h),
            name = (string)frame.filename,
            pivot = pivot,
            alignment = (int)SpriteAlignment.Custom, 
        };
        // Metadata.pivot seems to not really take, so set it on the sprite as a whole as well.
        textureImporter.spritePivot = metadata.pivot;
        metadataList.Add(metadata);
      }
      
      
      TextureImporterSettings settings = new TextureImporterSettings();
      textureImporter.ReadTextureSettings(settings);
      settings.spriteAlignment = (int)SpriteAlignment.Custom;
      textureImporter.SetTextureSettings(settings);
      textureImporter.spritesheet = metadataList.ToArray();
    }
    
    private static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths) {
      AddAllAnimationAssets(importedAssets.AsEnumerable().Concat(movedAssets.AsEnumerable()));
      // Just don't worry about deleting for now.
      //DeleteRemovedAnimationAssets(deletedAssets.AsEnumerable().Concat(movedFromAssetPaths.AsEnumerable()));
    }
    
    private static void AddAllAnimationAssets(IEnumerable<string> importedAssets) {
      var shouldRefresh = false;
      foreach (var importedAssetPath in importedAssets) {
        if (importedAssetPath.Contains("Sprites") && Path.GetExtension(importedAssetPath) == "json") {
          // If we're freshly importing the json, it's very likely at the same time as the png, so it won't have gotten
          // properly processed. To handle this, mark the associated png as dirty to trigger a reimport after the json
          // is already imported.
          var pngAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(Path.ChangeExtension(importedAssetPath, "png"));
          if (pngAsset == null) {
            continue;
          }
          
          EditorUtility.SetDirty(pngAsset);
          shouldRefresh = true;
        }
        
        if (AssetDatabase.GetMainAssetTypeAtPath(importedAssetPath) != typeof(Texture2D)) {
          continue;
        }
        
        var jsonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(Path.ChangeExtension(importedAssetPath, "json"));
        if (jsonAsset == null) {
          continue;
        }
        
        AddAnimationAsset(importedAssetPath, jsonAsset);
      }

      if (shouldRefresh) {
        AssetDatabase.Refresh();
      }
    }
    
    private static void AddAnimationAsset(string importedAssetPath, TextAsset jsonAsset) {
      var path = Path.ChangeExtension(importedAssetPath, "asset");
      var animatedSprite = AssetDatabase.LoadAssetAtPath<DirectionalAnimatedSprite>(path);
      if (animatedSprite == null) {
        animatedSprite = ScriptableObject.CreateInstance<DirectionalAnimatedSprite>();
        AssetDatabase.CreateAsset(animatedSprite, path);
      }

      var spriteList = new List<Sprite>();
      foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(importedAssetPath)) {
        // Anecdotally, the sprites always seem to be in order.
        if (asset is Sprite sprite) {
          spriteList.Add(sprite);
        }
      }
      animatedSprite.frames = spriteList.ToArray();
      animatedSprite.animations = new();
      
      dynamic jsonContent = JsonConvert.DeserializeObject(jsonAsset.text);
      foreach (var animation in jsonContent.meta.frameTags) {
        var animationName = ParseAnimationName((string)animation.name);
        if (!animatedSprite.animations.ContainsKey(animationName.name)) {
          animatedSprite.animations[animationName.name] = new DirectionalAnimation(animationName.name);
        }
        animatedSprite.animations[animationName.name].AddAnimation(
            animationName.direction,
            (int)animation.from,
            (int)animation.to,
            // Just use the duration of the first frame for the whole animation
            (float)jsonContent.frames[(int)animation.from].duration / 1000f);
      }
      
      EditorUtility.SetDirty(animatedSprite);
    }

    private static (string name, FacingDirection direction) ParseAnimationName(string jsonName) {
      var split = jsonName.Split('_');
      if (split.Length != 2) {
        Debug.LogWarning($"Unrecognized animation format: {jsonName}");
        return (jsonName, FacingDirection.NorthEast);
      }

      if (!ValidDirections.Contains(split[1])) {
        Debug.LogWarning($"Unrecognized direction in animation: {jsonName}");
        return (split[0].ToLower(), FacingDirection.NorthEast);
      }
      
      var direction = split.Last() switch {
          "nw" => FacingDirection.NorthWest,
          "ne" => FacingDirection.NorthEast,
          "sw" => FacingDirection.SouthWest,
          "se" => FacingDirection.SouthEast, 
          _ => FacingDirection.NorthEast,
      };

      return (split[0].ToLower(), direction);
    }
  }
}