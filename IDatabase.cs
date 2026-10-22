using System.Collections;
using System.Collections.Generic;

namespace Drafts.Database
{
    public interface IDatabase
    {
        public IEnumerable Items { get; }
        public T Get<T>(int index);
        public T Find<T>(uint id);
        public IReadOnlyList<T> GetAll<T>();
    }
}