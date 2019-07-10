using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CombineAssets
{
    public class DirElement : Element
    {
        public DirElement(string path, string root)
            : base(path)
        {
            Name = new DirectoryInfo(path).Name;
            Location = path.Substring(new DirectoryInfo(root).FullName.Length);

            if (Location.StartsWith("\\"))
                Location = Location.Substring(1);

            if (string.IsNullOrWhiteSpace(Location))
                Location = @"\";

            Location = Location.Replace('\\', '/');
            PopulateChildren(path, root);
        }

        private void PopulateChildren(string path, string root)
        {
            _children.Clear();

            List<FileElement> files = Directory
                .EnumerateFiles(path)
                .Where(Utils.IsImage)
                .Select(filePath => new FileElement(filePath))
                .ToList();

            files.Sort(new FileComparer());

            _children.AddRange(files);

            IEnumerable<DirElement> dirs = Directory
                .EnumerateDirectories(path)
                .OrderBy(f => f)
                .Select(folder => new DirElement(folder, root));

            _children.AddRange(dirs);
        }

        private class FileComparer : IComparer<FileElement>
        {
            public int Compare(FileElement x, FileElement y)
            {
                int nameCompare = x.Name.CompareTo(y.Name);

                return nameCompare != 0
                    ? nameCompare
                    : x.Index.GetValueOrDefault().CompareTo(y.Index.GetValueOrDefault());
            }
        }
    }
}
