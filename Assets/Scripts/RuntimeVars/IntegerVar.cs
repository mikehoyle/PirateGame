﻿using UnityEngine;

namespace RuntimeVars {
  [CreateAssetMenu(menuName = "Vars/IntegerVar")]
  public class IntegerVar : ScriptableObject {
    [SerializeField] private int value;

    public int Value {
      get => value;
      set => this.value = value;
    }
  }
}