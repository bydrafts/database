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
        object Id { get; }
        int Index { get; }
    }

    public class DatabaseSO<T> : ScriptableObject where T : Object, IDatabaseItemInternal
    {
        [Header("Readonly")]
        [SerializeField] private T[] items;

        private Dictionary<object, T> _idMap;
        private Dictionary<object, T> IdMap => _idMap ??= Items.ToDictionary(t => t.Id, t => t);
        private readonly Dictionary<Type, IEnumerable> _categoryMap = new();

        public IEnumerable<T> Items => items.Skip(1);
        public T this[int index] => items[index];
        public T Get(int index) => index < 1 || index > items.Length ? null : items[index];
        public I Get<I>(int index) => Get(index) is I i ? i : default;
        public T Find(object id) => IdMap.GetValueOrDefault(id);
        public I Find<I>(object id) => Find(id) is I i ? i : default;
        public int IdToIndex(object id) => Find(id).Index;
        public object IndexToId(int index) => Get(index).Id;

        public IReadOnlyList<I> GetAll<I>()
        {
            if (!_categoryMap.TryGetValue(typeof(I), out var list))
                _categoryMap[typeof(I)] = list = Items.OfType<I>().ToArray();
            return (I[])list;
        }

        protected void __SetItems(IEnumerable<T> newItems)
        {
            items = newItems.Prepend(null).ToArray();
            for (var i = 1; i < items.Length; i++)
                items[i].SetIndex(i); // runtime index
            _idMap = null;
            _categoryMap.Clear();
        }

#if UNITY_EDITOR
        public List<T> FetchItemsFromAssets(string path = "Assets")
        {
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { path });
            var paths = guids.Select(UnityEditor.AssetDatabase.GUIDToAssetPath);
            var found = paths.Select(UnityEditor.AssetDatabase.LoadAssetAtPath<T>);
            __SetItems(found);

            var conflicts = new List<T>();
            var testIds = new Dictionary<object, T>();

            foreach (var item in Items)
                if (!testIds.TryAdd(item.Id, item))
                {
                    conflicts.Add(item);
                    Debug.LogError($"{item.name} conflicts with {testIds[item.Id].name}", item);
                }

            foreach (var item in Items)
                UnityEditor.EditorUtility.SetDirty(item);
            UnityEditor.EditorUtility.SetDirty(this);

            return conflicts;
        }
#endif
    }
}