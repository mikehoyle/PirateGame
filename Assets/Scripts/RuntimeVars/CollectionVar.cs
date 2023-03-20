using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeVars {
  public class CollectionVar<T> : ScriptableObject, ICollection<T>, IList<T> {
    protected readonly List<T> Items;

    public CollectionVar() {
      Items = new();
    }

    public void Add(T unit) {
      Items.Add(unit);
    }
    public void Clear() {
      Items.Clear();
    }
    public bool Contains(T item) {
      return Items.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex) {
      Items.CopyTo(array, arrayIndex);
    }

    bool ICollection<T>.Remove(T item) {
      return Items.Remove(item);
    }

    public int Count => Items.Count;
    public bool IsReadOnly => false;

    public void Remove(T item) {
      Items.Remove(item);
    }
    
    public IEnumerator<T> GetEnumerator() {
      return Items.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
    public int IndexOf(T item) {
      return Items.IndexOf(item);
    }
    
    public void Insert(int index, T item) {
      Items.Insert(index, item);
    }
    public void RemoveAt(int index) {
      Items.RemoveAt(index);
    }
    
    public T this[int index] {
      get => Items[index];
      set => Items[index] = value;
    }
  }
}