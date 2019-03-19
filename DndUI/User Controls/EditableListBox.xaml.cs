using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DndUI
{
	/// <summary>
	/// Interaction logic for EditableListBox.xaml. ItemsSource must be an 
	/// enumerable of IListEntry items.
	/// </summary>

	public partial class EditableListBox : UserControl
	{
		public static readonly DependencyProperty DataFileNameProperty = DependencyProperty.Register("DataFileName", typeof(string), typeof(EditableListBox), new FrameworkPropertyMetadata(null));
		
		public static readonly RoutedEvent ClickAddEvent = EventManager.RegisterRoutedEvent("ClickAdd", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EditableListBox));

		public event RoutedEventHandler ClickAdd
		{
			add { AddHandler(ClickAddEvent, value); }
			remove { RemoveHandler(ClickAddEvent, value); }
		}

		protected virtual void OnClickAdd()
		{
			RoutedEventArgs eventArgs = new RoutedEventArgs(ClickAddEvent);
			RaiseEvent(eventArgs);
		}

		public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(EditableListBox));

		public event SelectionChangedEventHandler SelectionChanged
		{
			add { AddHandler(SelectionChangedEvent, value); }
			remove { RemoveHandler(SelectionChangedEvent, value); }
		}

		protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			//SelectionChangedEventArgs previewEventArgs = new SelectionChangedEventArgs(PreviewSelectionChangedEvent);
			//RaiseEvent(previewEventArgs);
			//if (previewEventArgs.Handled)
			//	return;
			//SelectionChangedEventArgs eventArgs = new SelectionChangedEventArgs(SelectionChangedEvent);
			//RaiseEvent(eventArgs);
			List<TextBox> list = savedNames.Keys.ToList();
			foreach (object textBox in list)
				TextBox_LostFocus(textBox, null);

			if (e.AddedItems.Count > 0)
				SelectedItem = e.AddedItems[0];
			else
				SelectedItem = null;

			e.RoutedEvent = SelectionChangedEvent;
			RaiseEvent(e);
		}

		public string DataFileName
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (string)GetValue(DataFileNameProperty);
			}
			set
			{
				SetValue(DataFileNameProperty, value);
			}
		}
		public object SelectedItem { get; set; }

		Dictionary<TextBox, string> savedNames = new Dictionary<TextBox, string>();

		public EditableListBox()
		{
			InitializeComponent();
			Box.DataContext = this;
			Box.SelectionChanged += Box_SelectionChanged;
			TitleLabel.DataContext = this;

			Loaded += (s, e) =>
			{ // at this point is the control ready
				Window window = Window.GetWindow(this);  // get the parent window
				if (window != null)
					window.Closing += (s1, e1) => Disposing(); //disposing logic here
			};
		}

		object Disposing()
		{
			SaveEffects();
			return null;
		}

		private void Box_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			OnSelectionChanged(e);
		}

		private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
		{
			TextBox tb = (TextBox)((Grid)((TextBlock)sender).Parent).Children[1];
			tb.Visibility = Visibility.Visible;
			((TextBlock)sender).Visibility = Visibility.Collapsed;
			if (!savedNames.ContainsKey(tb))
				savedNames.Add(tb, tb.Text);
		}

		private void TextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			TextBlock tb = (TextBlock)((Grid)((TextBox)sender).Parent).Children[0];
			tb.Visibility = Visibility.Visible;
			((TextBox)sender).Visibility = Visibility.Collapsed;
		}

		private void TextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				if (sender is TextBox tb)
				{
					tb.Visibility = Visibility.Collapsed;
					return;
				}

			if (e.Key == Key.Escape)
				if (sender is TextBox tb)
				{
					if (savedNames.ContainsKey(tb))
						tb.Text = savedNames[tb];
					tb.Visibility = Visibility.Collapsed;
				}
		}

		private void TextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is TextBox tb)
				if ((bool)e.NewValue)
					savedNames.Add(tb, tb.Text);
				else if (savedNames.ContainsKey(tb))
				{
					if (savedNames[tb] != tb.Text)
						isDirty = true;
					tb.Visibility = Visibility.Collapsed;
					savedNames.Remove(tb);
				}
		}

		public IEnumerable ItemsSource
		{
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(EditableListBox), null);

		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("TitleProperty", typeof(string), typeof(EditableListBox), new PropertyMetadata("", new PropertyChangedCallback(TitleChanged)));
		bool isDirty;

		private void MenuDeleteClicked(object sender, RoutedEventArgs e)
		{
			DeleteSelectedEntry();
		}

		private void Box_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
				MenuDeleteClicked(sender, e);
		}

		private void BtnDuplicate_Click(object sender, RoutedEventArgs e)
		{
			// Use reflection to dynamically call Duplicate<T>
			IEnumerable itemsSource = ItemsSource;
			object selectedItem = Box.SelectedItem;
			if (selectedItem == null)
				return;
			Type entryType = selectedItem.GetType();
			MethodInfo method = typeof(EditableListBox).GetMethod("Duplicate");
			MethodInfo generic = method.MakeGenericMethod(entryType);
			object[] parameters = { itemsSource };
			generic.Invoke(this, parameters);
		}

		private void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			OnClickAdd();
		}

		private void BtnDelete_Click(object sender, RoutedEventArgs e)
		{
			DeleteSelectedEntry();
		}

		private void DeleteSelectedEntry()
		{
			if (!(Box.SelectedItem is IListEntry listEntry))
				return;

			if (ItemsSource == null)
				return;

			MessageBoxResult result = MessageBox.Show($"Delete this ({listEntry.Name})?",
																"Confirm",
																MessageBoxButton.YesNo,
																MessageBoxImage.Question);

			if (result != MessageBoxResult.Yes)
				return;

			// Use reflection to call the Remove method...
			Type sourceType = ItemsSource.GetType();
			MethodInfo method = sourceType.GetMethod("Remove");
			if (method == null)
				return;

			object[] parameters = { listEntry };
			method.Invoke(ItemsSource, parameters);
		}

		public void Duplicate<T>(ObservableCollection<T> itemList) where T : IListEntry
		{
			if (SelectedItem is T effectEntry)
			{
				T item = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(effectEntry));
				item.Name += " - Copy";
				itemList.Add(item);
			}
		}
		public ObservableCollection<T> LoadEntries<T>(string fileName = null) where T: ListEntry
		{
			if (fileName == null)
				fileName = DataFileName;

			List<T> loadedEntries = null;
			if (fileName != null)
				loadedEntries = Storage.Load<List<T>>(fileName);

			foreach (T entry in loadedEntries)
				entry.AfterLoad();

			ObservableCollection<T> results;
			if (loadedEntries != null)
				results = new ObservableCollection<T>(loadedEntries);
			else
				results = new ObservableCollection<T>();

			results.CollectionChanged += Results_CollectionChanged;
			ItemsSource = results;
			isDirty = false;
			return results;
		}

		private void Results_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			isDirty = true;
		}

		public void SaveEffects(string fileName = null)
		{
			if (!isDirty)  // Only really save if dirty/changed.
				return;

			if (fileName == null)
			{
				fileName = DataFileName;
				if (fileName == null)
					return;
			}

			if (ItemsSource != null)
			{
				object firstItem = null;
				IEnumerator enumerator = ItemsSource.GetEnumerator();
				if (enumerator.MoveNext())
					firstItem = enumerator.Current;

				if (firstItem == null)
					return;

				Type entryType = firstItem.GetType();
				MethodInfo method = typeof(Enumerable).GetMethod("ToList");
				if (method == null)
					return;

				MethodInfo genericMethod = method.MakeGenericMethod(entryType);
				if (genericMethod == null)
					return;

				object[] parameters = { ItemsSource };

				object data = genericMethod.Invoke(ItemsSource, parameters);
				if (data is List<ItemViewModel> items)
				{
					foreach (ItemViewModel itemViewModel in items)
						itemViewModel.PrepForSerialization();
				}
				Storage.Save(fileName, data);
			}
			isDirty = false;
		}

		public void SetDirty()
		{
			isDirty = true;
		}

		static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is EditableListBox lb)
				lb.TitleLabel.Text = lb.Title;
		}
	}
}
