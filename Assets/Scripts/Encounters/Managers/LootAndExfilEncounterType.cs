using System.Collections;
using Events;
using State.Encounter;
using UnityEngine;
using UnityEngine.UI;

namespace Encounters.Managers {
  public class LootAndExfilEncounterType : EncounterType {
    [SerializeField] private string exfilHintText;
    
    private Text _actionHint;
    private GameObject _exfilButton;

    private void Awake() {
      _actionHint = transform.Find("ActionHint").GetComponent<Text>();
      _actionHint.gameObject.SetActive(false);
      _exfilButton = transform.Find("ExfilButton").gameObject;
      _exfilButton.SetActive(false);
    }

    protected override void OnEnable() {
      Dispatch.Encounters.EncounterStart.RegisterListener(OnEncounterStart);
      Dispatch.Encounters.ItemCollected.RegisterListener(OnItemCollected);
    }

    protected override void OnDisable() {
      Dispatch.Encounters.EncounterStart.UnregisterListener(OnEncounterStart);
      Dispatch.Encounters.ItemCollected.UnregisterListener(OnItemCollected);
    }

    private void OnEncounterStart() {
      _actionHint.gameObject.SetActive(true);
      StartCoroutine(FlashObjective());
    }

    private IEnumerator FlashObjective() {
      for (int i = 0; i < 6; i++) {
        yield return new WaitForSeconds(0.5f);
        _actionHint.gameObject.SetActive(!_actionHint.gameObject.activeInHierarchy);
      }
    }

    private void OnItemCollected(CollectableInstance item) {
      if (!item.isPrimaryObjective) {
        return;
      }

      _actionHint.text = exfilHintText;
      _exfilButton.SetActive(true);
      StartCoroutine(FlashObjective());
    }
  }
}