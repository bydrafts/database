using System;
using UnityEngine;

namespace Drafts.Database
{
    public abstract class DatabaseItem : ScriptableObject, IDatabaseItemInternal
    {
        [SerializeField] protected Sprite icon;
        
        [NonSerialized] protected string _id;
        [NonSerialized] protected string _displayName;
        [NonSerialized] protected string _description;

        public virtual int Index { get; private set; }
        public virtual string Id => _id ??= $"{GetType().Name} {name}".ToLower();
        public virtual string DisplayName => _displayName ??= name;
        public virtual string Description => _description ??= Id + "-desc";
        public virtual Sprite Icon => icon;
        
        public virtual bool Discovered { get; set; }

        void IDatabaseItemInternal.SetIndex(int i) => Index = i;
    }
}