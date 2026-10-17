using UnityEngine;

#if ENABLE_LOCALIZATION
using UnityEngine.Localization;
#endif

namespace Drafts.Database
{
    public abstract class DatabaseItem : ScriptableObject, IDatabaseItemInternal
    {
        public static UIntGenerator IdGenerator { get; } = new();

        [SerializeField] protected uint id;

#if ENABLE_LOCALIZATION
        [SerializeField] protected LocalizedString displayName;
        [SerializeField] protected LocalizedString description;
        public virtual string DisplayName => displayName.Localize(this);
        public virtual string Description => description.Localize(this);
#else
        [SerializeField] protected string displayName;
        [SerializeField] protected string description;
        public virtual string DisplayName => displayName;
        public virtual string Description => description;
#endif
        [SerializeField] protected Sprite icon;

        public virtual int Index { get; private set; }
        public virtual uint Id => id;
        public virtual Sprite Icon => icon;
        public virtual bool Discovered { get; set; }

        void IDatabaseItemInternal.SetIndex(int i) => Index = i;
        
        [ContextMenu("Reroll Id")]
        private void RerollId() => id = IdGenerator.Generate();
    }
}