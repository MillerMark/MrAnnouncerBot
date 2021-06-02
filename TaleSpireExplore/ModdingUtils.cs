using TMPro;
using Unity.Mathematics;
using System;
using System.Net;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using Bounce.Unmanaged;
using BepInEx;
using BepInEx.Logging;
using DataModel;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using System.Windows.Forms;
using TaleSpireCore;

namespace ModdingTales
{
	public static class ModdingUtils
	{
		public static Copied beingCopied;
		private static Queue<BoardInfo> boardsToLoad = new Queue<BoardInfo>();
		//private static bool movingCreature = false;
		//private static Queue<MoveAction> moveQueue = new Queue<MoveAction>();
		//public delegate string Command(params string[] args);
		public static Dictionary<string, Func<string[], string>> Commands = new Dictionary<string, Func<string[], string>>();
		public static List<MoveAction> currentActions = new List<MoveAction>();
		public static string[] customStatNames = new string[4] { string.Empty, string.Empty, string.Empty, string.Empty };
		private static ManualLogSource parentLogger;
		private static BaseUnityPlugin parentPlugin;
		public static Queue<SayTextData> sayTextQueue = new Queue<SayTextData>();
		private static bool serverStarted = false;
		public static Queue<SlabData> slabQueue = new Queue<SlabData>();
		public static float3 slabSize;
		public static bool slabSizeResponse;
		public static string slabSizeSlab = string.Empty;
		public static CreaturePreviewBoardAsset spawnCreature = null;
		public static float3 spawnCreaturePos;

		static ModdingUtils()
		{
			Commands.Add("SelectNextPlayerControlled", SelectNextPlayerControlled);
			Commands.Add("SelectPlayerControlledByAlias", SelectPlayerControlledByAlias);
			Commands.Add("GetPlayerControlledList", GetPlayerControlledList);
			Commands.Add("GetCreatureList", GetCreatureList);
			Commands.Add("SetCreatureHp", SetCreatureHp);
			Commands.Add("SetCreatureStat", SetCreatureStat);
			Commands.Add("GetCreatureStats", GetCreatureStats);
			Commands.Add("PlayEmote", PlayEmote);
			Commands.Add("Knockdown", Knockdown);
			Commands.Add("SelectCreatureByCreatureId", SelectCreatureByCreatureId);
			Commands.Add("MoveCreature", MoveCreature);
			Commands.Add("GetCameraLocation", GetCameraLocation);
			Commands.Add("MoveCamera", MoveCamera);
			Commands.Add("SetCameraHeight", SetCameraHeight);
			Commands.Add("RotateCamera", RotateCamera);
			Commands.Add("ZoomCamera", ZoomCamera);
			Commands.Add("TiltCamera", TiltCamera);
			Commands.Add("SayText", SayText);
			Commands.Add("SetTime", SetTime);
			Commands.Add("SetCustomStatName", SetCustomStatName);
			Commands.Add("CreateSlab", CreateSlab);
			Commands.Add("GetSlabSize", GetSlabSize);
			Commands.Add("GetCreatureAssets", GetCreatureAssets);
			Commands.Add("AddCreature", AddCreature);
			Commands.Add("KillCreature", KillCreature);
			Commands.Add("GetBoards", GetBoards);
			Commands.Add("GetCurrentBoard", GetCurrentBoard);
			Commands.Add("LoadBoard", LoadBoard);
			Commands.Add("GetCreatures", GetCreatures);
			Commands.Add("Target", Target);
		}

		private static string AddCreature(string[] input)
		{
			return AddCreature(
				input[0],
				input[1],
				input[2],
				input[3],
				input[4],
				input[5],
				input[6],
				input[7],
				input[8],
				input[9],
				input[10],
				input[11],
				input[12],
				input[13],
				input[14],
				input[15],
				input[16],
				input[17]);
		}

		private static CustomCreatureData convertCreatureData(CreatureData cd)
		{
			// This is because NGuid does not serialize nicely
			CustomCreatureData ccd = new CustomCreatureData();
			ccd.Alias = cd.Alias;
			ccd.BoardAssetId = cd.BoardAssetId.ToString();
			ccd.CreatureId = cd.CreatureId.ToString();
			ccd.UniqueId = cd.UniqueId.ToString();
			//ccd.Position = cd.Position;
			ccd.Position = new VectorDto(cd.Position.x, cd.Position.y, cd.Position.z);
			ccd.Rotation = new Euler(
				cd.Rotation.x.ToDegrees(),
				cd.Rotation.y.ToDegrees(),
				cd.Rotation.z.ToDegrees(),
				0/* cd.Rotation.w */);
			ccd.Alias = cd.Alias;
			ccd.AvatarThumbnailUrl = string.Empty; // cd.AvatarThumbnailUrl
			ccd.Colors = new Color[] { new Color(cd.Color.R, cd.Color.G, cd.Color.B) };
			ccd.Hp = cd.Hp;
			ccd.Inventory = string.Empty; // cd.Inventory;
			ccd.Stat0 = cd.Stat0;
			ccd.Stat1 = cd.Stat1;
			ccd.Stat2 = cd.Stat2;
			ccd.Stat3 = cd.Stat3;
			ccd.TorchState = cd.TorchState;
			ccd.ExplicitlyHidden = cd.ExplicitlyHidden;
			return ccd;
		}

		private static string CreateSlab(string[] input)
		{
			return CreateSlab(input[0], input[1], input[2], input[3]);
		}

