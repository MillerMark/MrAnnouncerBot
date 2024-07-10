#nullable enable
using Imaging;
using System;
using System.Linq;

#if LiveVideoAnimationSandbox
#endif

namespace DHDM
{
    public interface ICanEditFrames
    {
        void UpdateEverythingOnTimeline();
        void MoveObsSourcesIntoPosition();
        void UpdateValuePreviews(ObsTransformEdit? liveFeedEdit = null);
        bool SelectionExists { get; }
        int SelectionLeft { get; }
        int FrameIndex { get; }
        int SelectionRight { get; }
        void SetDeltaAttributeInFrame(int frameIndex, ObsFramePropertyAttribute attribute, double value);
    }

    public interface IVideoEditorCommand
    {
        string Name { get; }
        void Execute();
        void Undo();
    }
}
