using System.Collections.Generic;
using RuntimeVars.Encounters.Events;
using Terrain;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter.HoverDetails {
  public class HoverDetails : MonoBehaviour {
    [SerializeField] private EncounterEvents encounterEvents;
    
    private List<Text> _detailRows;
    
    private void Awake() {
      _detailRows = new();
      foreach (Transform child in transform) {
        _detailRows.Add(child.GetComponent<Text>());
      }
      
      encounterEvents.mouseHover.RegisterListener(OnMouseHover);
      gameObject.SetActive(false);
    }

    private void OnDestroy() {
      encounterEvents.mouseHover.UnregisterListener(OnMouseHover);
    }

    private void OnMouseHover(Vector3Int hoveredTile) {
      var hoveredObject = SceneTerrain.GetTileOccupant(hoveredTile);
      
      if (hoveredObject == null
          || !hoveredObject.TryGetComponent<IDisplayDetailsProvider>(out var detailsProvider)) {
        gameObject.SetActive(false);
        return;
      }

      var displayDetails = detailsProvider.GetDisplayDetails();
      _detailRows[0].text = displayDetails.Name;
      _detailRows[0].gameObject.SetActive(true);
      
      for (int i = 1; i < _detailRows.Count; i++) {
        if (displayDetails.AdditionalDetails.Count >= i) {
          _detailRows[i].text = displayDetails.AdditionalDetails[i - 1];
          _detailRows[i].gameObject.SetActive(true); 
        } else {
          _detailRows[i].gameObject.SetActive(false); 
        }
      }
      gameObject.SetActive(true);
    }
  }
}