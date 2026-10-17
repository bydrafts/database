using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Drafts.Database
{
    public class RuntimeDatabase<T> where T : IDatabaseItemInternal
    {
        private List<T> _items;
        private Dictionary<uint, T> _idMap;
        private Dictionary<uint, T> IdMap => _idMap ??= Items.ToDictionary(t => t.Id, t => t);
        private readonly Dictionary<Type, IEnumerable> _categoryMap = new();

        public IEnumerable<T> Items => _items.Skip(1);
        public T this[int index] => _items[index];
        public T Get(int index) => index < 1 || index > _items.Count ? default : _items[index];
        public T Find(uint id) => IdMap.GetValueOrDefault(id);
        public T1 Get<T1>(int index) => Get(index) is T1 i ? i : default;
        public T1 Find<T1>(uint id) => Find(id) is T1 i ? i : default;
        public int IdToIndex(uint id) => Find(id).Index;
        public uint IndexToId(int index) => Get(index).Id;

        public RuntimeDatabase(IEnumerable<T> items)
        {
            _items = items.Prepend(default).ToList();
            for (var i = 1; i < _items.Count; i++)
                _items[i].SetIndex(i); // runtime index
            _idMap = null;
            _categoryMap.Clear();
        }

        public IReadOnlyList<T1> GetAll<T1>()
        {
            if (!_categoryMap.TryGetValue(typeof(T1), out var list))
                _categoryMap[typeof(T1)] = list = Items.OfType<T1>().ToArray();
            return (IReadOnlyList<T1>)list;
        }

        public void __AddItem(T item)
        {
            if (!_idMap.TryAdd(item.Id, item))
            {
                Debug.LogError($"Duplicate id: {item}", item as Object);
                return;
            }

            item.SetIndex(_items.Count);
            _items.Add(item);
            _categoryMap.Clear();
        }
    }
}