		static string ExecuteCommand(string command)
		{
			try
			{
				if (command == "x")
				{
					return "Got the x!\n";
				}
				//UnityEngine.Debug.Log("Command: \"" + command + "\"");
				var parts = command.Split(' ');
				UnityEngine.Debug.Log(parts[0].Trim());
				//UnityEngine.Debug.Log(string.Join(" ", parts.Skip(1)).Trim().Split(','));
				return Commands[parts[0].Trim()].Invoke(string.Join(" ", parts.Skip(1)).Trim().Split(','));
			}
			catch (Exception ex)
			{
				return new ApiResponse(ex.Message + ex.StackTrace, "Unknown command").ToString();
			}
		}

		private static string GetBoards(string[] input)
		{
			return GetBoards();
		}

		private static string GetCameraLocation(string[] input)
		{
			return GetCameraLocation();
		}

		private static string GetCreatureAssets(string[] input)
		{
			return GetCreatureAssets();
		}

		private static string GetCreatureList(string[] input)
		{
			return GetCreatureList();
		}

		private static string GetCreatureStats(string[] input)
		{
			return GetCreatureStats(input[0]);
		}

		private static string GetCurrentBoard(string[] input)
		{
			return GetCurrentBoard();
		}

		private static Vector3 GetMoveVector(CreatureKeyMoveBoardTool.Dir dir)
		{
			Vector3 newPosition = Vector3.zero;
			switch (dir)
			{
				case CreatureKeyMoveBoardTool.Dir.FORWARD:
					newPosition = CameraController.Forward;
					break;
				case CreatureKeyMoveBoardTool.Dir.BACKWARDS:
					newPosition = -CameraController.Forward;
					break;
				case CreatureKeyMoveBoardTool.Dir.LEFT:
					newPosition = -CameraController.Right;
					break;
				case CreatureKeyMoveBoardTool.Dir.RIGHT:
					newPosition = CameraController.Right;
					break;
			}
			float num = -1f;
			Vector3[] array = new Vector3[] { Vector3.forward, -Vector3.forward, Vector3.right, -Vector3.right };
			Vector3 b = Vector3.forward;
			for (int i = 0; i < array.Length; i++)
			{
				float num2 = Vector3.Dot(newPosition, array[i]);
				if (num2 > num)
				{
					num = num2;
					b = array[i];
				}
			}
			newPosition = b;
			return newPosition;
		}

		private static string GetPlayerControlledList(string[] input)
		{
			return GetPlayerControlledList();
		}

		private static string GetSlabSize(string[] input)
		{
			return GetSlabSize(input[0]).Result;
		}

		private static string KillCreature(string[] input)
		{
			return KillCreature(input[0]);
		}

		private static string Knockdown(string[] input)
		{
			return Knockdown(input[0]);
		}

		private static string Knockdown(string creatureId)
		{
			//CreatureBoardAsset creatureBoardAsset;
			//if (PhotonSimpleSingletonBehaviour<CreatureManager>.Instance
			//	.TryGetAsset(new NGuid(creatureId), out creatureBoardAsset))
			//{
			//	Creature creature = creatureBoardAsset.Creature;
			//	creature.Knockdown();
			//	return new APIResponse("Emote successful").ToString();
			//	;
			//} else
			//{
			//	return new APIResponse("Failed to emote").ToString();
			//}
			return "";
		}

		private static string LoadBoard(string[] input)
		{
			return LoadBoard(input[0]);
		}

		private static string MoveCamera(string[] input)
		{
			return MoveCamera(input[0], input[1], input[2], input[3]);
		}

		private static string MoveCreature(string[] input)
		{
			return MoveCreature(input[0], input[1], input[2], input[3]);
		}

		private static string MoveCreature(string creatureId, string direction, string steps, string carryCreature)
		{
			bool useHandle = false;
			if (carryCreature != string.Empty)
			{
				useHandle = bool.Parse(carryCreature);
			}
			CreatureKeyMoveBoardTool.Dir dir = (CreatureKeyMoveBoardTool.Dir)Enum.Parse(
				typeof(CreatureKeyMoveBoardTool.Dir),
				direction,
				true);
			StartMove(new MoveAction { guid = creatureId, dir = dir, steps = float.Parse(steps), useHandle = useHandle });

			return new ApiResponse("Move successful").ToString();
		}

		private static string PlayEmote(string[] input)
		{
			return PlayEmote(input[0], input[1]);
		}

		private static string PlayEmote(string creatureId, string emote)
		{
			//CreatureBoardAsset creatureBoardAsset;
			//if (PhotonSimpleSingletonBehaviour<CreatureManager>.Instance
			//	.TryGetAsset(new NGuid(creatureId), out creatureBoardAsset))
			//{
			//	Creature creature = creatureBoardAsset.Creature;
			//	creature.PlayEmote(emote);
			//	return new APIResponse("Emote successful").ToString();
			//	;
			//} else
			//{
			//	return new APIResponse("Failed to emote").ToString();
			//}
			return "";
		}

		private static string RotateCamera(string[] input)
		{
			return RotateCamera(input[0], input[1]);
		}

		private static string SayText(string[] input)
		{
			return SayText(input[0], input[1]);
		}

		private static string SetTime(string[] input)
		{
			return SetTime(input[0]);
		}

		private static string SelectCreatureByCreatureId(string[] input)
		{
			return SelectCreatureByCreatureId(input[0]);
		}

		private static string SelectNextPlayerControlled(string[] input)
		{
			return SelectNextPlayerControlled();
		}

		private static string SelectPlayerControlledByAlias(string[] input)
		{
			return SelectPlayerControlledByAlias(input[0]);
		}

		private static string SetCameraHeight(string[] input)
		{
			return SetCameraHeight(input[0], input[1]);
		}

		private static string SetCreatureHp(string[] input)
		{
			return SetCreatureHp(input[0], input[1], input[2]);
		}

