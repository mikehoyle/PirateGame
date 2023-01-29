using System;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Construction {
  public class BuildMenuController : MonoBehaviour {
    private VerticalLayoutGroup _container;
    
    private void Awake() {
      _container = GetComponentInChildren<VerticalLayoutGroup>();
      
    }
    
    
    public static BuildMenuController Get() {
      return GameObject.FindGameObjectWithTag(Tags.BuildMenu).GetComponent<BuildMenuController>();
    }
  }
}