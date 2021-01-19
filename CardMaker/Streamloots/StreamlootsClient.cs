using System;
using Newtonsoft;
using Newtonsoft.Json;
using RestClient.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using DndCore;
using Streamloots;
using System.Collections.Generic;

namespace CardMaker
{
	public class StreamlootsClient
	{
		static HttpClient client = new HttpClient();
		const string STR_FirstDeckId = "5fb683dd69cff000391be2b2";
		const string STR_DragonHumpersSlug = "dragonhumpers";

		static StreamlootsClient()
		{
		}

		public StreamlootsClient(MySecureString bearerToken)
		{
			client.DefaultRequestHeaders.Add("authorization", $"Bearer {bearerToken.GetStr()}");
			//TestApi();
		}

		List<SetCardViewModel> GetNewCards()
		{
			List<SetCardViewModel> result = new List<SetCardViewModel>();
			SetCardViewModel setCardViewModel = new SetCardViewModel();
			setCardViewModel.name = "My new cool card";
			result.Add(setCardViewModel);
			return result;
			//setCardViewModel.actionType = 
		}
		List<SetCardUpdateViewModel> GetNewCards(SetCardViewModel prototype)
		{
			List<SetCardUpdateViewModel> result = new List<SetCardUpdateViewModel>();
			SetCardUpdateViewModel updatedTestCard = new SetCardUpdateViewModel(prototype);
			updatedTestCard.name = "My next SUPER COOL card";
			updatedTestCard.dropLimit = null;
			updatedTestCard.description = "This is my description.";
			updatedTestCard.descriptionDetailed = "This is my descriptionDetailed.";
			updatedTestCard.fragments = null;
			updatedTestCard.redemptionLimit = new RedemptionLimit();
			result.Add(updatedTestCard);
			return result;
			//setCardViewModel.actionType = 
		}
		async void TestApi()
		{
			await TestAddCard();
			//List<SetCardViewModel> cardsInFirstDeck = await GetCardsInDeck(STR_FirstDeckId);
			//if (cardsInFirstDeck.Count > 0)
			//	await AddCards(STR_FirstDeckId, GetNewCards(cardsInFirstDeck[0]));

			//List<SetCardViewModel> cardsInFirstDeck = await GetCardsInDeck(STR_FirstDeckId);
			//await updateSet(STR_FirstDeckId, new UpdateSetViewModel() { name = "Starter Deck", craftableCards = true, @default = true, imageUrl = "https://static.wikia.nocookie.net/harrypotter/images/e/eb/PDS.png/revision/latest?cb=20140730215243" });

			//await GetSet(STR_FirstDeckId);

			//await CreateSet(new CreateSetViewModel() { name = "Second Deck", slug = "dragonhumpers", craftableCards = true, @default = true, imageUrl = "https://i.redd.it/63k6vcu21qtz.png" });
		}

		private async Task TestAddCard()
		{
			SetCollectionResult setCollectionResult = await GetSets();
			if (setCollectionResult.data.Count > 0)
			{
				SetCardViewModel firstSet = setCollectionResult.data[0];
				SetCardWithImageViewModel newCard = new SetCardWithImageViewModel();
				newCard.name = "My super card.";
				newCard.description = "This is my new card.";
				newCard.descriptionDetailed = "More info than you need.";
				newCard.order = 1;
				newCard.rarity = "COMMON";
				newCard.imageUrl = "https://cdn.streamloots.com/uploads/5ef69e223924ad00349d42b7/0cd9de9b-e81c-4c9e-9fd4-f45faf8df4cd.png";
				newCard.rarityCardProbability = 1;
				await AddCard(firstSet._id, newCard);
			}
		}

		public async Task<SetViewModel> GetSet(string setId)
		{
			if (string.IsNullOrWhiteSpace(setId))
				return null;
			string setAsStr = await client.GetStringAsync($"https://api.streamloots.com/sets/{setId}");
			return JsonConvert.DeserializeObject<SetViewModel>(setAsStr);
		}

