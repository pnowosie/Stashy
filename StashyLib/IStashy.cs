using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StashyLib
{
    public interface IStashy<K>
    {
        void Save<T1>(T1 t1, K id);
        T1 Load<T1>(K id) where T1 : new();
        IEnumerable<T1> LoadAll<T1>() where T1 : new();
        void Delete<T1>(K id);
        K GetNewId<T1>() where T1 : new();
    }
}
