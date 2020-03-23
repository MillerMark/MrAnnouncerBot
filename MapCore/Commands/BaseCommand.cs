using System;
using System.Linq;
using System.Runtime.Remoting;
using System.Collections.Generic;
using Newtonsoft.Json;

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

		public List<IItemProperties> SelectedItems { get; set; }
		
		public List<IStampProperties> SelectedStamps
		{
			get
			{
				return SelectedItems.OfType<IStampProperties>().ToList();
			}
		}

		List<Guid> stampGuids = new List<Guid>();


		protected void ClearSelectedStamps(Map map)
		{
			map.SelectedItems.Clear();
			SelectedItems.Clear();
			UpdateSelectedStampGuids();
		}

		void UpdateSelectedStampGuids()
		{
			stampGuids.Clear();
			stampGuids.AddRange(SelectedItems.Select(stampProperties => stampProperties.Guid));
		}

		protected void AddSelectedItem(Map map, IItemProperties item)
		{
			map.SelectedItems.Add(item);
			SelectedItems.Add(item);
			UpdateSelectedStampGuids();
		}

		protected void RemoveSelectedItem(Map map, IItemProperties item)
		{
			map.SelectedItems.Remove(item);
			SelectedItems.Remove(item);
			UpdateSelectedStampGuids();
		}

		protected void SetSelectedItems(Map map, List<IItemProperties> items)
		{
			map.SelectedItems = items;
			SelectedItems = items;
			UpdateSelectedStampGuids();
		}

		protected void SetSelectedItems(Map map, List<Guid> stampItems)
		{
			map.SelectItemsByGuid(stampItems);
			SelectedItems = map.GetItems(stampItems);
			this.stampGuids = stampItems;
		}

		void LoadSelectedStamps(Map map)
		{
			SelectedItems = map.GetItems(stampGuids);
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

		public void Execute(Map map, List<IItemProperties> selectedStamps)
		{
			SelectedItems = selectedStamps;
			PrepareForExecution(map);
			Redo(map);
			// TODO: consider adding a diff virtual method call here to optimize undo data.
		}

		/// <summary>
		/// Called just before the first time execution of a command.
		/// This is your chance to save data for the undo.
		/// </summary>
		/// <param name="map"></param>
		protected virtual void PrepareForExecution(Map map)
		{
			stampGuids.Clear();
			foreach (IItemProperties itemProperties in SelectedItems)
				stampGuids.Add(itemProperties.Guid);
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
	}
}
