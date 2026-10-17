#if ENABLE_LOCALIZATION
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Localization;

namespace Drafts.Database
{
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
}
#endif