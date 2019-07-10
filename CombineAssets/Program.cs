using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CombineAssets
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("USAGE:");
                Console.WriteLine("       CombineAssets path [rootPath]");
                return;
            }

            string dir = args[0];
            string rootPath = args.Length > 1 ? args[1] : "";
            WriteAssets(dir, rootPath);
        }

        private static string CalcIndent(int level) => new String(' ', level * 2);

        private static string GetFileText(FileElement file, int level)
        {
            string indent = CalcIndent(level);
            string extension = Path.GetExtension(file.ElementPath).Substring(1);
            string data = Convert.ToBase64String(File.ReadAllBytes(file.ElementPath));
            return $@"{indent}""data:image/{extension};base64,{data}""";
        }

        private static string GetDirText(DirElement dir, int level)
        {
            string indent = CalcIndent(level++);
            string indent2 = CalcIndent(level++);
            var sb = new StringBuilder();
            sb.AppendLine($"{indent}{{");

            IEnumerable<IGrouping<string, FileElement>> groups = dir.Children
                .OfType<FileElement>()
                .Where(f => Utils.IsImage(f.ElementPath))
                .GroupBy(c => c.Name);

            bool first = true;

            foreach (IGrouping<string, FileElement> group in groups)
            {
                if (!first)
                    sb.AppendLine(",");

                sb.AppendLine($@"{indent2}""{group.Key.ToLower()}"": [");
                bool firstChild = true;

                foreach (FileElement file in group)
                {
                    if (!firstChild)
                        sb.AppendLine(",");

                    sb.Append($"{GetFileText(file, level)}");
                    firstChild = false;
                }

                sb.AppendLine();
                sb.Append($"{indent2}]");
                first = false;
            }

            sb.AppendLine();
            sb.AppendLine($"{indent}}}");
            return sb.ToString();
        }

        private static IEnumerable<DirElement> GetDirAndSubDir(DirElement dir)
            => new[] { dir }.Concat(dir.Children.OfType<DirElement>().SelectMany(GetDirAndSubDir));

        private static void WriteDirs(DirElement dir)
        {
            var filename = "images.json";
            var filePath = Path.Combine(dir.ElementPath, filename);

            using (var file = new StreamWriter(filePath, true))
            {
                int level = 0;
                string indent = CalcIndent(level++);
                string indent2 = CalcIndent(level);
                file.WriteLine($"{indent}{{");
                bool first = true;

                IEnumerable<DirElement> subs = GetDirAndSubDir(dir)
                    .Where(d => d.Children.OfType<FileElement>().Any(f => Utils.IsImage(f.ElementPath)));

                foreach (DirElement sub in subs)
                {
                    if (!first)
                        file.WriteLine(",");

                    file.Write($@"{indent2}""{sub.Location.ToLower()}"": ");
                    file.Write($"{GetDirText(sub, level).Trim()}");
                    first = false;
                }

                file.WriteLine();
                file.WriteLine($"{indent}}}");
            }
        }

        private static void WriteAssets(string dir, string rootPath = "")
        {
            if (string.IsNullOrWhiteSpace(rootPath))
            {
                var folder = new DirectoryInfo(dir);

                while (folder.Name.ToLower() != "wwwroot" && folder.Parent != null)
                {
                    folder = folder.Parent;
                }

                rootPath = folder.FullName;
            }

            var root = new DirElement(dir, rootPath);
            WriteDirs(root);
        }
    }
}
