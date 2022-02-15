using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;

namespace DndCore
{

	[Document("DnD")]
	[Sheet("Items")]
	public class DndItem
	{
		[Column]
		[Indexer]
		public string Id { get; set; }
		[Column]
		public string Name { get; set; }
		[Column]
		public string Description { get; set; }
		[Column]
		public string OnConsume { get; set; }
		[Column]
		public string OnEquip { get; set; }
		[Column]
		public string OnUnequip { get; set; }
		[Column]
		public string OnDiscard { get; set; }
		[Column]
		public string OnAcquire { get; set; }
		[Column]
		public string OnImpact { get; set; }
		[Column]
		public bool Consumable { get; set; }
		[Column]
		public bool Magic { get; set; }
		[Column]
		public bool Silvered { get; set; }
		[Column]
		public bool Adamantine { get; set; }
		[Column]
		public string EquipTime { get; set; }
		[Column]
		public string UnequipTime { get; set; }
		[Column]
		public double Weight { get; set; }
		[Column]
		public int MinStrToCarry { get; set; }
		[Column]
		public double CostValue { get; set; }

		public DndItem()
		{

		}
	}
}