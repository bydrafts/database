using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Drafts.Database
{
    public class FlexibleDatabaseSO<T> : ScriptableObject, IDatabase where T : class, IDatabaseItemInternal
    {
        [Header("Readonly")]
        [SerializeField] private Object[] items;

        private Dictionary<uint, T> _idMap;
        private Dictionary<uint, T> IdMap => _idMap ??= Items.ToDictionary(t => t.Id, t => t);
        private readonly Dictionary<Type, IEnumerable> _categoryMap = new();

        IEnumerable IDatabase.Items => items.Skip(1);
        public IEnumerable<T> Items => items.OfType<T>().Skip(1);
        public T this[int index] => items[index] as T;
        public T Get(int index) => index < 1 || index > items.Length ? null : items[index] as T;
        public I Get<I>(int index) => Get(index) is I i ? i : default;
        public T Find(uint id) => IdMap.GetValueOrDefault(id);
        public I Find<I>(uint id) => Find(id) is I i ? i : default;
        public int IdToIndex(uint id) => Find(id).Index;
        public uint IndexToId(int index) => Get(index).Id;

        public IReadOnlyList<I> GetAll<I>()
        {
            if (!_categoryMap.TryGetValue(typeof(I), out var list))
                _categoryMap[typeof(I)] = list = Items.OfType<I>().ToArray();
            return (I[])list;
        }

        protected void __SetItems(IEnumerable<Object> newItems)
        {
            items = newItems.Prepend(null).ToArray();
            for (var i = 1; i < items.Length; i++)
                (items[i] as T)!.SetIndex(i); // runtime index
            _idMap = null;
            _categoryMap.Clear();
        }

        public void UpdateIndexes()
        {
            for (var i = 1; i < items.Length; i++)
                (items[i] as T)!.SetIndex(i);
        }

#if UNITY_EDITOR
        public List<T> FetchItemsFromAssets<I>(string path = "Assets") where I : Object
        {
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(I).Name}", new[] { path });
            var paths = guids.Select(UnityEditor.AssetDatabase.GUIDToAssetPath);
            var found = paths.Select(UnityEditor.AssetDatabase.LoadAssetAtPath<I>);
            __SetItems(found.OfType<T>().Cast<Object>());

            var conflicts = new List<T>();
            var testIds = new Dictionary<object, T>();

            foreach (var item in Items)
                if (!testIds.TryAdd(item.Id, item))
                {
                    conflicts.Add(item);
                    var a = item as Object;
                    var b = testIds[item.Id] as Object;
                    Debug.LogError($"{a!.name} conflicts with {b!.name}", a);
                }

            foreach (var item in items.Skip(1))
                UnityEditor.EditorUtility.SetDirty(item);
            UnityEditor.EditorUtility.SetDirty(this);

            return conflicts;
        }
#endif
    }
}