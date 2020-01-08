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
using MapUI;
using System.Windows.Threading;

namespace DndMapSpike
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<Stamp> selectedStamps;
		public const string StampsPath = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\DragonH\Maps\Stamps";
		MapEditModes mapEditMode = MapEditModes.Select;
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
		List<BaseTexture> tileTextures = new List<BaseTexture>();
		List<BaseTexture> debrisTextures = new List<BaseTexture>();
		MapSelection tileSelection;
		WallBuilder wallBuilder = new WallBuilder();
		int clickCounter;
		public Map Map { get; set; }
		public MainWindow()
		{
			clickTimer = new Timer(225);
			clickTimer.Elapsed += new ElapsedEventHandler(EvaluateClicks);
			fileWatchTimer = new DispatcherTimer();
			fileWatchTimer.Interval = TimeSpan.FromMilliseconds(330);
			fileWatchTimer.Tick += FileWatchTimer_Tick;
			InitializeComponent();
			Map = new Map();
			Map.WallsChanged += Map_WallsChanged;
			tileSelection = new MapSelection();
			tileSelection.SelectionChanged += Selection_SelectionChanged;
			tileSelection.SelectionCommitted += Selection_SelectionCommitted;
			tileSelection.SelectionCancelled += Selection_SelectionCancelled;

			//ImportDonJonMap("The Delve of Lamprica.txt");
			//ImportDonJonMap("The Pit of Alan the Necromancer.txt");
			//ImportDonJonMap("The Barrow of Elemental Horror.txt");
			//ImportDonJonMap("The Hive of the Vampire Princess.txt");
			//ImportDonJonMap("The Dark Chambers of Ages.txt");
			//ImportDonJonMap("The Barrow of Emirkol the Chaotic.txt");
			InitializeLayers();
			LoadImageResources();
			//ImportDonJonMap("Test New Map.txt");
			//ImportDonJonMap("The Tomb of Baleful Ruin.txt");
			//ImportDonJonMap("The Dark Lair of Sorrows.txt");
			//ImportDonJonMap("The Forsaken Tunnels of Death.txt");
			ImportDonJonMap("The Dark Lair of the Demon Baron.txt");
			LoadFloorTiles();
			//LoadDebris();
			AddFileSystemWatcher();
			BuildStampsUI();
		}

		public List<StampFolder> Directories = new List<StampFolder>();

		void BuildStampsUI()
		{
			Directories.Clear();
			IEnumerable<string> directories = System.IO.Directory.EnumerateDirectories(StampsPath);
			foreach (string directory in directories)
			{
				Directories.Add(new StampFolder(directory));
			}
			tbcStamps.ItemsSource = Directories;
		}

		private void Map_WallsChanged(object sender, EventArgs e)
		{
			wallLayer.ClearAll();
			wallBuilder.BuildWalls(Map, wallLayer);
		}


		FileSystemWatcher fileSystemWatcher;
		void AddFileSystemWatcher()
		{
			fileSystemWatcher = new FileSystemWatcher(TextureUtils.TileFolder, "*.png");
			fileSystemWatcher.Changed += FileSystemWatcher_Changed;
			fileSystemWatcher.EnableRaisingEvents = true;
		}

		private void FileWatchTimer_Tick(object sender, EventArgs e)
		{
			fileWatchTimer.Stop();
			LoadFloorTiles();
		}

		private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
		{
			fileWatchTimer.Stop();
			fileWatchTimer.Start();
		}

		void ProcessRawPngFiles(string[] pngFiles, List<BaseTexture> textures)
		{
			textures.Clear();
			foreach (string fileName in pngFiles)
			{
				TextureUtils.GetTextureNameAndKey(fileName, out string baseName, out string textureName, out string imageKey);
				BaseTexture texture = textures.FirstOrDefault(x => x.BaseName == baseName);
				if (texture == null)
				{
					texture = TextureUtils.CreateTexture(baseName, textureName);
					textures.Add(texture);
				}

				texture.AddImage(fileName, imageKey);
			}
		}
		private void LoadFloorTiles()
		{
			string[] pngFiles = Directory.GetFiles(TextureUtils.TileFolder, "*.png");
			ProcessRawPngFiles(pngFiles, tileTextures);
			lstFlooring.ItemsSource = tileTextures;
		}

		private void Selection_SelectionCancelled(object sender, EventArgs e)
		{
			SetRubberBandVisibility(Visibility.Hidden);
		}

		private void Selection_SelectionCommitted(object sender, EventArgs e)
		{

			if (tileSelection.SelectionType == SelectionType.Replace)
			{
				Map.ClearSelection();
				ClearSelectionUI();
			}
			SetRubberBandVisibility(Visibility.Hidden);
			ShowSelection();

			bool selectionExists = true;
			if (tileSelection.SelectionType == SelectionType.Remove || tileSelection.SelectionType == SelectionType.None)
				selectionExists = Map.SelectionExists();

			if (tileSelection.SelectionType == SelectionType.Replace)
				tileSelection.Clear();

			EnableSelectionControls(selectionExists);
		}

		private void ShowSelection()
		{
			tileSelection.GetPixelRect(out int left, out int top, out int width, out int height);
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
			bool selectionExists = Map.SelectionExists();
			EnableSelectionControls(selectionExists);
			if (!selectionExists)
			{
				ClearSelectionUI();
			}
		}

		void EnableSelectionControls(bool isEnabled)
		{
			lstFlooring.IsEnabled = isEnabled;
			//lstDebris.IsEnabled = isEnabled;
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
						AddHorizontalLine(allLines, tile, Tile.Height);
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
						AddVerticalLine(allLines, tile, Tile.Width);
				}
			}

			outline.Stroke = new SolidColorBrush(Color.FromRgb(0, 72, 255));
			outline.StrokeThickness = DBL_SelectionLineThickness;
			outline.Data = allLines;
			Panel.SetZIndex(outline, int.MaxValue);
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
					switch (tileSelection.SelectionType)
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
			selectorPanel.Opacity = 0.4;
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
			tileSelection.Clear();
			Map.ClearSelection();
			ClearSelectionUI();
			EnableSelectionControls(false);
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
			tileSelection.GetPixelRect(out int left, out int top, out int width, out int height);
			ShowSelectionRubberBand(left, top, width, height);
		}

		private void ShowSelectionRubberBand(int left, int top, int width, int height)
		{
			int margin = tileSelection.BorderThickness;

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
			Panel.SetZIndex(selector, int.MaxValue);
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
			floor.Width = Tile.Width;
			floor.Height = Tile.Height;
			floor.Fill = new SolidColorBrush(fillColor);
			return floor;
		}

		Image doorLightHorizontal;
		Image doorLightVertical;

		void AddDoors(Tile tile)
		{
			if (!(tile is FloorSpace floorSpace))
				return;

			if (!floorSpace.HasDoors)
				return;

			foreach (Door door in floorSpace.Doors)
			{
				int xOffset;
				int yOffset;
				Image image;
				if (door.Position == DoorPosition.Bottom || door.Position == DoorPosition.Top)
				{
					xOffset = (Tile.Width - Door.Span) / 2;
					yOffset = (Tile.Height - Door.Thickness) / 2;
					image = doorLightHorizontal;
					if (door.Position == DoorPosition.Bottom)
						yOffset += Tile.Height / 2;
					else
						yOffset -= Tile.Height / 2;
				}
				else
				{
					xOffset = (Tile.Width - Door.Thickness) / 2;
					yOffset = (Tile.Height - Door.Span) / 2;
					image = doorLightVertical;
					if (door.Position == DoorPosition.Right)
						xOffset += Tile.Width / 2;
					else
						xOffset -= Tile.Width / 2;
				}

				doorLayer.BlendImageOverTile(image, tile, xOffset, yOffset);
			}
		}

		void ImportDonJonMap(string fileName)
		{
			content.Children.Clear();
			Map.Load(fileName);
			allLayers.AddImagesToCanvas(content);
			SetCanvasSizeFromMap();

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
					AddFloorOverlay(tile);
					AddDoors(tile);
				}

				AddSelector(tile);
			}

			LoadFinalCanvasElements();
			SelectAllRoomsAndCorridors();
			OutlineWalls();
			ClearSelection();
		}

		void SelectAllRooms()
		{
			FloodSelectAll<Room>(SelectionType.Add);
		}

		void SelectAllCorridors()
		{
			FloodSelectAll<Corridor>(SelectionType.Add);
		}

		void SelectAllRoomsAndCorridors()
		{
			SelectAllRooms();
			SelectAllCorridors();
		}

		private void AddSelector(Tile tile)
		{
			FrameworkElement selectorPanel = GetSelectorPanel();
			selectorPanel.Tag = tile;
			tile.SelectorPanel = selectorPanel;
			AddElementOverTile(tile, selectorPanel);
			selectorPanel.IsHitTestVisible = true;
		}

		private void LoadFinalCanvasElements()
		{
			int numChildren = content.Children.Count;
			allLayers.SetZIndex(numChildren);
			BuildRubberBandSelector(numChildren + allLayers.Count);
		}

		private void BuildRubberBandSelector(int zIndex)
		{
			rubberBandOutsideSelector = new Rectangle();
			rubberBandOutsideSelector.Stroke = Brushes.Black;
			rubberBandOutsideSelector.StrokeThickness = tileSelection.BorderThickness;
			rubberBandOutsideSelector.Opacity = 0.85;

			rubberBandInsideSelector = new Rectangle();
			rubberBandInsideSelector.Stroke = Brushes.White;
			rubberBandInsideSelector.Opacity = 0.85;
			rubberBandInsideSelector.StrokeThickness = tileSelection.BorderThickness;

			rubberBandFillSelector = new Rectangle();
			rubberBandFillSelector.Fill = Brushes.Blue;
			rubberBandFillSelector.Opacity = 0.25;
			SetRubberBandVisibility(Visibility.Hidden);

			zIndex++;
			content.Children.Add(rubberBandFillSelector);
			Panel.SetZIndex(rubberBandFillSelector, zIndex++);
			content.Children.Add(rubberBandInsideSelector);
			Panel.SetZIndex(rubberBandInsideSelector, zIndex++);
			content.Children.Add(rubberBandOutsideSelector);
			Panel.SetZIndex(rubberBandOutsideSelector, zIndex++);
		}

		private void RemoveRubberBandSelector()
		{
			content.Children.Remove(rubberBandFillSelector);
			content.Children.Remove(rubberBandInsideSelector);
			content.Children.Remove(rubberBandOutsideSelector);
		}

		Rectangle rubberBandOutsideSelector;
		Rectangle rubberBandInsideSelector;
		Rectangle rubberBandFillSelector;
		Timer clickTimer;
		DispatcherTimer fileWatchTimer;

		private void AddElementOverTile(Tile tile, UIElement element)
		{
			if (element == null)
				return;
			tile.UIElementFloor = element;
			element.IsHitTestVisible = false;
			Canvas.SetLeft(element, tile.PixelX);
			Canvas.SetTop(element, tile.PixelY);
			content.Children.Add(element);
		}

		Tile GetTileUnderMouse(Point position)
		{
			Map.PixelsToColumnRow(position.X, position.Y, out int column, out int row);
			return Map.GetTile(column, row);
		}

		Point lastMouseDownPoint;
		DateTime lastMouseDownTime;
		private void Selector_MouseDown(object sender, MouseButtonEventArgs e)
		{
			mouseIsDown = true;
			lastMouseDownEventArgs = e;
			lastMouseDownPoint = lastMouseDownEventArgs.GetPosition(content);
			lastMouseDownTime = DateTime.Now;
			mouseDownSender = sender as FrameworkElement;
			clickTimer.Stop();
			clickCounter++;
			clickTimer.Start();
		}

		bool mouseIsDown;
		private void CvsMap_MouseMove(object sender, MouseEventArgs e)
		{
			if (!tileSelection.Selecting)
				return;

			Tile tile = GetTileUnderMouse(e.GetPosition(content));
			if (tile != null)
				tileSelection.SelectTo(tile);
		}

		private void CvsMap_MouseUp(object sender, MouseButtonEventArgs e)
		{
			mouseIsDown = false;
			if (!tileSelection.Selecting)
				return;
			content.ReleaseMouseCapture();
			tileSelection.Commit();
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
			zoomAndPanControl.ZoomAboutPoint(zoomAndPanControl.ContentScale * 0.9, contentZoomCenter);
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
			if (draggingStamps)
			{
				Point cursor = e.GetPosition(content);
				StopDraggingStamps(cursor);
				return;
			}
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

		bool draggingStamps;
		private void ZoomAndPanControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (activeStampCanvas != null)
			{
				Point cursor = e.GetPosition(content);
				if (draggingStamps && e.LeftButton == MouseButtonState.Released)
				{
					StopDraggingStamps(cursor);
					return;
				}
				Canvas.SetLeft(activeStampCanvas, cursor.X - stampMouseOffsetX);
				Canvas.SetTop(activeStampCanvas, cursor.Y - stampMouseOffsetY);
				return;
			}
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

		private void StopDraggingStamps(Point curContentMousePoint)
		{
			double secondsSinceMouseDown = (DateTime.Now - lastMouseDownTime).TotalSeconds;
			int deltaX = (int)Math.Round(curContentMousePoint.X - lastMouseDownPoint.X);
			int deltaY = (int)Math.Round(curContentMousePoint.Y - lastMouseDownPoint.Y);
			int totalXY = Math.Abs(deltaX) + Math.Abs(deltaY);
			if (secondsSinceMouseDown > 0.6 || secondsSinceMouseDown > 0.3 && totalXY > 20)
			{
				MoveSelectedStamps(deltaX, deltaY);
			}

			draggingStamps = false;
			if (activeStampCanvas != null)
			{
				content.Children.Remove(activeStampCanvas);
				activeStampCanvas = null;
			}
		}

		private void MoveSelectedStamps(int deltaX, int deltaY)
		{
			stampsLayer.BeginUpdate();
			try
			{
				foreach (Stamp stamp in SelectedStamps)
					stamp.Move(deltaX, deltaY);
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			StampsAreSelected();
		}

		private void ZoomAndPanControl_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (MapEditMode != MapEditModes.Select)
				return;
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

			tileSelection.SelectionType = selectionType;

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

			if (floorSpace.Parent is Room)
				FloodSelectAll<Room>(selectionType);
			else if (floorSpace.Parent is Corridor)
				FloodSelectAll<Corridor>(selectionType);
		}

		private void FloodSelectAll<T>(SelectionType selectionType) where T : MapRegion
		{
			tileSelection.SelectionType = selectionType;
			List<Tile> allRegionTiles = Map.GetAllMatchingTiles<T>();
			SelectAllTiles(allRegionTiles);
		}

		private void EvaluateClicks(object source, ElapsedEventArgs e)
		{
			clickTimer.Stop();

			try
			{
				if (MapEditMode == MapEditModes.Stamp)
					HandleStampMouseDown();
				else if (MapEditMode == MapEditModes.Select)
					HandleMouseDownSelectForceUI();
			}
			finally
			{
				clickCounter = 0;
			}
		}

		private void HandleStampMouseDown()
		{
			if (activeStamp != null && lastMouseDownEventArgs != null)
			{
				AddStampAtLastMouseDownForceUI();
			}
		}

		private void AddStampAtLastMouseDownForceUI()
		{
			Dispatcher.Invoke(() =>
			{
				AddStampAtLastMouseDown();
			});
		}

		private void AddStampAtLastMouseDown()
		{
			int x = (int)Math.Round(lastMouseDownPoint.X);
			int y = (int)Math.Round(lastMouseDownPoint.Y);
			stampsLayer.AddStampNow(new Stamp(activeStamp.FileName, x, y));
		}

		private void HandleMouseDownSelectForceUI()
		{
			Dispatcher.Invoke(() =>
			{
				HandleMouseDownSelect();
			});
		}

		Stamp GetStampAt(Point point)
		{
			Stamp stamp = stampsLayer.GetStampAt(point);
			return stamp;
		}

		
		public List<Stamp> SelectedStamps
		{
			get
			{
				if (selectedStamps == null)
					selectedStamps = new List<Stamp>();

				return selectedStamps;
			}
			set
			{
				selectedStamps = value;
			}
		}

		void GetStampSelectedBounds(out int left, out int top, out int width, out int height)
		{
			left = int.MaxValue;
			top = int.MaxValue;
			int right = 0;
			int bottom = 0;
			foreach (Stamp stamp in SelectedStamps)
			{
				int stampLeft = stamp.GetLeft();
				int stampTop = stamp.GetTop();
				int stampRight = stampLeft + stamp.Width;
				int stampBottom = stampTop + stamp.Height;
				if (stampLeft < left)
					left = stampLeft;
				if (stampTop < top)
					top = stampTop;
				if (stampRight > right)
					right = stampRight;
				if (stampBottom > bottom)
					bottom = stampBottom;
			}
			width = Math.Max(0, right - left);
			height = Math.Max(0, bottom - top);
		}
		private void HandleMouseDownSelect()
		{
			Stamp stamp = GetStampAt(lastMouseDownPoint);
			if (stamp != null)
			{
				ClearSelection();
				SelectionType selectionType = GetSelectionType(SelectedStamps.Count > 0);
				if (selectionType == SelectionType.Replace)
				{
					if (SelectedStamps.IndexOf(stamp) < 0)
						SelectedStamps.Clear();
				}

				if (selectionType == SelectionType.Remove)
				{
					SelectedStamps.Remove(stamp);
				}
				else if (SelectedStamps.IndexOf(stamp) < 0)
					SelectedStamps.Add(stamp);


				if (SelectedStamps.Count > 0)
					StampsAreSelected();
				else
					StampsAreNotSelected();
				return;
			}

			Tile baseSpace = GetTile(mouseDownSender);
			if (baseSpace != null)
				HandleTileClick(baseSpace);
		}

		private void StampsAreNotSelected()
		{
			SetRubberBandVisibility(Visibility.Hidden);
			ShowTilingControls();
		}

		private void ShowTilingControls()
		{
			spWallControls.Visibility = Visibility.Visible;
			spWallLabels.Visibility = Visibility.Visible;
			spDoorLabels.Visibility = Visibility.Visible;
			spDoorControls.Visibility = Visibility.Visible;
			lstFlooring.Visibility = Visibility.Visible;
		}

		void CreateFloatingStampsForSelection()
		{
			draggingStamps = true;
			if (activeStampCanvas != null)
				activeStampCanvas.Children.Clear();
			foreach (Stamp stamp in SelectedStamps)
				CreateFloatingStamp(stamp);
		}
		private void StampsAreSelected()
		{
			GetStampSelectedBounds(out int left, out int top, out int width, out int height);
			ShowSelectionRubberBand(left, top, width, height);
			spWallControls.Visibility = Visibility.Collapsed;
			spWallLabels.Visibility = Visibility.Collapsed;
			spDoorLabels.Visibility = Visibility.Collapsed;
			spDoorControls.Visibility = Visibility.Collapsed;
			lstFlooring.Visibility = Visibility.Collapsed;
			CreateFloatingStampsForSelection();
		}

		private void HandleTileClick(Tile baseSpace)
		{
			if (clickCounter == 1)
				HandleSingleClickTileSelect(baseSpace);
			else if (clickCounter == 2)
				HandleDoubleClickTileSelect(baseSpace);
			else if (clickCounter == 3)
				HandleTripleClickTileSelect(baseSpace);
		}

		private void HandleTripleClickTileSelect(Tile baseSpace)
		{
			FloodSelectAll(baseSpace, GetSelectionType(Map.SelectionExists()));
		}

		private void HandleDoubleClickTileSelect(Tile baseSpace)
		{
			FloodSelect(baseSpace, GetSelectionType(Map.SelectionExists()));
		}

		private void HandleSingleClickTileSelect(Tile baseSpace)
		{
			if (!mouseIsDown || Keyboard.IsKeyDown(Key.Space))
				return;

			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)) // Alt key is reserved for zooming into a rect.
				return;

			SelectionType selectionType = GetSelectionType(Map.SelectionExists());

			if (selectionType == SelectionType.Replace)
				ClearSelection();

			SetRubberBandVisibility(Visibility.Visible);
			tileSelection.StartSelection(baseSpace, selectionType);
			content.CaptureMouse();
		}

		private SelectionType GetSelectionType(bool selectionExists)
		{
			SelectionType selectionType = SelectionType.Replace;
			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
				selectionType = SelectionType.Add;
			else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
				selectionType = SelectionType.Remove;

			if (selectionType == SelectionType.Add && !selectionExists)
				selectionType = SelectionType.Replace;
			return selectionType;
		}

		private void Button_ApplyFlooring(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button button))
				return;
			if (!(button.Tag is BaseTexture texture))
				return;
			
			List<Tile> selection = Map.GetSelection();
			RemoveRubberBandSelector();
			foreach (Tile tile in selection)
			{
				content.Children.Remove(tile.UIElementFloor as UIElement);
				content.Children.Remove(tile.UIElementOverlay as UIElement);
				content.Children.Remove(tile.SelectorPanel as UIElement);

				//AddElementOverTile(tile, texture.GetImage(tile.Column, tile.Row));
				AddFloorFile(tile, texture.GetImage(tile.Column, tile.Row));
				AddFloorOverlay(tile);
				AddSelector(tile);
			}
			LoadFinalCanvasElements();
		}
		private void Button_ApplyDebris(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button button))
				return;
			BaseTexture texture = (BaseTexture)button.Tag;
			List<Tile> selection = Map.GetSelection();
			foreach (Tile tile in selection)
			{
				AddDebris(tile, texture.GetImage(tile.Column, tile.Row));
			}
		}

		private void AddFloorOverlay(Tile tile)
		{
			depthLayer.DrawImageOverTile(imageLightTile, tile);
		}

		private void AddFloorFile(Tile tile, Image image)
		{
			floorLayer.DrawImageOverTile(image, tile);
		}

		private void AddDebris(Tile tile, Image image)
		{
			debrisLayer.ClearAtTile(tile);
			debrisLayer.DrawImageOverTile(image, tile);
		}

		private void SetCanvasSizeFromMap()
		{
			content.Width = Map.WidthPx;
			content.Height = Map.HeightPx;
			theGrid.Width = Map.WidthPx;
			theGrid.Height = Map.HeightPx;
			allLayers.SetSize(Map.WidthPx, Map.HeightPx);
		}

		Layer depthLayer = new Layer();
		Layer doorLayer = new Layer();
		Layer debrisLayer = new Layer();
		StampsLayer stampsLayer = new StampsLayer();
		Layer wallLayer = new Layer() { OuterMargin = Tile.Width / 2 };
		Layer floorLayer = new Layer();
		Layers allLayers = new Layers();

		void InitializeLayers()
		{
			allLayers.Add(floorLayer);
			allLayers.Add(depthLayer);
			allLayers.Add(debrisLayer);
			allLayers.Add(stampsLayer);
			allLayers.Add(doorLayer);
			allLayers.Add(wallLayer);
		}

		Image imageHeavyTile;
		Image imageLightTile;
		Image LoadDoor(string fileName)
		{
			string doorPath = System.IO.Path.Combine(TextureUtils.DoorsFolder, fileName);
			Image image = new Image();
			image.Source = new BitmapImage(new Uri(doorPath));
			return image;
		}
		void LoadImageResources()
		{
			string heavyTilePath = System.IO.Path.Combine(TextureUtils.TileFolder, "TileOverlay", "Heavy.png");
			imageHeavyTile = new Image();
			imageHeavyTile.Source = new BitmapImage(new Uri(heavyTilePath));
			string lightTilePath = System.IO.Path.Combine(TextureUtils.TileFolder, "TileOverlay", "Light.png");
			imageLightTile = new Image();
			imageLightTile.Source = new BitmapImage(new Uri(lightTilePath));

			doorLightHorizontal = LoadDoor("LightWoodenHorizontal.png");
			doorLightVertical = LoadDoor("LightWoodenVertical.png");
		}

		private void BtnBottomWall_Click(object sender, RoutedEventArgs e)
		{
			AddSegments(WallSide.Bottom);
		}

		void AddDoors(DoorPosition doorPosition)
		{
			bool adding = !(Keyboard.Modifiers.HasFlag(ModifierKeys.Control));
			List<Tile> selection = Map.GetSelection();
			foreach (Tile tile in selection)
				if (tile is FloorSpace floorSpace)
					floorSpace.SetDoor(doorPosition, adding);

			UpdateDoors();
		}

		void UpdateDoors()
		{
			doorLayer.ClearAll();
			foreach (Tile tile in Map.Tiles)
				AddDoors(tile);
		}

		void AddSegments(WallSide wallSide)
		{
			bool adding = !(Keyboard.Modifiers.HasFlag(ModifierKeys.Control));

			List<Tile> selection = Map.GetSelection();
			Map.BeginWallUpdate();
			try
			{
				foreach (Tile tile in selection)
				{
					int column = tile.Column;
					int row = tile.Row;
					WallOrientation wallOrientation = WallOrientation.None;
					int rowOffset = 0;
					int columnOffset = 0;
					switch (wallSide)
					{
						case WallSide.Left:
							column--;
							wallOrientation = WallOrientation.Vertical;
							break;
						case WallSide.Top:
							row--;
							wallOrientation = WallOrientation.Horizontal;
							break;
						case WallSide.Right:
							column++;
							columnOffset = -1;
							wallOrientation = WallOrientation.Vertical;
							break;
						case WallSide.Bottom:
							row++;
							rowOffset = -1;
							wallOrientation = WallOrientation.Horizontal;
							break;


					}
					if (!Map.TileExists(column, row) || !Map.AllTiles[column, row].Selected)
						Map.SetWall(column + columnOffset, row + rowOffset, wallOrientation, adding);
				}
			}
			finally
			{
				Map.EndWallUpdate();
			}
		}
		private void BtnRightWall_Click(object sender, RoutedEventArgs e)
		{
			AddSegments(WallSide.Right);
		}

		private void BtnTopWall_Click(object sender, RoutedEventArgs e)
		{
			AddSegments(WallSide.Top);
		}

		private void BtnLeftWall_Click(object sender, RoutedEventArgs e)
		{
			AddSegments(WallSide.Left);
		}

		private void BtnOutlineWall_Click(object sender, RoutedEventArgs e)
		{
			OutlineWalls();
		}

		private void OutlineWalls()
		{
			Map.BeginWallUpdate();
			try
			{
				AddSegments(WallSide.Left);
				AddSegments(WallSide.Top);
				AddSegments(WallSide.Right);
				AddSegments(WallSide.Bottom);
			}
			finally
			{
				Map.EndWallUpdate();
			}
		}

		private void BtnLeftDoor_Click(object sender, RoutedEventArgs e)
		{
			AddDoors(DoorPosition.Left);
		}

		private void BtnTopDoor_Click(object sender, RoutedEventArgs e)
		{
			AddDoors(DoorPosition.Top);
		}

		private void BtnRightDoor_Click(object sender, RoutedEventArgs e)
		{
			AddDoors(DoorPosition.Right);
		}

		private void BtnBottomDoor_Click(object sender, RoutedEventArgs e)
		{
			AddDoors(DoorPosition.Bottom);
		}

		private void BtnSelect_MouseMove(object sender, MouseEventArgs e)
		{

		}

		private void BtnSelect_Checked(object sender, RoutedEventArgs e)
		{
			MapEditMode = MapEditModes.Select;
		}

		private void BtnStamp_Checked(object sender, RoutedEventArgs e)
		{
			ClearSelection();
			MapEditMode = MapEditModes.Stamp;
		}

		public MapEditModes MapEditMode
		{
			get
			{
				return mapEditMode;
			}
			set
			{
				if (mapEditMode == value)
				{
					return;
				}

				mapEditMode = value;
				OnEditModeChanged();
			}
		}

		void OnEditModeChanged()
		{
			if (MapEditMode == MapEditModes.Stamp)
			{
				SelectedStamps.Clear();
				SetRubberBandVisibility(Visibility.Hidden);
				Cursor = Cursors.Hand;
				lstFlooring.Visibility = Visibility.Collapsed;
				spStamps.Visibility = Visibility.Visible;
				if (activeStampCanvas != null)
					Panel.SetZIndex(activeStampCanvas, int.MaxValue);
			}
			else if (MapEditMode == MapEditModes.Select)
			{
				Cursor = Cursors.Arrow;
				lstFlooring.Visibility = Visibility.Visible;
				spStamps.Visibility = Visibility.Collapsed;
				content.Children.Remove(activeStampCanvas);
				activeStampCanvas = null;
				activeStamp = null;
			}
		}

		Canvas activeStampCanvas;
		Image activeStampImage;
		Stamp activeStamp;
		MouseButtonEventArgs lastMouseDownEventArgs;

		private void StampButton_Click(object sender, RoutedEventArgs e)
		{
			if (activeStampCanvas != null)
				content.Children.Remove(activeStampCanvas);
			if (!(sender is Button button))
				return;
			if (!(button.Tag is Stamp knownStamp))
				return;
			CreateFloatingStamp(knownStamp);
		}

		double stampMouseOffsetX;
		double stampMouseOffsetY;
		void CreateFloatingStamp(Stamp knownStamp, int x = 0, int y = 0)
		{
			activeStamp = knownStamp;
			if (activeStampCanvas == null)
			{
				activeStampCanvas = new Canvas();
				content.Children.Add(activeStampCanvas);
				activeStampCanvas.IsHitTestVisible = false;
				Panel.SetZIndex(activeStampCanvas, int.MaxValue);
			}
			Image image = new Image();
			image.Source = new BitmapImage(new Uri(knownStamp.FileName));
			TransformGroup transformGroup = new TransformGroup();
			RotateTransform rotation = null;
			switch (knownStamp.Rotation)
			{
				case StampRotation.Ninety:
					rotation = new RotateTransform(90);
					break;
				case StampRotation.OneEighty:
					rotation = new RotateTransform(180);
					break;
				case StampRotation.TwoSeventy:
					rotation = new RotateTransform(270);
					break;
			}
			if (rotation != null)
				transformGroup.Children.Add(rotation);

			if (transformGroup.Children.Count > 0)
				image.LayoutTransform = transformGroup;
			image.Opacity = 0.5;
			image.IsHitTestVisible = false;
			stampMouseOffsetX = image.Source.Width / 2;
			stampMouseOffsetY = image.Source.Height / 2;
			activeStampCanvas.Children.Add(image);
			Canvas.SetLeft(image, x);
			Canvas.SetTop(image, y);
		}

		private void BtnRotateRight_Click(object sender, RoutedEventArgs e)
		{
			stampsLayer.BeginUpdate();
			try
			{
				foreach (Stamp stamp in SelectedStamps)
				{
					stamp.RotateRight();
				}

			}
			finally
			{
				stampsLayer.EndUpdate();
			}
		}

		private void BtnRotateLeft_Click(object sender, RoutedEventArgs e)
		{
			stampsLayer.BeginUpdate();
			try
			{
				foreach (Stamp stamp in SelectedStamps)
				{
					stamp.RotateLeft();
				}

			}
			finally
			{
				stampsLayer.EndUpdate();
			}
		}
	}
}

