using System;
using System.Linq;
using WpfEditorControls;

#if LiveVideoAnimationSandbox
#endif

namespace DHDM
{
    public class VisualizerSelector
    {
        List<ISelectableVisualizer> selectableVisualizers = new List<ISelectableVisualizer>();

        public void AddSelectableVisualizer(ISelectableVisualizer selectableVisualizer)
        {
            selectableVisualizers.Add(selectableVisualizer);
        }

        public void SelectVisualizer(ISelectableVisualizer selectedVisualizer)
        {
            SelectedVisualizer = selectedVisualizer;
            foreach (ISelectableVisualizer selectableVisualizer in selectableVisualizers)
                if (selectableVisualizer == selectedVisualizer)
                    selectableVisualizer.Select();
                else
                    selectableVisualizer.ClearSelection();
        }
        public ISelectableVisualizer? SelectedVisualizer { get; set; }
    }
}
