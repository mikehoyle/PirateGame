using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace StaticConfig {
  [CreateAssetMenu(fileName = "CompositeUnitSprite", menuName = "ScriptableObjects/Composite Unit Sprite", order = 0)]
  public class CompositeUnitSpriteScriptableObject : ScriptableObject {
    // Currently used for directory name and gameobject name
    public static readonly Dictionary<Layer, string> LayerToString = new() {
        [Layer.Hair1] = "hair1",
        [Layer.Head] = "head",
        [Layer.Torso] = "torso",
        [Layer.Sword] = "sword",
        [Layer.Legs] = "legs",
        [Layer.BackArm] = "back_arm",
        [Layer.Hair2] = "hair2",
    };

    [Serializable]
    public sealed class ComponentDictionary : SerializedDictionary<string, CompositeSpriteComponentScriptableObject> { }

    public enum Layer {
      None = -1,
      Hair1 = 0,
      Head = 1,
      Torso = 2,
      Sword = 3,
      Legs = 4,
      BackArm = 5,
      Hair2 = 6,
    }

    // Where index is keyed on Layer enum
    public ComponentDictionary[] layerOptions;

    public void EnsureFieldsInitialized() {
      layerOptions ??= new ComponentDictionary[7];

      if (layerOptions.Length < 7) {
        Array.Resize(ref layerOptions, 7);
      }

      for (int i = 0; i < layerOptions.Length; i++) {
        layerOptions[i] ??= new();
      }
    }

    public ComponentDictionary GetOptions(Layer layer) {
      return layerOptions[(int)layer];
    }
  }
}