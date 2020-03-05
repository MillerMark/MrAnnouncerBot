using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
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
using Newtonsoft.Json;
using Microsoft.Win32;

namespace DndMapSpike
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		Stack<BaseCommand> redoStack = new Stack<BaseCommand>();
		Stack<BaseCommand> undoStack = new Stack<BaseCommand>();
		bool ctrlKeyDown;
		private const int StampButtonSize = 36;
		const string MapSaveFolder = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\DragonH\Maps\Data";
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
		MapSelection tileSelection;
		WallBuilder wallBuilder = new WallBuilder();
		int clickCounter;
		public Map Map { get; set; }
		public MainWindow()
		{
			SerializedStamp.PrepareStampForSerialization += PrepareStampForSerialization;
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
			AssignMapToStampsLayer();
			HookupMapEvents();
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
			//ImportDonJonMap("The Dark Crypts of the Shadow Countess.txt");
			//ImportDonJonMap("The Forsaken Tunnels of Death.txt");
			//ImportDonJonMap("The Dark Lair of the Demon Baron.txt");
			ImportDonJonMap("SmallMap.txt");
			//ImportDonJonMap("The Dungeon of Selima the Awesome.txt");
			LoadFloorTiles();
			//LoadDebris();
			AddFileSystemWatcher();
			BuildStampsUI();
			BuildCharacterUI();
		}

		private void HookupMapEvents()
		{
			Map.WallsChanged += Map_WallsChanged;
			Map.ReconstitutingStamps += ReconstituteStamps;
			Map.SelectingItems += Map_SelectingItems;
			Map.CreatingGroup += Map_CreatingGroup;
		}

		private void Map_CreatingGroup(object sender, CreateGroupEventArgs ea)
		{
			SelectionContents selectionContents = GetSelectionContents(Map.SelectedItems);
			if (selectionContents == SelectionContents.AllStamps)
				ea.Group = GroupHelper.Create<StampGroup>(ea.Stamps);
			else
				ea.Group = GroupHelper.Create<MapCharacterGroup>(ea.Stamps);
		}

		private void Map_SelectingItems(object sender, SelectItemsEventArgs ea)
		{
			Map.SelectedItems.Clear();
			if (ea.ItemsGuids != null)
				foreach (Guid guid in ea.ItemsGuids)
				{
					IItemProperties itemFromGuid = Map.GetItemFromGuid(guid);
					if (itemFromGuid == null)
					{
						System.Diagnostics.Debugger.Break();
					}
					else
						Map.SelectedItems.Add(itemFromGuid);
				}
			UpdateItemSelectionUI();
		}

		private void ReconstituteStamps(object sender, ReconstituteStampsEventArgs ea)
		{
			ReconstituteStamp(ea.SerializedStamp, ea.Stamps);
		}

		private static void ReconstituteStamp(SerializedStamp serializedStamp, List<IItemProperties> stamps)
		{
			stamps.Add(MapElementFactory.CreateStampFrom(serializedStamp));
		}

		private void PrepareStampForSerialization(object sender, SerializedStampEventArgs ea)
		{
			PrepareStampForSerialization(ea.Properties, ea.Stamp);
		}

		private static void PrepareStampForSerialization(IItemProperties stampInstance, SerializedStamp stamp)
		{
			//if (stamp.TypeName == "StampGroup" && stampInstance is StampGroup stampGroup)
			if (stampInstance is IGroup group)
				foreach (IItemProperties childStamp in group.Children)
				{
					SerializedStamp serializedStamp = SerializedStamp.From(childStamp);
					stamp.AddChild(serializedStamp);
				}
		}

		private void ZoomAndPanControl_ZoomLevelChanged(object sender, EventArgs e)
		{
			if (Map.SelectedItems.Count > 0)
				UpdateItemSelectionUI();
		}

		public List<StampFolder> StampDirectories = new List<StampFolder>();

		void BuildStampsUI()
		{
			const string StampsPath = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\DragonH\Maps\Stamps";

			StampDirectories.Clear();
			IEnumerable<string> directories = Directory.EnumerateDirectories(StampsPath);
			foreach (string directory in directories)
			{
				StampDirectories.Add(new StampFolder(directory));
			}
			tbcStamps.ItemsSource = StampDirectories;
		}

		void BuildCharacterUI()
		{
			const string CharacterFolder = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\DragonH\Maps\Miniatures";
			IEnumerable<string> directories = Directory.EnumerateDirectories(CharacterFolder);
			List<MapCharacter> characters = new List<MapCharacter>();
			foreach (string directory in directories)
			{
				CharacterFinder.FindCharacters(directory, characters);
			}
			lbCharacters.ItemsSource = characters;
		}

		private void Map_WallsChanged(object sender, EventArgs e)
		{
			RebuildWalls();
		}

		private void RebuildWalls()
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
				Map.ClearTileSelection();
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

		void EnableSelectionControls(bool tileSelectionExists)
		{
			Visibility tileSelectionControlVisibility = tileSelectionExists ? Visibility.Visible : Visibility.Collapsed;
			Visibility stampControlVisibility = !tileSelectionExists ? Visibility.Visible : Visibility.Collapsed;

			tbcStampsAndCharacters.Visibility = stampControlVisibility;
			lstFlooring.Visibility = tileSelectionControlVisibility;
			spWallDoorControls.Visibility = tileSelectionControlVisibility;
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
					Tile tile = Map.TileMap[column, row];
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
					Tile tile = Map.TileMap[column, row];
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
			Map.ClearTileSelection();
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
			if (itemSelectionCanvas != null)
				itemSelectionCanvas.Children.Clear();
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
			if (!(tile is Tile floorSpace))
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
			ClearMapCanvasChildren();
			Map.Load(fileName);
			PrepareUIForNewMap();

			LoadFinalCanvasElements();
			SelectAllRoomsAndCorridors();
			OutlineWalls();
			AddSelectionCanvas();
			ClearSelection();
		}

		private void PrepareUIForNewMap()
		{
			allLayers.AddImagesToCanvas(content);
			SetCanvasSizeFromMap();

			AddTileOverlays();
		}

		private void ClearMapCanvasChildren()
		{
			content.Children.Clear();
		}

		private void AddTileOverlays()
		{
			foreach (Tile tile in Map.Tiles)
			{
				UIElement floorUI = null;

				if (tile.SpaceType != SpaceType.None)
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
		}

		void AddSelectionCanvas()
		{
			if (itemSelectionCanvas != null)
				itemSelectionCanvas.Children.Clear();

			itemSelectionCanvas = new Canvas();
			content.Children.Add(itemSelectionCanvas);
			MoveSelectionCanvasToTop();
		}

		private void MoveSelectionCanvasToTop()
		{
			Panel.SetZIndex(itemSelectionCanvas, int.MaxValue);
		}

		void SelectAllRooms()
		{
			FloodSelectAll(RegionType.Room, SelectionType.Add);
		}

		void SelectAllCorridors()
		{
			FloodSelectAll(RegionType.Corridor, SelectionType.Add);
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
			RemoveRubberBandSelector();
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
		bool lastMouseDownWasRightButton;
		private void Selector_MouseDown(object sender, MouseButtonEventArgs e)
		{
			mouseIsDown = true;
			lastMouseDownEventArgs = e;
			lastMouseDownWasRightButton = lastMouseDownEventArgs.RightButton == MouseButtonState.Pressed;
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
			if (canvas.Visibility == Visibility.Hidden)
				canvas.Visibility = Visibility.Visible;
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
		bool needToHideOriginalStamps;

		private void ZoomAndPanControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (activeStampCanvas != null)
			{
				DragCanvas(activeStampCanvas, e);
				return;
			}
			if (selectDragStampCanvas != null)
			{
				if (needToHideOriginalStamps)
				{
					needToHideOriginalStamps = false;
					if (e.LeftButton == MouseButtonState.Pressed)
					{
						ClearStampSelection();
						HideSelectedStamps();
					}
				}
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
			else
			{

			}
		}

		private void StopDraggingStamps(Point curContentMousePoint)
		{
			double secondsSinceMouseDown = (DateTime.Now - lastMouseDownTime).TotalSeconds;
			double deltaX = curContentMousePoint.X - lastMouseDownPoint.X;
			double deltaY = curContentMousePoint.Y - lastMouseDownPoint.Y;
			double totalXY = Math.Abs(deltaX) + Math.Abs(deltaY);
			if (secondsSinceMouseDown > 0.6 || secondsSinceMouseDown > 0.3 && totalXY > 20)
			{
				bool copy = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
				if (copy)
					CopySelectedStamps(deltaX, deltaY);
				else
					MoveSelectedStamps(deltaX, deltaY);
			}

			draggingStamps = false;
			ShowSelectedStamps();
			ClearFloatingCanvases();
		}
		void SetSelectedStampVisibility(bool isVisible)
		{
			stampsLayer.BeginUpdate();
			try
			{
				foreach (IItemProperties item in Map.SelectedItems)
					item.Visible = isVisible;
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
		}

		void HideSelectedStamps()
		{
			SetSelectedStampVisibility(false);
		}

		void ShowSelectedStamps()
		{
			SetSelectedStampVisibility(true);
		}

		private void MoveSelectedStamps(double deltaX, double deltaY)
		{
			ExecuteCommand("Move", new MoveData(deltaX, deltaY));
		}

		private void CopySelectedStamps(double deltaX, double deltaY)
		{
			ShowSelectedStamps();
			ExecuteCommand("Copy", new MoveData(deltaX, deltaY));
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

			if (!(baseSpace is Tile floorSpace))
				return;

			if (floorSpace.Parent == null)
				return;

			tileSelection.SelectionType = selectionType;

			//if (selection.SelectionType == SelectionType.Replace)
			//{
			//	Map.ClearSelection();
			//	ClearSelectionUI();
			//}

			List<Tile> roomTiles = floorSpace.Parent.Tiles.ConvertAll(x => x as Tile);
			SelectAllTiles(roomTiles);
		}

		void FloodSelectAll(Tile baseSpace, SelectionType selectionType)
		{
			if (selectionType == SelectionType.None)
				return;

			if (!(baseSpace is Tile floorSpace))
				return;

			if (floorSpace.Parent == null)
				return;

			FloodSelectAll(floorSpace.Parent.RegionType, selectionType);
		}

		private void FloodSelectAll(RegionType regionType, SelectionType selectionType)
		{
			tileSelection.SelectionType = selectionType;
			List<Tile> allRegionTiles = Map.GetAllMatchingTiles(regionType);
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
			double x = lastMouseDownPoint.X;
			double y = lastMouseDownPoint.Y;
			if (activeStamp is MapCharacter)
			{
				IItemProperties newCharacter = new MapCharacter(activeStamp.FileName, x, y);
				ExecuteCommand("AddItem", new ItemPropertiesData(newCharacter));
			}
			else
			{
				IStampProperties newStamp = new Stamp(activeStamp.FileName, x, y);
				ExecuteCommand("AddItem", new ItemPropertiesData(newStamp));
			}
		}

		void SelectItem(IItemProperties stamp)
		{
			if (Map.SelectedItems.IndexOf(stamp) < 0)
			{
				SelectionType selectionType = GetSelectionType(Map.SelectedItems.Count > 0);
				if (selectionType != SelectionType.Add)
					Map.SelectedItems.Clear();
				Map.SelectedItems.Add(stamp);
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

		//IStampProperties GetStampAt(double x, double y)
		//{
		//	return stampsLayer.GetItemAt(x, y) as IStampProperties;
		//}

		IItemProperties GetItemAt(double x, double y)
		{
			return stampsLayer.GetItemAt(x, y);
		}


		void GetItemSelectionBounds(List<IItemProperties> selectedStamps, out double left, out double top, out double width, out double height)
		{
			left = double.MaxValue;
			top = double.MaxValue;
			double right = 0;
			double bottom = 0;
			foreach (IItemProperties stamp in selectedStamps)
			{
				if (stamp == null)
					continue;
				double stampLeft = stamp.GetLeft();
				double stampTop = stamp.GetTop();
				double stampRight = stampLeft + stamp.Width;
				double stampBottom = stampTop + stamp.Height;
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

		void ShowItemContextMenu(IFloatingItem item, string contextMenuName)
		{
			if (item == null)
				return;
			ContextMenu ctxItem = FindResource(contextMenuName) as ContextMenu;
			if (ctxItem == null)
				return;
			ctxItem.IsOpen = true;
			item.Image.ContextMenu = ctxItem;
		}

		private void HandleMouseDownSelect()
		{
			if (Keyboard.IsKeyDown(Key.Space))  // Space + mouse down = pan view
				return;
			IItemProperties item = GetItemAt(lastMouseDownPoint.X, lastMouseDownPoint.Y);
			if (item != null)
			{
				SelectStamp(item);
				return;
			}


			ClearStampSelection();
			Map.SelectedItems.Clear();

			Tile baseSpace = GetTile(mouseDownSender);
			if (baseSpace != null)
				HandleTileClick(baseSpace);
		}

		private void SelectStamp(IItemProperties stamp)
		{
			ClearSelection();

			if (lastMouseDownWasRightButton)
			{
				if (Map.SelectedItems.IndexOf(stamp) < 0)
				{
					Map.SelectedItems.Clear();
					Map.SelectedItems.Add(stamp);
				}
				ShowItemContextMenu(stamp as IFloatingItem, "ctxStamp");
				return;
			}
			SelectionType selectionType = GetSelectionType(Map.SelectedItems.Count > 0);
			if (selectionType == SelectionType.Replace)
			{
				if (Map.SelectedItems.IndexOf(stamp) < 0)
					Map.SelectedItems.Clear();
			}

			if (selectionType == SelectionType.Remove)
			{
				Map.SelectedItems.Remove(stamp);
			}
			else if (Map.SelectedItems.IndexOf(stamp) < 0)
				Map.SelectedItems.Add(stamp);


			if (Map.SelectedItems.Count > 0)
			{
				draggingStamps = true;
				needToHideOriginalStamps = true;
				UpdateStampSelection();
				GetTopLeftOfStampSelection(out double left, out double top);

				mouseDragAdjustX = lastMouseDownPoint.X - left;
				mouseDragAdjustY = lastMouseDownPoint.Y - top;
			}
			else
				StampsAreNotSelected();
			return;
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

		void GetTopLeftOfStampSelection(out double left, out double top)
		{
			left = int.MaxValue;
			top = int.MaxValue;
			foreach (IItemProperties stamp in Map.SelectedItems)
			{
				double stampLeft = stamp.GetLeft();
				double stampTop = stamp.GetTop();

				if (stampLeft < left)
					left = stampLeft;
				if (stampTop < top)
					top = stampTop;
			}
		}

		void AddSelectedStampsToFloatingUI()
		{
			ClearFloatingCanvases();

			GetTopLeftOfStampSelection(out double left, out double top);
			mouseDragAdjustX = lastMouseDownPoint.X - left;
			mouseDragAdjustY = lastMouseDownPoint.Y - top;

			foreach (IItemProperties stamp in Map.SelectedItems)
			{
				CreateFloatingStamp(left, top, stamp as IFloatingItem);
			}
		}

		private void CreateFloatingStamp(double left, double top, IFloatingItem stamp)
		{
			if (stamp == null)
				return;

			double stampLeft = stamp.GetLeft();
			double stampTop = stamp.GetTop();
			double relativeX = stampLeft - left;
			double relativeY = stampTop - top;
			FloatingStamp(ref selectDragStampCanvas, stamp);
			stamp.CreateFloating(selectDragStampCanvas, relativeX, relativeY);
		}

		private void UpdateStampSelection()
		{
			HideTilingControls();
			AddSelectedStampsToFloatingUI();
			UpdateItemSelectionUI();
		}

		void CreateResizeCorner(ResizeTracker resizeTracker, double left, double top, SizeDirection sizeDirection)
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
			itemSelectionCanvas.Children.Add(ellipse);
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
			double scaleAdjust = newWidth / resizeTracker.Item.Width;

			ExecuteCommand("Scale", new ScaleData(scaleAdjust));

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
			Point newPosition = e.GetPosition(itemSelectionCanvas);
			newPosition.Offset(-mouseResizeOffsetX, -mouseResizeOffsetY);
			double oppositeX = resizeOppositeCornerPosition.X;
			double oppositeY = resizeOppositeCornerPosition.Y;
			double deltaX = newPosition.X - oppositeX;
			double deltaY = newPosition.Y - oppositeY;
			double newAspectRatio = deltaX / deltaY;
			IItemProperties item = resizeTracker.Item;
			double originalAspectRatio = item.Width / item.Height;
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
					rightAdjust = -newWidth - item.Width;
					bottomAdjust = -newHeight - item.Height;
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

					leftAdjust = item.Width - newWidth;
					bottomAdjust = newHeight - item.Height;
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
					rightAdjust = -item.Width - newWidth;

					topAdjust = newHeight + item.Height;
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
					leftAdjust = item.Width - newWidth;
					topAdjust = item.Height - newHeight;
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
			left = item.X - width / 2;
			top = item.Y - height / 2;
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
			Point absolutePosition = e.GetPosition(itemSelectionCanvas);
			absolutePosition.Offset(-mouseResizeOffsetX, -mouseResizeOffsetY);
			IItemProperties item = resizeTracker.Item;

			switch (resizeTracker.GetDirection(ellipse))
			{
				case SizeDirection.NorthWest:
					resizeOppositeCornerPosition = new Point(absolutePosition.X + item.Width, absolutePosition.Y + item.Height);
					break;
				case SizeDirection.NorthEast:
					resizeOppositeCornerPosition = new Point(absolutePosition.X - item.Width, absolutePosition.Y + item.Height);
					break;
				case SizeDirection.SouthWest:
					resizeOppositeCornerPosition = new Point(absolutePosition.X + item.Width, absolutePosition.Y - item.Height);
					break;
				case SizeDirection.SouthEast:
					resizeOppositeCornerPosition = new Point(absolutePosition.X - item.Width, absolutePosition.Y - item.Height);
					break;
			}
			activeStampResizing = item;
			ellipse.CaptureMouse();
		}

		void StackScaleButtons(string buttonName, double compareScale, double? selectedButtonScale, double buttonSize, double rowColumnSpacer, double x, ref double y, double maxTop)
		{
			if (y < maxTop)
				return;
			Viewbox btnScale = CreateButton(buttonName, x - buttonSize, y, buttonSize);
			if (!selectedButtonScale.HasValue || selectedButtonScale.Value != compareScale)
			{
				btnScale.Visibility = Visibility.Visible;
				y -= buttonSize + rowColumnSpacer;
			}
			else
				btnScale.Visibility = Visibility.Hidden;
		}

		bool HasAtLeastOneGroupSelected()
		{
			return Map.SelectedItems.Any(x => x is IGroup);
		}

		Dictionary<ContentControl, PropertyEditorStatus> propEdStatus = new Dictionary<ContentControl, PropertyEditorStatus>();

		StackPanel GetPropertyEditors(ContentControl propertyGrid)
		{
			if (propertyGrid == null)
				return null;

			if (!(propertyGrid.Content is Viewbox viewbox))
				return null;

			if (!(viewbox.Child is Grid grid))
				return null;

			foreach (FrameworkElement uIElement in grid.Children)
			{
				if (uIElement.Name == "spPropertyEditors")
					return uIElement as StackPanel;
			}
			return null;
		}

		UIElement GetBooleanEditor(PropertyValueData propertyValueData)
		{
			BooleanEditor checkBox = new BooleanEditor();
			checkBox.PropertyName = propertyValueData.Name;
			checkBox.PropertyType = propertyValueData.Type;
			checkBox.Content = propertyValueData.DisplayText;
			checkBox.Foreground = Brushes.White;
			checkBox.Margin = new Thickness(4, 0, 4, 4);
			if (!propertyValueData.HasInconsistentValues)
				checkBox.IsChecked = propertyValueData.BoolValue.Value;
			else
				checkBox.IsChecked = null;

			checkBox.Checked += BooleanCheckBox_Changed;
			checkBox.Unchecked += BooleanCheckBox_Changed;
			return checkBox;
		}

		UIElement GetTextEditor(PropertyValueData propertyValueData)
		{
			TextEditor textEditor = new TextEditor();
			textEditor.Initialize(propertyValueData);
			textEditor.TextChanged += TextEditor_TextChanged;

			//PropertyInfo propertyInfo = propertyValueData.FirstInstance.GetType().GetProperty(propertyValueData.Name);

			string text = propertyValueData.StringValue;
			if (propertyValueData.HasInconsistentValues)
				text = "";
			textEditor.AddText(text);
			return textEditor;
		}

		UIElement GetDoubleEditor(PropertyValueData propertyValueData, int decimalPoints)
		{
			DoubleEditor doubleEditor = new DoubleEditor(decimalPoints);
			doubleEditor.Initialize(propertyValueData);
			doubleEditor.TextChanged += DoubleEditor_TextChanged;

			double? numValue = propertyValueData.NumericValue;
			if (propertyValueData.HasInconsistentValues)
				doubleEditor.AddText(null);
			else
				doubleEditor.AddText(numValue.ToString());

			return doubleEditor;
		}

		UIElement GetEnumEditor(PropertyValueData propertyValueData)
		{
			EnumEditor enumEditor = new EnumEditor();
			enumEditor.ValueChanged += EnumEditor_ValueChanged;
			enumEditor.PropertyName = propertyValueData.Name;
			enumEditor.PropertyType = propertyValueData.Type;

			PropertyInfo propertyInfo = propertyValueData.FirstInstance.GetType().GetProperty(propertyValueData.Name);
			enumEditor.AddDisplayText(propertyValueData.DisplayText);
			Type propType = propertyInfo.PropertyType;
			foreach (object value in Enum.GetValues(propType))
			{
				MemberInfo[] memberInfo = propType.GetMember(value.ToString());
				string name = Enum.GetName(propType, value);
				if (memberInfo.Length > 0)
				{
					object[] attributes = memberInfo[0].GetCustomAttributes(typeof(DisplayTextAttribute), false);
					if (attributes != null && attributes.Length > 0)
						if (attributes[0] is DisplayTextAttribute displayTextAttribute)
							name = displayTextAttribute.DisplayText;
				}


				bool optionChecked = (int)value == propertyValueData.NumericValue;
				if (propertyValueData.HasInconsistentValues)
					optionChecked = false;
				enumEditor.AddOption(name, (int)value, optionChecked);
			}

			//Foreground = Brushes.White;
			// .Margin = new Thickness(4, 0, 4, 4);
			//if (propertyValueData.BoolValue.HasValue)
			//	enumEditor.IsChecked = propertyValueData.BoolValue.Value;
			//enumEditor.Checked += BooleanCheckBox_Changed;
			//enumEditor.Unchecked += BooleanCheckBox_Changed;
			return enumEditor;
		}

		private void ChangeProperty<T>(string propertyName, T value)
		{
			ExecuteCommand<T>("ChangeProperty", new ChangeData<T>(propertyName, value));
		}

		private void DoubleEditor_TextChanged(object sender, TextChangedEventArgs ea)
		{
			if (!(sender is DoubleEditor doubleEditor))
				return;
			if (doubleEditor.Value != null)
				ChangeProperty(doubleEditor.PropertyName, doubleEditor.Value);
		}

		private void TextEditor_TextChanged(object sender, TextChangedEventArgs ea)
		{
			if (!(sender is TextEditor textEditor))
				return;
			if (textEditor.Value != null)
				ChangeProperty(textEditor.PropertyName, textEditor.Value);
		}

		private void EnumEditor_ValueChanged(object sender, ValueChangedEventArgs ea)
		{
			if (!(sender is EnumEditor enumEditor))
				return;
			if (enumEditor.Value.HasValue)
				ChangeProperty(enumEditor.PropertyName, enumEditor.Value.Value);
		}

		private void BooleanCheckBox_Changed(object sender, RoutedEventArgs e)
		{
			if (!(sender is BooleanEditor booleanEditor))
				return;
			ChangeProperty(booleanEditor.PropertyName, booleanEditor.IsChecked == true);
		}

		UIElement GetEditor(PropertyValueData propertyValueData)
		{
			switch (propertyValueData.Type)
			{
				case PropertyType.Boolean:
					return GetBooleanEditor(propertyValueData);
				case PropertyType.Enum:
					return GetEnumEditor(propertyValueData);
				case PropertyType.String:
					return GetTextEditor(propertyValueData);
				case PropertyType.Number:
					return GetDoubleEditor(propertyValueData, propertyValueData.NumDecimalPlaces);
			}
			return null;
		}

		BooleanEditor GetBooleanEditorForProperty(List<UIElement> editors, string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
				return null;
			foreach (UIElement uIElement in editors)
				if (uIElement is IPropertyEditor propertyEditor)
				{
					if (propertyEditor.PropertyName == propertyName)
						return uIElement as BooleanEditor;
				}
			return null;
		}
		void PopulatePropertyGrid(ContentControl propertyGrid)
		{
			StackPanel spPropertyEditors = GetPropertyEditors(propertyGrid);

			spPropertyEditors.Children.Clear();

			PropertyValueComparer propertyValueComparer = new PropertyValueComparer();

			// TODO: First collect all properties and whether they are the same or not.
			foreach (IItemProperties stampProperties in Map.SelectedItems)
			{
				propertyValueComparer.Compare(stampProperties);
			}

			List<UIElement> editors = new List<UIElement>();
			foreach (PropertyValueData propertyValueData in propertyValueComparer.Comparisons)
			{
				UIElement editor = GetEditor(propertyValueData);
				editors.Add(editor);
				if (editor != null)
					spPropertyEditors.Children.Add(editor);
			}

			foreach (UIElement uIElement in editors)
				if (uIElement is IHasDependentProperty dependentPropertyHolder)
				{
					BooleanEditor booleanEditor = GetBooleanEditorForProperty(editors, dependentPropertyHolder.DependentProperty);
					if (booleanEditor != null)
						booleanEditor.SetDependentUI(uIElement);
				}
		}

		bool HasInterface<T>(List<Type> interfaces)
		{
			return interfaces.Contains(typeof(T));
		}

		void AddItemSelectionButtons(double? selectedButtonScale, double? selectedHueShift, double? selectedSaturation, double? selectedLightness, double? selectedContrast, double left, double top, double width, double height, List<Type> consistentInterfaces, SelectionContents selectionContents)
		{
			double contentScale = zoomAndPanControl.ContentScale;
			double rowColumnSpacer = 3 / contentScale;
			double right = left + width;
			double bottom = top + height;
			double bottomRow = bottom + rowColumnSpacer;
			double buttonSize = StampButtonSize / contentScale;
			double topRow = top - buttonSize - rowColumnSpacer;
			double alignRow = topRow - buttonSize - rowColumnSpacer;
			double distributeRow = alignRow - buttonSize - rowColumnSpacer;
			double alignColumn = right + buttonSize + rowColumnSpacer;
			double distributeColumn = alignColumn + buttonSize + rowColumnSpacer;
			if (HasInterface<IArrangeable>(consistentInterfaces))
			{
				CreateButton("btnRotateLeft", left - buttonSize, topRow, buttonSize);
				CreateButton("btnRotateRight", right, topRow, buttonSize);
			}
			double middleButtonX = left + width / 2 - buttonSize / 2;
			double middleButtonY = top + height / 2 - buttonSize / 2;

			if (width >= buttonSize && HasInterface<IArrangeable>(consistentInterfaces))
			{
				CreateButton("btnFlipVertical", middleButtonX, topRow, buttonSize);
			}

			if (height >= buttonSize && HasInterface<IArrangeable>(consistentInterfaces))
				CreateButton("btnFlipHorizontal", right, middleButtonY, buttonSize);

			double yPos = bottom - buttonSize;
			if (HasInterface<IScalable>(consistentInterfaces))
			{
				StackScaleButtons("btnScale33Percent", (double)1 / 3, selectedButtonScale, buttonSize, rowColumnSpacer, left, ref yPos, top);
				StackScaleButtons("btnScale50Percent", (double)1 / 2, selectedButtonScale, buttonSize, rowColumnSpacer, left, ref yPos, top);
				StackScaleButtons("btnScale100Percent", 1, selectedButtonScale, buttonSize, rowColumnSpacer, left, ref yPos, top);
				StackScaleButtons("btnScale200Percent", 2, selectedButtonScale, buttonSize, rowColumnSpacer, left, ref yPos, top);
			}

			CreateButton("btnSendToBack", right, bottomRow, buttonSize);
			CreateButton("btnBringToFront", right + buttonSize + rowColumnSpacer, bottomRow, buttonSize);

			//bool hasColorMod = selectedHueShift.HasValue && selectedHueShift.Value != 0 ||
			//									 selectedSaturation.HasValue && selectedSaturation.Value != 0 ||
			//									 selectedLightness.HasValue && selectedLightness.Value != 0 ||
			//									 selectedContrast.HasValue && selectedContrast.Value != 0;

			double pixelWidth = width * contentScale;

			ContentControl colorControls = null;

			if (pixelWidth > minWidthPropertyEditor)
			{
				AddPropertyEditor(rowColumnSpacer, right, bottomRow, buttonSize, pixelWidth);
			}

			if (pixelWidth > minWidthColorControlEditor)
			{
				double newWidth = GetNewWidth(pixelWidth, minWidthColorControlEditor, maxWidthColorControlEditor);
				if (HasInterface<IModifiableColor>(consistentInterfaces))
					colorControls = CreateColorControls(selectedHueShift, selectedSaturation, selectedLightness, selectedContrast, left, bottomRow, buttonSize, newWidth);
			}

			if (colorControls != null)
			{
				ShowPropertyEditor(colorControls);
			}

			bool hasMoreThanOneSelected = Map.SelectionHasAtLeast<IItemProperties>(2);
			bool hasAtLeastOneGroupSelected = HasAtLeastOneGroupSelected();
			double secondRow = bottomRow + buttonSize + rowColumnSpacer;
			double groupUngroupLeft = right;
			if (hasMoreThanOneSelected && selectionContents != SelectionContents.Mix)
			{
				CreateButton("btnGroup", groupUngroupLeft, secondRow, buttonSize);
				groupUngroupLeft += buttonSize + rowColumnSpacer;
			}
			if (hasAtLeastOneGroupSelected)
			{
				CreateButton("btnUngroup", groupUngroupLeft, secondRow, buttonSize);
			}

			if (hasMoreThanOneSelected)
			{
				double longButtonLength = buttonSize * 1.6;
				double longButtonIndent = (longButtonLength - buttonSize) / 2;
				if (width >= buttonSize)
				{
					CreateButton("btnAlignHorizontalCenter", middleButtonX, alignRow, buttonSize);
					if (Map.SelectionHasAtLeast<IItemProperties>(3))
						CreateButton("btnDistributeHorizontally", middleButtonX - longButtonIndent, distributeRow, longButtonLength, buttonSize);
					if (width >= 3 * buttonSize + 2 * rowColumnSpacer)
					{
						CreateButton("btnAlignLeft", left, alignRow, buttonSize);
						CreateButton("btnAlignRight", right - buttonSize, alignRow, buttonSize);
					}
				}
				if (height >= buttonSize)
				{
					CreateButton("btnAlignVerticalCenter", alignColumn, middleButtonY, buttonSize);
					if (Map.SelectionHasAtLeast<IItemProperties>(3))
						CreateButton("btnDistributeVertically", distributeColumn, middleButtonY - longButtonIndent, buttonSize, longButtonLength);
					if (height >= 3 * buttonSize + 2 * rowColumnSpacer)
					{
						CreateButton("btnAlignTop", alignColumn, top, buttonSize);
						CreateButton("btnAlignBottom", alignColumn, bottom - buttonSize, buttonSize);
					}
				}
			}
		}

		private void AddPropertyEditor(double rowColumnSpacer, double right, double bottomRow, double buttonSize, double pixelWidth)
		{
			double newWidth = GetNewWidth(pixelWidth, minWidthPropertyEditor, maxWidthPropertyEditor);

			double propGridRightEdge = right - rowColumnSpacer;
			ContentControl propertyGrid = GetPropertyGrid();
			propertyGrid.Width = newWidth;
			itemSelectionCanvas.Children.Add(propertyGrid);
			Panel.SetZIndex(propertyGrid, int.MaxValue);
			Canvas.SetLeft(propertyGrid, propGridRightEdge - propertyGrid.Width);
			Canvas.SetTop(propertyGrid, bottomRow);
			CreateButton("btnShowPropertyEditor", propGridRightEdge - buttonSize, bottomRow, buttonSize);
			if (!propEdStatus.ContainsKey(propertyGrid))
				propEdStatus.Add(propertyGrid, new PropertyEditorStatus("btnShowPropertyEditor"));
			ShowPropertyEditor(propertyGrid);
			PopulatePropertyGrid(propertyGrid);
		}

		private double GetNewWidth(double pixelWidth, int minimumWidth, int maximumWidth)
		{
			double newWidth = pixelWidth;
			double maxWidth = maximumWidth / zoomAndPanControl.ContentScale;
			double minWidth = minimumWidth / zoomAndPanControl.ContentScale;
			if (newWidth > maxWidth)
				newWidth = maxWidth;
			if (newWidth < minWidth)
				newWidth = minWidth;
			return newWidth;
		}

		private const int minWidthPropertyEditor = 150;
		private const int maxWidthPropertyEditor = 500;
		private const int minWidthColorControlEditor = 150;
		private const int maxWidthColorControlEditor = 220;

		private ContentControl CreateColorControls(double? selectedHueShift, double? selectedSaturation, double? selectedLightness, double? selectedContrast, double left, double bottomRow, double buttonSize, double newWidth)
		{
			ContentControl colorControls = GetColorControls();
			colorControls.Width = newWidth; // Math.Min(width, maxPixelWidth / zoomAndPanControl.ContentScale);
			itemSelectionCanvas.Children.Add(colorControls);
			//Canvas.SetLeft(colorControls, left + (width - colorControls.Width) / 2);
			Canvas.SetLeft(colorControls, left);
			Canvas.SetTop(colorControls, bottomRow);
			SetSlider(GetActiveHueSlider(), selectedHueShift);
			SetSlider(GetActiveSaturationSlider(), selectedSaturation);
			SetSlider(GetActiveLightnessSlider(), selectedLightness);
			SetSlider(GetActiveContrastSlider(), selectedContrast);
			CreateButton("btnShowColorControls", left, bottomRow, buttonSize);

			if (!propEdStatus.ContainsKey(colorControls))
				propEdStatus.Add(colorControls, new PropertyEditorStatus("btnShowColorControls"));

			return colorControls;
		}

		bool changingInternally;

		private void SetSlider(Slider slider, double? value)
		{
			if (slider == null)
				return;

			slider.IsEnabled = value.HasValue;
			if (value.HasValue)
			{
				changingInternally = true;
				try
				{
					slider.Value = value.Value;
				}
				finally
				{
					changingInternally = false;
				}
			}
		}

		private Viewbox CreateButton(string buttonName, double x, double y, double buttonWidth, double buttonHeight = -1)
		{
			if (buttonHeight == -1)
				buttonHeight = buttonWidth;
			Viewbox viewbox = FindResource(buttonName) as Viewbox;
			viewbox.Visibility = Visibility.Visible;
			viewbox.Width = buttonWidth;
			viewbox.Height = buttonHeight;
			viewbox.Cursor = Cursors.Hand;
			if (viewbox.Parent is Canvas canvas)
				canvas.Children.Remove(viewbox);
			itemSelectionCanvas.Children.Add(viewbox);
			Canvas.SetLeft(viewbox, x);
			Canvas.SetTop(viewbox, y);
			return viewbox;
		}

		void AddStampSelectionUI(IItemProperties item)
		{
			double left = item.GetLeft();
			double top = item.GetTop();
			double right = left + item.Width;
			double bottom = top + item.Height;

			ResizeTracker resizeTracker = new ResizeTracker(item);
			AddItemSelectionRect(item, left, top, resizeTracker);
			CreateResizeCorner(resizeTracker, left, top, SizeDirection.NorthWest);
			CreateResizeCorner(resizeTracker, right, top, SizeDirection.NorthEast);
			CreateResizeCorner(resizeTracker, left, bottom, SizeDirection.SouthWest);
			CreateResizeCorner(resizeTracker, right, bottom, SizeDirection.SouthEast);
		}

		private void AddItemSelectionRect(IItemProperties item, double left, double top, ResizeTracker resizeTracker = null)
		{
			Rectangle selectionRect = new Rectangle();
			if (resizeTracker != null)
				resizeTracker.SelectionRect = selectionRect;
			selectionRect.Width = item.Width;
			selectionRect.Height = item.Height;
			Canvas.SetLeft(selectionRect, left);
			Canvas.SetTop(selectionRect, top);
			selectionRect.Stroke = Brushes.Gray;
			selectionRect.StrokeThickness = 1 / zoomAndPanControl.ContentScale;
			itemSelectionCanvas.Children.Add(selectionRect);
		}

		void AddItemSelectionButtons(double? selectedButtonScale, double? selectedHueShift, double? selectedSaturation, double? selectedLightness, double? selectedContrast, List<Type> consistentInterfaces, SelectionContents selectionContents)
		{
			GetItemSelectionBounds(Map.SelectedItems, out double left, out double top, out double width, out double height);
			AddItemSelectionButtons(selectedButtonScale, selectedHueShift, selectedSaturation, selectedLightness, selectedContrast, left, top, width, height, consistentInterfaces, selectionContents);
		}

		void UpdateItemSelectionUI()
		{
			ClearSelectionUI();
			double? selectedButtonScale, selectedHueShift, selectedSaturation, selectedLightness, selectedContrast;
			List<Type> consistentInterfaces;
			SelectionContents selectionContents;
			GetConsistentModSettings(out selectedButtonScale, out selectedHueShift, out selectedSaturation, out selectedLightness, out selectedContrast, out consistentInterfaces, out selectionContents);

			AddItemSelectionButtons(selectedButtonScale, selectedHueShift, selectedSaturation, selectedLightness, selectedContrast, consistentInterfaces, selectionContents);
			foreach (IItemProperties stamp in Map.SelectedItems)
				AddStampSelectionUI(stamp);

			MoveSelectionCanvasToTop();
		}

		void AddAllInterfaces(List<Type> consistentInterfaces, IItemProperties item)
		{
			foreach (Type type in Known.Interfaces)
			{
				if (type.IsAssignableFrom(item.GetType()))
				{
					consistentInterfaces.Add(type);
				}
			}
		}

		void RemoveMissingInterfaces(List<Type> consistentInterfaces, IItemProperties item)
		{
			if (consistentInterfaces.Count == 0)
				return;

			List<Type> interfacesToRemove = new List<Type>();

			foreach (Type type in consistentInterfaces)
				if (!type.IsAssignableFrom(item.GetType()))
					interfacesToRemove.Add(type);

			foreach (Type type in interfacesToRemove)
				consistentInterfaces.Remove(type);
		}
		public enum SelectionContents
		{
			AllStamps,
			AllCharacters,
			Mix
		}

		SelectionContents GetSelectionContents(List<IItemProperties> selectedItems)
		{
			SelectionContents selectionContents = SelectionContents.Mix;
			bool firstTime = true;
			foreach (IItemProperties item in selectedItems)
			{
				if (firstTime)
				{
					if (item is IStampProperties)
						selectionContents = SelectionContents.AllStamps;
					else
						selectionContents = SelectionContents.AllCharacters;
					firstTime = false;
				}
				else
				{
					if (item is IStampProperties)
					{
						if (selectionContents != SelectionContents.AllStamps)
							return SelectionContents.Mix;
					}
					else if (selectionContents == SelectionContents.AllStamps)
						return SelectionContents.Mix;
				}
			}
			return selectionContents;
		}

		private void GetConsistentModSettings(out double? selectedButtonScale, out double? selectedHueShift, out double? selectedSaturation, out double? selectedLightness, out double? selectedContrast, out List<Type> consistentInterfaces, out SelectionContents selectionContents)
		{
			consistentInterfaces = new List<Type>();
			selectedButtonScale = null;
			selectedHueShift = null;
			selectedSaturation = null;
			selectedLightness = null;
			selectedContrast = null;
			selectionContents = GetSelectionContents(Map.SelectedItems);
			bool firstTime = true;
			foreach (IItemProperties item in Map.SelectedItems)
			{
				if (firstTime)
					AddAllInterfaces(consistentInterfaces, item);
				else
					RemoveMissingInterfaces(consistentInterfaces, item);

				if (item is IScalable scalable)
				{
					if (firstTime)
						selectedButtonScale = scalable.Scale;
					else if (selectedButtonScale != scalable.Scale)
						selectedButtonScale = null;
				}
				else
					selectedButtonScale = null;

				if (item is IModifiableColor color)
				{
					if (firstTime)
					{
						selectedHueShift = color.HueShift;
						selectedSaturation = color.Saturation;
						selectedLightness = color.Lightness;
						selectedContrast = color.Contrast;
					}
					else
					{
						if (selectedHueShift != color.HueShift)
							selectedHueShift = null;
						if (selectedSaturation != color.Saturation)
							selectedSaturation = null;
						if (selectedLightness != color.Lightness)
							selectedLightness = null;
						if (selectedContrast != color.Contrast)
							selectedContrast = null;
					}
				}
				else
				{
					selectedHueShift = null;
					selectedSaturation = null;
					selectedLightness = null;
					selectedContrast = null;
				}

				firstTime = false;
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
			this.Title = $"Tile (col = {baseSpace.Column}, row = {baseSpace.Row})";
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
			ZoomToTileSelection();
		}

		void ZoomToTileSelection()
		{
			Map.GetSelectionBoundaries(out int left, out int top, out int right, out int bottom);
			if (right == 0 || bottom == 0 || left == int.MaxValue || top == int.MaxValue)
				return;  // No selection!!?
			zoomAndPanControl.AnimatedZoomTo(new Rect(left - Tile.Width / 2.0, top - Tile.Height / 2.0, right - left + Tile.Width, bottom - top + Tile.Height));
		}
		private void HandleDoubleClickTileSelect(Tile baseSpace)
		{
			FloodSelect(baseSpace, GetSelectionType(Map.SelectionExists()));
			ZoomToTileSelection();
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
				if (!tile.IsFloor)
					tile.IsFloor = true;
				content.Children.Remove(tile.UIElementFloor as UIElement);
				content.Children.Remove(tile.UIElementOverlay as UIElement);
				content.Children.Remove(tile.SelectorPanel as UIElement);

				string imageFileName = null;
				AddFloorTile(tile, texture.GetImage(tile.Column, tile.Row, ref imageFileName));
				tile.ImageFileName = imageFileName;
				tile.BaseTextureName = texture.BaseName;
				AddFloorOverlay(tile);
				AddSelector(tile);
			}
			Map.UpdateIfNeeded();
			LoadFinalCanvasElements();
		}
		//private void Button_ApplyDebris(object sender, RoutedEventArgs e)
		//{
		//	if (!(sender is Button button))
		//		return;
		//	BaseTexture texture = (BaseTexture)button.Tag;
		//	List<Tile> selection = Map.GetSelection();
		//	foreach (Tile tile in selection)
		//	{
		//		AddDebris(tile, texture.GetImage(tile.Column, tile.Row));
		//	}
		//}

		private void AddFloorOverlay(Tile tile)
		{
			depthLayer.DrawImageOverTile(imageLightTile, tile);
		}

		private void AddFloorTile(Tile tile, Image image)
		{
			floorLayer.DrawImageOverTile(image, tile);
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
		// TODO: Need to move this...
		StampsLayer stampsLayer = new StampsLayer();
		Layer wallLayer = new Layer() { OuterMargin = Tile.Width / 2 };
		Layer floorLayer = new Layer();
		Layers allLayers = new Layers();

		void AssignMapToStampsLayer()
		{
			stampsLayer.Map = Map;
		}

		void InitializeLayers()
		{
			allLayers.Add(floorLayer);
			allLayers.Add(depthLayer);
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
				if (tile is Tile floorSpace)
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
					if (!Map.TileExists(column, row) || !Map.TileMap[column, row].Selected)
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
			Map.SelectedItems.Clear();
			SetRubberBandVisibility(Visibility.Hidden);
			Cursor = Cursors.Hand;
			lstFlooring.Visibility = Visibility.Collapsed;
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
		IItemProperties activeStamp;
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
			if (!(button.Tag is Stamp stamp))
				return;
			FloatingStamp(ref activeStampCanvas, stamp);
			stamp.CreateFloating(activeStampCanvas);
			mouseDragAdjustX = -stamp.GetLeft();
			mouseDragAdjustY = -stamp.GetTop();
			MapEditMode = MapEditModes.Stamp;
		}

		private void FloatingStamp(ref Canvas canvas, IItemProperties stamp)
		{
			activeStamp = stamp;
			if (canvas != null)
				return;

			canvas = new Canvas();
			content.Children.Add(canvas);
			canvas.IsHitTestVisible = false;
			canvas.Visibility = Visibility.Hidden;
			Panel.SetZIndex(canvas, int.MaxValue);
		}

		double mouseDragAdjustX;
		double mouseDragAdjustY;
		Canvas itemSelectionCanvas;
		IItemProperties activeStampResizing;
		Timer hueShiftUpdateTimer;
		Timer saturationUpdateTimer;
		Timer lightnessUpdateTimer;
		Timer contrastUpdateTimer;


		private void CancelStampMode_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (MapEditMode == MapEditModes.Stamp)
				MapEditMode = MapEditModes.Select;
			else
			{
				if (Map.SelectionExists() || Map.SelectedItems.Count > 0)
				{
					ClearSelectionUI();
					ClearSelection();
					Map.SelectedItems.Clear();
				}
				else
					zoomAndPanControl.AnimatedZoomTo(new Rect(0, 0, content.Width, content.Height));
			}
		}

		// TODO: Delete...
		public static void SaveDefaultTemplate()
		{
			var control = Application.Current.FindResource(typeof(Button));
			using (XmlTextWriter writer = new XmlTextWriter(@"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\DndMapSpike\ButtonDefaultTemplate.xml", System.Text.Encoding.UTF8))
			{
				writer.Formatting = System.Xml.Formatting.Indented;
				XamlWriter.Save(control, writer);
			}
		}

		private void btnRotateRight_MouseDown(object sender, MouseButtonEventArgs e)
		{
			ExecuteCommand("RotateRight");
		}

		private void btnRotateLeft_MouseDown(object sender, MouseButtonEventArgs e)
		{
			ExecuteCommand("RotateLeft");
		}

		private void btnFlipVertical_MouseDown(object sender, MouseButtonEventArgs e)
		{
			ExecuteCommand("VerticalFlip");
		}

		private void btnFlipHorizontal_MouseDown(object sender, MouseButtonEventArgs e)
		{
			ExecuteCommand("HorizontalFlip");
		}

		public enum CommandExecutionType
		{
			Execute,
			Undo,
			Redo
		}

		string lastCommandChangeID;
		void ExecuteCommand<T>(string commandType, object data = null)
		{
			ExecutingCommand();
			ExecuteCommand(CommandFactory.Create<T>(commandType, data));
		}

		private void ExecutingCommand()
		{
			if (interactiveChangeID != null && lastCommandChangeID == interactiveChangeID)
			{
				BaseCommand poppedCommand = undoStack.Pop();
				poppedCommand.Undo(Map);
			}

			lastCommandChangeID = interactiveChangeID;
		}

		void ExecuteCommand(string commandType, object data = null)
		{
			ExecutingCommand();
			ExecuteCommand(CommandFactory.Create(commandType, data));
		}

		void ExecuteCommand(BaseCommand command)
		{
			if (command == null)
				return;
			redoStack.Clear();
			undoStack.Push(command);
			ExecuteCommand(command, CommandExecutionType.Execute);
		}

		private void ExecuteCommand(BaseCommand command, CommandExecutionType executionType)
		{
			if (command.WorksOnStamps)
			{
				stampsLayer.BeginUpdate();
				try
				{
					ShowSelectedStamps(); // Need for the move. Consider **only** calling when it's actually a Move.
					switch (executionType)
					{
						case CommandExecutionType.Execute:
							command.Execute(Map, Map.SelectedItems);
							break;
						case CommandExecutionType.Undo:
							command.Undo(Map);
							break;
						case CommandExecutionType.Redo:
							command.Redo(Map);
							break;
					}
				}
				finally
				{
					stampsLayer.EndUpdate();
				}
				UpdateItemSelectionUI();
			}
		}

		private void btnScale100Percent_MouseDown(object sender, MouseButtonEventArgs e)
		{
			ExecuteCommand("ScaleAbsolutePercent", new DoubleData(1));
		}

		private void btnScale200Percent_MouseDown(object sender, MouseButtonEventArgs e)
		{
			ExecuteCommand("ScaleAbsolutePercent", new DoubleData(2));
		}

		private void btnScale50Percent_MouseDown(object sender, MouseButtonEventArgs e)
		{
			ExecuteCommand("ScaleAbsolutePercent", new DoubleData(0.5));
		}

		private void btnScale33Percent_MouseDown(object sender, MouseButtonEventArgs e)
		{
			ExecuteCommand("ScaleAbsolutePercent", new DoubleData(1.0 / 3));
		}

		private void btnSendToBack_MouseDown(object sender, MouseButtonEventArgs e)
		{
			ExecuteCommand("SendToBack");
		}

		private void btnBringToFront_MouseDown(object sender, MouseButtonEventArgs e)
		{
			ExecuteCommand("BringToFront");
		}

		private void HueShiftUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			hueShiftUpdateTimer.Stop();

			Dispatcher.Invoke(() =>
			{
				ExecuteCommand("HueChange", new DoubleData(hueShiftToApply));
			});
		}

		private void SaturationUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			saturationUpdateTimer.Stop();

			Dispatcher.Invoke(() =>
			{
				ExecuteCommand("SaturationChange", new DoubleData(saturationToApply));
			});
		}
		private void LightnessUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			lightnessUpdateTimer.Stop();

			Dispatcher.Invoke(() =>
			{
				ExecuteCommand("LightnessChange", new DoubleData(lightnessToApply));
			});
		}
		private void ContrastUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			contrastUpdateTimer.Stop();

			Dispatcher.Invoke(() =>
			{
				ExecuteCommand("ContrastChange", new DoubleData(contrastToApply));
			});
		}

		double hueShiftToApply;
		double saturationToApply;
		double lightnessToApply;
		double contrastToApply;

		private void HueShiftSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (changingInternally)
				return;
			if (!(sender is Slider slider))
				return;

			StartInteractiveChange("Hue");
			hueShiftToApply = slider.Value;
			hueShiftUpdateTimer.Stop();
			hueShiftUpdateTimer.Start();
		}

		private void SaturationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (changingInternally)
				return;
			if (!(sender is Slider slider))
				return;

			StartInteractiveChange("Saturation");
			saturationToApply = slider.Value;
			saturationUpdateTimer.Stop();
			saturationUpdateTimer.Start();
		}
		private void LightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (changingInternally)
				return;
			if (!(sender is Slider slider))
				return;

			StartInteractiveChange("Lightness");
			lightnessToApply = slider.Value;
			lightnessUpdateTimer.Stop();
			lightnessUpdateTimer.Start();
		}

		private void ContrastSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (changingInternally)
				return;
			if (!(sender is Slider slider))
				return;

			StartInteractiveChange("Contrast");
			contrastToApply = slider.Value;
			contrastUpdateTimer.Stop();
			contrastUpdateTimer.Start();
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

			if (!(ccColorControls.Content is Viewbox viewbox))
				return null;

			if (!(viewbox.Child is Grid grid))
				return null;

			foreach (UIElement uIElement in grid.Children)
				if (uIElement is StackPanel stackPanel)
					foreach (UIElement stackChild in stackPanel.Children)
						if (stackChild is Slider slider && slider.Name == sliderName)
							return slider;

			return null;
		}

		private void HueShiftReset_Click(object sender, RoutedEventArgs e)
		{
			//! TODO: Needs to play with undo/redo!
			ResetHueSlider();
			stampsLayer.BeginUpdate();
			try
			{
				foreach (IStampProperties stamp in Map.Filter<IStampProperties>())
					stamp.HueShift = 0;
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateItemSelectionUI();
		}

		private void ResetHueSlider()
		{
			Slider activeHueSlider = GetActiveHueSlider();
			if (activeHueSlider != null)
				activeHueSlider.Value = 0;
		}

		private void SaturationReset_Click(object sender, RoutedEventArgs e)
		{
			//! TODO: Needs to play with undo/redo!

			ResetSaturationSlider();
			stampsLayer.BeginUpdate();
			try
			{
				foreach (IStampProperties stamp in Map.Filter<IStampProperties>())
					stamp.Saturation = 0;
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateItemSelectionUI();
		}

		private void ResetSaturationSlider()
		{
			Slider activeSaturationSlider = GetActiveSaturationSlider();
			if (activeSaturationSlider != null)
				activeSaturationSlider.Value = 0;
		}

		private void LightnessReset_Click(object sender, RoutedEventArgs e)
		{
			//! TODO: Needs to play with undo/redo!

			ResetLightnessSlider();
			stampsLayer.BeginUpdate();
			try
			{
				foreach (IStampProperties stamp in Map.Filter<IStampProperties>())
					stamp.Lightness = 0;
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateItemSelectionUI();
		}

		private void ResetLightnessSlider()
		{
			Slider activeLightnessSlider = GetActiveLightnessSlider();
			if (activeLightnessSlider != null)
				activeLightnessSlider.Value = 0;
		}

		private void ContrastReset_Click(object sender, RoutedEventArgs e)
		{
			//! TODO: Needs to play with undo/redo!

			ResetContrastSlider();
			stampsLayer.BeginUpdate();
			try
			{
				foreach (IStampProperties stamp in Map.Filter<IStampProperties>())
					stamp.Contrast = 0;
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
			UpdateItemSelectionUI();
		}

		private void ResetContrastSlider()
		{
			Slider activeContrastSlider = GetActiveContrastSlider();
			if (activeContrastSlider != null)
				activeContrastSlider.Value = 0;
		}

		void CloseTheRest(ContentControl notThisContentControl)
		{
			foreach (ContentControl contentControl in propEdStatus.Keys)
			{
				if (contentControl != notThisContentControl)
					ShowPropertyEditor(contentControl, OpenClosedStatus.Closed);
			}
		}

		void ShowPropertyEditor(ContentControl contentControl)
		{
			PropertyEditorStatus propertyEditorStatus = propEdStatus[contentControl];
			ShowPropertyEditor(contentControl, propertyEditorStatus.OpenClosed);
		}

		void ShowPropertyEditor(ContentControl contentControl, OpenClosedStatus newStatus)
		{
			Viewbox launchButton = FindResource(propEdStatus[contentControl].LaunchButtonName) as Viewbox;
			switch (newStatus)
			{
				case OpenClosedStatus.Open:
					contentControl.Visibility = Visibility.Visible;
					launchButton.Visibility = Visibility.Hidden;
					CloseTheRest(contentControl);
					break;
				case OpenClosedStatus.Closed:
					contentControl.Visibility = Visibility.Hidden;
					launchButton.Visibility = Visibility.Visible;
					break;
			}
			propEdStatus[contentControl].OpenClosed = newStatus;
		}

		private void btnShowColorControls_MouseDown(object sender, MouseButtonEventArgs e)
		{
			ShowPropertyEditor(GetColorControls(), OpenClosedStatus.Open);
		}

		private ContentControl GetColorControls()
		{
			return FindResource("ccColorControls") as ContentControl;
		}

		private ContentControl GetPropertyGrid()
		{
			return FindResource("ccPropertyGrid") as ContentControl;
		}

		private void DeleteSelected_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ExecuteCommand("DeleteStamps");
		}

		double? clipboardSaturation;
		double? clipboardLightness;
		double? clipboardContrast;
		double? clipboardHue;

		void CopyColorModifications()
		{
			double? selectedButtonScale;
			List<Type> consistentInterfaces;
			SelectionContents selectionContents;
			GetConsistentModSettings(out selectedButtonScale, out clipboardHue, out clipboardSaturation, out clipboardLightness, out clipboardContrast, out consistentInterfaces, out selectionContents);
		}
		private void CopyColorMod_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			CopyColorModifications();
		}

		private void PasteColorMod_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			PasteColorMods();
		}

		private void PasteColorMods()
		{
			stampsLayer.BeginUpdate();
			try
			{
				foreach (IStampProperties stamp in Map.Filter<IStampProperties>())
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
			UpdateItemSelectionUI();
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
				foreach (IStampProperties stamp in Map.Filter<IStampProperties>())
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
			UpdateItemSelectionUI();
		}

		private void CopyColorModMenuItem_Click(object sender, RoutedEventArgs e)
		{
			CopyColorModifications();
		}

		private void PasteColorModMenuItem_Click(object sender, RoutedEventArgs e)
		{
			PasteColorMods();
		}

		private void GroupSelection_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			GroupSelection();
		}

		private void UngroupSelection_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			UngroupSelection();
		}

		private void GroupSelection()
		{
			if (!Map.SelectedItems.Any())
				return;

			ExecuteCommand("StampGrouping", GroupOperation.Group);
		}

		private void UngroupSelection()
		{
			ExecuteCommand("StampGrouping", GroupOperation.Ungroup);
		}

		private void btnGroup_MouseDown(object sender, MouseButtonEventArgs e)
		{
			GroupSelection();
		}

		private void btnUngroup_MouseDown(object sender, MouseButtonEventArgs e)
		{
			UngroupSelection();
		}

		private void btnAlignHorizontalCenter_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Align(StampAlignment.HorizontalCenter, GetAverageHorizontalCenterInSelection());
		}

		private double GetAverageHorizontalCenterInSelection()
		{
			double totalCenterX = 0;
			foreach (IItemProperties stamp in Map.SelectedItems)
				totalCenterX += stamp.X;

			return totalCenterX / Map.SelectedItems.Count;
		}
		private double GetAverageVerticalCenterInSelection()
		{
			double totalCenterY = 0;
			foreach (IItemProperties stamp in Map.SelectedItems)
				totalCenterY += stamp.Y;

			return totalCenterY / Map.SelectedItems.Count;
		}

		private void btnAlignVerticalCenter_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Align(StampAlignment.VerticalCenter, GetAverageVerticalCenterInSelection());
		}

		private void btnAlignLeft_MouseDown(object sender, MouseButtonEventArgs e)
		{
			double furthestLeft = GetFurthestLeftInSelection();

			if (furthestLeft < int.MaxValue)
				Align(StampAlignment.Left, furthestLeft);
		}

		private double GetFurthestLeftInSelection()
		{
			double furthestLeft = int.MaxValue;
			foreach (IItemProperties stamp in Map.SelectedItems)
			{
				double left = stamp.GetLeft();
				if (left < furthestLeft)
					furthestLeft = left;
			}

			return furthestLeft;
		}

		private void btnAlignRight_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Align(StampAlignment.Right, GetFurthestRightInSelection());
		}

		private double GetFurthestRightInSelection()
		{
			double furthestRight = 0;
			foreach (IItemProperties stamp in Map.SelectedItems)
			{
				double right = stamp.GetRight();
				if (right > furthestRight)
					furthestRight = right;
			}

			return furthestRight;
		}

		private void btnAlignTop_MouseDown(object sender, MouseButtonEventArgs e)
		{
			double furthestTop = GetFurthestTopInSelection();

			if (furthestTop < int.MaxValue)
				Align(StampAlignment.Top, furthestTop);
		}

		private void Align(StampAlignment alignment, double value)
		{
			ExecuteCommand("AlignOrDistribute", new AlignmentData(alignment, value));
		}

		private void Distribute(StampAlignment alignment, double alignValue, double spaceBetween)
		{
			ExecuteCommand("AlignOrDistribute", new AlignmentData(alignment, alignValue, spaceBetween));
		}

		private double GetFurthestTopInSelection()
		{
			double furthestTop = int.MaxValue;
			foreach (IItemProperties stamp in Map.SelectedItems)
			{
				double top = stamp.GetTop();
				if (top < furthestTop)
					furthestTop = top;
			}

			return furthestTop;
		}

		private void btnAlignBottom_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Align(StampAlignment.Bottom, GetFurthestBottomInSelection());
		}

		private double GetFurthestBottomInSelection()
		{
			double furthestBottom = 0;
			foreach (IItemProperties stamp in Map.SelectedItems)
			{
				double bottom = stamp.GetBottom();
				if (bottom > furthestBottom)
					furthestBottom = bottom;
			}

			return furthestBottom;
		}

		private void btnDistributeHorizontally_MouseDown(object sender, MouseButtonEventArgs e)
		{
			double furthestLeft, spaceBetweenStamps;
			GetFurthestLeftAndSpaceBetweenSelectedStamps(out furthestLeft, out spaceBetweenStamps);
			Distribute(StampAlignment.DistributeHorizontally, furthestLeft, spaceBetweenStamps);
		}

		private void GetFurthestLeftAndSpaceBetweenSelectedStamps(out double furthestLeft, out double spaceBetweenStamps)
		{
			double furthestRight = 0;
			furthestLeft = double.MaxValue;
			foreach (IItemProperties stamp in Map.SelectedItems)
			{
				double centerX = stamp.X;
				double top = stamp.GetLeft();
				if (centerX > furthestRight)
					furthestRight = centerX;
				if (centerX < furthestLeft)
					furthestLeft = centerX;
			}
			spaceBetweenStamps = (furthestRight - furthestLeft) / (Map.SelectedItems.Count - 1);
		}

		private void btnDistributeVertically_MouseDown(object sender, MouseButtonEventArgs e)
		{
			double furthestTop, spaceBetweenStamps;
			GetFurthestTopAndSpaceBetweenStamps(out furthestTop, out spaceBetweenStamps);
			Distribute(StampAlignment.DistributeVertically, furthestTop, spaceBetweenStamps);
		}

		private void GetFurthestTopAndSpaceBetweenStamps(out double furthestTop, out double spaceBetweenStamps)
		{
			double furthestBottom = 0;
			furthestTop = double.MaxValue;
			foreach (IItemProperties stamp in Map.SelectedItems)
			{
				double centerY = stamp.Y;
				double top = stamp.GetTop();
				if (centerY > furthestBottom)
					furthestBottom = centerY;
				if (centerY < furthestTop)
					furthestTop = centerY;
			}
			spaceBetweenStamps = (furthestBottom - furthestTop) / (Map.SelectedItems.Count - 1);
		}

		public bool CtrlKeyDown
		{
			get { return ctrlKeyDown; }
			set
			{
				if (ctrlKeyDown == value)
					return;

				ctrlKeyDown = value;
				if (draggingStamps)
					SetSelectedStampVisibility(ctrlKeyDown);
			}
		}

		private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			CtrlKeyDown = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
		}

		private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			CtrlKeyDown = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
		}

		BaseTexture GetTexture(string baseTextureName)
		{
			return tileTextures.FirstOrDefault(x => x.BaseName == baseTextureName);
		}

		void Load(string fileName)
		{
			ClearMapCanvasChildren();

			string mapStr = File.ReadAllText(fileName);

			Map = JsonConvert.DeserializeObject<Map>(mapStr);
			HookupMapEvents();
			AssignMapToStampsLayer();

			Map.FileName = fileName;
			stampsLayer.BeginUpdate();
			try
			{
				Map.Reconstitute();
				PrepareUIForNewMap();
				RemoveRubberBandSelector();
				AddTilesFromMap();
				Map.UpdateIfNeeded();
				RebuildWalls();
				LoadFinalCanvasElements();
				AddSelectionCanvas();
			}
			finally
			{
				stampsLayer.EndUpdate();
			}
		}

		private void AddTilesFromMap()
		{
			foreach (Tile tile in Map.Tiles)
			{
				BaseTexture baseTexture = GetTexture(tile.BaseTextureName);
				if (baseTexture != null)
				{
					string imageFileName = tile.ImageFileName;
					AddFloorTile(tile, baseTexture.GetImage(tile.Column, tile.Row, ref imageFileName));
				}

				if (tile.IsFloor)
					AddFloorOverlay(tile);

				AddSelector(tile);
			}
		}

		string GetMapSaveFileName()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.InitialDirectory = MapSaveFolder;
			saveFileDialog.Filter = "Map file (*.json)|*.json";
			saveFileDialog.DefaultExt = "*.json";
			if (saveFileDialog.ShowDialog() == true)
				return saveFileDialog.FileName;
			return null;
		}

		void Save()
		{
			if (string.IsNullOrWhiteSpace(Map.FileName))
			{
				Map.FileName = GetMapSaveFileName();
			}
			Map.Save();
		}

		private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Save();
		}

		private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			string newFileName = GetMapSaveFileName();
			if (newFileName == null)
				return;
			Map.FileName = newFileName;
			Save();
		}
		private void Load_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Map file (*.json)|*.json";
			openFileDialog.DefaultExt = "*.json";
			openFileDialog.InitialDirectory = MapSaveFolder;
			if (openFileDialog.ShowDialog() == true)
				Load(openFileDialog.FileName);
		}

		private void Redo_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (redoStack.Count == 0)
				return;
			BaseCommand command = redoStack.Pop();
			undoStack.Push(command);
			Redo(command);
		}

		private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (undoStack.Count == 0)
				return;
			BaseCommand command = undoStack.Pop();
			redoStack.Push(command);
			Undo(command);
		}

		private void Undo(BaseCommand command)
		{
			ExecuteCommand(command, CommandExecutionType.Undo);
		}

		private void Redo(BaseCommand command)
		{
			ExecuteCommand(command, CommandExecutionType.Redo);
		}

		string interactiveChangeID;
		void StartInteractiveChange(string changeName)
		{
			if (interactiveChangeID == changeName)
				return;
			interactiveChangeID = changeName;
		}

		void StopInteractiveChange()
		{
			interactiveChangeID = null;
			lastCommandChangeID = null;
		}

		private void ContrastSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			StopInteractiveChange();
		}

		private void HueSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			StopInteractiveChange();
		}

		private void SaturationSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			StopInteractiveChange();
		}

		private void LightnessSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			StopInteractiveChange();
		}

		private void BtnCloseColorProps_Click(object sender, RoutedEventArgs e)
		{
			ShowPropertyEditor(GetColorControls(), OpenClosedStatus.Closed);
		}

		private void BtnClosePropertyGrid_Click(object sender, RoutedEventArgs e)
		{
			ShowPropertyEditor(GetPropertyGrid(), OpenClosedStatus.Closed);
		}

		private void BtnOpenPropertyGrid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			ShowPropertyEditor(GetPropertyGrid(), OpenClosedStatus.Open);
		}

		private void ZoomAndPanControl_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			double multiplier = 1;
			bool ctrlKeyDown = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
			bool shiftKeyDown = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
			bool altKeyDown = Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);
			if (shiftKeyDown)
				if (ctrlKeyDown)
					multiplier = Tile.Width / 2;
				else
					multiplier = 5;
			else if (ctrlKeyDown)
				multiplier = Tile.Width / 4;
			else if (altKeyDown)
				multiplier = Tile.Width;
			;
			if (e.Key == Key.Left || e.Key == Key.System && e.SystemKey == Key.Left)
			{
				MoveSelectedStamps(-1 * multiplier, 0);
				e.Handled = true;
			}
			else if (e.Key == Key.Right || e.Key == Key.System && e.SystemKey == Key.Right)
			{
				MoveSelectedStamps(1 * multiplier, 0);
				e.Handled = true;
			}
			else if (e.Key == Key.Up || e.Key == Key.System && e.SystemKey == Key.Up)
			{
				MoveSelectedStamps(0, -1 * multiplier);
				e.Handled = true;
			}
			else if (e.Key == Key.Down || e.Key == Key.System && e.SystemKey == Key.Down)
			{
				MoveSelectedStamps(0, 1 * multiplier);
				e.Handled = true;
			}
		}

		private void BtnAddCharacter_Click(object sender, RoutedEventArgs e)
		{
			if (activeStampCanvas != null)
			{
				activeStampCanvas.Children.Clear();
			}

			if (!(sender is Button button))
				return;
			if (!(button.Tag is MapCharacter character))
				return;
			FloatingStamp(ref activeStampCanvas, character);
			character.CreateFloating(activeStampCanvas);
			mouseDragAdjustX = -character.GetLeft();
			mouseDragAdjustY = -character.GetTop();
			MapEditMode = MapEditModes.Stamp;
		}
	}
}

