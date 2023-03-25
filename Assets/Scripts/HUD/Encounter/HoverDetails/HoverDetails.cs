using System.Collections.Generic;
using Common;
using Events;
using Optional;
using Terrain;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter.HoverDetails {
  public class HoverDetails : MonoBehaviour {

    private Option<IDisplayDetailsProvider> _currentDetailsProvider;
    private List<Text> _detailRows;
    
    private void Awake() {
      _detailRows = new();
      foreach (Transform child in transform) {
        _detailRows.Add(child.GetComponent<Text>());
      }
      _currentDetailsProvider = Option.None<IDisplayDetailsProvider>();
      Dispatch.Encounters.MouseHover.RegisterListener(OnMouseHover);
      gameObject.SetActive(false);
    }

    private void OnDestroy() {
      Dispatch.Encounters.MouseHover.UnregisterListener(OnMouseHover);
    }

    private void Update() {
      if (!_currentDetailsProvider.TryGet(out var detailsProvider)) {
        gameObject.SetActive(false);
        return;
      }
      
      var displayDetails = detailsProvider.GetDisplayDetails();
      _detailRows[0].text = displayDetails.Name;
      _detailRows[0].gameObject.SetActive(true);
      
      for (int i = 1; i < _detailRows.Count; i++) {
        if (displayDetails.AdditionalDetails.Count >= i) {
          _detailRows[i].gameObject.SetActive(true);
          _detailRows[i].text = displayDetails.AdditionalDetails[i - 1];
        } else {
          _detailRows[i].gameObject.SetActive(false); 
        }
      }
    }

    private void OnMouseHover(Vector3Int hoveredTile) {
      if (!SceneTerrain.TryGetComponentAtTile<IDisplayDetailsProvider>(hoveredTile, out var detailsProvider)) {
        _currentDetailsProvider = Option.None<IDisplayDetailsProvider>();
        gameObject.SetActive(false);
        return;
      }

      _currentDetailsProvider = Option.Some(detailsProvider);
      gameObject.SetActive(true);
    }
  }
}