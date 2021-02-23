using System;
using System.IO;
using System.Linq;
using SysPath = System.IO.Path;

namespace CardMaker
{
	public static class CardFactory
	{
		public static Random random = new Random();

		public static void SetRarity(Card card, int basePowerLevel, int actualPowerLevel)
		{
			int delta = Math.Abs(actualPowerLevel) - basePowerLevel;
			switch (delta)
			{
				case 1:
					card.Rarity = Rarity.Rare;
					break;
				case 2:
					card.Rarity = Rarity.Epic;
					break;
				case 3:
					card.Rarity = Rarity.Legendary;
					break;
				default:
					card.Rarity = Rarity.Common;
					break;
			}
		}
		static void QuickAddLayerDetails(Card card, string pngFile)
		{
			string layerName = Path.GetFileNameWithoutExtension(pngFile);

			int closeBracketPos = layerName.IndexOf("]");
			if (closeBracketPos > 0)
				layerName = layerName.Substring(closeBracketPos + 1).Trim();

			int caretSymbolPos = layerName.IndexOf("^");
			if (caretSymbolPos > 0)
				layerName = layerName.Substring(0, caretSymbolPos).Trim();

			int openParenPos = layerName.IndexOf("(");
			if (openParenPos > 0)
				layerName = layerName.Substring(0, openParenPos).Trim();
			card.GetLayerDetails(layerName);
		}

		private static void QuickAddCoreLayers(Card card, string stylePath)
		{
			string[] pngFiles = Directory.GetFiles(SysPath.Combine(Folders.Assets, stylePath), "*.png");
			foreach (string pngFile in pngFiles)
				QuickAddLayerDetails(card, pngFile);
		}

		private static void QuickAddBackgroundLayers(Card card, string stylePath)
		{
			const string backgroundFolderName = Folders.SharedBackAdornments;
			string basePath = SysPath.GetDirectoryName(stylePath);
			string backFolder = SysPath.Combine(Folders.Assets, basePath, backgroundFolderName);
			if (!Directory.Exists(backFolder))
				return;
			string[] pngFiles = Directory.GetFiles(backFolder, "*.png");
			foreach (string pngFile in pngFiles)
				QuickAddLayerDetails(card, pngFile);
		}

		private static void QuickAddForegroundLayers(Card card, string stylePath)
		{
			const string foregroundFolderName = Folders.TopBackAdornments;
			string basePath = SysPath.GetDirectoryName(stylePath);
			string foregroundFolder = SysPath.Combine(Folders.Assets, basePath, foregroundFolderName);
			if (!Directory.Exists(foregroundFolder))
				return;
			string[] pngFiles = Directory.GetFiles(foregroundFolder, "*.png");
			foreach (string pngFile in pngFiles)
				QuickAddLayerDetails(card, pngFile);
		}

		public static void QuickAddAllLayerDetails(Card card)
		{
			QuickAddBackgroundLayers(card, card.StylePath);
			QuickAddCoreLayers(card, card.StylePath);
			QuickAddForegroundLayers(card, card.StylePath);
		}

		public static void AddPlayerNpcRecipientField(CardData CardData, Card card, string itemName)
		{
			Field field = new Field(card) { Label = $"Player/NPC to receive this {itemName}:", Name = "recipient", ParentCard = card, Required = true, IsDirty = true };
			CardData.AllKnownFields.Add(field);
		}
	}
}

