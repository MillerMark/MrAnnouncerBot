using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TaleSpireCore
{
	public static class TaleSpireClient
	{
		//private const string TaleSpireMachine = "192.168.22.42";
		public const string TaleSpireMachineIpAddress = "192.168.22.41";

		public static ApiResponse SendMessageToServer(string command, float num)
		{
			return Invoke(command, new string[] { num.ToString() });
		}

		static ApiResponse ToApiResponse(string data)
		{
			try
			{
				return JsonConvert.DeserializeObject<ApiResponse>(data);
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
				return new ApiResponse(ex.Message, "Failed to deserialize response!", data);
			}
		}

		public static ApiResponse Invoke(string command, string[] msgparams)
		{
			// Data buffer for incoming data.  
			byte[] bytes = new byte[4 * 1024 * 1024];

			// Connect to a remote device.  
			try
			{
				int port = 999;
				IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(TaleSpireMachineIpAddress), port);
					
				// Create a TCP/IP  socket.  
				Socket sender = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

				// Connect the socket to the remote endpoint. Catch any errors.  
				try
				{
					sender.Connect(localEndPoint);

					Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());

					// Encode the data string into a byte array.  

					// Encode commas in the strings...
					for (int i = 0; i < msgparams.Length; i++)
						if (msgparams[i] == null)
							msgparams[i] = "";
						else
							msgparams[i] = msgparams[i].Replace(',', '⁞');

					byte[] msg = Encoding.UTF8.GetBytes(command + " " + string.Join(",", msgparams));

					// Send the data through the socket.  
					int bytesSent = sender.Send(msg);
					Console.WriteLine("Bytes sent:" + bytesSent.ToString());
					Console.WriteLine("Command Sent: " + Encoding.UTF8.GetString(msg, 0, bytesSent));
					sender.ReceiveTimeout = 3000;
					// Receive the response from the remote device.  
					string data = string.Empty;
					int bytesRec = 0;
					int sleeps = 0;
					while (sender.Available == 0 && sleeps < 1000)
					{
						System.Threading.Thread.Sleep(30);
						sleeps++;
					}
					while (sender.Available > 0)
					{
						bytesRec = sender.Receive(bytes);
						data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
					}
					//int bytesRec = sender.Receive(bytes, 0, sender.Available, SocketFlags.None);
					//int bytesRec = sender.Receive(bytes);
					//var data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
					Console.WriteLine("Server responded bytes: {0} {1}", bytesRec, data);
					// Release the socket.  
					sender.Shutdown(SocketShutdown.Both);
					sender.Close();
					return ToApiResponse(data);
				} catch (ArgumentNullException ane)
				{
					Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
					return ApiResponse.FromException(ane);
				} catch (SocketException se)
				{
					Console.WriteLine("SocketException : {0}", se.ToString());
					return ApiResponse.FromException(se);
				} catch (Exception e)
				{
					Console.WriteLine("Unexpected exception : {0}", e.ToString());
					return ApiResponse.FromException(e);
				}
			} catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				return ApiResponse.FromException(e);
			}
		}

		public static ApiResponse Invoke(string command)
		{
			return Invoke(command, new string[] { });
		}

		public static ApiResponse Invoke(string command, string msgparams)
		{
			return Invoke(command, new string[] { msgparams });
		}

		public static void ClearActiveTurnIndicator()
		{
			Invoke("ClearActiveTurnIndicator");
		}

		public static void ShowDamage(string taleSpireId, int damageHealth, string bloodColor = "#980000")
		{
			Invoke("ShowDamage", new string[] { taleSpireId, damageHealth.ToString(), bloodColor });
		}

		public static void AddHitPoints(string taleSpireId, int health)
		{
			Invoke("AddHitPoints", new string[] { taleSpireId, health.ToString() });
		}

		public static void AddTempHitPoints(string taleSpireId, int health)
		{
			Invoke("AddTempHitPoints", new string[] { taleSpireId, health.ToString() });
		}

		public static void Wiggle(string taleSpireId)
		{
			Invoke("WiggleCreature", taleSpireId);
		}

		public static void RegisterAllies(List<string> allies)
		{
			Invoke("RegisterAllies", allies.ToArray());
		}

		public static void RegisterNeutrals(List<string> neutrals)
		{
			Invoke("RegisterNeutrals", neutrals.ToArray());
		}

		public static void TargetCreatures(List<string> charactersToTarget)
		{
			Invoke("TargetCreatures", charactersToTarget.ToArray());
		}

		public static void SetTargeted(string taleSpireId, bool isTargeted)
		{
			if (string.IsNullOrWhiteSpace(taleSpireId))
				return;
			Invoke("SetTargeted", new string[] { taleSpireId, isTargeted.ToString() });
		}

		public static void Speak(string taleSpireId, string message)
		{
			Invoke("Speak", new string[] { taleSpireId, message });
		}

		public static void Select(string taleSpireId)
		{
			Invoke("Select", new string[] { taleSpireId });
		}

		public static void SelectOne(string taleSpireId)
		{
			Invoke("SelectOne", new string[] { taleSpireId });
		}

		public static void LookAt(string taleSpireId)
		{
			Invoke("LookAt", new string[] { taleSpireId });
		}

		public static void SpinAround(string taleSpireId)
		{
			Invoke("SpinAround", new string[] { taleSpireId });
		}

		public static void RestoreCamera()
		{
			Invoke("RestoreCamera");
		}

		public static void AttachEffect(string effectName, string spellId, string taleSpireId, float enlargeTime, float lifeTime, float shrinkTime, float secondsDelayStart, float rotation)
		{
			Invoke("AttachEffect", new string[] { effectName, spellId, taleSpireId, lifeTime.ToString(), enlargeTime.ToString(), secondsDelayStart.ToString(), shrinkTime.ToString(), rotation.ToString() });
			//enlargeTime.ToString(), secondsDelayStart.ToString(), rotation.ToString() });
		}

		public static void PlayEffectAtCreatureBase(string effectName, string spellId, string taleSpireId, float lifeTime = 0, float enlargeTime = 0, float secondsDelayStart = 0, float shrinkTime = 0, float rotation = 0)
		{
			Invoke("PlayEffectAtCreatureBase", new string[] { effectName, spellId, taleSpireId, lifeTime.ToString(), enlargeTime.ToString(), secondsDelayStart.ToString(), shrinkTime.ToString(), rotation.ToString() });
		}

		public static void CreatureCastSpell(string effectName, string spellId, string taleSpireId, float lifeTime = 0, float enlargeTime = 0, float secondsDelayStart = 0, float shrinkTime = 0, float rotation = 0, bool isMoveable = false)
		{
			Invoke("CreatureCastSpell", new string[] { effectName, spellId, taleSpireId, lifeTime.ToString(), enlargeTime.ToString(), secondsDelayStart.ToString(), shrinkTime.ToString(), rotation.ToString(), isMoveable.ToString() });
		}

		public static void PlayEffectAtPosition(string effectName, string spellId, VectorDto vector, float lifeTime = 0, float enlargeTime = 0, float secondsDelayStart = 0, float shrinkTime = 0, float rotation = 0, bool isMoveable = false)
		{
			Invoke("PlayEffectAtPosition", new string[] { effectName, spellId, vector.GetXyzStr(), lifeTime.ToString(), enlargeTime.ToString(), secondsDelayStart.ToString(), shrinkTime.ToString(), rotation.ToString(), isMoveable.ToString() });
		}

		public static void BuildWall(string effectName, string spellId, float wallLength, float distanceBetweenWallEffectsFeet, float lifeTime = 0, float enlargeTime = 0, float secondsDelayStart = 0, float shrinkTime = 0, float rotation = 0)
		{
			Invoke("BuildWall", new string[] { effectName, spellId, wallLength.ToString(), distanceBetweenWallEffectsFeet.ToString(), 
																lifeTime.ToString(), enlargeTime.ToString(), secondsDelayStart.ToString(), 
																shrinkTime.ToString(), rotation.ToString() });
		}

		public static void PlayEffectOnCollision(string effectName, string spellId, float lifeTime, float enlargeTime, float secondsDelayStart, bool useIntendedTarget, float shrinkTime, float rotation, bool hitFloor = false)
		{
			Invoke("PlayEffectOnCollision", new string[] { effectName, spellId, lifeTime.ToString(), enlargeTime.ToString(), secondsDelayStart.ToString(), useIntendedTarget.ToString(), shrinkTime.ToString(), rotation.ToString(), hitFloor.ToString() });
		}

		public static void ClearAttached(string spellId, string taleSpireId)
		{
			Invoke("ClearAttached", new string[] { spellId, taleSpireId });
		}

		public static void ClearSpell(string spellId, float shrinkTime)
		{
			Invoke("ClearSpell", new string[] { spellId, shrinkTime.ToString() });
		}

		public static CharacterPosition GetPosition(string taleSpireId)
		{
			ApiResponse response = Invoke("GetCreature", taleSpireId);
			if (response.Result == ResponseType.Failure)
				return null;

			return response.GetData<CharacterPosition>();
		}

		public static int GetRulerCount()
		{
			ApiResponse response = Invoke("GetRulerCount");
			if (response.Result == ResponseType.Failure)
				return 0;

			return response.ToInt();
		}

		public static CharacterPosition GetSelectedMini()
		{
			ApiResponse response = Invoke("GetSelectedMini");
			if (response.Result == ResponseType.Failure)
				return null;

			return response.GetData<CharacterPosition>();
		}

		public static VectorDto GetFlashlightPosition()
		{
			ApiResponse response = Invoke("GetFlashlightPosition");
			if (response.Result == ResponseType.Failure)
				return null;

			return response.GetData<VectorDto>();
		}

		public static void StartTargeting(string shape, string dimensionsFeet, string casterTaleSpireId, float rangeInFeet)
		{
			Invoke("StartTargeting", new string[] { shape, dimensionsFeet, casterTaleSpireId, rangeInFeet.ToString() });
		}

		public static CharacterPositions GetAllCreaturesInVolume(VectorDto vectorDto, string shape, string dimensionStr, string whatSide = "All")
		{
			whatSide = whatSide.Replace(',', '|');
			ApiResponse response = Invoke("GetAllCreaturesInVolume", new string[] { vectorDto.GetXyzStr(), shape, dimensionStr, whatSide });

			if (response == null || response.Result == ResponseType.Failure)
				return null;

			return response.GetData<CharacterPositions>();
		}

		public static void TargetsAreReady()
		{
			Invoke("Target", "Ready");
		}

		public static void RemoveTargetingUI()
		{
			Invoke("Target", "RemoveUI");
		}

		public static void LookAtPoint(VectorDto point)
		{
			Invoke("LookAtPoint", point.GetXyzStr());
		}

		public static void SpinAroundPoint(VectorDto point)
		{
			Invoke("SpinAroundPoint", point.GetXyzStr());
		}

		public static void FlashlightOn()
		{
			Invoke("Flashlight", "On");
		}

		public static void FlashlightOff()
		{
			Invoke("Flashlight", "Off");
		}

		public static void MakeMiniVisible(string creatureId)
		{
			Invoke("MakeMiniVisible", creatureId);
		}

		public static void MakeMiniInvisible(string creatureId)
		{
			Invoke("MakeMiniInvisible", creatureId);
		}

		public static void CleanUpTargets()
		{
			Invoke("Target", "CleanUp");
		}

		public static void LaunchProjectile(string effectName, string taleSpireId, string kind, int count, float speed, string fireCollisionEventOn, float launchTimeVariance, float targetVariance,
																				string spellId, string projectileSize, float projectileSizeMultiplier, float bezierPathMultiplier, List<string> targets)
		{
			List<string> msgParams = new List<string>() { effectName, taleSpireId, kind, 
				count.ToString(), speed.ToString(), fireCollisionEventOn, launchTimeVariance.ToString(), targetVariance.ToString(),
			spellId, projectileSize, projectileSizeMultiplier.ToString(), bezierPathMultiplier.ToString() };

			msgParams.AddRange(targets);
			Invoke("LaunchProjectile", msgParams.ToArray());
		}
		public static ApiResponse GetCreatures()
		{
			return Invoke("GetCreatures");
		}
	}
}
