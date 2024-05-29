using System;
using System.Collections.Generic;
using System.Linq;
using WpfEditorControls;

#if LiveVideoAnimationSandbox
#endif

namespace DHDM
{
    public class VisualizerSelector
    {
        public event EventHandler<ISelectableVisualizer>? SelectionChanged;
        
        protected void OnSelectionChanged(object? sender, ISelectableVisualizer e)
        {
            SelectionChanged?.Invoke(sender, e);
        }

        List<ISelectableVisualizer> selectableVisualizers = new List<ISelectableVisualizer>();

        public void AddSelectableVisualizer(ISelectableVisualizer selectableVisualizer)
        {
            selectableVisualizers.Add(selectableVisualizer);
        }

        public bool SelectVisualizer(ISelectableVisualizer selectedVisualizer)
        {
            bool shouldFireChangedEvent = SelectedVisualizer != selectedVisualizer;
            SelectedVisualizer = selectedVisualizer;
            foreach (ISelectableVisualizer selectableVisualizer in selectableVisualizers)
                if (selectableVisualizer == selectedVisualizer)
                    selectableVisualizer.Select();
                else
                    selectableVisualizer.ClearSelection();
            if (shouldFireChangedEvent)
                OnSelectionChanged(this, selectedVisualizer);
            return shouldFireChangedEvent;
        }
        public ISelectableVisualizer? SelectedVisualizer { get; set; }
    }
}
