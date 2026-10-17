using System.Runtime.CompilerServices;
using UnityEngine;

#if ENABLE_LOCALIZATION
using UnityEngine.Localization;
#endif

namespace Drafts.Database
{
    public abstract class DatabaseItem : ScriptableObject, IDatabaseItemInternal
    {
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
    }

#if ENABLE_LOCALIZATION
    public static class ObjectLocalizationHelper
    {
        public static string Localize(this LocalizedString s, Object ctx, [CallerMemberName] string m = null)
        {
            try
            {
                return s.GetLocalizedString();
            }
            catch
            {
                var n = s.TableReference.TableCollectionName;
                var k = s.TableEntryReference.Key;
                Debug.LogWarning($"Localization missing: {n} - {k}", ctx);
                return $"{ctx?.name} {m}";
            }
        }
    }
#endif
}