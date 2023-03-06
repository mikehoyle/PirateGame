using System.Collections;
using State.Unit;
using StaticConfig.Sprites;
using Terrain;
using UnityEngine;

namespace Common.Animation {
  public class EphemeralAnimation : MonoBehaviour {
    private SpriteRenderer _spriteRenderer;

    private void Awake() {
      _spriteRenderer = GetComponent<SpriteRenderer>();
      _spriteRenderer.enabled = false;
    }

    public Coroutine PlayThenDie(
        Vector3Int target,
        DirectionalAnimatedSprite sprite,
        string animationName,
        FacingDirection direction = FacingDirection.SouthEast) {
      transform.position = SceneTerrain.CellBaseWorldStatic(target);
      return StartCoroutine(PlayThenDieInternal(sprite, sprite.GetAnimation(animationName, direction)));
    }

    private IEnumerator PlayThenDieInternal(
        DirectionalAnimatedSprite sprite, DirectionalAnimation.Animation? animationNullable) {
      if (animationNullable == null) {
        Debug.LogWarning("Cannot play ephemeral animation for unknown animation");
        yield break;
      }

      var animation = animationNullable.Value;
      var currentFrame = animation.startFrame;
      _spriteRenderer.enabled = true;
      while (currentFrame <= animation.endFrame) {
        _spriteRenderer.sprite = sprite.frames[currentFrame];
        yield return new WaitForSeconds(animation.frameDurationSecs);
        currentFrame++;
      }
      Destroy(gameObject);
    }
  }
}