using System.IO;
using System.Text.RegularExpressions;

namespace CombineAssets
{
    public class FileElement : Element
    {
        public FileElement(string path)
            : base(path)
        {
            string fullName = Path.GetFileNameWithoutExtension(path);
            Location = Path.GetDirectoryName(path);

            var groups = Regex.Match(fullName, @"^(.+?)(\d*)$").Groups;
            Name = groups[1].Value;

            if (!string.IsNullOrWhiteSpace(groups[2].Value))
                Index = int.Parse(groups[2].Value);
        }

        public int? Index { get; set; }
    }
}
