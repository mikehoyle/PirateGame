using Common.Animation;
using Common.Grid;
using Encounters.Effects;
using Encounters.Grid;
using Events;
using Terrain;
using UnityEngine;

namespace Encounters.Obstacles {
  public class PlacedObject : MonoBehaviour, IPlacedOnGrid, IDirectionalAnimatable {
    [SerializeField] private bool blocksAllMovement;
    [SerializeField] private bool claimsTile;
    [SerializeField] private bool blocksLineOfSight;
    // -1 for infinite
    [SerializeField] private int numEffectApplicationsBeforeDeath = -1;
    [SerializeReference, SerializeReferenceButton]
    private StatusEffect onWalkOverEffect;
    
    private SceneTerrain _terrain;
    private GridIndicators _indicators;
    private EncounterActor _actor;
    private ParticleSystem _particles;

    public Vector3Int Position { get; private set; }
    public bool BlocksAllMovement => blocksAllMovement;
    public bool ClaimsTile => claimsTile;
    public bool BlocksLineOfSight => blocksLineOfSight;
    public FacingDirection FacingDirection => FacingDirection.SouthEast;
    public string AnimationState => "idle";
#pragma warning disable CS0067
    public event IDirectionalAnimatable.RequestOneOffAnimation OneOffAnimation;
#pragma warning restore CS0067

    private void Awake() {
      _terrain = SceneTerrain.Get();
      _indicators = GridIndicators.Get();
      _particles = GetComponentInChildren<ParticleSystem>();
    }

    public void Init(EncounterActor actor, Vector3Int position) {
      Position = position;
      transform.position = GridUtils.CellAnchorWorld(position);
      // TODO(P0): Ensure this doesn't break if the actor dies.
      _actor = actor;
      if (SceneTerrain.TryGetComponentAtTile<EncounterActor>(position, out var victim)) {
        HandleWalkOver(victim);
      }
    }

    public void OnEnable() {
      Dispatch.Encounters.UnitAddedMidEncounter.RegisterListener(OnUnitAdded);
    }

    public void OnDisable() {
      Dispatch.Encounters.UnitAddedMidEncounter.UnregisterListener(OnUnitAdded);
    }
    
    private void OnUnitAdded(EncounterActor actor) {
      if (actor.Position == Position) {
        HandleWalkOver(actor);
      }
    }

    public virtual void HandleWalkOver(EncounterActor victim) {
      if (onWalkOverEffect != null) {
        var effect = onWalkOverEffect.ApplyTo(victim);
        effect.PreCalculateEffect(_actor, 1); 
        _particles.Play();
      }

      if (numEffectApplicationsBeforeDeath > -1) {
        numEffectApplicationsBeforeDeath -= 1;
        if (numEffectApplicationsBeforeDeath <= 0) {
          // TODO
          Destroy(gameObject);
        }
      }
    }
  }
}