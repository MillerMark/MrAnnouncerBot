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
using System.Xml;
using System.Windows.Markup;

namespace DndMapSpike
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const int StampButtonSize = 36;
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
			hueShiftUpdateTimer = new Timer(125);
			hueShiftUpdateTimer.Elapsed += HueShiftUpdateTimer_Elapsed;
			saturationUpdateTimer = new Timer(125);
			saturationUpdateTimer.Elapsed += SaturationUpdateTimer_Elapsed;
			lightnessUpdateTimer = new Timer(125);
			lightnessUpdateTimer.Elapsed += LightnessUpdateTimer_Elapsed;
			contrastUpdateTimer = new Timer(125);
			contrastUpdateTimer.Elapsed += ContrastUpdateTimer_Elapsed;
			clickTimer = new Timer(225);
			clickTimer.Elapsed += new ElapsedEventHandler(EvaluateClicks);
			fileWatchTimer = new DispatcherTimer();
			fileWatchTimer.Interval = TimeSpan.FromMilliseconds(330);
			fileWatchTimer.Tick += FileWatchTimer_Tick;
			InitializeComponent();
			zoomAndPanControl.ZoomLevelChanged += ZoomAndPanControl_ZoomLevelChanged;
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

		private void ZoomAndPanControl_ZoomLevelChanged(object sender, EventArgs e)
		{
			if (SelectedStamps.Count > 0)
				UpdateStampSelectionUI();
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

		void Process120x120FloorTiles(string[] pngFiles, List<BaseTexture> textures)
		{
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
		void ProcessBigTextures(string[] pngFiles, List<BaseTexture> textures)
		{
			foreach (string fileName in pngFiles)
			{
				BaseTexture texture = TextureUtils.CreateTexture(fileName, "Big");
				textures.Add(texture);
				texture.AddImage(fileName, string.Empty);
			}
		}
		private void LoadFloorTiles()
		{
			tileTextures.Clear();
			string[] pngFiles = Directory.GetFiles(TextureUtils.TileFolder, "*.png");
			Process120x120FloorTiles(pngFiles, tileTextures);
			pngFiles = Directory.GetFiles(TextureUtils.BigTextureFolder, "*.png");
			ProcessBigTextures(pngFiles, tileTextures);
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
			bool tileSelectionExists = Map.SelectionExists();
			EnableSelectionControls(tileSelectionExists);
			if (tileSelectionExists)
				ShowTilingControls();
			else
			{
				ClearSelectionUI();
				HideTilingControls();
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
			ClearStampSelection();
			foreach (Tile tile in Map.Tiles)
				if (tile.SelectorPanel is FrameworkElement selectorPanel)
					selectorPanel.Opacity = 0;
		}

		private void ClearStampSelection()
		{
			stampSelectionCanvas.Children.Clear();
		}

		private void Selection_SelectionChanged(object sender, EventArgs e)
		{
			ShowSelectionRubberBand();
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
			Panel.SetZIndex(selector, int.MaxValue - 1);
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
			AddSelectionCanvas();
			ClearSelection();
		}
		void AddSelectionCanvas()
		{
			stampSelectionCanvas = new Canvas();
			content.Children.Add(stampSelectionCanvas);
			MoveSelectionCanvasToTop();
		}

		private void MoveSelectionCanvasToTop()
		{
			Panel.SetZIndex(stampSelectionCanvas, int.MaxValue);
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

		void DragCanvas(Canvas canvas, MouseEventArgs e)
		{
			Point cursor = e.GetPosition(content);
			if (draggingStamps && e.LeftButton == MouseButtonState.Released)
			{
				StopDraggingStamps(cursor);
				return;
			}
			Canvas.SetLeft(canvas, cursor.X - mouseDragAdjustX);
			Canvas.SetTop(canvas, cursor.Y - mouseDragAdjustY);
		}

		bool draggingStamps;
		private void ZoomAndPanControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (activeStampCanvas != null)
			{
				DragCanvas(activeStampCanvas, e);
				return;
			}
			if (selectDragStampCanvas != null)
			{
				DragCanvas(selectDragStampCanvas, e);
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
			ClearFloatingCanvases();
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
			UpdateStampSelection();
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
			Stamp newStamp = new Stamp(activeStamp.FileName, x, y);
			stampsLayer.AddStampNow(newStamp);
			SelectStamp(newStamp);
		}

		void SelectStamp(Stamp stamp)
		{
			if (SelectedStamps.IndexOf(stamp) < 0)
			{
				SelectionType selectionType = GetSelectionType(SelectedStamps.Count > 0);
				if (selectionType != SelectionType.Add)
					SelectedStamps.Clear();
				SelectedStamps.Add(stamp);
				UpdateStampSelection();
			}
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
			if (Keyboard.IsKeyDown(Key.Space))  // Space + mouse down = pan view
				return;
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
				{
					draggingStamps = true;
					UpdateStampSelection();
					GetTopLeftOfStampSelection(out int left, out int top);

					mouseDragAdjustX = lastMouseDownPoint.X - left;
					mouseDragAdjustY = lastMouseDownPoint.Y - top;
				}
				else
					StampsAreNotSelected();
				return;
			}
			else
			{
				ClearStampSelection();
				SelectedStamps.Clear();
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

		void GetTopLeftOfStampSelection(out int left, out int top)
		{
			left = int.MaxValue;
			top = int.MaxValue;
			foreach (Stamp stamp in SelectedStamps)
			{
				int stampLeft = stamp.GetLeft();
				int stampTop = stamp.GetTop();

				if (stampLeft < left)
					left = stampLeft;
				if (stampTop < top)
					top = stampTop;
			}
		}

		void AddSelectedStampsToFloatingUI()
		{
			ClearFloatingCanvases();

			GetTopLeftOfStampSelection(out int left, out int top);

			// TODO: Figure out top left of selection
			foreach (Stamp stamp in SelectedStamps)
			{
				int stampLeft = stamp.GetLeft();
				int stampTop = stamp.GetTop();
				stamp.RelativeX = stampLeft - left;
				stamp.RelativeY = stampTop - top;
				CreateFloatingStamp(ref selectDragStampCanvas, stamp, stamp.RelativeX, stamp.RelativeY);
			}
		}
		private void UpdateStampSelection()
		{
			HideTilingControls();
			AddSelectedStampsToFloatingUI();
			UpdateStampSelectionUI();
		}

		void CreateResizeCorner(ResizeTracker resizeTracker, int left, int top, SizeDirection sizeDirection, Stamp stamp)
		{
			Ellipse ellipse = new Ellipse();
			resizeTracker.AddCorner(ellipse, sizeDirection);
			double sizeDiameter = ResizeTracker.ResizeHandleDiameter / zoomAndPanControl.ContentScale;
			ellipse.Width = sizeDiameter;
			ellipse.Height = sizeDiameter;
			ellipse.Fill = Brushes.White;
			ellipse.Stroke = Brushes.Gray;
			ellipse.StrokeThickness = 1 / zoomAndPanControl.ContentScale;
			Canvas.SetLeft(ellipse, left - sizeDiameter / 2);
			Canvas.SetTop(ellipse, top - sizeDiameter / 2);
			switch (sizeDirection)
			{
				case SizeDirection.NorthWest:
				case SizeDirection.SouthEast:
					ellipse.Cursor = Cursors.SizeNWSE;
					break;
				case SizeDirection.NorthEast:
				case SizeDirection.SouthWest:
					ellipse.Cursor = Cursors.SizeNESW;
					break;
			}
			ellipse.Tag = resizeTracker;
			ellipse.MouseDown += ResizeTracker_MouseDown;
			ellipse.MouseMove += ResizeTracker_MouseMove;
			ellipse.MouseUp += ResizeTracker_MouseUp;
			stampSelectionCanvas.Children.Add(ellipse);
		}

		Point resizeOppositeCornerPosition;
		double mouseResizeOffsetX;
		double mouseResizeOffsetY;

		private void ResizeTracker_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (!(sender is Ellipse ellipse))
				return;
			ellipse.ReleaseMouseCapture();

			if (!(ellipse.Tag is ResizeTracker resizeTracker))
				return;

			double left, top, right, bottom;
			SizeDirection direction = resizeTracker.GetDirection(ellipse);
			GetNewTrackerBounds(resizeTracker, direction, e, out left, out top, out right, out bottom);

			double newWidth = right - left;
			double scaleAdjust = newWidth / resizeTracker.Stamp.Width;

			stampsLayer.BeginUpdate();
			try
			{
				foreach (Stamp stamp in SelectedStamps)
					stamp.Scale *= scaleAdjust;

			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateStampSelectionUI();


			activeStampResizing = null;
		}



		private void ResizeTracker_MouseMove(object sender, MouseEventArgs e)
		{
			if (activeStampResizing == null)
				return;
			else
			{
				if (!(sender is Ellipse ellipse))
					return;

				if (!(ellipse.Tag is ResizeTracker resizeTracker))
					return;

				double left, top, right, bottom;
				SizeDirection direction = resizeTracker.GetDirection(ellipse);
				GetNewTrackerBounds(resizeTracker, direction, e, out left, out top, out right, out bottom);

				resizeTracker.Reposition(zoomAndPanControl.ContentScale, left, top, right, bottom);
			}
		}

		private void GetNewTrackerBounds(ResizeTracker resizeTracker, SizeDirection direction, MouseEventArgs e, out double left, out double top, out double right, out double bottom)
		{
			left = 0;
			top = 0;
			right = 0;
			bottom = 0;
			if (resizeTracker == null)
				return;
			Point newPosition = e.GetPosition(stampSelectionCanvas);
			newPosition.Offset(-mouseResizeOffsetX, -mouseResizeOffsetY);
			double oppositeX = resizeOppositeCornerPosition.X;
			double oppositeY = resizeOppositeCornerPosition.Y;
			double deltaX = newPosition.X - oppositeX;
			double deltaY = newPosition.Y - oppositeY;
			double newAspectRatio = deltaX / deltaY;
			Stamp stamp = resizeTracker.Stamp;
			double originalAspectRatio = (double)stamp.Width / stamp.Height;
			double newX = newPosition.X;
			double newY = newPosition.Y;
			double leftAdjust = 0;
			double topAdjust = 0;
			double rightAdjust = 0;
			double bottomAdjust = 0;


			// For debugging...
			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
			{

			}
			double newWidth = originalAspectRatio * deltaY;
			double newHeight = deltaX / originalAspectRatio;
			bool dataIsGood = true;
			switch (direction)
			{
				case SizeDirection.NorthWest:
					if (originalAspectRatio < newAspectRatio)
					{
						newX = oppositeX + newWidth;
						newHeight = newWidth / originalAspectRatio;
					}
					else
					{
						newY = oppositeY + newHeight;
						newWidth = newHeight * originalAspectRatio;
					}
					rightAdjust = -newWidth - stamp.Width;
					bottomAdjust = -newHeight - stamp.Height;
					dataIsGood = newX < oppositeX && newY < oppositeY;
					break;
				case SizeDirection.NorthEast:
					if (originalAspectRatio < newAspectRatio)
					{
						newX = oppositeX + newWidth;
						newHeight = newWidth / originalAspectRatio;
					}
					else
					{
						newY = oppositeY - newHeight;
						newWidth = newHeight * originalAspectRatio;
					}
					dataIsGood = newX > oppositeX && newY < oppositeY;

					leftAdjust = stamp.Width - newWidth;
					bottomAdjust = newHeight - stamp.Height;
					if (bottomAdjust != 0)
					{

					}
					break;
				case SizeDirection.SouthWest:
					//topAdjust = -(stamp.Height + newHeight);
					if (originalAspectRatio < newAspectRatio)
					{
						newX = oppositeX - newWidth;
						newHeight = newWidth / originalAspectRatio;
					}
					else
					{
						newY = oppositeY - newHeight;
						newWidth = newHeight * originalAspectRatio;
					}
					rightAdjust = -stamp.Width - newWidth;

					topAdjust = newHeight + stamp.Height;
					dataIsGood = newX < oppositeX && newY > oppositeY;
					break;
				case SizeDirection.SouthEast:
					if (originalAspectRatio < newAspectRatio)
					{
						newX = oppositeX + newWidth;
						newHeight = newWidth / originalAspectRatio;
					}
					else
					{
						newY = oppositeY + newHeight;
						newWidth = newHeight * originalAspectRatio;
					}
					dataIsGood = newX > oppositeX && newY > oppositeY;
					if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
					{

					}
					leftAdjust = stamp.Width - newWidth;
					topAdjust = stamp.Height - newHeight;
					break;
			}

			const double minSide = 20;

			double width;
			double height;
			if (dataIsGood)
			{
				left = Math.Min(oppositeX + leftAdjust, newX);
				top = Math.Min(oppositeY + topAdjust, newY);
				right = Math.Max(oppositeX + rightAdjust, newX);
				bottom = Math.Max(oppositeY + bottomAdjust, newY);

				width = right - left;
				height = bottom - top;

				if (width < minSide)
				{
					width = minSide;
					height = width / originalAspectRatio;
				}
				else if (height < minSide)
				{
					height = minSide;
					width = height * originalAspectRatio;
				}
				else
					return;

			}
			else
			{
				width = minSide;
				height = width / originalAspectRatio;
			}
			left = stamp.X - width / 2;
			top = stamp.Y - height / 2;
			right = left + width;
			bottom = top + height;

		}

		private void ResizeTracker_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!(sender is Ellipse ellipse))
				return;
			if (!(ellipse.Tag is ResizeTracker resizeTracker))
				return;
			Point ellipsePosition = e.GetPosition(ellipse);
			mouseResizeOffsetX = ellipsePosition.X - ResizeTracker.ResizeHandleDiameter / 2;
			mouseResizeOffsetY = ellipsePosition.Y - ResizeTracker.ResizeHandleDiameter / 2;
			Point absolutePosition = e.GetPosition(stampSelectionCanvas);
			absolutePosition.Offset(-mouseResizeOffsetX, -mouseResizeOffsetY);
			Stamp stamp = resizeTracker.Stamp;

			switch (resizeTracker.GetDirection(ellipse))
			{
				case SizeDirection.NorthWest:
					resizeOppositeCornerPosition = new Point(absolutePosition.X + stamp.Width, absolutePosition.Y + stamp.Height);
					break;
				case SizeDirection.NorthEast:
					resizeOppositeCornerPosition = new Point(absolutePosition.X - stamp.Width, absolutePosition.Y + stamp.Height);
					break;
				case SizeDirection.SouthWest:
					resizeOppositeCornerPosition = new Point(absolutePosition.X + stamp.Width, absolutePosition.Y - stamp.Height);
					break;
				case SizeDirection.SouthEast:
					resizeOppositeCornerPosition = new Point(absolutePosition.X - stamp.Width, absolutePosition.Y - stamp.Height);
					break;
			}
			activeStampResizing = stamp;
			resizeDiagonal = stamp.Diagonal;
			ellipse.CaptureMouse();
		}

		void StackScaleButtons(string buttonName, double compareScale, double? selectedButtonScale, double buttonSize, double x, ref double y, int maxTop)
		{
			if (y < maxTop)
				return;
			Viewbox btnScale100Percent = CreateButton(buttonName, x - buttonSize, y, buttonSize);
			if (!selectedButtonScale.HasValue || selectedButtonScale.Value != compareScale)
			{
				btnScale100Percent.Visibility = Visibility.Visible;
				y -= buttonSize;
			}
			else
				btnScale100Percent.Visibility = Visibility.Hidden;
		}

		void AddStampSelectionButtons(double? selectedButtonScale, double? selectedHueShift, double? selectedSaturation, double? selectedLightness, double? selectedContrast, int left, int top, int width, int height)
		{
			int right = left + width;
			int bottom = top + height;
			double buttonSize = StampButtonSize / zoomAndPanControl.ContentScale;
			double topRow = top - buttonSize;
			CreateButton("btnRotateLeft", left - buttonSize, topRow, buttonSize);
			CreateButton("btnRotateRight", right, topRow, buttonSize);
			if (width >= buttonSize)
				CreateButton("btnFlipVertical", left + width / 2 - buttonSize / 2, topRow, buttonSize);
			if (height >= buttonSize)
				CreateButton("btnFlipHorizontal", right, top + height / 2 - buttonSize / 2, buttonSize);

			double yPos = bottom - buttonSize;
			StackScaleButtons("btnScale33Percent", (double)1 / 3, selectedButtonScale, buttonSize, left, ref yPos, top);
			StackScaleButtons("btnScale50Percent", (double)1 / 2, selectedButtonScale, buttonSize, left, ref yPos, top);
			StackScaleButtons("btnScale100Percent", 1, selectedButtonScale, buttonSize, left, ref yPos, top);
			StackScaleButtons("btnScale200Percent", 2, selectedButtonScale, buttonSize, left, ref yPos, top);

			CreateButton("btnSendToBack", right, bottom, buttonSize);
			CreateButton("btnBringToFront", right + buttonSize, bottom, buttonSize);
			
			bool hasColorMod = selectedHueShift.HasValue && selectedHueShift.Value != 0 ||
												 selectedSaturation.HasValue && selectedSaturation.Value != 0 ||
												 selectedLightness.HasValue && selectedLightness.Value != 0 || 
												 selectedContrast.HasValue && selectedContrast.Value != 0;

			double pixelWidth = width * zoomAndPanControl.ContentScale;
			const double maxPixelWidth = 400;
			ContentControl ccColorControls = null;
			if (pixelWidth > 60)
			{
				ccColorControls = GetColorControls();
				ccColorControls.FontSize = 15 / zoomAndPanControl.ContentScale;
				ccColorControls.Visibility = Visibility.Visible;
				ccColorControls.Width = Math.Min(width, maxPixelWidth / zoomAndPanControl.ContentScale);
				stampSelectionCanvas.Children.Add(ccColorControls);
				Canvas.SetLeft(ccColorControls, left + (width - ccColorControls.Width) / 2);
				Canvas.SetTop(ccColorControls, bottom);
				SetSlider(GetActiveHueSlider(), selectedHueShift);
				SetSlider(GetActiveSaturationSlider(), selectedSaturation);
				SetSlider(GetActiveLightnessSlider(), selectedLightness);
				SetSlider(GetActiveContrastSlider(), selectedContrast);
			}

			if (!hasColorMod)
			{
				if (ccColorControls != null)
					ccColorControls.Visibility = Visibility.Hidden;
				CreateButton("btnShowColorControls", left, bottom, buttonSize);
			}
		}

		private void SetSlider(Slider slider, double? value)
		{
			if (slider == null)
				return;

			slider.IsEnabled = value.HasValue;
			if (value.HasValue)
				slider.Value = value.Value;
		}

		private Viewbox CreateButton(string buttonName, double x, double y, double buttonSize)
		{
			Viewbox viewbox = FindResource(buttonName) as Viewbox;
			viewbox.Visibility = Visibility.Visible;
			viewbox.Width = buttonSize;
			viewbox.Height = buttonSize;
			viewbox.Cursor = Cursors.Hand;
			stampSelectionCanvas.Children.Add(viewbox);
			Canvas.SetLeft(viewbox, x);
			Canvas.SetTop(viewbox, y);
			return viewbox;
		}

		void AddStampSelectionUI(Stamp stamp)
		{
			int left = stamp.GetLeft();
			int top = stamp.GetTop();
			int right = left + stamp.Width;
			int bottom = top + stamp.Height;

			ResizeTracker resizeTracker = new ResizeTracker(stamp);
			AddStampSelectionRect(resizeTracker, stamp, left, top);
			CreateResizeCorner(resizeTracker, left, top, SizeDirection.NorthWest, stamp);
			CreateResizeCorner(resizeTracker, right, top, SizeDirection.NorthEast, stamp);
			CreateResizeCorner(resizeTracker, left, bottom, SizeDirection.SouthWest, stamp);
			CreateResizeCorner(resizeTracker, right, bottom, SizeDirection.SouthEast, stamp);
		}

		private void AddStampSelectionRect(ResizeTracker resizeTracker, Stamp stamp, int left, int top)
		{
			Rectangle selectionRect = new Rectangle();
			resizeTracker.SelectionRect = selectionRect;
			selectionRect.Width = stamp.Width;
			selectionRect.Height = stamp.Height;
			Canvas.SetLeft(selectionRect, left);
			Canvas.SetTop(selectionRect, top);
			selectionRect.Stroke = Brushes.Gray;
			selectionRect.StrokeThickness = 1 / zoomAndPanControl.ContentScale;
			stampSelectionCanvas.Children.Add(selectionRect);
		}

		void AddStampSelectionButtons(double? selectedButtonScale, double? selectedHueShift, double? selectedSaturation, double? selectedLightness, double? selectedContrast)
		{
			GetStampSelectedBounds(out int left, out int top, out int width, out int height);
			AddStampSelectionButtons(selectedButtonScale, selectedHueShift, selectedSaturation, selectedLightness, selectedContrast, left, top, width, height);
		}

		void UpdateStampSelectionUI()
		{
			ClearSelectionUI();
			double? selectedButtonScale, selectedHueShift, selectedSaturation, selectedLightness, selectedContrast;
			GetConsistentModSettings(out selectedButtonScale, out selectedHueShift, out selectedSaturation, out selectedLightness, out selectedContrast);

			AddStampSelectionButtons(selectedButtonScale, selectedHueShift, selectedSaturation, selectedLightness, selectedContrast);
			foreach (Stamp stamp in SelectedStamps)
				AddStampSelectionUI(stamp);

			MoveSelectionCanvasToTop();
		}

		private void GetConsistentModSettings(out double? selectedButtonScale, out double? selectedHueShift, out double? selectedSaturation, out double? selectedLightness, out double? selectedContrast)
		{
			selectedButtonScale = null;
			selectedHueShift = null;
			selectedSaturation = null;
			selectedLightness = null;
			selectedContrast = null;
			foreach (Stamp stamp in SelectedStamps)
			{
				if (selectedButtonScale == null)
					selectedButtonScale = stamp.Scale;
				else if (selectedButtonScale != stamp.Scale)
				{
					selectedButtonScale = null;
					break;
				}
				if (selectedHueShift == null)
					selectedHueShift = stamp.HueShift;
				else if (selectedHueShift != stamp.HueShift)
				{
					selectedHueShift = null;
					break;
				}
				if (selectedSaturation == null)
					selectedSaturation = stamp.Saturation;
				else if (selectedSaturation != stamp.Saturation)
				{
					selectedSaturation = null;
					break;
				}
				if (selectedLightness == null)
					selectedLightness = stamp.Lightness;
				else if (selectedLightness != stamp.Lightness)
				{
					selectedLightness = null;
					break;
				}
				if (selectedContrast == null)
					selectedContrast = stamp.Contrast;
				else if (selectedContrast != stamp.Contrast)
				{
					selectedContrast = null;
					break;
				}
			}
		}

		private void HideTilingControls()
		{
			spWallControls.Visibility = Visibility.Collapsed;
			spWallLabels.Visibility = Visibility.Collapsed;
			spDoorLabels.Visibility = Visibility.Collapsed;
			spDoorControls.Visibility = Visibility.Collapsed;
			lstFlooring.Visibility = Visibility.Collapsed;
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
			if (mouseIsDown && Keyboard.IsKeyDown(Key.Space))
				return;

			SelectionType selectionType = GetSelectionType(Map.SelectionExists());
			if (!mouseIsDown && selectionType == SelectionType.Replace)
			{
				ClearSelection();
				return;
			}

			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)) // Alt key is reserved for zooming into a rect.
				return;



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
					return;

				mapEditMode = value;
				OnEditModeChanged();
			}
		}

		void OnEditModeChanged()
		{
			if (MapEditMode == MapEditModes.Stamp)
			{
				EnterStampMode();
			}
			else if (MapEditMode == MapEditModes.Select)
			{
				EnterSelectMode();
			}
		}

		private void EnterSelectMode()
		{
			Cursor = Cursors.Arrow;
			//lstFlooring.Visibility = Visibility.Visible;
			//spStamps.Visibility = Visibility.Collapsed;
			ClearFloatingCanvases();
			activeStamp = null;
		}

		private void EnterStampMode()
		{
			SelectedStamps.Clear();
			SetRubberBandVisibility(Visibility.Hidden);
			Cursor = Cursors.Hand;
			lstFlooring.Visibility = Visibility.Collapsed;
			spStamps.Visibility = Visibility.Visible;
			MoveFloatingCanvasesToTop();
		}

		private void MoveFloatingCanvasesToTop()
		{
			if (activeStampCanvas != null)
				Panel.SetZIndex(activeStampCanvas, int.MaxValue);
			if (selectDragStampCanvas != null)
				Panel.SetZIndex(selectDragStampCanvas, int.MaxValue);
		}

		Canvas activeStampCanvas;
		Canvas selectDragStampCanvas;
		Stamp activeStamp;
		MouseButtonEventArgs lastMouseDownEventArgs;

		void ClearFloatingCanvases()
		{
			ClearCanvas(ref selectDragStampCanvas);
			ClearCanvas(ref activeStampCanvas);
		}

		private void ClearCanvas(ref Canvas canvas)
		{
			if (canvas != null)
			{
				content.Children.Remove(canvas);
				canvas = null;
			}
		}

		private void StampButton_Click(object sender, RoutedEventArgs e)
		{
			if (activeStampCanvas != null)
			{
				activeStampCanvas.Children.Clear();
			}

			if (!(sender is Button button))
				return;
			if (!(button.Tag is Stamp knownStamp))
				return;
			CreateFloatingStamp(ref activeStampCanvas, knownStamp);
			MapEditMode = MapEditModes.Stamp;
		}

		double mouseDragAdjustX;
		double mouseDragAdjustY;
		Canvas stampSelectionCanvas;
		Stamp activeStampResizing;
		double resizeDiagonal;
		Timer hueShiftUpdateTimer;
		Timer saturationUpdateTimer;
		Timer lightnessUpdateTimer;
		Timer contrastUpdateTimer;
		void CreateFloatingStamp(ref Canvas canvas, Stamp knownStamp, int x = 0, int y = 0)
		{
			activeStamp = knownStamp;
			if (canvas == null)
			{
				canvas = new Canvas();
				content.Children.Add(canvas);
				canvas.IsHitTestVisible = false;
				Panel.SetZIndex(canvas, int.MaxValue);
			}
			Image image = new Image();
			image.Source = new BitmapImage(new Uri(knownStamp.FileName));
			ScaleTransform scaleTransform = null;
			if (knownStamp.FlipVertically || knownStamp.FlipHorizontally || knownStamp.Scale != 1)
			{
				scaleTransform = new ScaleTransform();
				scaleTransform.ScaleX = knownStamp.ScaleX;
				scaleTransform.ScaleY = knownStamp.ScaleY;
			}
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
			if (scaleTransform != null)
				transformGroup.Children.Add(scaleTransform);

			if (transformGroup.Children.Count > 0)
				image.LayoutTransform = transformGroup;
			image.Opacity = 0.5;
			image.IsHitTestVisible = false;
			mouseDragAdjustX = image.Source.Width / 2;
			mouseDragAdjustY = image.Source.Height / 2;
			canvas.Children.Add(image);
			Canvas.SetLeft(image, x);
			Canvas.SetTop(image, y);
		}

		private void CancelStampMode_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (MapEditMode == MapEditModes.Stamp)
				MapEditMode = MapEditModes.Select;
			else
			{
				ClearSelectionUI();
				ClearSelection();
				SelectedStamps.Clear();
			}
		}

		// TODO: Delete...
		public static void SaveDefaultTemplate()
		{
			var control = Application.Current.FindResource(typeof(Button));
			using (XmlTextWriter writer = new XmlTextWriter(@"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\DndMapSpike\ButtonDefaultTemplate.xml", System.Text.Encoding.UTF8))
			{
				writer.Formatting = Formatting.Indented;
				XamlWriter.Save(control, writer);
			}
		}

		private void btnRotateRight_MouseDown(object sender, MouseButtonEventArgs e)
		{
			stampsLayer.BeginUpdate();
			try
			{
				foreach (Stamp stamp in SelectedStamps)
					stamp.RotateRight();
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateStampSelectionUI();
		}

		private void btnRotateLeft_MouseDown(object sender, MouseButtonEventArgs e)
		{
			stampsLayer.BeginUpdate();
			try
			{
				foreach (Stamp stamp in SelectedStamps)
					stamp.RotateLeft();
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateStampSelectionUI();
		}

		private void btnFlipVertical_MouseDown(object sender, MouseButtonEventArgs e)
		{
			stampsLayer.BeginUpdate();
			try
			{
				foreach (Stamp stamp in SelectedStamps)
					stamp.FlipVertically = !stamp.FlipVertically;
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateStampSelectionUI();
		}

		private void btnFlipHorizontal_MouseDown(object sender, MouseButtonEventArgs e)
		{
			stampsLayer.BeginUpdate();
			try
			{
				foreach (Stamp stamp in SelectedStamps)
					stamp.FlipHorizontally = !stamp.FlipHorizontally;
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateStampSelectionUI();
		}

		private void btnScale100Percent_MouseDown(object sender, MouseButtonEventArgs e)
		{
			stampsLayer.BeginUpdate();
			try
			{
				foreach (Stamp stamp in SelectedStamps)
					stamp.Scale = 1;
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateStampSelectionUI();
		}

		private void btnScale200Percent_MouseDown(object sender, MouseButtonEventArgs e)
		{
			stampsLayer.BeginUpdate();
			try
			{
				foreach (Stamp stamp in SelectedStamps)
					stamp.Scale = 2;
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateStampSelectionUI();
		}

		private void btnScale50Percent_MouseDown(object sender, MouseButtonEventArgs e)
		{
			stampsLayer.BeginUpdate();
			try
			{
				foreach (Stamp stamp in SelectedStamps)
					stamp.Scale = 0.5;
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateStampSelectionUI();
		}

		private void btnScale33Percent_MouseDown(object sender, MouseButtonEventArgs e)
		{
			stampsLayer.BeginUpdate();
			try
			{
				foreach (Stamp stamp in SelectedStamps)
					stamp.Scale = (double)1 / 3;
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateStampSelectionUI();
		}

		private void btnSendToBack_MouseDown(object sender, MouseButtonEventArgs e)
		{
			stampsLayer.BeginUpdate();
			try
			{
				stampsLayer.RemoveAllStamps(SelectedStamps);
				stampsLayer.SortStampsByZOrder(SelectedStamps.Count);
				stampsLayer.InsertStamps(0, SelectedStamps);
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateStampSelectionUI();
		}

		private void btnBringToFront_MouseDown(object sender, MouseButtonEventArgs e)
		{
			stampsLayer.BeginUpdate();
			try
			{
				stampsLayer.RemoveAllStamps(SelectedStamps);
				stampsLayer.SortStampsByZOrder();
				stampsLayer.AddStamps(SelectedStamps);
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateStampSelectionUI();
		}

		private void HueShiftUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			hueShiftUpdateTimer.Stop();

			Dispatcher.Invoke(() =>
			{
				stampsLayer.BeginUpdate();
				try
				{
					foreach (Stamp stamp in SelectedStamps)
						stamp.HueShift = hueShiftToApply;
				}
				finally
				{
					stampsLayer.EndUpdate();
				}
				UpdateStampSelectionUI();
			});
		}

		private void SaturationUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			saturationUpdateTimer.Stop();

			Dispatcher.Invoke(() =>
			{
				stampsLayer.BeginUpdate();
				try
				{
					foreach (Stamp stamp in SelectedStamps)
						stamp.Saturation = saturationToApply;
				}
				finally
				{
					stampsLayer.EndUpdate();
				}
				UpdateStampSelectionUI();
			});
		}
		private void LightnessUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			lightnessUpdateTimer.Stop();

			Dispatcher.Invoke(() =>
			{
				stampsLayer.BeginUpdate();
				try
				{
					foreach (Stamp stamp in SelectedStamps)
						stamp.Lightness = lightnessToApply;
				}
				finally
				{
					stampsLayer.EndUpdate();
				}
				UpdateStampSelectionUI();
			});
		}
		private void ContrastUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			contrastUpdateTimer.Stop();

			Dispatcher.Invoke(() =>
			{
				stampsLayer.BeginUpdate();
				try
				{
					foreach (Stamp stamp in SelectedStamps)
						stamp.Contrast = contrastToApply;
				}
				finally
				{
					stampsLayer.EndUpdate();
				}
				UpdateStampSelectionUI();
			});
		}

		double hueShiftToApply;
		double saturationToApply;
		double lightnessToApply;
		double contrastToApply;

		private void HueShiftSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!(sender is Slider slider))
				return;

			hueShiftToApply = slider.Value;
			hueShiftUpdateTimer.Stop();
			hueShiftUpdateTimer.Start();
		}

		private void SaturationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!(sender is Slider slider))
				return;

			saturationToApply = slider.Value;
			saturationUpdateTimer.Stop();
			saturationUpdateTimer.Start();
		}
		private void LightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!(sender is Slider slider))
				return;

			lightnessToApply = slider.Value;
			lightnessUpdateTimer.Stop();
			lightnessUpdateTimer.Start();
		}

		Slider GetActiveHueSlider()
		{
			return GetActiveSlider("HueSlider");
		}

		Slider GetActiveSaturationSlider()
		{
			return GetActiveSlider("SaturationSlider");
		}

		Slider GetActiveLightnessSlider()
		{
			return GetActiveSlider("LightnessSlider");
		}

		Slider GetActiveContrastSlider()
		{
			return GetActiveSlider("ContrastSlider");
		}

		private Slider GetActiveSlider(string sliderName)
		{

			ContentControl ccColorControls = GetColorControls();
			if (ccColorControls == null)
				return null;

			if (!(ccColorControls.Content is StackPanel stackPanel))
				return null;

			foreach (UIElement uIElement in stackPanel.Children)
				if (uIElement is Slider slider && slider.Name == sliderName)
					return slider;
			return null;
		}

		private void HueShiftReset_Click(object sender, RoutedEventArgs e)
		{
			ResetHueSlider();
			stampsLayer.BeginUpdate();
			try
			{
				foreach (Stamp stamp in SelectedStamps)
					stamp.HueShift = 0;
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateStampSelectionUI();
		}

		private void ResetHueSlider()
		{
			Slider activeHueSlider = GetActiveHueSlider();
			if (activeHueSlider != null)
				activeHueSlider.Value = 0;
		}

		private void SaturationReset_Click(object sender, RoutedEventArgs e)
		{
			ResetSaturationSlider();
			stampsLayer.BeginUpdate();
			try
			{
				foreach (Stamp stamp in SelectedStamps)
					stamp.Saturation = 0;
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateStampSelectionUI();
		}

		private void ResetSaturationSlider()
		{
			Slider activeSaturationSlider = GetActiveSaturationSlider();
			if (activeSaturationSlider != null)
				activeSaturationSlider.Value = 0;
		}

		private void LightnessReset_Click(object sender, RoutedEventArgs e)
		{
			ResetLightnessSlider();
			stampsLayer.BeginUpdate();
			try
			{
				foreach (Stamp stamp in SelectedStamps)
					stamp.Lightness = 0;
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateStampSelectionUI();
		}

		private void ResetLightnessSlider()
		{
			Slider activeLightnessSlider = GetActiveLightnessSlider();
			if (activeLightnessSlider != null)
				activeLightnessSlider.Value = 0;
		}

		private void ContrastReset_Click(object sender, RoutedEventArgs e)
		{
			ResetContrastSlider();
			stampsLayer.BeginUpdate();
			try
			{
				foreach (Stamp stamp in SelectedStamps)
					stamp.Contrast = 0;
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateStampSelectionUI();
		}

		private void ResetContrastSlider()
		{
			Slider activeContrastSlider = GetActiveContrastSlider();
			if (activeContrastSlider != null)
				activeContrastSlider.Value = 0;
		}

		private void btnShowColorControls_MouseDown(object sender, MouseButtonEventArgs e)
		{
			ContentControl ccColorControls = GetColorControls();
			if (ccColorControls != null)
			{
				ccColorControls.Visibility = Visibility.Visible;
				Viewbox btnShowColorControls = FindResource("btnShowColorControls") as Viewbox;
				if (btnShowColorControls != null)
					btnShowColorControls.Visibility = Visibility.Hidden;
			}
		}

		private ContentControl GetColorControls()
		{
			return FindResource("ccColorControls") as ContentControl;
		}

		private void DeleteSelected_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			stampsLayer.BeginUpdate();
			try
			{
				stampsLayer.RemoveAllStamps(SelectedStamps);
				SelectedStamps.Clear();
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateStampSelectionUI();
		}

		double? clipboardSaturation;
		double? clipboardLightness;
		double? clipboardContrast;
		double? clipboardHue;

		private void CopyColorMod_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			double? selectedButtonScale;
			GetConsistentModSettings(out selectedButtonScale, out clipboardHue, out clipboardSaturation, out clipboardLightness, out clipboardContrast);
		}

		private void PasteColorMod_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			stampsLayer.BeginUpdate();
			try
			{
				foreach (Stamp stamp in SelectedStamps)
				{
					if (clipboardLightness.HasValue)
						stamp.Lightness = clipboardLightness.Value;
					if (clipboardContrast.HasValue)
						stamp.Contrast = clipboardContrast.Value;
					if (clipboardHue.HasValue)
						stamp.HueShift = clipboardHue.Value;
					if (clipboardSaturation.HasValue)
						stamp.Saturation = clipboardSaturation.Value;
				}
				
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateStampSelectionUI();
		}

		private void ContrastSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!(sender is Slider slider))
				return;

			contrastToApply = slider.Value;
			contrastUpdateTimer.Stop();
			contrastUpdateTimer.Start();
		}

		private void ResetAllColorMods_Click(object sender, RoutedEventArgs e)
		{
			ResetSaturationSlider();
			ResetLightnessSlider();
			ResetHueSlider();
			ResetContrastSlider();
			stampsLayer.BeginUpdate();
			try
			{
				foreach (Stamp stamp in SelectedStamps)
				{
					stamp.HueShift = 0;
					stamp.Saturation = 0;
					stamp.Lightness = 0;
					stamp.Contrast = 0;
				}
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateStampSelectionUI();
		}
	}
}

