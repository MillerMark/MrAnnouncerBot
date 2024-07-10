#nullable enable
using Imaging;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

#if LiveVideoAnimationSandbox
#endif

namespace DHDM
{
    public class ObsAttributeData
    {
        public int FrameLeft {get; set; }
        public int FrameRight {get; set; }
        public double? SingleValue {get; set; }
        public List<double>? IndividualValues { get; set; }
        public bool SelectionExists => FrameLeft != FrameRight;
        public ObsAttributeData()
        {
            
        }
        public void SetSingleValue(double? value, int frameIndex)
        {
            FrameLeft = frameIndex;
            FrameRight = frameIndex;
            SingleValue = value;
        }

        public void SetSingleValue(double value, int selectionLeft, int selectionRight)
        {
            FrameLeft = selectionLeft;
            FrameRight = selectionRight;
            SingleValue = value;
        }

        public void SetIndividualValues(List<double> existingValues, int left, int right)
        {
            IndividualValues = new List<double>(existingValues);
            FrameLeft = left;
            FrameRight = right;
        }

        public double GetIndividualValueByFrameIndex(int i)
        {
            if (IndividualValues == null)
            {
                ArgumentNullException.ThrowIfNull(SingleValue);
                return SingleValue.Value;
            };

            return IndividualValues[i - FrameLeft];
        }
    }
    public class VideoCommandExecutor
    {
        Stack<VideoEditorCommand> undoStack = new ();
        Stack<VideoEditorCommand> redoStack = new ();
        public void Execute(VideoEditorCommand videoEditorCommand)
        {
            videoEditorCommand.Execute();
            undoStack.Push(videoEditorCommand);
            redoStack.Clear();
        }

        public void Undo()
        {
            if (!undoStack.Any())
                return;

            VideoEditorCommand undoCommand = undoStack.Pop();
            undoCommand.Undo();
            redoStack.Push(undoCommand);

        }
        public void Redo()
        {
            if (!redoStack.Any())
                return;
            VideoEditorCommand redoCommand = redoStack.Pop();
            redoCommand.Execute();
            undoStack.Push(redoCommand);
        }

        public void ClearUndoAndRedoStacks()
        {
            undoStack.Clear();
            redoStack.Clear();
        }

        public VideoCommandExecutor()
        {
            
        }
    }
    public class DeltaVideoEditorCommand : VideoEditorCommand
    {
        string name;
        ICanEditFrames canEditFrames;
        public ObsAttributeData? UndoData { get; set; }
        public ObsAttributeData ForwardData { get; set; }

        public DeltaVideoEditorCommand(ICanEditFrames canEditFrames, ObsFramePropertyAttribute attribute, double value)
        {
            this.canEditFrames = canEditFrames;
            ForwardData = new ObsAttributeData()
            {
                FrameLeft = canEditFrames.SelectionLeft,
                FrameRight = canEditFrames.SelectionRight,
            };
            ForwardData.SetSingleValue(value, canEditFrames.SelectionLeft, canEditFrames.SelectionRight);

            Attribute = attribute;
            name = $"Delta {attribute}";
        }

        public override string Name => name;
        public ObsFramePropertyAttribute Attribute { get; set; }

        public override void Execute()
        {
            Execute(ForwardData);
        }
        void Execute(ObsAttributeData data)
        {
            if (data.SelectionExists)
            {
                for (int i = data.FrameLeft; i < data.FrameRight; i++)
                    canEditFrames.SetDeltaAttributeInFrame(i, Attribute, data.GetIndividualValueByFrameIndex(i));

                canEditFrames.UpdateEverythingOnTimeline();
            }
            else
            {
                canEditFrames.SetDeltaAttributeInFrame(data.FrameLeft, Attribute, data.SingleValue!.Value);
            }
            canEditFrames.MoveObsSourcesIntoPosition();
            canEditFrames.UpdateValuePreviews();
        }

        public override void Undo()
        {
            ArgumentNullException.ThrowIfNull(UndoData);
            Execute(UndoData);
        }

        public void SetUndoData(ObsAttributeData undoDataForAttribute)
        {
            UndoData = undoDataForAttribute;
        }
    }
    public abstract class VideoEditorCommand : IVideoEditorCommand
    {
        public abstract string Name { get; }
        public abstract void Execute();
        public abstract void Undo();
    }
}
