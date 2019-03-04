using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DndUI
{
	//class VmEffectsList: INotifyPropertyChanged
	//{
	//	public VmEffectsList()
	//	{
	//		Items = new ObservableCollection<EffectEntry>();

	//		Add = new RelayCommand(DoAdd);
	//		Delete = new RelayCommand(DoDelete);
	//	}

	//	public ObservableCollection<EffectEntry> Items { get; set; }

	//	public ICommand Add { get; }
	//	public ICommand Delete { get; }

	//	public event PropertyChangedEventHandler PropertyChanged;

	//	protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
	//	{
	//		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	//	}

	//	private EffectEntry _selectedItem;

	//	public EffectEntry SelectedItem
	//	{
	//		get => _selectedItem;
	//		set
	//		{
	//			_selectedItem = value;
	//			OnPropertyChanged();
	//		}
	//	}


	//	private void DoAdd(object selectedItem)
	//	{
	//		EffectEntry newItem = new EffectEntry(EffectKind.Animation, "New Effect") { };

	//		Items.Add(newItem);

	//		newItem.IsSelected = true;
	//	}

	//	private void DoDelete(object selectedItem)
	//	{
	//		if (!(selectedItem is EffectEntry selected))
	//			return;

	//		Items.Remove(selected);
	//	}

	//}
}
