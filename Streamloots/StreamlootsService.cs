using BotCore;
using DndCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Streamloots
{
	public delegate void CardEventHandler(object sender, CardEventArgs ea);
	public class StreamlootsService : IStreamlootsService, IDisposable
	{
		public static event CardEventHandler CardRedeemed;
		public static event CardEventHandler CardsPurchased;

		protected virtual void OnCardsPurchased(object sender, CardEventArgs ea)
		{
			CardsPurchased?.Invoke(sender, ea);
		}
		protected void OnCardRedeemed(object sender, CardEventArgs ea)
		{
			CardRedeemed?.Invoke(sender, ea);
		}

		private MySecureString streamlootsID;

		private WebRequest webRequest;
		private Stream responseStream;

		private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

		public StreamlootsService(MySecureString streamlootsID)
		{
			this.streamlootsID = streamlootsID;
		}

		public Task<bool> Connect()
		{
			try
			{
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
				Task.Run(() => { BackgroundCheck(); }, cancellationTokenSource.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

				return Task.FromResult(true);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			return Task.FromResult(false);
		}

		public Task Disconnect()
		{
			cancellationTokenSource.Cancel();
			if (webRequest != null)
			{
				webRequest.Abort();
				webRequest = null;
			}
			if (responseStream != null)
			{
				responseStream.Close();
				responseStream = null;
			}
			return Task.FromResult(0);
		}

		private void BackgroundCheck()
		{
			//`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			//`! !!!                                                                                      !!!
			//`! !!!  Turn off Debug Visualizer before stepping through this method live on the stream!!! !!!
			//`! !!!                                                                                      !!!
			//`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

			webRequest = WebRequest.Create(string.Format("https://widgets.streamloots.com/alerts/{0}/media-stream", streamlootsID.GetStr()));
			((HttpWebRequest)webRequest).AllowReadStreamBuffering = false;
			WebResponse response = webRequest.GetResponse();
			responseStream = response.GetResponseStream();

			UTF8Encoding encoder = new UTF8Encoding();
			string cardStr = string.Empty;
			var buffer = new byte[100000];
			while (true)
			{
				try
				{
					while (responseStream.CanRead)
					{
						int len = responseStream.Read(buffer, 0, 100000);
						if (len <= 10)
							continue;

						string text = encoder.GetString(buffer, 0, len);
						if (string.IsNullOrEmpty(text))
							continue;

						cardStr += text;
						try
						{
							JObject cardObject = JObject.Parse("{ " + cardStr + " }");
							if (cardObject == null)
								continue;

							if (!cardObject.ContainsKey("data"))
								continue;

							cardStr = string.Empty;
							JObject cardData = cardObject.Value<JObject>("data");
							if (!cardData.ContainsKey("data"))
								continue;

							JObject cardDataData = cardData.Value<JObject>("data");
							if (!cardDataData.ContainsKey("type"))
								continue;

							string type = cardDataData.Value<string>("type");
							switch (type.ToLower())
							{
								case "purchase":
									ProcessChestPurchase(cardObject);
									break;
								case "redemption":
									ProcessCardRedemption(cardObject);
									break;
								default:
									Console.WriteLine($"Unknown Streamloots packet type: {type}");
									break;
							}
						}
						catch (Exception) { }   // To handle the case where partial packet data is received
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debugger.Break();
					Console.WriteLine(ex.ToString());

					webRequest = WebRequest.Create(string.Format("https://widgets.streamloots.com/alerts/{0}/media-stream", streamlootsID.GetStr()));
					((HttpWebRequest)webRequest).AllowReadStreamBuffering = false;
					response = webRequest.GetResponse();
					responseStream = response.GetResponseStream();

					encoder = new UTF8Encoding();
					cardStr = string.Empty;
					buffer = new byte[100000];
				}
			}
		}

		void ShowCardEvent(CardDto cardDto)
		{
			
		}

		void ShowPurchase(StreamlootsPurchase purchase)
		{
			CardDto cardDto = new CardDto(purchase);
			ShowCardEvent(cardDto);
			OnCardsPurchased(this, new CardEventArgs(cardDto));
		}

		void ShowCardRedeemed(StreamlootsCard card)
		{
			CardDto cardDto = new CardDto(card);
			ShowCardEvent(cardDto);
			OnCardRedeemed(this, new CardEventArgs(cardDto));
	}

		private void ProcessChestPurchase(JObject cardObject)
		{
			StreamlootsPurchase purchase = GetPurchase(cardObject);
			ShowPurchase(purchase);
			
			//if (purchase != null)
			//{
			//	Console.WriteLine("Purchase By: " + purchase.Purchaser);
			//	Console.WriteLine("Gifted to: " + ((!string.IsNullOrEmpty(purchase.Recipient)) ? purchase.Recipient : "NONE"));
			//	Console.WriteLine("Quantity: " + purchase.Quantity);
			//	Console.WriteLine("Chest Image: " + purchase.imageUrl);
			//	Console.WriteLine("Chest Sound: " + purchase.soundUrl);
			//	Console.WriteLine();
			//}
		}

		private void ProcessCardRedemption(JObject cardObject)
		{
			StreamlootsCard card = GetCard(cardObject);
			ShowCardRedeemed(card);

			//if (card != null)
			//{
			//	Console.WriteLine("Card Name: " + card.CardName);
			//	Console.WriteLine("Card Image: " + card.imageUrl);
			//	Console.WriteLine("Card Sound: " + card.soundUrl);
			//	Console.WriteLine("Redeemed By: " + card.UserName);
			//	Console.WriteLine("Message: " + card.message);
			//	Console.WriteLine("Secret: " + card.GetField("secret"));
			//	Console.WriteLine();
			//}
		}

		private static T GetStreamlootsObject<T>(JObject cardObject)
		{
			return cardObject["data"].ToObject<T>();
		}

		private static StreamlootsPurchase GetPurchase(JObject cardObject)
		{
			return GetStreamlootsObject<StreamlootsPurchase>(cardObject);
		}

		private static StreamlootsCard GetCard(JObject cardObject)
		{
			StreamlootsCard streamlootsCard = GetStreamlootsObject<StreamlootsCard>(cardObject);
			streamlootsCard.Initialize();
			return streamlootsCard;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// Dispose managed state (managed objects).
					cancellationTokenSource.Dispose();
					if (webRequest != null)
					{
						webRequest.Abort();
						webRequest = null;
					}
					if (responseStream != null)
					{
						responseStream.Close();
						responseStream = null;
					}
				}

				// Free unmanaged resources (unmanaged objects) and override a finalizer below.
				// Set large fields to null.

				disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion
	}
}
