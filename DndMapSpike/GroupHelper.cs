using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using MapCore;

namespace DndMapSpike
{
	public static class GroupHelper
	{
		public static void CloneGroupFrom(this IGroup targetGroup, IGroup sourceGroup)
		{
			targetGroup.X = sourceGroup.X;
			targetGroup.Y = sourceGroup.Y;
			targetGroup.Width = sourceGroup.Width;
			targetGroup.Height = sourceGroup.Height;
			foreach (IItemProperties item in sourceGroup.Children)
				targetGroup.Children.Add(item.Clone());
		}

		public static void BlendStampImage(IGroup stampGroup, StampsLayer stampsLayer, double xOffset, double yOffset)
		{
			if (!stampGroup.Visible)
				return;
			foreach (IItemProperties stamp in stampGroup.Children)
				if (stamp is IFloatingItem floatingItem)
					floatingItem.BlendStampImage(stampsLayer, xOffset + stampGroup.X, yOffset + stampGroup.Y);
		}
		public static bool ContainsPoint(IGroup stampGroup, double x, double y)
		{
			Point relativeTestPoint = new Point(x - stampGroup.X, y - stampGroup.Y);
			return stampGroup.Children.Any(s => s.ContainsPoint(relativeTestPoint.X, relativeTestPoint.Y));
		}
		public static TGroup DeserializeGroup<TGroup>(SerializedStamp stamp) where TGroup : IGroup, new()
		{
			TGroup result = new TGroup();
			result.Deserialize(stamp);
			if (stamp.Children != null)
				foreach (SerializedStamp childStamp in stamp.Children)
					result.Children.Add(MapElementFactory.CreateStampFrom(childStamp));
			return result;
		}

		public static void Ungroup(IGroup group, List<IItemProperties> ungroupedStamps)
		{
			if (group.Locked)
				return;

			for (int i = 0; i < group.Children.Count; i++)
			{
				IItemProperties stamp = group.Children[i];
				stamp.Move(group.X, group.Y);
				stamp.ZOrder = group.ZOrder + i;
				ungroupedStamps.Add(stamp);
			}
		}

		public static void CalculateSizeAndPosition(this IGroup result, double xOffset = 0, double yOffset = 0)
		{
			List<IItemProperties> children = result.Children;
			if (children.Count == 0)
				return;
			double leftMost = children.Min(x => x.GetLeft() + xOffset);
			double topMost = children.Min(x => x.GetTop() + yOffset);
			double rightMost = children.Max(x => x.GetLeft() + xOffset + x.Width);
			double bottomMost = children.Max(x => x.GetTop() + yOffset + x.Height);
			result.ZOrder = children.Max(x => x.ZOrder);

			result.Width = rightMost - leftMost;
			result.Height = bottomMost - topMost;
			result.X = (leftMost + rightMost) / 2.0;
			result.Y = (topMost + bottomMost) / 2.0;
		}

		static void PositionContainedStampsRelativeToCenter(IGroup result)
		{
			foreach (IItemProperties stamp in result.Children)
			{
				stamp.X -= result.X;
				stamp.Y -= result.Y;
			}
		}

		public static void CreateFrom(IGroup result, List<IItemProperties> stamps)
		{
			List<IItemProperties> sortedStamps = stamps.OrderBy(x => x.ZOrder).ToList();
			for (int i = 0; i < sortedStamps.Count; i++)
			{
				sortedStamps[i].ZOrder = i + 1;
			}
			result.Children = sortedStamps;

			result.CalculateSizeAndPosition();
			PositionContainedStampsRelativeToCenter(result);
		}

		public static IGroup Create<T>(List<IItemProperties> stamps) where T: IGroup, new()
		{
			T result = new T();
			CreateFrom(result, stamps);
			return result;
		}

		public static Image GetImage(this IGroup group)
		{
			if (group.Children.FirstOrDefault() is IFloatingItem stamp)
				return stamp.Image;
			return null;
		}

		public static void CreateFloatingImages(this IGroup group, Canvas canvas, double left, double top)
		{
			foreach (IItemProperties stamp in group.Children)
				if (stamp is IFloatingItem floatingItem)
					floatingItem.CreateFloating(canvas, stamp.GetLeft() + left + group.Width / 2.0, stamp.GetTop() + top + group.Height / 2.0);
		}
		public static IItemProperties Copy<T>(this IGroup group, double deltaX, double deltaY) where T: IGroup, new()
		{
			T result = new T();
			result.CloneGroupFrom(group);
			result.Move(deltaX, deltaY);
			return result;
		}
	}
}

