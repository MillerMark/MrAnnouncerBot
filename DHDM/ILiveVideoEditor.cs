using System;
using System.Linq;

namespace DHDM
{
    public interface ILiveVideoEditor
    {
        void ShowImageFront(string fileName);
        void ShowImageBack(string fileName);
        void PreloadImageFront(string fileName, int startIndex, int stopIndex, int digitCount);
        void PreloadImageBack(string fileName, int startIndex, int stopIndex, int digitCount);
    }
}
