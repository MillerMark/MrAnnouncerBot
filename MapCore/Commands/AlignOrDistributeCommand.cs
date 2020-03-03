using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class AlignOrDistributeCommand : BaseStampAbsoluteValueCommand
	{
		public StampAlignment Alignment { get; set; }
		public double SpaceBetween { get; set; }
		public AlignOrDistributeCommand()
		{

		}

		protected override void OnDataChanged()
		{
			base.OnDataChanged();
			if (Data is AlignmentData alignmentData)
			{
				Alignment = alignmentData.Alignment;
				SpaceBetween = alignmentData.SpaceBetween;
			}
		}

		protected override void PrepareForExecution(Map map)
		{
			base.PrepareForExecution(map);
			foreach (IItemProperties stampProperties in SelectedItems)
				switch (Alignment)
				{
					case StampAlignment.Left:
					case StampAlignment.HorizontalCenter:
					case StampAlignment.DistributeHorizontally:
					case StampAlignment.Right:
						SaveValue(stampProperties, stampProperties.X);
						break;
					case StampAlignment.Top:
					case StampAlignment.VerticalCenter:
					case StampAlignment.DistributeVertically:
					case StampAlignment.Bottom:
						SaveValue(stampProperties, stampProperties.Y);
						break;
				}
		}

		void DistributeHorizontally()
		{
			double stampX = RedoValue;
			List<IItemProperties> sortedStamps = SelectedItems.OrderBy(s => s.X).ToList();
			foreach (IItemProperties stamp in sortedStamps)
			{
				stamp.X = stampX;
				stampX += SpaceBetween;
			}
		}

		void DistributeVertically()
		{
			double stampY = RedoValue;
			List<IItemProperties> sortedStamps = SelectedItems.OrderBy(s => s.Y).ToList();
			foreach (IItemProperties stamp in sortedStamps)
			{
				stamp.Y = stampY;
				stampY += SpaceBetween;
			}
		}

		protected override void ActivateRedo(Map map)
		{
			if (Alignment == StampAlignment.DistributeHorizontally)
			{
				DistributeHorizontally();
				return;
			}
			if (Alignment == StampAlignment.DistributeVertically)
			{
				DistributeVertically();
				return;
			}
			foreach (IItemProperties stamp in SelectedItems)
				switch (Alignment)
				{
					case StampAlignment.Left:
						stamp.X = RedoValue + stamp.Width / 2.0;  break;
					case StampAlignment.Right:
						stamp.X = RedoValue - stamp.Width / 2.0;  break;
					case StampAlignment.Top:
						stamp.Y = RedoValue + stamp.Height / 2.0; break;
					case StampAlignment.Bottom:
						stamp.Y = RedoValue - stamp.Height / 2.0; break;

					case StampAlignment.HorizontalCenter:
						stamp.X = RedoValue;  break;
					case StampAlignment.VerticalCenter:
						stamp.Y = RedoValue;  break;
				}
		}

		protected override void ActivateUndo(Map map)
		{
			foreach (IItemProperties stampProperties in SelectedItems)
				switch (Alignment)
				{
					case StampAlignment.Left:
					case StampAlignment.HorizontalCenter:
					case StampAlignment.DistributeHorizontally:
					case StampAlignment.Right:
						stampProperties.X = GetSavedValue(stampProperties);
						break;
					case StampAlignment.Top:
					case StampAlignment.VerticalCenter:
					case StampAlignment.DistributeVertically:
					case StampAlignment.Bottom:
						stampProperties.Y = GetSavedValue(stampProperties);
						break;
				}
		}
	}
}