		private static string SetCreatureStat(string[] input)
		{
			return SetCreatureStat(input[0], input[1], input[2], input[3]);
		}

		private static string SetCustomStatName(string[] input)
		{
			return SetCustomStatName(input[0], input[1]);
		}

		private static void StartMove(MoveAction ma)
		{
			//PhotonSimpleSingletonBehaviour<CreatureManager>.Instance.TryGetAsset(new NGuid(ma.guid), out ma.asset);
			//if (ma.useHandle)
			//{
			//	ma.handle = MovableHandle.Spawn();
			//	ma.handle.Attach(ma.asset);
			//}
			//ma.asset.Pickup();
			//ma.moveTime = 0;
			//ma.StartLocation = ma.asset.transform.position;
			////Debug.Log("Start: " + ma.StartLocation);
			//var movePos = GetMoveVector(ma.dir) * ma.steps;
			////Debug.Log("MoveVec: " + movePos);
			//ma.DestLocation = Explorer.RoundToCreatureGrid(ma.StartLocation + movePos);
			////Debug.Log("Dest: " + ma.DestLocation);
			//currentActions.Add(ma);
		}

		private const string TaleSpireMachine = "192.168.22.42";
		const string STR_SpherePrefix = "Sphere";
		private static void StartSocketServer()
		{
			if (serverStarted)
			{
				return;
			}
			serverStarted = true;
			Task.Factory
				.StartNew(
					() =>
					{
						int port = 999;
						Debug.Log($"Starting Modding Socket at {TaleSpireMachine} and Port: {port}");
						//byte[] buffer = new Byte[4096];
						IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(TaleSpireMachine), port);
						Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

						try
						{
							listener.Bind(localEndPoint);
							listener.Listen(10);

							while (true)
							{
								while (true)
								{
									Socket socket = listener.Accept();
									string data = string.Empty;

									byte[] buffer = TaleSpireUtils.ReceiveAll(socket);
									int bytesRec = buffer.Length;
									data += Encoding.UTF8.GetString(buffer, 0, bytesRec);

									byte[] cmdResult = Encoding.UTF8.GetBytes(ExecuteCommand(data));
									socket.Send(cmdResult);

									socket.Shutdown(SocketShutdown.Both);
									socket.Close();
								}
							}
						}
						catch (Exception e)
						{
							UnityEngine.Debug.Log(e.ToString());
						}
					});
		}

		private static string TiltCamera(string[] input)
		{
			return TiltCamera(input[0], input[1]);
		}

		private static void UpdateBoardLoad()
		{
			if (boardsToLoad.Count > 0)
			{
				BoardInfo bi = boardsToLoad.Dequeue();
				SingletonBehaviour<BoardSaverManager>.Instance.Load(bi);
			}
		}

		private static void UpdateMove()
		{
			RaycastHit[] creatureHits = new RaycastHit[10];
			for (int i = currentActions.Count() - 1; i >= 0; i--)
			{
				//Debug.Log("Updating: " + i);
				//Debug.Log(currentActions[i]);
				MoveAction ma = currentActions[i];
				ma.moveTime += (Time.deltaTime / (ma.steps * 0.6f));
				currentActions[i] = ma;

				Ray ray = new Ray(currentActions[i].asset.transform.position + new Vector3(0f, 1.5f, 0f), -Vector3.up);
				int num = Physics.SphereCastNonAlloc(ray, 0.32f, creatureHits, 2f, 2048);
				Debug.DrawRay(ray.origin, ray.direction * 10f, Color.white);
				float num2 = Explorer.GetTileHeightAtLocation(currentActions[i].asset.transform.position, 0.4f, 4f);

				var currentPos = Vector3.Lerp(
					currentActions[i].asset.transform.position,
					currentActions[i].DestLocation,
					currentActions[i].moveTime);

				//currentPos.y = Explorer.GetTileHeightAtLocation(currentPos, 0.4f, 4f) + 0.05f;// + 1.5f;
				currentActions[i].asset.RotateTowards(currentPos);
				currentActions[i].asset.MoveTo(currentPos);
				//Debug.Log("Drop check:" + currentPos + " dest:" + currentActions[i].DestLocation);
				if (currentPos.x == currentActions[i].DestLocation.x && currentPos.z == currentActions[i].DestLocation.z)
				{
					//Debug.Log("Dropping");
					currentActions[i].asset.Drop(currentPos, currentPos.y);
					if (currentActions[i].useHandle)
					{
						currentActions[i].handle.Detach();
						PhotonNetwork.Destroy(currentActions[i].handle.gameObject);
					}
					var creatureNGuid = new NGuid(currentActions[i].guid);
					//CameraController.LookAtCreature(creatureNGuid);
					currentActions.RemoveAt(i);
				}
			}
		}

		private static string ZoomCamera(string[] input)
		{
			return ZoomCamera(input[0], input[1]);
		}

