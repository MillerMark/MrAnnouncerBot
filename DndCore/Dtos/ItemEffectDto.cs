using System;
using System.Linq;

namespace DndCore
{
	public class ItemEffectDto
	{
		public string name { get; set; }
		public string index { get; set; }
		public string kind { get; set; }
		public string effect { get; set; }
		public string effectAvailableWhen { get; set; }
		public string playToEndOnExpire { get; set; }
		public string hue { get; set; }
		public string saturation { get; set; }
		public string brightness { get; set; }
		public string opacity { get; set; }
		public string scale { get; set; }
		public string rotation { get; set; }
		public string degreesOffset { get; set; }
		public string flipHorizontal { get; set; }
		public string moveLeftRight { get; set; }
		public string moveUpDown { get; set; }
		public string fade { get; set; }
		public string dieRollEffects { get; set; }
		public string trailingEffects { get; set; }
		public string startSound { get; set; }
		public string endSound { get; set; }
		public string lifespan { get; set; }
		public string fadeIn { get; set; }
		public string fadeOut { get; set; }

		public ItemEffectDto()
		{

		}
	}
}
