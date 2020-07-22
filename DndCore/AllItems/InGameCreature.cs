using System;
using System.Linq;
using Newtonsoft.Json;
using GoogleHelper;

namespace DndCore
{
	[SheetName("DnD Game")]
	[TabName("Creatures")]
	public class InGameCreature
	{
		[Column]
		public string Name { get; set; }
		
		[Column]
		public string Kind { get; set; }
		
		[Column]
		[JsonIgnore]
		public string ImageUrlOverride { get; set; }

		public string ImageURL
		{
			get
			{
				if (Creature is Monster monster)
					return monster.ImageUrl;
				return string.Empty;
			}
		}

		[Column]
		[JsonIgnore]
		public string ImageCropOverride { get; set; }

		[Column]
		[JsonIgnore]
		public int HitPoints { get; set; }

		[Column]
		public int Index { get; set; }

		[Column]
		public bool IsEnemy { get; set; }

		public double CropX
		{
			get
			{
				if (Creature is Monster monster)
					return monster.ImageCropInfo.X * monster.ImageCropInfo.DpiFactor;
				return 0;
			}
		}
		public double CropY
		{
			get
			{
				if (Creature is Monster monster)
					return monster.ImageCropInfo.Y * monster.ImageCropInfo.DpiFactor;
				return 0;
			}
		}

		public double CropWidth
		{
			get
			{
				if (Creature is Monster monster)
					return monster.ImageCropInfo.Width * monster.ImageCropInfo.DpiFactor;
				return PictureCropInfo.MinWidth;
			}
		}

		public string Alignment
		{
			get
			{
				if (Creature is Monster monster)
					return monster.alignmentStr;
				return string.Empty;
			}
		}

		public double Health
		{
			get
			{
				return (Creature.HitPoints + Creature.tempHitPoints) / Creature.maxHitPoints;
			}
		}
		

		// TODO: Implement this.
		public bool IsTargeted { get; set; }

		[JsonIgnore]
		public bool IsSelected { get; set; }


		Creature creature;
		[JsonIgnore]
		public Creature Creature
		{
			get
			{
				if (creature == null)
				{
					Monster monster = Monster.Clone(AllMonsters.GetByKind(Kind));
					if (monster != null)
					{
						// TODO: Fix race (or use "Kind") field and use Name for the monster's name.
						if (!string.IsNullOrWhiteSpace(Name))
							monster.Name = Name;
						if (HitPoints > 0)
						{
							if (HitPoints > monster.maxHitPoints)
								monster.maxHitPoints = HitPoints;

							monster.HitPoints = HitPoints;
						}

						if (!string.IsNullOrWhiteSpace(ImageUrlOverride))
							monster.ImageUrl = ImageUrlOverride;
						if (!string.IsNullOrWhiteSpace(ImageCropOverride))
							monster.ImageCropInfo = PictureCropInfo.FromStr(ImageCropOverride);
						creature = monster;
					}
				}
				return creature;
			}
		}
		
		public InGameCreature()
		{

		}
		
		public static InGameCreature FromMonster(Monster monster)
		{
			InGameCreature inGameCreature = new InGameCreature();
			inGameCreature.Kind = monster.Kind;
			return inGameCreature;
		}
	}
}