		public async Task<SetViewModel> CreateSet(CreateSetViewModel setData)
		{
			ByteArrayContent byteContent = ToSerializedBytes(setData);
			//string serializedBytes = byteContent.ReadAsStringAsync().GetAwaiter().GetResult();
			HttpResponseMessage httpResponseMessage = await client.PostAsync($"https://api.streamloots.com/sets", byteContent);
			
			string result = await httpResponseMessage.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<SetViewModel>(result);
		}

		public async Task<List<SetCardViewModel>> GetCardsInDeck(string setId)
		{
			string content = await client.GetStringAsync($"https://api.streamloots.com/sets/{setId}/cards");
			return JsonConvert.DeserializeObject<List<SetCardViewModel>>(content);
		}

		public async Task<SetCollectionResult> GetSets()
		{
			string content = await client.GetStringAsync($"https://api.streamloots.com/sets?slug=dragonhumpers");
			return JsonConvert.DeserializeObject<SetCollectionResult>(content);
		}

		public async Task<SetCardViewModel> AddCard(string setId, SetCardUpdateViewModel newCard)
		{
			List<SetCardUpdateViewModel> newCards = new List<SetCardUpdateViewModel>();
			newCards.Add(newCard);
			List<SetCardViewModel> cards = await AddCards(setId, newCards);
			if (cards != null && cards.Count > 0)
				return cards[0];
			return null;
		}

		public async Task<List<SetCardViewModel>> AddCards(string setId, List<SetCardUpdateViewModel> newCards)
		{
			foreach (SetCardUpdateViewModel setCardUpdateViewModel in newCards)
			{
				setCardUpdateViewModel.SelfValidate();
			}
			ByteArrayContent byteContent = ToSerializedBytes(newCards);
			HttpResponseMessage httpResponseMessage = await client.PostAsync($"https://api.streamloots.com/sets/{setId}/cards", byteContent);
			string result = await httpResponseMessage.Content.ReadAsStringAsync();
			List<SetCardViewModel> cardsAdded = JsonConvert.DeserializeObject<SetCardResult>(result).cards;
			return cardsAdded;
		}

		public async Task UpdateSet(string setId, UpdateSetViewModel updateData)
		{
			ByteArrayContent byteContent = ToSerializedBytes(updateData);
			HttpResponseMessage httpResponseMessage = await client.PutAsync($"https://api.streamloots.com/sets/{setId}", byteContent);
			string result = httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
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

		/// <summary>
		/// Creates a new deck (a streamloots "set") with the specified name, and returns the setId for that deck.
		/// </summary>
		/// <param name="deckName">The name of the deck to create.</param>
		/// <returns>The setId of the new deck (streamloots "set")</returns>
		async Task<string> AddDeck(string deckName)
		{
			CreateSetViewModel setData = new CreateSetViewModel();
			setData.name = deckName;
			setData.slug = STR_DragonHumpersSlug;
			setData.craftableCards = true;
			setData.@default = false;
			setData.imageUrl = "https://static.streamloots.com/b355d1ef-d931-4c16-a48f-8bed0076401b/collections/collection_icon_001.png";
			SetViewModel setCardViewModel = await CreateSet(setData);
			return setCardViewModel._id;
		}

		public async Task<SetCardViewModel> AddCard(Card card)
		{
			if (card.ParentDeck == null)
			{
				System.Diagnostics.Debugger.Break();
				return null;
			}
			if (await GetSet(card.ParentDeck.SetId) == null)
				card.ParentDeck.SetId = await AddDeck(card.ParentDeck.Name);
			//return await AddCard(card.ParentDeck.SetId, card.ToSetCardUpdateViewModel());
			return await AddCard(card.ParentDeck.SetId, card.ToSetCardWithImageViewModel());
		}
	}
}
