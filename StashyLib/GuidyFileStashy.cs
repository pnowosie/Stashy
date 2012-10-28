namespace StashyLib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Hosting;
    using System.Xml;
    using System.Xml.Serialization;

    public partial class GuidyFileStashy : IStashy<Guid>
    {
        public Guid GetNewId<T1>() where T1 : new()
        {
            return Guid.NewGuid();
        }

        public void Save<T1>(T1 t1, Guid gId)
        {
            var xmlFileName = Path.Combine(GetObjectPath<T1>(), gId.ToString());
            EnsurePathExists(Path.GetDirectoryName(xmlFileName));
            var xsZer = new XmlSerializer(typeof(T1));
            using (var writer = new StreamWriter(xmlFileName))
            {
                xsZer.Serialize(writer, t1);
            }
        }

        public T1 Load<T1>(Guid gId) where T1 : new()
        {
            var xmlFileName = Path.Combine(GetObjectPath<T1>(), gId.ToString());
            EnsurePathExists(Path.GetDirectoryName(xmlFileName));
            return Load<T1>(xmlFileName);
        }

        public IEnumerable<T1> LoadAll<T1>() where T1 : new()
        {
            var xmlFilePath = GetObjectPath<T1>();
            EnsurePathExists(Path.GetDirectoryName(xmlFilePath));
            var all = new List<T1>();

            foreach (var xmlFileName in Directory.EnumerateFiles(xmlFilePath))
            {
                all.Add(Load<T1>(xmlFileName));
            }

            return all;
        }

        public void Delete<T1>(Guid gId)
        {
            var xmlFileName = Path.Combine(GetObjectPath<T1>(), gId.ToString());
            EnsurePathExists(Path.GetDirectoryName(xmlFileName));
            File.Delete(xmlFileName);
        }

        public static T1 Load<T1>(string xmlFileName) where T1 : new()
        {
            if (!File.Exists(xmlFileName))
            {
                throw new FileNotFoundException("File cannot be loaded for serialization", xmlFileName);
            }
            var xsZer = new XmlSerializer(typeof(T1));
            using (var xreader = new XmlTextReader(File.OpenRead(xmlFileName)))
            {
                T1 result = (T1)xsZer.Deserialize(xreader);
                return result;
            }
        }

        private static string GetObjectPath<T1>()
        {
            var path = "~/App_Data/" + typeof(T1) + "/";
            var fullyQualifiedPath = VirtualPathUtility.Combine(VirtualPathUtility.AppendTrailingSlash(HttpRuntime.AppDomainAppVirtualPath), path);
            var objectPath = HostingEnvironment.MapPath(fullyQualifiedPath);
            return objectPath;
        }

        private static void EnsurePathExists(string path)
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
