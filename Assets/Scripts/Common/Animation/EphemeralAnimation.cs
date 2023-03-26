using System.Collections;
using Common.Grid;
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
        float targetHeight,
        DirectionalAnimatedSprite sprite,
        string animationName,
        FacingDirection direction = FacingDirection.SouthEast) {
      var position = GridUtils.CellBaseWorld(target);
      position.y += targetHeight;
      transform.position = position;
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