using Common.Grid;
using RuntimeVars.Encounters;
using UnityEngine;

namespace Encounters {
  public class SelectionEffect : MonoBehaviour {
    private ParticleSystem _particles;

    private void Awake() {
      _particles = GetComponentInChildren<ParticleSystem>();
      _particles.gameObject.SetActive(false);
    }

    private void Update() {
      if (!CurrentSelection.Instance.TryGetUnit<EncounterActor>(out var unit)) {
        _particles.gameObject.SetActive(false);
        return;
      }

      transform.position = GridUtils.CellCenterWorld(unit.Position);
      _particles.gameObject.SetActive(true);
    }
  }
}