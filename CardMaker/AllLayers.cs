using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace CardMaker
{
	public class AllLayers: INotifyPropertyChanged
	{
		public List<LayerDetails> Details { get; set; } = new List<LayerDetails>();
		public AllLayers()
		{

		}

		bool isDirty;

		public bool IsDirty
		{
			get => isDirty;
			set
			{
				if (isDirty == value)
					return;
				isDirty = value;
				
				OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsDirty)));
			}
		}
		

		public event PropertyChangedEventHandler PropertyChanged;

		void Add(LayerDetails layerDetails)
		{
			Details.Add(layerDetails);
			//OnPropertyChanged(new PropertyChangedEventArgs("Details"));
		}

		public LayerDetails Get(string layerName)
		{
			LayerDetails layerDetails = Details.FirstOrDefault(x => x.Name == layerName);
			if (layerDetails == null)
			{
				layerDetails = new LayerDetails() { Name = layerName };
				layerDetails.PossibleLinkedPropertyChanged += LayerDetails_PossibleLinkedPropertyChanged;

				layerDetails.PropertyChanged += LayerDetails_PropertyChanged;
				Add(layerDetails);
			}
			return layerDetails;
		}

		public event LinkedPropertyChangedEventHandler LinkedPropertyChanged;
		protected virtual void OnLinkedPropertyChanged(object sender, LinkedPropertyEventArgs ea)
		{
			LinkedPropertyChanged?.Invoke(sender, ea);
		}

		private void LayerDetails_PossibleLinkedPropertyChanged(object sender, LinkedPropertyEventArgs ea)
		{
			OnLinkedPropertyChanged(sender, ea);
		}

		private void LayerDetails_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged(e);
		}

		private void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
			if (e.PropertyName != nameof(IsDirty))
				IsDirty = true;
		}

		public void Clear()
		{
			Details.Clear();
			OnPropertyChanged(new PropertyChangedEventArgs("Details"));
		}
	}
}
