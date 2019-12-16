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
using System.Timers;

namespace DndMapSpike
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		MapSelection selection;
		int clickCounter;
		public Map Map { get; set; }
		public MainWindow()
		{
			clickTimer = new Timer(225);
			clickTimer.Elapsed += new ElapsedEventHandler(EvaluateClicks);
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

			if (selection.SelectionType == SelectionType.Replace)
			{
				Map.ClearSelection();
				ClearSelectionUI();
			}
			SetRubberBandVisibility(Visibility.Hidden);
			ShowSelection();

			if (selection.SelectionType == SelectionType.Replace)
			{
				selection.Clear();
			}
		}

		private void ShowSelection()
		{
			selection.GetPixelRect(out int left, out int top, out int width, out int height);
			List<Tile> tilesInPixelRect = Map.GetTilesInPixelRect(left, top, width, height);
			List<Tile> tilesOutsidePixelRect = Map.GetTilesOutsidePixelRect(left, top, width, height);

			SelectSpaces(tilesInPixelRect, true);
			SelectSpaces(tilesOutsidePixelRect, false);
			CreateSelectionOutline();
		}

		private void CreateSelectionOutline()
		{
			cvsMap.Children.Remove(outline);
			outline = new System.Windows.Shapes.Path();

			PathGeometry allLines = new PathGeometry();

			// BUG: Pretty sure we are not drawing any lines when the selections ends at the far right/bottom column/row of the map.

			for (int column = 0; column < Map.NumColumns; column++)
			{
				bool inSelection = false;
				for (int row = 0; row < Map.NumRows; row++)
				{
					Tile tile = Map.AllTiles[column, row];
					if (!inSelection && tile.Selected)    // Add top line...
					{
						inSelection = true;
						AddHorizontalLine(allLines, tile);
					}
					else if (inSelection && !tile.Selected)   // Add bottom line...
					{
						inSelection = false;
						AddHorizontalLine(allLines, tile);
					}
					if (row == Map.NumRows - 1 && inSelection)  // Bottommost tile and still in selection...
						AddHorizontalLine(allLines, tile, Map.TileSizePx);
				}
			}

			for (int row = 0; row < Map.NumRows; row++)
			{
				bool inSelection = false;
				for (int column = 0; column < Map.NumColumns; column++)
				{
					Tile tile = Map.AllTiles[column, row];
					if (!inSelection && tile.Selected)    // Add left line...
					{
						inSelection = true;
						AddVerticalLine(allLines, tile);
					}
					else if (inSelection && !tile.Selected)   // Add right line...
					{
						inSelection = false;
						AddVerticalLine(allLines, tile);
					}
					if (column == Map.NumColumns - 1 && inSelection)  // Rightmost tile and still in selection...
						AddVerticalLine(allLines, tile, Map.TileSizePx);
				}
			}

			outline.Stroke = new SolidColorBrush(Color.FromRgb(0, 72, 255));
			outline.StrokeThickness = 10;
			outline.Data = allLines;
			cvsMap.Children.Add(outline);
		}

		private static void AddHorizontalLine(PathGeometry allLines, Tile tile, int offset = 0)
		{
			tile.GetPixelCoordinates(out int left, out int top, out int right, out int _);
			allLines.AddGeometry(new LineGeometry(new Point(left, top + offset), new Point(right, top + offset)));
		}

		private static void AddVerticalLine(PathGeometry allLines, Tile tile, int offset = 0)
		{
			tile.GetPixelCoordinates(out int left, out int top, out int _, out int bottom);
			allLines.AddGeometry(new LineGeometry(new Point(left + offset, top), new Point(left + offset, bottom)));
		}

		System.Windows.Shapes.Path outline;

		private void SelectSpaces(List<Tile> spaces, bool tileRubberBanded)
		{
			foreach (Tile baseSpace in spaces)
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

		private static void Deselect(Tile baseSpace, FrameworkElement selectorPanel)
		{
			baseSpace.Selected = false;
			selectorPanel.Opacity = 0.8;
		}

		private static void Select(Tile baseSpace, FrameworkElement selectorPanel)
		{
			baseSpace.Selected = true;
			selectorPanel.Opacity = 0.0;
		}

		private void SetRubberBandVisibility(Visibility visibility)
		{
			rubberBandOutsideSelector.Visibility = visibility;
			rubberBandInsideSelector.Visibility = visibility;
			rubberBandFillSelector.Visibility = visibility;
		}

		void ClearSelection()
		{
			cvsMap.Children.Remove(outline);
			selection.Clear();
			ClearSelectionUI();
		}

		private void ClearSelectionUI()
		{
			foreach (Tile baseSpace in Map.Spaces)
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
			SetSelectorSize(rubberBandOutsideSelector, left, top, width, height);
			int margin = selection.BorderThickness;
			SetSelectorSize(rubberBandInsideSelector, left + margin, top + margin, width - 2 * margin, height - 2 * margin);
			SetSelectorSize(rubberBandFillSelector, left, top, width, height);
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

		FrameworkElement mouseDownSender;

		Tile GetBaseSpace(FrameworkElement frameworkElement)
		{
			if (frameworkElement == null)
				return null;
			if (frameworkElement.Tag is Tile baseSpace)
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
			floor.Width = Map.TileSizePx + 1;
			floor.Height = Map.TileSizePx + 1;
			floor.Fill = new SolidColorBrush(fillColor);
			return floor;
		}

		void LoadMap(string fileName)
		{
			cvsMap.Children.Clear();
			Map.Load(fileName);
			foreach (Tile tile in Map.Spaces)
			{
				UIElement floorUI = null;

				if (tile is FloorSpace)
				{
					if (tile.SpaceType == SpaceType.Room)
						floorUI = GetRoomFloor();
					else if (tile.SpaceType == SpaceType.Corridor)
						floorUI = GetCorridorFloor();

					AddElementOverTile(tile, floorUI);
				}

				AddSelector(tile);
			}

			SetCanvasSizeFromMap();
			BuildRubberBandSelector();
		}

		private void AddSelector(Tile tile)
		{
			FrameworkElement selectorPanel = GetSelectorPanel();
			selectorPanel.Tag = tile;
			tile.SelectorPanel = selectorPanel;
			AddElementOverTile(tile, selectorPanel);
		}

		private void SetCanvasSizeFromMap()
		{
			cvsMap.Width = Map.WidthPx;
			cvsMap.Height = Map.HeightPx;
		}

		private void BuildRubberBandSelector()
		{
			rubberBandOutsideSelector = new Rectangle();
			rubberBandOutsideSelector.Stroke = Brushes.Black;
			rubberBandOutsideSelector.StrokeThickness = selection.BorderThickness;
			rubberBandOutsideSelector.Opacity = 0.85;
			rubberBandInsideSelector = new Rectangle();
			rubberBandInsideSelector.Stroke = Brushes.White;
			rubberBandInsideSelector.Opacity = 0.85;
			rubberBandInsideSelector.StrokeThickness = selection.BorderThickness;
			rubberBandFillSelector = new Rectangle();
			rubberBandFillSelector.Fill = Brushes.Blue;
			rubberBandFillSelector.Opacity = 0.25;
			SetRubberBandVisibility(Visibility.Hidden);

			cvsMap.Children.Add(rubberBandFillSelector);
			cvsMap.Children.Add(rubberBandInsideSelector);
			cvsMap.Children.Add(rubberBandOutsideSelector);
		}

		Rectangle rubberBandOutsideSelector;
		Rectangle rubberBandInsideSelector;
		Rectangle rubberBandFillSelector;
		Timer clickTimer;

		private void AddElementOverTile(Tile tile, UIElement element)
		{
			if (element == null)
				return;
			Canvas.SetLeft(element, tile.Column * Map.TileSizePx);
			Canvas.SetTop(element, tile.Row * Map.TileSizePx);
			cvsMap.Children.Add(element);
		}

		Tile GetBaseSpaceUnderMouse(Point position)
		{
			Map.PixelsToColumnRow(position.X, position.Y, out int column, out int row);
			return Map.GetBaseSpace(column, row);
		}

		private void Selector_MouseDown(object sender, MouseButtonEventArgs e)
		{
			mouseIsDown = true;
			mouseDownSender = sender as FrameworkElement;
			clickTimer.Stop();
			clickCounter++;
			clickTimer.Start();
		}

		bool mouseIsDown;
		private void CvsMap_MouseMove(object sender, MouseEventArgs e)
		{
			if (!selection.Selecting)
				return;

			Tile baseSpace = GetBaseSpaceUnderMouse(e.GetPosition(cvsMap));
			if (baseSpace != null)
				selection.SelectTo(baseSpace);
		}

		private void CvsMap_MouseUp(object sender, MouseButtonEventArgs e)
		{
			mouseIsDown = false;
			if (!selection.Selecting)
				return;
			cvsMap.ReleaseMouseCapture();
			selection.Commit();
		}

		private void EvaluateClicks(object source, ElapsedEventArgs e)
		{
			clickTimer.Stop();
			
			Dispatcher.Invoke(() =>
			{
				Tile baseSpace = GetBaseSpace(mouseDownSender);
				if (baseSpace != null)
				{
					if (clickCounter == 1)
					{
						if (mouseIsDown)
						{
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
					}
					else if (clickCounter == 2)
					{

					}
				}
			});

			clickCounter = 0;
		}
	}
}

