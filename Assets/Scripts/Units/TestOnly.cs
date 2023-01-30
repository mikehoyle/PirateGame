using System;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;

namespace Units {
  public class TestOnly : MonoBehaviour {
    private Sprite _sprite;
    
    private void Awake() {
      _sprite = GetComponent<Sprite>();
      var texture = GetComponent<Texture2D>();
      var spriteResolver = GetComponent<SpriteResolver>();
      var spriteAtlas = GetComponent<SpriteAtlas>();
      var spriteLibrary = GetComponent<SpriteLibrary>();
      
      
    }
  }
}