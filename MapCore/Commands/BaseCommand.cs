using System;
using System.Linq;
using System.Runtime.Remoting;
using System.Collections.Generic;

namespace MapCore
{
	public abstract class BaseCommand
	{
		#region Serialization???
		//public string SelectionData { get; set; }
		//public string ExecuteData { get; set; }
		//public string UndoData { get; set; }
		//public abstract void PrepareForSerialization();
		//public abstract void Reconstitute();
		#endregion

		string typeName;
		public string TypeName
		{
			get
			{
				if (typeName == null)
					typeName = this.GetType().Name;
				return typeName;
			}
			set
			{
				typeName = value;
			}
		}
		public bool WorksOnStamps { get; set; }
		// TODO: This has to change to support adding and deleting stam
		public List<IStampProperties> SelectedStamps { get; set; }
		public object Data { get; set; }

		public void Execute(Map map, List<IStampProperties> selectedStamps)
		{
			SelectedStamps = selectedStamps;
			Redo(map);
		}

		public abstract void Redo(Map map);
		public abstract void Undo(Map map);
		public BaseCommand()
		{

		}

		public static explicit operator BaseCommand(ObjectHandle v)
		{
			throw new NotImplementedException();
		}
	}
}
