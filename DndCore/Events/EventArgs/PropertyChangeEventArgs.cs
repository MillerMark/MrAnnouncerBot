using System;

namespace DndCore
{
	public class PropertyChangeEventArgs : EventArgs
	{

		public PropertyChangeEventArgs()
		{

		}

		public PropertyChangeEventArgs(Creature creature, string propertyName, double deltaValue)
		{
			Creature = creature;
			PropertyName = propertyName;
			DeltaValue = deltaValue;
		}

		public Creature Creature { get; }
		public string PropertyName { get; }
		public double DeltaValue { get; }
	}
}
