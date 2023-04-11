using System;
using System.Collections;
using Common.Grid;
using Encounters;
using FMODUnity;
using MilkShake;
using UnityEngine;

namespace Units {
  public class UnitDropIn : MonoBehaviour {
    [SerializeField] private int dropInHeight = 6;
    [SerializeField] private float dropInGravity = 15;
    [SerializeField] private ShakePreset dropInShake;
    [SerializeField] private EventReference dropInImpactSound;
    
    private IPlacedOnGrid _unit;

    private void Awake() {
      _unit = GetComponent<IPlacedOnGrid>();
    }

    public IEnumerator DropIn(Action onCompleteCallback) {
      var currentPosition = GridUtils.CellCenterWorld(_unit.Position);
      var destinationY = currentPosition.y;
      currentPosition.y += dropInHeight;
      var currentVelocityY = 3f;

      while (currentPosition.y > destinationY) {
        currentVelocityY += (dropInGravity * Time.deltaTime);
        currentPosition.y -= (currentVelocityY * Time.deltaTime);
        transform.position = currentPosition;
        yield return null;
      }

      transform.position = GridUtils.CellCenterWorld(_unit.Position);
      Shaker.ShakeAll(dropInShake);
      RuntimeManager.PlayOneShot(dropInImpactSound);
      onCompleteCallback();
    }
  }
}