		public static string AddCreature(
			string nguid,
			string x,
			string y,
			string z,
			string scale,
			string alias,
			string hpcurr,
			string hpmax,
			string stat1curr,
			string stat1max,
			string stat2curr,
			string stat2max,
			string stat3curr,
			string stat3max,
			string stat4curr,
			string stat4max,
			string torch,
			string hidden)
		{
			float3 pos = math.float3(float.Parse(x), float.Parse(y), float.Parse(z));
			spawnCreaturePos = pos;

			CreatureData data = new CreatureData();
			//CreatureData data = new CreatureData(
			//	new NGuid(nguid),
			//	NGuid.Empty,
			//	math.float3(float.Parse(x), float.Parse(y), float.Parse(z)),
			//	quaternion.identity,
			//	float.Parse(scale),
			//	alias,
			//	null,
			//	null,
			//	null,
			//	new CreatureStat(float.Parse(hpcurr), float.Parse(hpmax)),
			//	new CreatureStat(float.Parse(stat1curr), float.Parse(stat1max)),
			//	new CreatureStat(float.Parse(stat2curr), float.Parse(stat2max)),
			//	new CreatureStat(float.Parse(stat3curr), float.Parse(stat3max)),
			//	new CreatureStat(float.Parse(stat4curr), float.Parse(stat4max)),
			//	bool.Parse(torch),
			//	default(NGuid),
			//	bool.Parse(hidden));
			spawnCreature = CreaturePreviewBoardAsset.Spawn(data, pos, quaternion.identity);
			spawnCreature.Drop(math.float3(float.Parse(x), float.Parse(y), float.Parse(z)), float.Parse(y));

			return new ApiResponse("Creature Added").ToString();
		}

		public static string CreateSlab(string x, string y, string z, string slabText)
		{
			Debug.Log("X:" + x + " y:" + y + " z:" + z + " Slab: " + slabText);
			slabQueue.Enqueue(
				new SlabData { Position = new VectorDto(float.Parse(x), float.Parse(y), float.Parse(z)), SlabText = slabText });
			return new ApiResponse("Slab Paste Queued").ToString();
		}

		public static string GetBoards()
		{
			//Debug.Log("Current Board Name: " + BoardSessionManager.CurrentBoardInfo.BoardName);
			List<CustomBoardInfo> lbi = new List<CustomBoardInfo>();
			foreach (BoardInfo bi in CampaignSessionManager.MostRecentBoardList)
			{
				lbi.Add(
					new CustomBoardInfo
					{
						BoardId = BoardSessionManager.CurrentBoardInfo.Id.ToString(),
						BoardName = BoardSessionManager.CurrentBoardInfo.BoardName,
						BoardDesc = BoardSessionManager.CurrentBoardInfo.Description,
						CampaignId = BoardSessionManager.CurrentBoardInfo.CampaignId.ToString()
					});
			}
			return JsonConvert.SerializeObject(lbi);
		}

		public static string GetCameraLocation()
		{
			return JsonConvert.SerializeObject(
				new VectorDto(CameraController.Position.x, CameraController.CameraHeight, CameraController.Position.z));
		}

		public static string GetCreatureAssets()
		{
			return "";
			//var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
			//List<CustomBoardAssetData> cbad = new List<CustomBoardAssetData>();

			//DictionaryList<NGuid, BoardAssetData> b = (DictionaryList<NGuid, BoardAssetData>)typeof(BoardAssetDatabase).GetField("_lookup", flags).GetValue(null);

			//foreach (BoardAssetData bad in b.Values)
			//{
			//	if (bad.boardAssetType != "CREATURE")
			//	{
			//		continue;
			//	}
			//	cbad.Add(new CustomBoardAssetData
			//	{
			//		GUID = bad.GUID,
			//		boardAssetDesc = bad.boardAssetDesc,
			//		boardAssetGroup = bad.boardAssetGroup,
			//		boardAssetName = bad.boardAssetName,
			//		boardAssetType = bad.boardAssetType,
			//		seachString = bad.seachString
			//	});
			//}
			//return JsonConvert.SerializeObject(cbad);
		}

		public static string GetCreatureList()
		{
			try
			{
				List<CustomCreatureData> allCreatures = new List<CustomCreatureData>();

				var board = BoardSessionManager.Board;
				var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

				Dictionary<NGuid, CreatureData> creatures = (Dictionary<NGuid, CreatureData>)board.GetType()
					.GetField("_creatures", flags)
					.GetValue(board);
				foreach (KeyValuePair<NGuid, CreatureData> entry in creatures)
				{
					allCreatures.Add(convertCreatureData(entry.Value));
				}
				return JsonConvert.SerializeObject(allCreatures);
			}
			catch (Exception ex)
			{
				return new ApiResponse(ex.Message + ex.StackTrace, "Could not get creature list").ToString();
			}
		}

		public static string GetCreatureStats(string creatureId)
		{
			try
			{
				//CreatureData cd = BoardSessionManager.Board.GetCreatureData(new NGuid(creatureId));
				//List<CustomCreatureStat> creatureStats = new List<CustomCreatureStat>();
				//creatureStats.Add(new CustomCreatureStat(cd.Hp.Value, cd.Hp.Max));
				//for (int i = 0; i < 5; i++)
				//{
				//	CreatureStat stat = cd.StatByIndex(i);
				//	creatureStats.Add(new CustomCreatureStat(stat.Value, stat.Max));
				//}
				//return new APIResponse(JsonConvert.SerializeObject(creatureStats)).ToString();
			}
			catch (Exception ex)
			{
				//return new APIResponse(ex.Message + ex.StackTrace, "Could not get hp").ToString();
			}
			return "";
		}

		public static string GetCurrentBoard()
		{
			return JsonConvert.SerializeObject(
				new CustomBoardInfo
				{
					BoardId = BoardSessionManager.CurrentBoardInfo.Id.ToString(),
					BoardName = BoardSessionManager.CurrentBoardInfo.BoardName,
					BoardDesc = BoardSessionManager.CurrentBoardInfo.Description,
					CampaignId = BoardSessionManager.CurrentBoardInfo.CampaignId.ToString()
				});
		}

