using System;
using System.Linq;
using MapCore;

namespace DndMapSpike
{
	public class MapSelection
	{
		public event EventHandler SelectionChanged;
		public event EventHandler SelectionCommitted;
		public event EventHandler SelectionCancelled;
		public MapSelection()
		{

		}

		public bool Selecting { get; set; }
		public Tile Caret { get; set; }
		public Tile Anchor { get; set; }

		// TODO: Move this to a UI space, buddy. Come on.
		public int BorderThickness { get; set; } = 4;
		public SelectionType SelectionType { get; set; } = SelectionType.None;

		protected virtual void OnSelectionCommitted()
		{
			SelectionCommitted?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnSelectionChanged()
		{
			SelectionChanged?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnSelectionCancelled()
		{
			SelectionCancelled?.Invoke(this, EventArgs.Empty);
		}

		public void StartSelection(Tile baseSpace, SelectionType selectionType = SelectionType.Replace)
		{
			SelectionType = selectionType;
			Selecting = true;
			Anchor = baseSpace;
			Caret = baseSpace;
			OnSelectionChanged();
		}

		public void SelectTo(Tile baseSpace)
		{
			if (Caret == baseSpace)
				return;
			Caret = baseSpace;
			OnSelectionChanged();
		}

		public void Commit()
		{
			Selecting = false;
			OnSelectionCommitted();
			Clear();
		}

		public void Cancel()
		{
			Selecting = false;
			OnSelectionCancelled();
			Clear();
		}

		public void Clear()
		{
			Caret = null;
			Anchor = null;
			SelectionType = SelectionType.None;
		}

		public void GetPixelRect(out int left, out int top, out int width, out int height)
		{
			Caret.GetPixelCoordinates(out int caretLeft, out int caretTop, out int caretRight, out int caretBottom);
			Anchor.GetPixelCoordinates(out int anchorLeft, out int anchorTop, out int anchorRight, out int anchorBottom);
			left = Math.Min(caretLeft, anchorLeft);
			top = Math.Min(caretTop, anchorTop);
			int right = Math.Max(caretRight, anchorRight);
			int bottom = Math.Max(caretBottom, anchorBottom);
			width = right - left;
			height = bottom - top;
		}
	}
}
