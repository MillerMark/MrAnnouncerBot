using DndCore;
using DndCore.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace DndTests
{
	public static class TestStorageHelper
	{
		public static ItemViewModel GetExistingItem(string itemName)
		{
			ObservableCollection<ItemViewModel> loadEntriesFromFile = Storage.LoadEntriesFromFile<ItemViewModel>("items.json");
			foreach (ItemViewModel itemViewModel in loadEntriesFromFile)
			{
				if (itemViewModel.Name == itemName)
				{
					return itemViewModel;
				}
			}
			return null;
		}

	}
}
