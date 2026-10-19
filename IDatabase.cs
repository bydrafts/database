using System.Collections;

namespace Drafts.Database
{
    public interface IDatabase
    {
        public IEnumerable Items { get; }
        public T Get<T>(int index);
        public T Find<T>(uint id);
    }
}