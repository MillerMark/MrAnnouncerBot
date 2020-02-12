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
		// TODO: This has to change to support adding and deleting stamps
		public List<IStampProperties> SelectedStamps { get; set; }
		List<Guid> stampGuids = new List<Guid>();
		void LoadSelectedStamps(Map map)
		{
			SelectedStamps = map.GetStamps(stampGuids);
		}
		object data;
		public object Data
		{
			get
			{
				return data;
			}
			set
			{
				if (data == value)
					return;

				data = value;
				OnDataChanged();
			}
		}
		protected virtual void OnDataChanged()
		{
			
		}

		public void Execute(Map map, List<IStampProperties> selectedStamps)
		{
			SelectedStamps = selectedStamps;
			PrepareForExecution(selectedStamps);
			Redo(map);
		}

		/// <summary>
		/// Called just before the first time execution of a command.
		/// This is your chance to save data for the undo.
		/// </summary>
		/// <param name="selectedStamps"></param>
		protected virtual void PrepareForExecution(List<IStampProperties> selectedStamps)
		{
			stampGuids.Clear();
			foreach (IStampProperties stampProperties in selectedStamps)
				stampGuids.Add(stampProperties.Guid);
		}

		protected abstract void ActivateRedo(Map map);
		protected abstract void ActivateUndo(Map map);

		public virtual void Redo(Map map)
		{
			LoadSelectedStamps(map);
			ActivateRedo(map);
		}

		public virtual void Undo(Map map)
		{
			LoadSelectedStamps(map);
			ActivateUndo(map);
		}

		public BaseCommand()
		{

		}

		public static explicit operator BaseCommand(ObjectHandle v)
		{
			throw new NotImplementedException();
		}
	}
}
