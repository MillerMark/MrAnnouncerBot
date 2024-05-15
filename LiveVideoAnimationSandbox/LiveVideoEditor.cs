using DHDM;

namespace LiveVideoAnimationSandbox
{
    public class LiveVideoEditor : ILiveVideoEditor
    {
        public LiveVideoEditor()
        {

        }
        public void ShowImageFront(string? fileName) { }
        public void ShowImageBack(string? fileName) { }
        public void PreloadImageFront(string? fileName, int startIndex, int stopIndex, int digitCount) { }
        public void PreloadImageBack(string? fileName, int startIndex, int stopIndex, int digitCount) { }
    }
}
