using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public abstract class BaseStampAbsoluteValueCommand : BaseStampCommand
	{
		Dictionary<Guid, double> undoValues = new Dictionary<Guid, double>();

		public double RedoValue { get; set; }

		protected void SaveValue(IItemProperties itemProperties, double value)
		{
			if (undoValues.ContainsKey(itemProperties.Guid))
				undoValues[itemProperties.Guid] = value;
			else
				undoValues.Add(itemProperties.Guid, value);
		}

		protected double GetSavedValue(IItemProperties itemProperties)
		{
			if (!undoValues.ContainsKey(itemProperties.Guid))
			{
				System.Diagnostics.Debugger.Break();
				return 0;
			}

			return undoValues[itemProperties.Guid];
		}

		protected int GetSavedInt(IItemProperties stampProperties)
		{
			if (!undoValues.ContainsKey(stampProperties.Guid))
			{
				System.Diagnostics.Debugger.Break();
				return 0;
			}

			return (int)Math.Round(undoValues[stampProperties.Guid]);
		}

		public override void Redo(Map map)
		{
			if (Data is DoubleData doubleData)
				RedoValue = doubleData.Value;
			base.Redo(map);
		}

		public BaseStampAbsoluteValueCommand()
		{

		}

	}
}
