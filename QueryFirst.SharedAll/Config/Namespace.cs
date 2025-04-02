using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace QueryFirst
{
    public class Namespace
    {
        public static string SniffProjectNamespace(string fileOrFolderPath)
        {
            string folderToSearch;
            if (File.Exists(fileOrFolderPath))
                folderToSearch = Path.GetDirectoryName(fileOrFolderPath);
            else if (Directory.Exists(fileOrFolderPath))
                folderToSearch = fileOrFolderPath;
            else
                throw new ArgumentException(nameof(fileOrFolderPath));
            return RecurseFolders(folderToSearch);

        }
        private static string RecurseFolders(string folderToSearch)
        {
            var csproj = Directory.EnumerateFiles(folderToSearch)
                .Where(f => f.ToLower().EndsWith(".csproj"))
                .FirstOrDefault();
            if (csproj != null)
            {
                var projectDoc = new XmlDocument();
                projectDoc.Load(csproj);
                var rootNamespace = projectDoc.GetElementsByTagName("RootNamespace").Item(0)?.InnerText;
                return rootNamespace ?? Path.GetFileNameWithoutExtension(csproj);
            }
            else
            {
                var parent = Directory.GetParent(folderToSearch);
                if (parent == null) return null;
                else return RecurseFolders(parent.FullName);
            }
        }
    }
}
