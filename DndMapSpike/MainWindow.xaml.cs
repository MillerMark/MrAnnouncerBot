using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO;
using MapCore;

namespace DndMapSpike
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		MapSelection selection;
		public Map Map { get; set; }
		public MainWindow()
		{
			InitializeComponent();
			Map = new Map();
			selection = new MapSelection();
			selection.SelectionChanged += Selection_SelectionChanged;
			selection.SelectionCommitted += Selection_SelectionCommitted;
			selection.SelectionCancelled += Selection_SelectionCancelled;

			//LoadMap("The Delve of Lamprica.txt");
			//LoadMap("The Pit of Alan the Necromancer.txt");
			//LoadMap("The Barrow of Elemental Horror.txt");
			//LoadMap("The Hive of the Vampire Princess.txt");
			//LoadMap("The Dark Chambers of Ages.txt");
			LoadMap("The Barrow of Emirkol the Chaotic.txt");
		}

		private void Selection_SelectionCancelled(object sender, EventArgs e)
		{
			SetRubberBandVisibility(Visibility.Hidden);
		}

		private void Selection_SelectionCommitted(object sender, EventArgs e)
		{
			selection.GetPixelRect(out int left, out int top, out int width, out int height);

			if (selection.SelectionType == SelectionType.Replace)
			{
				selection.Clear();
				Map.ClearSelection();
				ClearSelectionUI();
			}
			SetRubberBandVisibility(Visibility.Hidden);

			List<BaseSpace> tilesInPixelRect = Map.GetTilesInPixelRect(left, top, width, height);
			List<BaseSpace> tilesOutsidePixelRect = Map.GetTilesOutsidePixelRect(left, top, width, height);

			SelectSpaces(tilesInPixelRect, true);
			SelectSpaces(tilesOutsidePixelRect, false);
		}

		private void SelectSpaces(List<BaseSpace> spaces, bool tileRubberBanded)
		{
			foreach (BaseSpace baseSpace in spaces)
			{
				if (baseSpace.SelectorPanel is FrameworkElement selectorPanel)
				{
					switch (selection.SelectionType)
					{
						case SelectionType.Replace:
							if (tileRubberBanded)
								Select(baseSpace, selectorPanel);
							else
								Deselect(baseSpace, selectorPanel);
							break;
						case SelectionType.Add:
							if (tileRubberBanded)
								Select(baseSpace, selectorPanel);
							break;
						case SelectionType.Remove:
							if (tileRubberBanded)
								Deselect(baseSpace, selectorPanel);
							break;
					}
				}
			}
		}

		private static void Deselect(BaseSpace baseSpace, FrameworkElement selectorPanel)
		{
			baseSpace.Selected = false;
			selectorPanel.Opacity = 0.8;
		}

		private static void Select(BaseSpace baseSpace, FrameworkElement selectorPanel)
		{
			baseSpace.Selected = true;
			selectorPanel.Opacity = 0.0;
		}

		private void SetRubberBandVisibility(Visibility visibility)
		{
			rectOutsideSelector.Visibility = visibility;
			rectInsideSelector.Visibility = visibility;
			rectFillSelector.Visibility = visibility;
		}

		void ClearSelection()
		{
			selection.Clear();
			ClearSelectionUI();
		}

		private void ClearSelectionUI()
		{
			foreach (BaseSpace baseSpace in Map.Spaces)
				if (baseSpace.SelectorPanel is FrameworkElement selectorPanel)
					selectorPanel.Opacity = 0;
		}

		private void Selection_SelectionChanged(object sender, EventArgs e)
		{
			ShowSelectionRubberBand();
			//if (selection.SelectionType == SelectionType.Replace)
			//	ClearSelectionUI();
		}
		void ShowSelectionRubberBand()
		{
			selection.GetPixelRect(out int left, out int top, out int width, out int height);
			SetSelectorSize(rectOutsideSelector, left, top, width, height);
			int margin = selection.BorderThickness;
			SetSelectorSize(rectInsideSelector, left + margin, top + margin, width - 2* margin, height - 2 * margin);
			SetSelectorSize(rectFillSelector, left, top, width, height);
		}

		private void SetSelectorSize(Rectangle selector, int left, int top, int width, int height)
		{
			Canvas.SetLeft(selector, left);
			Canvas.SetTop(selector, top);
			selector.Width = width;
			selector.Height = height;
			selector.Visibility = Visibility.Visible;
		}

		private Rectangle GetRoomFloor()
		{
			return GetTileRectangle(Color.FromRgb(255, 161, 161));
		}
		private FrameworkElement GetSelectorPanel()
		{
			Rectangle selector = GetTileRectangle(Colors.DimGray);
			selector.MouseDown += Selector_MouseDown;
			selector.Opacity = 0;
			return selector;
		}

		private void Selector_MouseDown(object sender, MouseButtonEventArgs e)
		{
			BaseSpace baseSpace = GetBaseSpace(sender);
			if (baseSpace == null)
				return;

			SelectionType selectionType = SelectionType.Replace;
			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
				selectionType = SelectionType.Add;
			else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
				selectionType = SelectionType.Remove;

			if (selectionType == SelectionType.Replace)
				ClearSelection();

			SetRubberBandVisibility(Visibility.Visible);
			selection.StartSelection(baseSpace, selectionType);
			cvsMap.CaptureMouse();
		}

		BaseSpace GetBaseSpace(object sender)
		{
			if (sender is FrameworkElement frameworkElement && frameworkElement.Tag is BaseSpace baseSpace)
				return baseSpace;
			return null;
		}

		private Rectangle GetCorridorFloor()
		{
			return GetTileRectangle(Color.FromRgb(161, 183, 255));
		}
		private Rectangle GetTileRectangle(Color fillColor)
		{
			Rectangle floor = new Rectangle();
			floor.Width = Map.pixelsPerTile + 1;
			floor.Height = Map.pixelsPerTile + 1;
			floor.Fill = new SolidColorBrush(fillColor);
			return floor;
		}

		void LoadMap(string fileName)
		{
			cvsMap.Children.Clear();
			Map.Load(fileName);
			foreach (BaseSpace tile in Map.Spaces)
			{
				UIElement floorUI = null;

				if (tile is FloorSpace)
				{
					if (tile.SpaceType == SpaceType.Room)
						floorUI = GetRoomFloor();
					else if (tile.SpaceType == SpaceType.Corridor)
						floorUI = GetCorridorFloor();
					else
						continue;
					AddElementOverTile(tile, floorUI);
				}

				FrameworkElement selectorPanel = GetSelectorPanel();
				selectorPanel.Tag = tile;
				tile.SelectorPanel = selectorPanel;
				AddElementOverTile(tile, selectorPanel);
			}

			cvsMap.Width = Map.Width;
			cvsMap.Height = Map.Height;

			rectOutsideSelector = new Rectangle();
			rectOutsideSelector.Stroke = Brushes.Black;
			rectOutsideSelector.StrokeThickness = selection.BorderThickness;
			rectOutsideSelector.Opacity = 0.85;
			rectInsideSelector = new Rectangle();
			rectInsideSelector.Stroke = Brushes.White;
			rectInsideSelector.Opacity = 0.85;
			rectInsideSelector.StrokeThickness = selection.BorderThickness;
			rectFillSelector = new Rectangle();
			rectFillSelector.Fill = Brushes.Blue;
			rectFillSelector.Opacity = 0.25;
			SetRubberBandVisibility(Visibility.Hidden);

			cvsMap.Children.Add(rectFillSelector);
			cvsMap.Children.Add(rectInsideSelector);
			cvsMap.Children.Add(rectOutsideSelector);
		}

		Rectangle rectOutsideSelector;
		Rectangle rectInsideSelector;
		Rectangle rectFillSelector;

		private void AddElementOverTile(BaseSpace tile, UIElement element)
		{
			Canvas.SetLeft(element, tile.Column * Map.pixelsPerTile);
			Canvas.SetTop(element, tile.Row * Map.pixelsPerTile);
			cvsMap.Children.Add(element);
		}

		BaseSpace GetBaseSpaceUnderMouse(Point position)
		{
			Map.PixelsToColumnRow(position.X, position.Y, out int column, out int row);
			return Map.GetBaseSpace(column, row);
		}

		private void CvsMap_MouseMove(object sender, MouseEventArgs e)
		{
			if (!selection.Selecting)
				return;

			BaseSpace baseSpace = GetBaseSpaceUnderMouse(e.GetPosition(cvsMap));
			if (baseSpace != null)
				selection.SelectTo(baseSpace);
		}

		private void CvsMap_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (!selection.Selecting)
				return;
			cvsMap.ReleaseMouseCapture();
			selection.Commit();
		}
	}
}
