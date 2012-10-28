namespace StashyLib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Hosting;
    using System.Xml;
    using System.Xml.Serialization;

    public class CachyFileStashy : IStashy<int>
    {
        private Dictionary<Type, Dictionary<int, Object>> cache = new Dictionary<Type, Dictionary<int, Object>>();

        public int GetNewId<T1>() where T1 : new()
        {
            int newId = 0;
            var all = this.LoadAll<T1>();

            if (all.Count() > 0)
            {
                newId = cache[typeof(T1)].Keys.Max() + 1;
            }
            return newId;
        }

        public void Save<T1>(T1 t1, int id)
        {
            var xmlFileName = Path.Combine(GetObjectPath<T1>(), id.ToString());
            EnsurePathExists(Path.GetDirectoryName(xmlFileName));
            var xsZer = new XmlSerializer(typeof(T1));
            using (var writer = new StreamWriter(xmlFileName))
            {
                xsZer.Serialize(writer, t1);
            }
            SafeCacheAdd<T1>(id, t1);
        }

        public T1 Load<T1>(int id) where T1 : new()
        {
            var xmlFileName = Path.Combine(GetObjectPath<T1>(), id.ToString());
            if (cache.ContainsKey(typeof(T1)) && cache[typeof(T1)].ContainsKey(id))
            {
                return (T1)cache[typeof(T1)][id];
            }
            EnsurePathExists(Path.GetDirectoryName(xmlFileName));
            var t1 = Load<T1>(xmlFileName);

            SafeCacheAdd<T1>(id, t1);
            return t1;
        }

        private void SafeCacheAdd<T1>(int id, T1 t1)
        {
            if (!cache.ContainsKey(typeof(T1)))
            {
                cache[typeof(T1)] = new Dictionary<int, object>();
            }
            cache[typeof(T1)][id] = t1;
        }

        public IEnumerable<T1> LoadAll<T1>() where T1 : new()
        {
            if (cache.ContainsKey(typeof(T1)))
            {
                return cache[typeof(T1)].Select(p => (T1)p.Value);
            }

            var xmlFilePath = GetObjectPath<T1>();
            EnsurePathExists(Path.GetDirectoryName(xmlFilePath));
            var all = new List<T1>();

            var dic = new Dictionary<int, Object>();

            foreach (var xmlFileName in Directory.EnumerateFiles(xmlFilePath))
            {
                var item = Load<T1>(xmlFileName);
                dic.Add(int.Parse(Path.GetFileNameWithoutExtension(xmlFileName)), item);
                all.Add(item);
            }
            cache[typeof(T1)] = dic;
            return all;
        }

        public void Delete<T1>(int id)
        {
            var xmlFileName = Path.Combine(GetObjectPath<T1>(), id.ToString());
            EnsurePathExists(Path.GetDirectoryName(xmlFileName));
            File.Delete(xmlFileName);
            cache[typeof(T1)].Remove(id);
        }

        private T1 Load<T1>(string xmlFileName) where T1 : new()
        {
            var xsZer = new XmlSerializer(typeof(T1));
            if (!File.Exists(xmlFileName))
            {
                throw new FileNotFoundException("File cannot be loaded for serialization", xmlFileName);
            }

            using (var xreader = new XmlTextReader(File.OpenRead(xmlFileName)))
            {
                T1 result = (T1)xsZer.Deserialize(xreader);
                return result;
            }
        }

        private string GetObjectPath<T1>()
        {
            var path = "~/App_Data/" + typeof(T1) + "/";
            string fullyQualifiedPath = VirtualPathUtility.Combine(VirtualPathUtility.AppendTrailingSlash(HttpRuntime.AppDomainAppVirtualPath), path);
            var objectPath = HostingEnvironment.MapPath(fullyQualifiedPath);
            return objectPath;
        }

        private void EnsurePathExists(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new DirectoryNotFoundException("Specified path was empty");
            }

            if (!Directory.Exists(path))
            {
                EnsurePathExists(Directory.GetParent(path).FullName);
                Directory.CreateDirectory(path);
            }
        }
    }

    //public interface IStashy<K>
    //{
    //    void Save<T1>(T1 t1, K id);
    //    T1 Load<T1>(K id) where T1 : new();
    //    IEnumerable<T1> LoadAll<T1>() where T1 : new();
    //    void Delete<T1>(K id);
    //    K GetNewId<T1>() where T1 : new();
    //}
}