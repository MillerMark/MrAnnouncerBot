using System;
using System.Linq;

namespace WpfEditorControls
{
    public interface ISelectableVisualizer
    {
        void Select();
        void ClearSelection();
    }
}
