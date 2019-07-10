using System.IO;
using System.Linq;

namespace CombineAssets
{
    public static class Utils
    {
        private static string[] _imageExtensions = new[] { ".png" };
        private static string[] _audioExtensions = new[] { ".wav", ".mp3" };

        public static bool IsImage(string filename) => _imageExtensions.Contains(Path.GetExtension(filename));
        public static bool IsAudio(string filename) => _audioExtensions.Contains(Path.GetExtension(filename));
        public static bool IsImageOrAudio(string filename) => IsImage(filename) || IsAudio(filename);
    }
}
