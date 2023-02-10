using System;
using UnityEditor.Rendering;
using UnityEngine;

namespace Encounters.SkillTest {
  public class SkillTestArm : MonoBehaviour {
    private const int MinX = -128;
    private const int MaxX = 128;
    private const float TargetPosition = 0.75f;

    private float _startTime;
    private float _duration;
    private bool _isRunning;
    
    // 0 - 1
    public float RelativePosition { get; set; }

    private void Awake() {
      RelativePosition = 0f;
      _isRunning = false;
    }

    private void Update() {
      UpdateRelativePosition();
      
      var localPosition = transform.localPosition;
      localPosition.x = ((MaxX - MinX) * RelativePosition) + MinX;
      transform.localPosition = localPosition;
    }
    private void UpdateRelativePosition() {
      if (!_isRunning) {
        return;
      }
      
      var t = (Time.time - _startTime) / _duration;
      RelativePosition = Mathf.SmoothStep(0, 1, t);
    }

    public void Run(float duration) {
      RelativePosition = 0;
      _startTime = Time.time;
      _duration = duration;
      _isRunning = true;
    }

    public float Stop() {
      _isRunning = false;
      var distanceFromTarget = Math.Max(0f, Math.Abs(TargetPosition - RelativePosition)); 
      return 1 - distanceFromTarget;
    }
  }
}