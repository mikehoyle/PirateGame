using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using State.Unit;
using StaticConfig.Sprites;
using UnityEditor;
using UnityEngine;

namespace EditorInternal {
  public class AsepriteSpritesheetPostprocessor : AssetPostprocessor {
    private static readonly List<string> ValidDirections = new() {
        "sw",
        "nw",
        "ne",
        "se",
    };
        
    private void OnPreprocessTexture() {
      var jsonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(Path.ChangeExtension(assetPath, "json"));
      if (jsonAsset != null) {
        ProcessSpritesheet(jsonAsset);
      }
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
      var metadataList = new List<SpriteMetaData>();
      foreach (var frame in jsonContent.frames) {
        var metadata = new SpriteMetaData {
            rect = new Rect(
                x: (float)frame.frame.x,
                y: (float)frame.frame.y,
                width: (float)frame.frame.w,
                height: (float)frame.frame.h),
            name = (string)frame.filename,
            // slice pivot is relative to bounding rectangle, so this is dead center.
            pivot = new Vector2(0.5f, 0.5f),
        };
        metadataList.Add(metadata);
      }
      
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
      foreach (var importedAssetPath in importedAssets) {
        if (AssetDatabase.GetMainAssetTypeAtPath(importedAssetPath) != typeof(Texture2D)) {
          return;
        }
        
        var jsonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(Path.ChangeExtension(importedAssetPath, "json"));
        if (jsonAsset == null) {
          return;
        }
        
        AddAnimationAsset(importedAssetPath, jsonAsset);
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