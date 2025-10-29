using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace Drafts.Database
{
    public interface IDatabaseItemInternal : IDatabaseItem
    {
        void SetIndex(int index);
    }

    public interface IDatabaseItem
    {
        string Id { get; }
        int Index { get; }
    }

    public class Database<T> : ScriptableObject where T : Object, IDatabaseItemInternal
    {
        [SerializeField] private T[] items;

        private Dictionary<string, T> _map;
        private Dictionary<string, T> Map => _map ??= Items.ToDictionary(t => t.Id, t => t);
        private readonly Dictionary<Type, IEnumerable> _categoryMap = new();

        public IEnumerable<T> Items => items.Skip(1);
        public T this[int index] => items[index];
        public T Get(int index) => index < 1 || index > items.Length ? null : items[index];
        public I Get<I>(int index) => Get(index) is I i ? i : default;
        public T Find(string id) => Map.GetValueOrDefault(id);
        public I Find<I>(string id) => Find(id) is I i ? i : default;
        public int IdToIndex(string id) => Find(id).Index;
        public string IndexToId(int index) => Get(index).Id;

        public IReadOnlyList<I> GetAll<I>()
        {
            if (!_categoryMap.TryGetValue(typeof(I), out var list))
                _categoryMap[typeof(I)] = list = Items.OfType<I>().ToArray();
            return (I[])list;
        }

        protected void __SetItems(IEnumerable<T> newItems)
        {
            items = newItems.Prepend(null).ToArray();
            var index = 1;
            foreach (var item in items.Skip(1))
                item.SetIndex(index++);
        }

#if UNITY_EDITOR
        protected void FetchItemsFromAssets(string path = "Assets")
        {
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { path });
            var paths = guids.Select(UnityEditor.AssetDatabase.GUIDToAssetPath);
            var found = paths.Select(UnityEditor.AssetDatabase.LoadAssetAtPath<T>);
            __SetItems(found.OrderBy(i => i.Id));

            foreach (var item in Items)
                UnityEditor.EditorUtility.SetDirty(item);
        }
#endif
    }
}