using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public abstract class BaseStampAbsoluteValueCommand : BaseStampCommand
	{
		Dictionary<Guid, double> undoValues = new Dictionary<Guid, double>();

		public double RedoValue { get; set; }

		protected void SaveValue(IStampProperties stampProperties, double value)
		{
			if (undoValues.ContainsKey(stampProperties.Guid))
				undoValues[stampProperties.Guid] = value;
			else
				undoValues.Add(stampProperties.Guid, value);
		}

		protected double GetSavedValue(IStampProperties stampProperties)
		{
			if (!undoValues.ContainsKey(stampProperties.Guid))
			{
				System.Diagnostics.Debugger.Break();
				return 0;
			}

			return undoValues[stampProperties.Guid];
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
