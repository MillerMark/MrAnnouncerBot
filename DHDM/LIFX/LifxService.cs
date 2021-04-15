//#define profiling
using BotCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DHDM
{
	public class LifxService
	{
		DateTime serviceStartTime = DateTime.Now;
		List<string> log = new List<string>();
		static Dictionary<string, LifxSetting> lastSettings = new Dictionary<string, LifxSetting>();
		static HttpClient client = new HttpClient();

		public async Task ChangeBulbSettings(string bulbLabel, string color, double brightness, double duration = 0.1)
		{
			LifxSetting lifxSetting = new LifxSetting()
			{
				Color = color,
				Brightness = Math.Round(brightness, 1),
				Duration = duration
			};

			if (!lastSettings.ContainsKey(bulbLabel))
				lastSettings.Add(bulbLabel, lifxSetting);
			else
			{
				if (lastSettings[bulbLabel] == lifxSetting && lastSettings[bulbLabel].AgeSeconds < 12)
					return;

				lastSettings[bulbLabel] = lifxSetting;
			}

			LightSettingDto lightSettingDto = new LightSettingDto()
			{
				color = color,
				brightness = lifxSetting.Brightness,
				duration = duration,
				fast = true,
				infrared = 0,
				power = "on"
			};

			log.Add($"{GetTimeStr()}: {bulbLabel} -> {color} in {duration}s.");
			ByteArrayContent byteContent = ToSerializedBytes(lightSettingDto);
			if (!bulbLabel.StartsWith("group:"))
				bulbLabel = "label:" + bulbLabel;
			HttpResponseMessage httpResponseMessage = await client.PutAsync($"https://api.lifx.com/v1/lights/{bulbLabel}/state", byteContent);
			if (await ErrorsReported(httpResponseMessage))
				return;
			
			//string result = httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
		}

		public LifxService(MySecureString bearerToken)
		{
			InitializeClient(bearerToken);
		}

		public void InitializeClient(MySecureString bearerToken)
		{
			client.DefaultRequestHeaders.Add("authorization", $"Bearer {bearerToken.GetStr()}");
		}

		async Task<bool> ErrorsReported(HttpResponseMessage httpResponseMessage, [CallerMemberName] string memberName = "")
		{
			if (!httpResponseMessage.IsSuccessStatusCode)
			{
				string result = await httpResponseMessage.Content.ReadAsStringAsync();
				log.Add($"{GetTimeStr()}: Error - {result}.");
				//System.Diagnostics.Debugger.Break();
				//System.Windows.MessageBox.Show(result, $"HTTP Response Error in {memberName}");
				return true;
			}
			return false;
		}

		private string GetTimeStr()
		{
			return Math.Round((DateTime.Now - serviceStartTime).TotalMilliseconds).ToString().PadLeft(7, ' ');
		}

		private static ByteArrayContent ToSerializedBytes(object updateData)
		{
			JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
			var myContent = JsonConvert.SerializeObject(updateData, jsonSerializerSettings);
			var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
			var byteContent = new ByteArrayContent(buffer);
			byteContent.Headers.Add("content-type", "application/json");
			return byteContent;
		}

		public async void RestoreDefaultBulbSettings()
		{
			await ChangeBulbSettings("Left", "#ffffff", 0.2, 0.3);
			await ChangeBulbSettings("Right", "#ffffff", 0.3, 0.3);
		}

		public async void TurnOffBulbs()
		{
			await ChangeBulbSettings("group:Fireball", "#ffffff", 0);
		}
	}
}