using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEditor;
using UnityEngine;

// Avoiding Editor namespace because it messes with a lot of things.
namespace EditorInternal {
  public class UnitSpritePostprocessor : AssetPostprocessor {
    private const string UnitAssetPath = "Sprites/Units/Composite";
    private const string SpritesPath = "Sprites/";
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
  }
}