using System;
using System.Linq;

namespace DndCore
{
	public class ItemEffect
	{
		public string name { get; set; }
		public int index { get; set; }
		public string effect { get; set; }
		public string effectAvailableWhen { get; set; }
		public bool playToEndOnExpire { get; set; }
		public string hue { get; set; }
		public int saturation { get; set; }
		public int brightness { get; set; }
		public double opacity { get; set; }
		public double scale { get; set; }
		public double rotation { get; set; }
		public double degreesOffset { get; set; }
		public bool flipHorizontal { get; set; }
		public double moveLeftRight { get; set; }
		public double moveUpDown { get; set; }
		public bool fade { get; set; }
		public string dieRollEffects { get; set; }
		public string trailingEffects { get; set; }
		public string startSound { get; set; }
		public string endSound { get; set; }

		public ItemEffect()
		{

		}

		public static ItemEffect From(ItemEffectDto dto)
		{
			ItemEffect result = new ItemEffect();

			result.name = dto.name;
			result.index = MathUtils.GetInt(dto.index);
			result.effect = dto.effect;
			result.effectAvailableWhen = dto.effectAvailableWhen;
			result.playToEndOnExpire = MathUtils.IsChecked(dto.playToEndOnExpire);
			result.hue = dto.hue;
			result.saturation = MathUtils.GetInt(dto.saturation, 100);
			result.brightness = MathUtils.GetInt(dto.brightness, 100);
			result.opacity = MathUtils.GetDouble(dto.opacity, 1);
			result.scale = MathUtils.GetDouble(dto.scale, 1);
			result.rotation = MathUtils.GetDouble(dto.rotation);
			result.degreesOffset = MathUtils.GetDouble(dto.degreesOffset);
			result.flipHorizontal = MathUtils.IsChecked(dto.flipHorizontal);
			result.moveLeftRight = MathUtils.GetDouble(dto.moveLeftRight);
			result.moveUpDown = MathUtils.GetDouble(dto.moveUpDown);
			result.fade = MathUtils.IsChecked(dto.fade);
			result.dieRollEffects = dto.dieRollEffects;
			result.trailingEffects = dto.trailingEffects;
			result.startSound = dto.startSound;
			result.endSound = dto.endSound;

			return result;
		}
	}
}