		public static string GetPlayerControlledList()
		{
			return "";
			//try
			//{
			//	List<CustomCreatureData> playerControlled = new List<CustomCreatureData>();
			//	NGuid[] creatureIds;

			//	if (BoardSessionManager.Board.TryGetPlayerOwnedCreatureIds(LocalPlayer.Id.Value, out creatureIds))
			//	{
			//		for (int i = 0; i < creatureIds.Length; i++)
			//		{
			//			playerControlled.Add(convertCreatureData(BoardSessionManager.Board.GetCreatureData(creatureIds[i])));
			//		}

			//		return JsonConvert.SerializeObject(playerControlled);
			//	} else
			//	{
			//		return "[]";
			//	}
			//} catch (Exception ex)
			//{
			//	return new APIResponse(ex.Message + ex.StackTrace, "Could not get player controlled list").ToString();
			//}
		}

		public static PostProcessLayer GetPostProcessLayer()
		{
			return Camera.main.GetComponent<PostProcessLayer>();
		}

		public static Slab GetSelectedSlab()
		{
			try
			{
				var test = SingletonBehaviour<SlabBuilderBoardTool>.Instance;
			}
			catch
			{
			}
			var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
			if (SingletonBehaviour<BoardToolManager>.HasInstance &&
				(SingletonBehaviour<BoardToolManager>.Instance.IsCurrentTool<SlabBuilderBoardTool>()))
			{
				var sbbt = SingletonBehaviour<SlabBuilderBoardTool>.Instance;
				Slab slab = (Slab)sbbt.GetType().GetField("_slab", flags).GetValue(sbbt);
				return slab;
			}
			else
			{
				return null;
			}
		}

		public static TilePreviewBoardAsset GetSelectedTileAsset()
		{
			try
			{
				var test = SingletonBehaviour<TileBuilderBoardTool>.Instance;
			}
			catch
			{
			}
			var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
			if (SingletonBehaviour<BoardToolManager>.HasInstance &&
				(SingletonBehaviour<BoardToolManager>.Instance.IsCurrentTool<TileBuilderBoardTool>()))
			{
				var btm = SingletonBehaviour<TileBuilderBoardTool>.Instance;
				TilePreviewBoardAsset selectedAsset = (TilePreviewBoardAsset)btm.GetType()
					.GetField("_selectedTileBoardAsset", flags)
					.GetValue(btm);
				return selectedAsset;
			}
			else
			{
				return null;
			}
		}

