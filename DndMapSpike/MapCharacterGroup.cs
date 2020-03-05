using System;
using System.Linq;
using System.Collections.Generic;
using MapCore;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace DndMapSpike
{
	public class MapCharacterGroup : MapCharacter, IGroup
	{
		public override string FileName
		{
			get
			{
				if (!children.Any())
					return string.Empty;
				return children.FirstOrDefault().FileName;
			}
			set
			{
				foreach (IItemProperties stamp in children)
				{
					stamp.FileName = value;
				}
			}
		}

		[JsonIgnore]
		public override Image Image
		{
			get
			{
				return this.GetImage();
			}
		}

		List<IItemProperties> children = new List<IItemProperties>();
		public MapCharacterGroup(): base()
		{
			TypeName = nameof(MapCharacterGroup);
		}

		public MapCharacterGroup(MapCharacterGroup stampGroup) : this()
		{
			this.CloneGroupFrom(stampGroup);
		}

		public override void BlendStampImage(StampsLayer stampsLayer, double xOffset = 0, double yOffset = 0)
		{
			GroupHelper.BlendStampImage(this, stampsLayer, xOffset, yOffset);
		}

		public override bool ContainsPoint(double x, double y)
		{
			return GroupHelper.ContainsPoint(this, x, y);
		}

		public List<IItemProperties> Children { get => children; set => children = value; }

		public static new MapCharacterGroup From(SerializedStamp stamp)
		{
			return GroupHelper.DeserializeGroup<MapCharacterGroup>(stamp);
		}

		void IGroup.Deserialize(SerializedStamp serializedStamp)
		{
			base.Deserialize(serializedStamp);
		}

		public void Ungroup(List<IItemProperties> ungroupedStamps)
		{
			GroupHelper.Ungroup(this, ungroupedStamps);
		}

		public override void CreateFloating(Canvas canvas, double left = 0, double top = 0)
		{
			this.CreateFloatingImages(canvas, left, top);
		}

		public override IItemProperties Copy(double deltaX, double deltaY)
		{
			return this.Copy<MapCharacterGroup>(deltaX, deltaY);
		}

		public override double Width { get; set; }
		public override double Height { get; set; }
	}
}

