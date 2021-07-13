using Bounce.BlobAssets;
using Bounce.Mathematics;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace TaleSpireCore.Decompiles
{
	/// <summary>
	/// Essentially a decompile of AssetDb.
	/// I've appended the number "2" when needed to avoid naming collisions.
	/// </summary>
	public class AssetDb2 : AssetDb
	{
		// Fields
		private static NativeList<BlobAssetReference<AssetPackIndex>> _assetPacksIndicies;
		private static NativeHashMap<NGuid, BlobView<PlaceableData>> _placeables;
		private static NativeList<BlobAssetReference<Unity.Physics.Collider>> _placeableColliders;
		private static readonly List<DbGroup> _tileGroups = new List<DbGroup>(0x80);
		private static readonly Dictionary<string, List<DbEntry>> _tilesByTag = new Dictionary<string, List<DbEntry>>(0x80);
		private static readonly HashList<string> _tileTags = new HashList<string>();
		private static readonly List<DbGroup> _propGroups = new List<DbGroup>(0x80);
		private static readonly Dictionary<string, List<DbEntry>> _propsByTag = new Dictionary<string, List<DbEntry>>(0x80);
		private static readonly HashList<string> _propTags = new HashList<string>();
		private static NativeHashMap<NGuid, BlobView<CreatureData>> _creatures;
		private static readonly List<DbGroup> _creatureGroups = new List<DbGroup>(0x80);
		private static readonly Dictionary<string, List<DbEntry>> _creaturesByTag = new Dictionary<string, List<DbEntry>>(0x80);
		private static readonly HashList<string> _creatureTags = new HashList<string>();
		private static NativeHashMap<NGuid, BlobView<MusicData>> _music;
		private static readonly Dictionary<NGuid, DbEntry> _indexData = new Dictionary<NGuid, DbEntry>(0x400);
		private static readonly List<Texture2D> _atlases = new List<Texture2D>(10);
		private static readonly Dictionary<NGuid, Sprite> _icons = new Dictionary<NGuid, Sprite>(0x200);
		private static readonly Queue<AtlasAsyncLoad> _loadingAtlases = new Queue<AtlasAsyncLoad>();
		private static readonly object _loadingAtlasQueueLock = new object();
		private static int _loadingAtlasCount;
		private static NativeList<BlobAssetReference<PlaceableData>> _dummyPlaceables;
		private static CollisionFilter _explorerTiles;
		private static CollisionFilter _explorerProps;
		private static string _cachedAssetDataPath = null;
		//public IconProvider AssetIconProvider;

		// Methods
		public static BlobAssetReference<PlaceableData> AddIdAsDummyPlaceable2(NGuid id, PlaceableKind kind)
		{
			using (BlobBuilder builder = new BlobBuilder(Allocator.TempJob, 0x1_0000))
			{
				BlobBuilder builder2 = builder;
				PlaceableData.ConstructDummyPlaceable(builder2, ref builder2.ConstructRoot<PlaceableData>(), id);
				BlobAssetReference<PlaceableData> bref = builder.CreateBlobAssetReference<PlaceableData>(Allocator.Persistent);
				_dummyPlaceables.Add(bref);
				_placeables.Add(id, bref.TakeView<PlaceableData>());
				return bref;
			}
		}

		private static void DisposeImpl()
		{
			if (_assetPacksIndicies.IsCreated)
			{
				_assetPacksIndicies.Dispose();
				_assetPacksIndicies = new NativeList<BlobAssetReference<AssetPackIndex>>();
			}
			if (_placeables.IsCreated)
			{
				_placeables.Dispose();
				_placeables = new NativeHashMap<NGuid, BlobView<PlaceableData>>();
			}
			if (_music.IsCreated)
			{
				_music.Dispose();
				_music = new NativeHashMap<NGuid, BlobView<MusicData>>();
			}
			if (_creatures.IsCreated)
			{
				_creatures.Dispose();
				_creatures = new NativeHashMap<NGuid, BlobView<CreatureData>>();
			}
			if (_placeableColliders.IsCreated)
			{
				_placeableColliders.Dispose();
				_placeableColliders = new NativeList<BlobAssetReference<Unity.Physics.Collider>>();
			}
			if (_dummyPlaceables.IsCreated)
			{
				_dummyPlaceables.Dispose();
				_dummyPlaceables = new NativeList<BlobAssetReference<PlaceableData>>();
			}
		}

		public static (DbEntry.EntryKind, List<DbGroup>)[] GetAllGroups2() =>
				new (DbEntry.EntryKind, List<DbGroup>)[] { (DbEntry.EntryKind.Tile, _tileGroups), (DbEntry.EntryKind.Prop, _propGroups), (DbEntry.EntryKind.Creature, _creatureGroups) };

		public static BlobView<CreatureData> GetCreatureData2(NGuid id) =>
				_creatures[id];

		private static DbGroup GetGroup(List<DbGroup> groups, string groupName)
		{
			DbGroup group;
			if (!TryGetGroup(groups, groupName, out group))
			{
				group = new DbGroup(groupName);
				groups.Add(group);
			}
			return group;
		}

		public static List<DbGroup> GetGroupsByKind2(DbEntry.EntryKind kind)
		{
			List<DbGroup> list;
			switch (kind)
			{
				case DbEntry.EntryKind.Tile:
					list = _tileGroups;
					break;

				case DbEntry.EntryKind.Creature:
					list = _creatureGroups;
					break;

				case DbEntry.EntryKind.Prop:
					list = _propGroups;
					break;

				default:
					list = new List<DbGroup>();
					break;
			}
			return list;
		}

		public static DbEntry GetIndexData2(NGuid id) => _indexData[id];

		public static BlobView<PlaceableData> GetPlaceableData2(NGuid id) =>
				_placeables[id];

		public static List<string> GetTagsByKinds2(IReadOnlyList<DbEntry.EntryKind> kinds)
		{
			List<string> list = new List<string>(0x80);
			int count = kinds.Count;
			for (int i = 0; i < count; i++)
			{
				IReadOnlyList<string> list2;
				if (TryGetTagsByKind(kinds[i], out list2))
				{
					list.AddRange(list2);
				}
			}
			return list;
		}

		public static string GetTaleWeaverDataDir2() => Path.Combine(AssetDataPath, "Taleweaver");

		private static void LoadAssetPack(string packDir)
		{
			string path = Path.Combine(packDir, "index");
			if (!File.Exists(path))
			{
				Debug.LogError("index missing: " + path);
			}
			else
			{
				NativeList<PlaceableLocalAssetData> list;
				NativeHashMap<NGuid, PlaceableLoadState> map;
				NativeList<PlaceableAssetCollider> list2;
				BlobAssetReference<AssetPackIndex> reference = LoadAssetPackData(path);
				_assetPacksIndicies.Add(reference);
				int count = _atlases.Count;
				ref BlobArray<Atlas> atlases = ref reference.Value.Atlases;
				for (int i = 0; i < atlases.Length; i++)
				{
					int2 size = atlases[i].Size;
					Texture2D item = new Texture2D(size.x, size.y);
					_atlases.Add(item);
					new AtlasAsyncLoad(Path.Combine(packDir, atlases[i].LocalPath.ToString()), item).Dispatch();
				}
				ref BlobArray<MusicData> music = ref reference.Value.Music;
				for (int j = 0; j < music.Length; j++)
				{
					BlobView<MusicData> view3 = music.TakeView<MusicData>(j);
					_music[view3.Value.Id] = view3;
				}
				BlobArrayView<PlaceableData> placeablesView = reference.Value.Placeables.TakeView<PlaceableData>();
				BlobArrayView<CreatureData> creaturesView = reference.Value.Creatures.TakeView<CreatureData>();
				Unity.Physics.Material material = BouncePhysics.ConvertMaterial(BouncePhysics.DefaultMaterial, null, false);
				PlaceableManager.GetInternalPlaceableData(out list, out map, out list2);
				JobHandle dependsOn = new JobHandle();
				JobHandle handle = ProcessPlaceableData.Create(placeablesView, _placeables, list, map, _placeableColliders).Schedule<ProcessPlaceableData>(dependsOn);
				dependsOn = new JobHandle();
				JobHandle handle3 = JobHandle.CombineDependencies(CreatePlaceableColliders.Create(placeablesView, _placeableColliders, material, _explorerTiles, _explorerProps).Schedule<CreatePlaceableColliders>(placeablesView.Length, 0x40, handle), ProcessCreatureData.Create(creaturesView, _creatures).Schedule<ProcessCreatureData>(dependsOn));
				PopulateIcons(placeablesView, creaturesView, count);
				PopulatePlaceableIndexes(placeablesView);
				PopulateCreatureIndex(creaturesView);
				handle3.Complete();
			}
		}

		private static BlobAssetReference<AssetPackIndex> LoadAssetPackData(string packIndexPath) => new StreamBinaryReader(packIndexPath, 0x1_0000L).Read<AssetPackIndex>();

		protected override void OnInstanceDestroy()
		{
			DisposeImpl();
		}

		protected override void OnInstanceSetup()
		{
			foreach (string str in Directory.GetDirectories(GetTaleWeaverDataDir()))
			{
				NGuid guid;
				if (NGuid.TryParse(Path.GetFileName(str), out guid))
				{
					LoadAssetPack(str);
				}
			}
		}

		protected override void OnSetupInternals()
		{
			_assetPacksIndicies = new NativeList<BlobAssetReference<AssetPackIndex>>(Allocator.Persistent);
			_placeables = new NativeHashMap<NGuid, BlobView<PlaceableData>>(0x200, Allocator.Persistent);
			_music = new NativeHashMap<NGuid, BlobView<MusicData>>(0x80, Allocator.Persistent);
			_placeableColliders = new NativeList<BlobAssetReference<Unity.Physics.Collider>>(0x400, Allocator.Persistent);
			_creatures = new NativeHashMap<NGuid, BlobView<CreatureData>>(0x400, Allocator.Persistent);
			_explorerTiles = BouncePhysicsLayers.GetCollisionFilterByLayerName("Explorer");
			_explorerProps = BouncePhysicsLayers.GetCollisionFilterByLayerName("Explorer Props");
			_dummyPlaceables = new NativeList<BlobAssetReference<PlaceableData>>(0, Allocator.Persistent);
		}

		private static void PopulateCreatureIndex(BlobArrayView<CreatureData> creaturesView)
		{
			int length = creaturesView.Length;
			for (int i = 0; i < length; i++)
			{
				Sprite sprite;
				ref CreatureData dataRef = ref creaturesView[i];
				string groupName = dataRef.Group.ToString();
				DbGroup group = GetGroup(_creatureGroups, groupName);
				_icons.TryGetValue(dataRef.Id, out sprite);
				DbEntry item = new DbEntry(dataRef.Id, DbEntry.EntryKind.Creature, dataRef.Name.ToString(), dataRef.Description.ToString(), group, dataRef.GroupTag.Value.Name.ToString(), dataRef.GroupTag.Value.Order, ref dataRef.Tags, dataRef.IsDeprecated, sprite);
				group.Entries.Add(item);
				_indexData[dataRef.Id] = item;
				PopulateTagEntry(_creaturesByTag, _creatureTags, item);
			}
		}

		private static void PopulateIcons(BlobArrayView<PlaceableData> placeablesView, BlobArrayView<CreatureData> creaturesView, int atlasStartIndex)
		{
			int length = placeablesView.Length;
			for (int i = 0; i < length; i++)
			{
				ref PlaceableData dataRef = ref placeablesView[i];
				int num3 = atlasStartIndex + dataRef.IconAtlasIndex;
				Texture2D texture = _atlases[num3];
				Rect rect = dataRef.IconAtlasRegion.Scale((float)texture.width, (float)texture.height);
				_icons.Add(dataRef.Id, Sprite.Create(texture, rect, Vector2.zero));
			}
			length = creaturesView.Length;
			for (int j = 0; j < length; j++)
			{
				ref CreatureData dataRef2 = ref creaturesView[j];
				int num5 = atlasStartIndex + dataRef2.IconAtlasIndex;
				Texture2D texture = _atlases[num5];
				Rect rect = dataRef2.IconAtlasRegion.Scale((float)texture.width, (float)texture.height);
				_icons.Add(dataRef2.Id, Sprite.Create(texture, rect, Vector2.zero));
			}
		}

		private static void PopulatePlaceableIndexes(BlobArrayView<PlaceableData> placeablesView)
		{
			int length = placeablesView.Length;
			for (int i = 0; i < length; i++)
			{
				Sprite sprite;
				(DbEntry.EntryKind, List<DbGroup>, Dictionary<string, List<DbEntry>>, HashList<string>) tuple;
				ref PlaceableData dataRef = ref placeablesView[i];
				string groupName = dataRef.Group.ToString();
				PlaceableKind kind2 = dataRef.Kind;
				if (kind2 == PlaceableKind.Tile)
				{
					tuple = (DbEntry.EntryKind.Tile, _tileGroups, _tilesByTag, _tileTags);
				}
				else
				{
					if (kind2 != PlaceableKind.Prop)
					{
						throw new Exception($"Unknown PlaceableKind '{dataRef.Kind}'");
					}
					tuple = (DbEntry.EntryKind.Prop, _propGroups, _propsByTag, _propTags);
				}
				DbEntry.EntryKind kind = tuple.Item1;
				Dictionary<string, List<DbEntry>> dst = tuple.Item3;
				DbGroup group = GetGroup(tuple.Item2, groupName);
				_icons.TryGetValue(dataRef.Id, out sprite);
				DbEntry item = new DbEntry(dataRef.Id, kind, dataRef.Name.ToString(), dataRef.Description.ToString(), group, dataRef.GroupTag.Value.Name.ToString(), dataRef.GroupTag.Value.Order, ref dataRef.Tags, dataRef.IsDeprecated, sprite);
				group.Entries.Add(item);
				_indexData[dataRef.Id] = item;
				PopulateTagEntry(dst, tuple.Item4, item);
			}
		}

		private static void PopulateTagEntry(Dictionary<string, List<DbEntry>> dst, HashList<string> tagSet, DbEntry entry)
		{
			string[] tags = entry.Tags;
			int length = tags.Length;
			for (int i = 0; i < length; i++)
			{
				List<DbEntry> list;
				string key = tags[i];
				if (!dst.TryGetValue(key, out list))
				{
					dst[key] = list = new List<DbEntry>();
					tagSet.Add(key);
				}
				list.Add(entry);
			}
		}

		public static bool TryGetCreatureData2(NGuid id, out BlobView<CreatureData> data) => _creatures.TryGetValue(id, out data);

		private static bool TryGetGroup(List<DbGroup> groups, string groupName, out DbGroup group)
		{
			int count = groups.Count;
			for (int i = 0; i < count; i++)
			{
				if (groups[i].Name == groupName)
				{
					group = groups[i];
					return true;
				}
			}
			group = null;
			return false;
		}

		public static bool TryGetIndexData2(NGuid id, out DbEntry data) => _indexData.TryGetValue(id, out data);

		public static bool TryGetPlaceableData2(NGuid id, out BlobView<PlaceableData> data) => _placeables.TryGetValue(id, out data);

		public static bool TryGetTagsByKind2(DbEntry.EntryKind kind, out IReadOnlyList<string> tags)
		{
			switch (kind)
			{
				case DbEntry.EntryKind.Tile:
					tags = _tileTags.List;
					return true;

				case DbEntry.EntryKind.Creature:
					tags = _creatureTags.List;
					return true;

				case DbEntry.EntryKind.Prop:
					tags = _propTags.List;
					return true;
			}
			tags = null;
			return false;
		}

		private void Update()
		{
			if (_loadingAtlasCount > 0)
			{
				object obj2 = _loadingAtlasQueueLock;
				lock (obj2)
				{
					while (_loadingAtlases.Count > 0)
					{
						_loadingAtlasCount--;
						_loadingAtlases.Dequeue().UploadToTextureAndComplete();
					}
				}
			}
		}

		// Properties
		public static IReadOnlyList<Texture2D> Atlases2 => _atlases;

		public static Dictionary<NGuid, Sprite> Icons2 => _icons;

		public static NativeHashMap<NGuid, BlobView<PlaceableData>> Placeables2 => _placeables;

		public static NativeHashMap<NGuid, BlobView<MusicData>> Music2 => _music;

		public static NativeList<BlobAssetReference<Unity.Physics.Collider>> PlaceableColliders2 => _placeableColliders;

		private static string AssetDataPath
		{
			get
			{
				if (string.IsNullOrEmpty(_cachedAssetDataPath))
				{
					_cachedAssetDataPath = Directory.GetParent(Application.dataPath).FullName;
				}
				return _cachedAssetDataPath;
			}
		}

		// Nested Types
		private class AtlasAsyncLoad
		{
			// Fields
			private Thread _thread;
			private string _path;
			private Texture2D _tex;
			private byte[] _data;

			// Methods
			public AtlasAsyncLoad(string path, Texture2D tex)
			{
				_path = path;
				_tex = tex;
				_thread = new Thread(new ThreadStart(Execute));
			}

			public void Dispatch()
			{
				AssetDb2._loadingAtlasCount++;
				_thread.Start();
			}

			private void Execute()
			{
				_data = File.ReadAllBytes(_path);
				object obj2 = AssetDb2._loadingAtlasQueueLock;
				lock (obj2)
				{
					AssetDb2._loadingAtlases.Enqueue(this);
				}
			}

			public void UploadToTextureAndComplete()
			{
				_tex.LoadImage(_data);
				_tex.Compress(true);
				_tex.filterMode = FilterMode.Bilinear;
				_tex.Apply(false, true);
			}
		}

		[StructLayout(LayoutKind.Sequential), BurstCompile(CompileSynchronously = true)]
		private struct CreatePlaceableColliders : IJobParallelFor
		{
			private const float BEVEL_RADIUS = 0f;
			private CollisionFilter _explorerTiles;
			private CollisionFilter _explorerProps;
			private Unity.Physics.Material _material;
			private BlobArrayView<PlaceableData> _unsafePlaceablesView;
			[NativeDisableContainerSafetyRestriction]
			private NativeList<BlobAssetReference<Unity.Physics.Collider>> _placeableColliders;
			public static AssetDb2.CreatePlaceableColliders Create(BlobArrayView<PlaceableData> placeablesView, 
				NativeList<BlobAssetReference<Unity.Physics.Collider>> placeableColliders,
				Unity.Physics.Material material, CollisionFilter explorerTiles, CollisionFilter explorerProps) =>
					new AssetDb2.CreatePlaceableColliders
					{
						_unsafePlaceablesView = placeablesView,
						_placeableColliders = placeableColliders,
						_explorerTiles = explorerTiles,
						_explorerProps = explorerProps,
						_material = material
					};

			public void Execute(int i)
			{
				ref PlaceableData dataRef = ref _unsafePlaceablesView[i];
				PlaceableCollidersIndex colliderIndex = dataRef.ColliderIndex;
				CollisionFilter filter = (dataRef.Kind == PlaceableKind.Prop) ? _explorerProps : _explorerTiles;
				for (int j = 0; j < colliderIndex.Count; j++)
				{
					Bounds bounds = dataRef.Colliders[j];
					BoxGeometry geometry = new BoxGeometry
					{
						Center = bounds.center,
						Orientation = quaternion.identity,
						Size = bounds.size,
						BevelRadius = 0f
					};
					_placeableColliders[colliderIndex.BaseIndex + j] = Unity.Physics.BoxCollider.Create(geometry, filter, _material);
				}
			}
		}

		public class DbEntry2
		{
			// Fields
			public static readonly string[] EntryKindNames = Enum.GetNames(typeof(EntryKind));
			public readonly NGuid Id;
			public readonly EntryKind Kind;
			public readonly string Name;
			public readonly string Description;
			public readonly string[] Tags;
			public readonly AssetDb.DbGroup Group;
			public readonly string GroupTagName;
			public readonly int GroupTagOrder;
			public readonly bool IsDeprecated;
			public readonly Sprite Icon;

			// Methods
			public DbEntry2(NGuid id, EntryKind kind, string name, string description, AssetDb.DbGroup group, string groupTagName, int groupTagOrder, ref BlobArray<BlobString> tags, bool isDeprecated, Sprite icon)
			{
				Id = id;
				Kind = kind;
				Name = name;
				Description = description;
				Group = group;
				GroupTagName = groupTagName;
				GroupTagOrder = groupTagOrder;
				IsDeprecated = isDeprecated;
				Icon = icon;
				Tags = new string[tags.Length];
				for (int i = 0; i < tags.Length; i++)
				{
					Tags[i] = tags[i].ToString();
				}
			}

			// Nested Types
			public enum EntryKind
			{
				Tile,
				Creature,
				Prop
			}
		}

		public class DbGroup2
		{
			// Fields
			public readonly string Name;
			public readonly List<AssetDb.DbEntry> Entries = new List<AssetDb.DbEntry>(8);

			// Methods
			public DbGroup2(string name)
			{
				Name = name;
			}
		}

		public abstract class IconProvider2 : MonoBehaviour
		{
			// Methods
			protected IconProvider2()
			{
			}

			public abstract Texture2D GetIcon(string name);
		}

		[StructLayout(LayoutKind.Sequential), BurstCompile(CompileSynchronously = true)]
		private struct ProcessCreatureData : IJob
		{
			private BlobArrayView<CreatureData> _unsafeCreaturesView;
			private NativeHashMap<NGuid, BlobView<CreatureData>> _creatures;
			public static AssetDb2.ProcessCreatureData Create(BlobArrayView<CreatureData> creaturesView, NativeHashMap<NGuid, BlobView<CreatureData>> creatures) =>
					new AssetDb2.ProcessCreatureData
					{
						_unsafeCreaturesView = creaturesView,
						_creatures = creatures
					};

			public void Execute()
			{
				int length = _unsafeCreaturesView.Length;
				for (int i = 0; i < length; i++)
				{
					NGuid id = _unsafeCreaturesView[i].Id;
					_creatures.Add(id, _unsafeCreaturesView.TakeView(i));
				}
			}
		}

		[StructLayout(LayoutKind.Sequential), BurstCompile(CompileSynchronously = true)]
		private struct ProcessPlaceableData : IJob
		{
			private BlobArrayView<PlaceableData> _unsafePlaceablesView;
			private NativeHashMap<NGuid, BlobView<PlaceableData>> _placeables;
			private NativeList<PlaceableLocalAssetData> _placeableAssetInfo;
			private NativeHashMap<NGuid, PlaceableLoadState> _placeableInfoMap;
			private NativeList<BlobAssetReference<Unity.Physics.Collider>> _placeableColliders;
			public static AssetDb2.ProcessPlaceableData Create(BlobArrayView<PlaceableData> placeablesView, NativeHashMap<NGuid, BlobView<PlaceableData>> placeables, NativeList<PlaceableLocalAssetData> placeableAssetInfo, NativeHashMap<NGuid, PlaceableLoadState> placeableInfoMap, NativeList<BlobAssetReference<Unity.Physics.Collider>> placeableColliders) =>
					new AssetDb2.ProcessPlaceableData
					{
						_unsafePlaceablesView = placeablesView,
						_placeables = placeables,
						_placeableAssetInfo = placeableAssetInfo,
						_placeableInfoMap = placeableInfoMap,
						_placeableColliders = placeableColliders
					};

			public void Execute()
			{
				int baseIndex = 0;
				int length = _unsafePlaceablesView.Length;
				for (int i = 0; i < length; i++)
				{
					ref PlaceableData placeableData = ref _unsafePlaceablesView[i];
					placeableData.ColliderBoundsBound = EnsureBoundsContainsOrigin(placeableData.ColliderBoundsBound);
					NGuid id = placeableData.Id;
					_placeables.Add(id, _unsafePlaceablesView.TakeView(i));
					PlaceableManager.ReservePlaceable(_placeableAssetInfo, _placeableInfoMap, ref placeableData);
					int num4 = placeableData.Colliders.Length;
					if (num4 > 0xff)
					{
						num4 = 0;
					}
					placeableData.ColliderIndex = new PlaceableCollidersIndex(baseIndex, (byte)num4);
					baseIndex += num4;
				}
				if (baseIndex > _placeableColliders.Length)
				{
					_placeableColliders.Resize(baseIndex, NativeArrayOptions.ClearMemory);
				}
			}

			private static Bounds EnsureBoundsContainsOrigin(Bounds bounds)
			{
				bounds.Encapsulate(Vector3.zero);
				return bounds;
			}
		}
	}
}