		public static async Task<string> GetSlabSize(string slabText)
		{
			int msPassed = 0;
			try
			{
				slabSizeResponse = false;
				slabSizeSlab = slabText;

				while (slabSizeResponse == false || msPassed > 1000)
				{
					msPassed++;
					await Task.Delay(1);
				}
				return JsonConvert.SerializeObject(new VectorDto(slabSize.x, slabSize.y, slabSize.z));
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message + ex.StackTrace);
				return new ApiResponse(ex.Message + ex.StackTrace, "Could not get slab size").ToString();
			}
		}

		public static void GetSlabSize()
		{
			if (slabSizeSlab != string.Empty)
			{
				var slabToPaste = slabSizeSlab;// slabQueue.Dequeue();
				slabSizeSlab = string.Empty;

				if (BoardSessionManager.Board.PushStringToTsClipboard(slabToPaste) == PushStringToTsClipboardResult.Success)
				{
					Copied mostRecentCopied_LocalOnly = BoardSessionManager.Board.GetMostRecentCopied_LocalOnly();
					if (mostRecentCopied_LocalOnly != null)
					{
						//slabSize = mostRecentCopied_LocalOnly.Bounds.size;
						slabSize = mostRecentCopied_LocalOnly.RoughBoundsInfo.CombinedBounds.size;

						slabSizeResponse = true;
					}
				}
				else
				{
					slabSize = new float3(0, 0, 0);
					slabSizeResponse = true;
				}
			}
		}

		public static TextMeshProUGUI GetUITextByName(string name)
		{
			TextMeshProUGUI[] texts = UnityEngine.Object.FindObjectsOfType<TextMeshProUGUI>();
			for (int i = 0; i < texts.Length; i++)
			{
				if (texts[i].name == name)
				{
					return texts[i];
				}
			}
			return null;
		}

		public static TextMeshProUGUI GetUITextContainsString(string contains)
		{
			TextMeshProUGUI[] texts = UnityEngine.Object.FindObjectsOfType<TextMeshProUGUI>();
			for (int i = 0; i < texts.Length; i++)
			{
				if (texts[i].text.Contains(contains))
				{
					return texts[i];
				}
			}
			return null;
		}

		public static void Initialize(BaseUnityPlugin parentPlugin, ManualLogSource logger, bool startSocket = false)
		{
			AppStateManager.UsingCodeInjection = true;
			ModdingUtils.parentPlugin = parentPlugin;
			ModdingUtils.parentLogger = logger;
			parentLogger.LogInfo("Inside initialize");
			SceneManager.sceneLoaded += OnSceneLoaded;
			// By default do not start the socket server. It requires the caller to also call OnUpdate in the plugin update method.
			if (startSocket)
			{
				StartSocketServer();
			}
		}

		public static string KillCreature(string creatureId)
		{
			//CreatureBoardAsset creatureBoardAsset;
			//if (PhotonSimpleSingletonBehaviour<CreatureManager>.Instance
			//	.TryGetAsset(new NGuid(creatureId), out creatureBoardAsset))
			//{
			//	Creature creature = creatureBoardAsset.Creature;
			//	creature.BoardAsset.RequestDelete();
			//	return new APIResponse("Delete request successful").ToString();
			//} else
			//{
			//	return new APIResponse("Failed to delete").ToString();
			//}
			return "";
		}

		public static string LoadBoard(string boardId)
		{
			foreach (BoardInfo bi in CampaignSessionManager.MostRecentBoardList)
			{
				if (bi.Id.ToString() == boardId)
				{
					boardsToLoad.Enqueue(bi);
					return new ApiResponse("Board load queued successfully").ToString();
				}
			}
			return new ApiResponse("Board not found").ToString();
		}

		public static string MoveCamera(string x, string y, string z, string absolute)
		{
			var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
			Transform t = (Transform)CameraController.Instance
				.GetType()
				.GetField("_camRotator", flags)
				.GetValue(CameraController.Instance);
			float zoom = (float)CameraController.Instance
				.GetType()
				.GetField("_targetZoomLerpValue", flags)
				.GetValue(CameraController.Instance);

			var babsolute = bool.Parse(absolute);
			if (babsolute)
			{
				//CameraController.MoveToPosition(newPos, true);
				CameraController.LookAtTargetXZ(new Vector2(float.Parse(x), float.Parse(z)));
			}
			else
			{
				//CameraController.MoveToPosition(newPos + (float3)CameraController.Position, true);
				CameraController.LookAtTargetXZ(
					new Vector2(float.Parse(x) + CameraController.Position.x, float.Parse(z) + CameraController.Position.z));
			}
			return new ApiResponse("Camera Move successful").ToString();
		}

		public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			try
			{
				parentLogger.LogInfo("On Scene Loaded" + scene.name);
				UnityEngine.Debug.Log("Loading Scene: " + scene.name);
				if (scene.name == "UI")
				{
					TextMeshProUGUI betaText = GetUITextByName("BETA");
					if (betaText)
					{
						betaText.text = "INJECTED BUILD - unstable mods";
					}
				}
				else
				{
					TextMeshProUGUI modListText = GetUITextByName("TextMeshPro Text");
					if (modListText)
					{
						BepInPlugin bepInPlugin = (BepInPlugin)Attribute.GetCustomAttribute(
							ModdingUtils.parentPlugin.GetType(),
							typeof(BepInPlugin));
						if (modListText.text.EndsWith("</size>"))
						{
							modListText.text += "\n\nMods Currently Installed:\n";
						}
						modListText.text += "\n" + bepInPlugin.Name + " - " + bepInPlugin.Version;
					}
				}
			}
			catch (Exception ex)
			{
				parentLogger.LogFatal(ex);
			}
		}

		// This only needs to be called from update if you are using the socket API or MoveCharacter calls.
		public static void OnUpdate()
		{
			if (spawnCreature != null)
			{
				CreatureManager.CreateAndAddNewCreature(spawnCreature.CreatureData, spawnCreaturePos, quaternion.identity);
				spawnCreature.DeleteAsset();
				spawnCreature = null;
			}
			UpdateMove();
			UpdateSpeech();
			UpdateCustomStatNames();
			UpdateSlab();
			GetSlabSize();
			UpdateBoardLoad();
		}

		public static string RotateCamera(string rotation, string absolute)
		{
			var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

			Transform t = (Transform)CameraController.Instance
				.GetType()
				.GetField("_camRotator", flags)
				.GetValue(CameraController.Instance);

			var babsolute = bool.Parse(absolute);
			if (babsolute)
			{
				t.localRotation = Quaternion.Euler(0f, float.Parse(rotation), 0f);
			}
			else
			{
				t.localRotation = Quaternion.Euler(0f, float.Parse(rotation) + t.localRotation.eulerAngles.y, 0f);
			}
			return new ApiResponse("Camera Move successful").ToString();
		}

		public static string SayText(string creatureId, string text)
		{
			sayTextQueue.Enqueue(new SayTextData { CreatureId = creatureId, Text = text });
			return new ApiResponse("Say queued successful").ToString();
		}

		public static string SetTime(string time)
		{
			if (float.TryParse(time, out float result))
			{
				AtmosphereManager.SetTimeOfDay(result);
				return new ApiResponse("SetTime successful").ToString();
			}
			return new ApiResponse($"Error converting time ({time}) to a float.").ToString();
		}

		public static string SelectCreatureByCreatureId(string guid)
		{
			return "";
			//try
			//{
			//	var creatureNGuid = new NGuid(guid);
			//	if (LocalClient.SelectedCreatureId == creatureNGuid)
			//	{
			//		return new APIResponse("Selected successfully").ToString();
			//	}
			//	LocalClient.SelectedCreatureId = creatureNGuid;
			//	CameraController.LookAtCreature(creatureNGuid);
			//	return new APIResponse("Selected successfully").ToString();
			//} catch (Exception ex)
			//{
			//	return new APIResponse(ex.Message, "Error selecting via nguid: " + guid).ToString();
			//}
		}

		public static string SelectNextPlayerControlled()
		{
			return "";
			//try
			//{
			//	NGuid[] creatureIds;
			//	if (BoardSessionManager.Board.TryGetPlayerOwnedCreatureIds(LocalPlayer.Id.Value, out creatureIds))
			//	{
			//		int i = 0;
			//		while (i < creatureIds.Length)
			//		{
			//			//Debug.Log(LocalClient.SelectedCreatureId);
			//			//Debug.Log(creatureIds[i]);
			//			//Debug.Log(BoardSessionManager.Board.GetCreatureData(creatureIds[i]).Alias);
			//			if (creatureIds[i] == LocalClient.SelectedCreatureId)
			//			{
			//				if (i + 1 < creatureIds.Length)
			//				{
			//					LocalClient.SelectedCreatureId = creatureIds[i + 1];
			//					CameraController.LookAtCreature(creatureIds[i + 1]);
			//					break;
			//				}
			//				LocalClient.SelectedCreatureId = creatureIds[0];
			//				CameraController.LookAtCreature(creatureIds[0]);
			//				break;
			//			} else
			//			{
			//				i++;
			//			}
			//		}
			//		return BoardSessionManager.Board.GetCreatureData(creatureIds[i]).Alias;
			//	} else
			//	{
			//		return string.Empty;
			//	}
			//} catch (Exception ex)
			//{
			//	return new APIResponse(ex.Message, "Unable to select next.").ToString();
			//}
		}

		public static string SelectPlayerControlledByAlias(string alias)
		{
			return "";
			//try
			//{
			//	NGuid[] creatureIds;
			//	if (BoardSessionManager.Board.TryGetPlayerOwnedCreatureIds(LocalPlayer.Id.Value, out creatureIds))
			//	{
			//		int i = 0;
			//		while (i < creatureIds.Length)
			//		{
			//			//Debug.Log(LocalClient.SelectedCreatureId);
			//			//Debug.Log(creatureIds[i]);
			//			//Debug.Log(BoardSessionManager.Board.GetCreatureData(creatureIds[i]).Alias);
			//			if (BoardSessionManager.Board.GetCreatureData(creatureIds[i]).Alias.ToLower() == alias.ToLower())
			//			{
			//				LocalClient.SelectedCreatureId = creatureIds[i];
			//				CameraController.LookAtCreature(creatureIds[i]);
			//				break;
			//			} else
			//			{
			//				i++;
			//			}
			//		}
			//		return BoardSessionManager.Board.GetCreatureData(creatureIds[i]).Alias;
			//	} else
			//	{
			//		return "[]";
			//	}
			//} catch (Exception)
			//{
			//	return new APIResponse("Failed to find alias", "Unable to select by alias: " + alias).ToString();
			//}
		}

		public static string SendOOBMessage(string message, AsyncCallback callback = null)
		{
			int port = 887;

			IPHostEntry ipHostInfo = Dns.GetHostEntry("d20armyknife.com");
			IPEndPoint localEndPoint = new IPEndPoint(ipHostInfo.AddressList[0], port);
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect(localEndPoint);
			byte[] byteData = Encoding.UTF8.GetBytes(message);
			if (callback != null)
			{
				socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(callback), socket);
				return string.Empty;
			}
			else
			{
				socket.Send(byteData);
				byte[] buffer = TaleSpireUtils.ReceiveAll(socket);
				int bytesRec = buffer.Length;
				string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);

				//Debug.Log("OOB Response: " + data);
				//Debug.Log("Buffer Len:" + bytesRec.ToString());
				return data;
			}
		}

		public static string SetCameraHeight(string height, string absolute)
		{
			if (bool.Parse(absolute))
			{
				CameraController.MoveToHeight(float.Parse(height), true);
			}
			else
			{
				CameraController.MoveToHeight(float.Parse(height) + CameraController.CameraHeight, true);
			}
			return new ApiResponse("Camera Move successful").ToString();
		}

		public static string SetCreatureHp(string creatureId, string currentHp, string maxHp)
		{
			return "";
			//try
			//{
			//	List<CustomCreatureData> allCreatures = new List<CustomCreatureData>();

			//	var board = BoardSessionManager.Board;

			//	board.SetCreatureStatByIndex(
			//		new NGuid(creatureId),
			//		new CreatureStat(float.Parse(currentHp), float.Parse(maxHp)),
			//		-1);
			//	SingletonBehaviour<BoardToolManager>.Instance.GetTool<CreatureMenuBoardTool>().CallUpdate();
			//	return new APIResponse(String.Format("Set HP to {0}:{1} for {2}", currentHp, maxHp, creatureId)).ToString();
			//} catch (Exception ex)
			//{
			//	return new APIResponse(ex.Message + ex.StackTrace, "Could not set hp").ToString();
			//}
		}

		public static string SetCreatureStat(string creatureId, string statIdx, string current, string max)
		{
			return "";
			//try
			//{
			//	List<CustomCreatureData> allCreatures = new List<CustomCreatureData>();

			//	var board = BoardSessionManager.Board;

			//	board.SetCreatureStatByIndex(
			//		new NGuid(creatureId),
			//		new CreatureStat(float.Parse(current), float.Parse(max)),
			//		int.Parse(statIdx) - 1);
			//	SingletonBehaviour<BoardToolManager>.Instance.GetTool<CreatureMenuBoardTool>().CallUpdate();
			//	return new APIResponse(String.Format("Set stat{0} to {1}:{2} for {3}", statIdx, current, max, creatureId)).ToString(
			//		);
			//} catch (Exception ex)
			//{
			//	return new APIResponse(ex.Message + ex.StackTrace, "Could not set stat").ToString();
			//}
		}

		public static string SetCustomStatName(string index, string newName)
		{
			Debug.Log("Index " + index + " new name: " + newName);
			customStatNames[int.Parse(index) - 1] = newName;
			return new ApiResponse("Stat Name Set").ToString();
		}

		public static string TiltCamera(string tilt, string absolute)
		{
			var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

			Transform t = (Transform)CameraController.Instance
				.GetType()
				.GetField("_tiltTransform", flags)
				.GetValue(CameraController.Instance);

			// TODO: Move this to the update method so it can be done with animation instead of just a sudden jolt. Same with rotation.
			var babsolute = bool.Parse(absolute);
			if (babsolute)
			{
				t.localRotation = Quaternion.Euler(float.Parse(tilt), 0f, 0f);
			}
			else
			{
				t.localRotation = Quaternion.Euler(t.localRotation.eulerAngles.x + float.Parse(tilt), 0f, 0f);
			}
			return new ApiResponse("Camera Move successful").ToString();
		}

		public static void UpdateCustomStatNames()
		{
			TextMeshProUGUI stat;
			for (int i = 0; i < customStatNames.Length; i++)
			{
				if (customStatNames[i] != string.Empty)
				{
					//Debug.Log("Inside statnames");
					//Debug.Log("Stat " + (i + 1));
					stat = GetUITextContainsString("Stat " + (i + 1));
					if (stat)
					{
						//Debug.Log("Found stat " + i);
						stat.text = customStatNames[i];
					}
				}
			}
		}

		public static void UpdateSlab()
		{
			while (slabQueue.Count > 0)
			{
				var slabToPaste = slabQueue.Dequeue();
				Debug.Log("Slab:");
				Debug.Log(slabToPaste);
				if (BoardSessionManager.Board.PushStringToTsClipboard(slabToPaste.SlabText) ==
					PushStringToTsClipboardResult.Success)
				{
					Copied mostRecentCopied_LocalOnly = BoardSessionManager.Board.GetMostRecentCopied_LocalOnly();
					if (mostRecentCopied_LocalOnly != null)
					{
						Debug.Log(
							"X:" +
								slabToPaste.Position.x +
								" y:" +
								slabToPaste.Position.x +
								" z:" +
								slabToPaste.Position.z +
								" Slab: " +
								slabToPaste.SlabText);
						BoardSessionManager.Board
							.PasteCopied(new Vector3(slabToPaste.Position.x, slabToPaste.Position.y, slabToPaste.Position.z), 0, 0UL);
						//BoardSessionManager.Board.PasteCopied(new Vector3(slabToPaste.Position.x, slabToPaste.Position.y, slabToPaste.Position.z), 0, 0UL);
					}
				}
			}
		}

		public static void UpdateSpeech()
		{
			return;
			//try
			//{
			//	var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
			//	while (sayTextQueue.Count > 0)
			//	{
			//		var sayText = sayTextQueue.Dequeue();
			//		CreatureBoardAsset creatureBoardAsset;
			//		if (PhotonSimpleSingletonBehaviour<CreatureManager>.Instance
			//			.TryGetAsset(new NGuid(sayText.CreatureId), out creatureBoardAsset))
			//		{
			//			Creature creature = creatureBoardAsset.Creature;
			//			creature.Speak(sayText.Text);
			//		}
			//	}

			//	var tbm = SingletonBehaviour<TextBubbleManager>.Instance;
			//	List<TextBubble> bubbles = (List<TextBubble>)tbm.GetType().GetField("_bubblesInUse", flags).GetValue(tbm);
			//	foreach (var bubble in bubbles)
			//	{
			//		TextMeshProUGUI bubbleText = (TextMeshProUGUI)bubble.GetType().GetField("_text", flags).GetValue(bubble);
			//		bubbleText.GetComponent<RectTransform>().localPosition = new Vector2(
			//			-(bubbleText.preferredWidth / 2),
			//			bubbleText.GetComponent<RectTransform>().localPosition.y);
			//	}
			//} catch (Exception ex)
			//{
			//	Debug.Log(ex.Message + ex.StackTrace);
			//}
		}

		public static string ZoomCamera(string zoom, string absolute)
		{
			var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
			Transform t = (Transform)CameraController.Instance
				.GetType()
				.GetField("_camRotator", flags)
				.GetValue(CameraController.Instance);
			float current_zoom = (float)CameraController.Instance
				.GetType()
				.GetField("_targetZoomLerpValue", flags)
				.GetValue(CameraController.Instance);
			float minFov = 0;
			float maxFov = 1;


			float newZoom;
			var babsolute = bool.Parse(absolute);
			if (babsolute)
			{
				newZoom = Mathf.Clamp(float.Parse(zoom), minFov, maxFov);
			}
			else
			{
				newZoom = Mathf.Clamp(current_zoom + float.Parse(zoom), minFov, maxFov);
			}
			CameraController.Instance
				.GetType()
				.GetField("_targetZoomLerpValue", flags)
				.SetValue(CameraController.Instance, newZoom);
			return new ApiResponse("Camera Move successful").ToString();

		}

		static void SwitchTargetOn()
		{
			Talespire.Target.On(40);
		}

		static void SwitchTargetOff()
		{
			Talespire.Target.Off();
		}

		static void TargetCleanUp()
		{
			Talespire.Target.CleanUp();
		}

		static void TargetSet()
		{
			Talespire.Target.Set();
		}

		static int ToInt(string value)
		{
			if (int.TryParse(value, out int result))
				return result;
			return 0;
		}

		static void ChangeTargetSphereSize(int newSize)
		{
			
		}

		static string Target(string command)
		{
			if (command == "On")
			{
				SwitchTargetOn();
				// !crIssue -log "Issue: Inline Method and Delete is not available."
			}
			else if (command == "Off")
				SwitchTargetOff();
			else if (command == "CleanUp")
				TargetCleanUp();
			else if (command == "Set")
				TargetSet();
			else if (command.StartsWith(STR_SpherePrefix))
				ChangeTargetSphereSize(ToInt(command.Substring(STR_SpherePrefix.Length)));
			else
				return ApiResponse.InvalidCommand(command);
			return ApiResponse.Good();
		}

		static string Target(string[] arg)
		{
			Talespire.Log.Debug("Target, with these parameters: ");
			foreach (string item in arg)
			{
				Talespire.Log.Debug($"  {item}");
			}

			if (arg.Length == 1)
			{
				return Target(arg[0]);
			}

			return ApiResponse.Good();
		}

		static string GetCreatures(string[] arg)
		{
			CharacterPositions characterPositions = Talespire.Minis.GetPositions();

			return ApiResponse.Good("Success", characterPositions);
		}
	}
}
