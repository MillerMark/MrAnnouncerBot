﻿using System;
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
using MapUI;

namespace DndMapSpike
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		const double DBL_SelectionLineThickness = 10;
		const double DBL_HalfSelectionLineThickness = DBL_SelectionLineThickness / 2;
		#region Fields from ZoomAndPan example
		/// <summary>
		/// Specifies the current state of the mouse handling logic.
		/// </summary>
		private MouseHandlingMode mouseHandlingMode = MouseHandlingMode.None;

		/// <summary>
		/// The point that was clicked relative to the ZoomAndPanControl.
		/// </summary>
		private Point origZoomAndPanControlMouseDownPoint;

		/// <summary>
		/// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
		/// </summary>
		private Point origContentMouseDownPoint;

		/// <summary>
		/// Records which mouse button clicked during mouse dragging.
		/// </summary>
		private MouseButton mouseButtonDown;

		/// <summary>
		/// Saves the previous zoom rectangle, pressing the backspace key jumps back to this zoom rectangle.
		/// </summary>
		private Rect prevZoomRect;

		/// <summary>
		/// Save the previous content scale, pressing the backspace key jumps back to this scale.
		/// </summary>
		private double prevZoomScale;

		/// <summary>
		/// Set to 'true' when the previous zoom rect is saved.
		/// </summary>
		private bool prevZoomRectSet = false;

		#endregion
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
			SelectAllTiles(tilesInPixelRect, tilesOutsidePixelRect);
		}

		private void SelectAllTiles(List<Tile> tilesToSelect, List<Tile> tilesNotSelected = null)
		{
			SelectTiles(tilesToSelect, true);
			if (tilesNotSelected == null)
				tilesNotSelected = Map.GetAllOtherSpaces(tilesToSelect);
			SelectTiles(tilesNotSelected, false);
			CreateSelectionOutline();
		}

		private void CreateSelectionOutline()
		{
			content.Children.Remove(outline);
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
			outline.StrokeThickness = DBL_SelectionLineThickness;
			outline.Data = allLines;
			content.Children.Add(outline);
		}

		private static void AddHorizontalLine(PathGeometry allLines, Tile tile, int offset = 0)
		{
			tile.GetPixelCoordinates(out int left, out int top, out int right, out int _);
			allLines.AddGeometry(new LineGeometry(new Point(left - DBL_HalfSelectionLineThickness, top + offset), new Point(right + DBL_HalfSelectionLineThickness, top + offset)));
		}

		private static void AddVerticalLine(PathGeometry allLines, Tile tile, int offset = 0)
		{
			tile.GetPixelCoordinates(out int left, out int top, out int _, out int bottom);
			allLines.AddGeometry(new LineGeometry(new Point(left + offset, top - DBL_HalfSelectionLineThickness), new Point(left + offset, bottom + DBL_HalfSelectionLineThickness)));
		}

		System.Windows.Shapes.Path outline;

		private void SelectTiles(List<Tile> tiles, bool select)
		{
			foreach (Tile tile in tiles)
			{
				if (tile.SelectorPanel is FrameworkElement selectorPanel)
				{
					switch (selection.SelectionType)
					{
						case SelectionType.Replace:
							if (select)
								Select(tile, selectorPanel);
							else
								Deselect(tile, selectorPanel);
							break;
						case SelectionType.Add:
							if (select)
								Select(tile, selectorPanel);
							break;
						case SelectionType.Remove:
							if (select)
								Deselect(tile, selectorPanel);
							break;
					}
				}
			}
		}

		private static void Deselect(Tile tile, FrameworkElement selectorPanel)
		{
			tile.Selected = false;
			selectorPanel.Opacity = 0.8;
		}

		private static void Select(Tile tile, FrameworkElement selectorPanel)
		{
			tile.Selected = true;
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
			content.Children.Remove(outline);
			outline = new System.Windows.Shapes.Path();
			selection.Clear();
			Map.ClearSelection();
			ClearSelectionUI();
		}

		private void ClearSelectionUI()
		{
			foreach (Tile tile in Map.Tiles)
				if (tile.SelectorPanel is FrameworkElement selectorPanel)
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
			int margin = selection.BorderThickness;
			selection.GetPixelRect(out int left, out int top, out int width, out int height);
			SetSelectorSize(rubberBandOutsideSelector, left - margin, top - margin, width + 2 * margin, height + 2 * margin);
			SetSelectorSize(rubberBandInsideSelector, left, top, width, height);
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

		Tile GetTile(FrameworkElement frameworkElement)
		{
			if (frameworkElement == null)
				return null;
			if (frameworkElement.Tag is Tile tile)
				return tile;
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
			content.Children.Clear();
			Map.Load(fileName);
			foreach (Tile tile in Map.Tiles)
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
			content.Width = Map.WidthPx;
			content.Height = Map.HeightPx;
			theGrid.Width = Map.WidthPx;
			theGrid.Height = Map.HeightPx;
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

			content.Children.Add(rubberBandFillSelector);
			content.Children.Add(rubberBandInsideSelector);
			content.Children.Add(rubberBandOutsideSelector);
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
			content.Children.Add(element);
		}

		Tile GetTileUnderMouse(Point position)
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

			Tile tile = GetTileUnderMouse(e.GetPosition(content));
			if (tile != null)
				selection.SelectTo(tile);
		}

		private void CvsMap_MouseUp(object sender, MouseButtonEventArgs e)
		{
			mouseIsDown = false;
			if (!selection.Selecting)
				return;
			content.ReleaseMouseCapture();
			selection.Commit();
		}

