using System;
using System.Linq;
using System.Collections.Generic;

namespace CardMaker
{
	public class PropertyLink
	{
		public string Name { get; set; }
		public int GroupNumber { get; set; }
		public List<CardImageLayer> Layers { get; set; } = new List<CardImageLayer>();
		public event LinkedPropertyChangedEventHandler LinkedPropertyChanged;

		protected virtual void OnLinkedPropertyChanged(object sender, LinkedPropertyEventArgs ea)
		{
			LinkedPropertyChanged?.Invoke(sender, ea);
		}

		public PropertyLink()
		{

		}
	}
}