private void ZoomOut_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ZoomOut(new Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));
		}

		private void ZoomIn_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ZoomIn(new Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));
		}

		private void JumpBackToPrevZoom_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			JumpBackToPrevZoom();
		}

		private void JumpBackToPrevZoom_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = prevZoomRectSet;
		}

		private void Fill_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			SavePrevZoomRect();
			zoomAndPanControl.AnimatedScaleToFit();
		}

		private void OneHundredPercent_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			SavePrevZoomRect();

			zoomAndPanControl.AnimatedZoomTo(1.0);
		}

		/// <summary>
		/// Jump back to the previous zoom level.
		/// </summary>
		private void JumpBackToPrevZoom()
		{
			zoomAndPanControl.AnimatedZoomTo(prevZoomScale, prevZoomRect);

			ClearPrevZoomRect();
		}

		/// <summary>
		/// Clear the memory of the previous zoom level.
		/// </summary>
		private void ClearPrevZoomRect()
		{
			prevZoomRectSet = false;
		}

		/// <summary>
		/// Zoom the viewport out, centering on the specified point (in content coordinates).
		/// </summary>
		private void ZoomOut(Point contentZoomCenter)
		{
			zoomAndPanControl.ZoomAboutPoint(zoomAndPanControl.ContentScale - 0.1, contentZoomCenter);
		}

		/// <summary>
		/// Zoom the viewport in, centering on the specified point (in content coordinates).
		/// </summary>
		private void ZoomIn(Point contentZoomCenter)
		{
			zoomAndPanControl.ZoomAboutPoint(zoomAndPanControl.ContentScale + 0.1, contentZoomCenter);
		}

		private void ZoomAndPanControl_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (mouseHandlingMode != MouseHandlingMode.None)
			{
				if (mouseHandlingMode == MouseHandlingMode.Zooming)
				{
					if (mouseButtonDown == MouseButton.Left)
					{
						// Shift + left-click zooms in on the content.
						ZoomIn(origContentMouseDownPoint);
					}
					else if (mouseButtonDown == MouseButton.Right)
					{
						// Shift + left-click zooms out from the content.
						ZoomOut(origContentMouseDownPoint);
					}
				}
				else if (mouseHandlingMode == MouseHandlingMode.DragZooming)
				{
					// When drag-zooming has finished we zoom in on the rectangle that was highlighted by the user.
					ApplyDragZoomRect();
				}

				zoomAndPanControl.ReleaseMouseCapture();
				mouseHandlingMode = MouseHandlingMode.None;
				e.Handled = true;
			}
		}

		//
		// Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
		//
		private void SavePrevZoomRect()
		{
			prevZoomRect = new Rect(zoomAndPanControl.ContentOffsetX, zoomAndPanControl.ContentOffsetY, zoomAndPanControl.ContentViewportWidth, zoomAndPanControl.ContentViewportHeight);
			prevZoomScale = zoomAndPanControl.ContentScale;
			prevZoomRectSet = true;
		}

		/// <summary>
		/// When the user has finished dragging out the rectangle the zoom operation is applied.
		/// </summary>
		private void ApplyDragZoomRect()
		{
			//
			// Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
			//
			SavePrevZoomRect();

			//
			// Retrieve the rectangle that the user dragged out and zoom in on it.
			//
			double contentX = Canvas.GetLeft(dragZoomBorder);
			double contentY = Canvas.GetTop(dragZoomBorder);
			double contentWidth = dragZoomBorder.Width;
			double contentHeight = dragZoomBorder.Height;
			zoomAndPanControl.AnimatedZoomTo(new Rect(contentX, contentY, contentWidth, contentHeight));

			FadeOutDragZoomRect();
		}

		private void FadeOutDragZoomRect()
		{
			AnimationHelper.StartAnimation(dragZoomBorder, Border.OpacityProperty, 0.0, 0.1,
					delegate (object sender, EventArgs e)
					{
						dragZoomCanvas.Visibility = Visibility.Collapsed;
					});
		}

		/// <summary>
		/// Update the position and size of the rectangle that user is dragging out.
		/// </summary>
		private void SetDragZoomRect(Point pt1, Point pt2)
		{
			double x, y, width, height;

			//
			// Determine x,y,width and height of the rect inverting the points if necessary.
			// 

			if (pt2.X < pt1.X)
			{
				x = pt2.X;
				width = pt1.X - pt2.X;
			}
			else
			{
				x = pt1.X;
				width = pt2.X - pt1.X;
			}

			if (pt2.Y < pt1.Y)
			{
				y = pt2.Y;
				height = pt1.Y - pt2.Y;
			}
			else
			{
				y = pt1.Y;
				height = pt2.Y - pt1.Y;
			}

			//
			// Update the coordinates of the rectangle that is being dragged out by the user.
			// The we offset and rescale to convert from content coordinates.
			//
			Canvas.SetLeft(dragZoomBorder, x);
			Canvas.SetTop(dragZoomBorder, y);
			dragZoomBorder.Width = width;
			dragZoomBorder.Height = height;
		}

		/// <summary>
		/// Initialize the rectangle that the use is dragging out.
		/// </summary>
		private void InitDragZoomRect(Point pt1, Point pt2)
		{
			SetDragZoomRect(pt1, pt2);

			dragZoomCanvas.Visibility = Visibility.Visible;
			dragZoomBorder.Opacity = 0.5;
		}

		private void ZoomAndPanControl_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			e.Handled = true;

			if (e.Delta > 0)
			{
				Point curContentMousePoint = e.GetPosition(content);
				ZoomIn(curContentMousePoint);
				if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
					zoomAndPanControl.AnimatedSnapTo(curContentMousePoint);
			}
			else if (e.Delta < 0)
			{
				Point curContentMousePoint = e.GetPosition(content);
				ZoomOut(curContentMousePoint);
			}
			
		}

		private void ZoomAndPanControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			//if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
			//{
			//	Point doubleClickPoint = e.GetPosition(content);
			//	zoomAndPanControl.AnimatedSnapTo(doubleClickPoint);
			//}
		}

		private void ZoomAndPanControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (mouseHandlingMode == MouseHandlingMode.Panning)
			{
				//
				// The user is left-dragging the mouse.
				// Pan the viewport by the appropriate amount.
				//
				Point curContentMousePoint = e.GetPosition(content);
				Vector dragOffset = curContentMousePoint - origContentMouseDownPoint;

				zoomAndPanControl.ContentOffsetX -= dragOffset.X;
				zoomAndPanControl.ContentOffsetY -= dragOffset.Y;

				e.Handled = true;
			}
			else if (mouseHandlingMode == MouseHandlingMode.Zooming)
			{
				Point curZoomAndPanControlMousePoint = e.GetPosition(zoomAndPanControl);
				Vector dragOffset = curZoomAndPanControlMousePoint - origZoomAndPanControlMouseDownPoint;
				double dragThreshold = 10;
				if (mouseButtonDown == MouseButton.Left &&
						(Math.Abs(dragOffset.X) > dragThreshold ||
						 Math.Abs(dragOffset.Y) > dragThreshold))
				{
					//
					// When Shift + left-down zooming mode and the user drags beyond the drag threshold,
					// initiate drag zooming mode where the user can drag out a rectangle to select the area
					// to zoom in on.
					//
					mouseHandlingMode = MouseHandlingMode.DragZooming;
					Point curContentMousePoint = e.GetPosition(content);
					InitDragZoomRect(origContentMouseDownPoint, curContentMousePoint);
				}

				e.Handled = true;
			}
			else if (mouseHandlingMode == MouseHandlingMode.DragZooming)
			{
				//
				// When in drag zooming mode continously update the position of the rectangle
				// that the user is dragging out.
				//
				Point curContentMousePoint = e.GetPosition(content);
				SetDragZoomRect(origContentMouseDownPoint, curContentMousePoint);

				e.Handled = true;
			}
		}

		private void ZoomAndPanControl_MouseDown(object sender, MouseButtonEventArgs e)
		{
			content.Focus();
			Keyboard.Focus(content);

			mouseButtonDown = e.ChangedButton;
			origZoomAndPanControlMouseDownPoint = e.GetPosition(zoomAndPanControl);
			origContentMouseDownPoint = e.GetPosition(content);
			if (origContentMouseDownPoint.X < 0 || origContentMouseDownPoint.X > content.Width || origContentMouseDownPoint.Y < 0 || origContentMouseDownPoint.Y > theGrid.Height)
			{
				if (Map.SelectionExists())
					ClearSelection();
			}

			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt) && e.ChangedButton == MouseButton.Left)
			{
				// Alt + left initiates special zooming to rect mode.
				mouseHandlingMode = MouseHandlingMode.Zooming;
			}
			else if (mouseButtonDown == MouseButton.Left && Keyboard.IsKeyDown(Key.Space))
			{
				// Just left-down plus space bar down initiates panning mode.
				mouseHandlingMode = MouseHandlingMode.Panning;
			}

			if (mouseHandlingMode != MouseHandlingMode.None)
			{
				// Capture the mouse so that we eventually receive the mouse up event.
				zoomAndPanControl.CaptureMouse();
				e.Handled = true;
			}
		}

		void FloodSelect(Tile baseSpace, SelectionType selectionType)
		{
			if (selectionType == SelectionType.None)
				return;

			if (!(baseSpace is FloorSpace floorSpace))
				return;

			selection.SelectionType = selectionType;

			//if (selection.SelectionType == SelectionType.Replace)
			//{
			//	Map.ClearSelection();
			//	ClearSelectionUI();
			//}

			List<Tile> roomTiles = floorSpace.Parent.Spaces.ConvertAll(x => x as Tile);
			SelectAllTiles(roomTiles);
		}

		void FloodSelectAll(Tile baseSpace, SelectionType selectionType)
		{
			if (selectionType == SelectionType.None)
				return;

			if (!(baseSpace is FloorSpace floorSpace))
				return;

			List<Tile> allRegionTiles;

			if (floorSpace.Parent is Room)
			{
				allRegionTiles = Map.GetAllMatchingTiles<Room>();
			}
			else if (floorSpace.Parent is Corridor)
			{
				allRegionTiles = Map.GetAllMatchingTiles<Corridor>();
			}
			else
				return;

			SelectAllTiles(allRegionTiles);
		}
		private void EvaluateClicks(object source, ElapsedEventArgs e)
		{
			clickTimer.Stop();

			Dispatcher.Invoke(() =>
			{
				Tile baseSpace = GetTile(mouseDownSender);
				if (baseSpace != null)
				{
					if (clickCounter == 1)
					{
						if (mouseIsDown && !Keyboard.IsKeyDown(Key.Space))
						{
							if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)) // Alt key is reserved for zooming into a rect.
								return;
							SelectionType selectionType = GetSelectionType();

							if (selectionType == SelectionType.Replace)
								ClearSelection();

							SetRubberBandVisibility(Visibility.Visible);
							selection.StartSelection(baseSpace, selectionType);
							content.CaptureMouse();
						}
					}
					else if (clickCounter == 2)
					{
						FloodSelect(baseSpace, GetSelectionType());
					}
					else if (clickCounter == 3)
					{
						FloodSelectAll(baseSpace, GetSelectionType());
					}
				}
			});

			clickCounter = 0;
		}

		private SelectionType GetSelectionType()
		{
			SelectionType selectionType = SelectionType.Replace;
			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
				selectionType = SelectionType.Add;
			else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
				selectionType = SelectionType.Remove;

			if (selectionType == SelectionType.Add && !Map.SelectionExists())
				selectionType = SelectionType.Replace;
			return selectionType;
		}
	}
}

