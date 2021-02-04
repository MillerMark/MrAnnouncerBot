using System;
using DndCore;
using System.Collections.Generic;
using System.Linq;
using Streamloots;
using System.Timers;
using Imaging;
using Newtonsoft.Json;

namespace DHDM
{
	public class ViewerQueueDto: CardBaseCommandDto
	{
		const int INT_MaxViewersInQueueToSend = 20;
		public List<ViewerRollDto> ViewerRollDto { get; set; }
		public ViewerQueueDto(Queue<DiceRoll> viewerRollQueue)
		{
			Command = "UpdateViewerRollQueue";
			ViewerRollDto = new List<ViewerRollDto>();

			int queuePosition = 0;
			foreach (DiceRoll diceRoll in viewerRollQueue)
			{
				ViewerRollDto viewerRollDto = new ViewerRollDto();
				if (diceRoll.DiceDtos != null && diceRoll.DiceDtos.Count > 0)
				{
					DiceDto diceDto = diceRoll.DiceDtos[0];
					viewerRollDto.Label = diceDto.Label;
					viewerRollDto.RollId = diceRoll.RollID;
					viewerRollDto.UserName = diceRoll.Viewer;
					viewerRollDto.FontColor = diceDto.BackColor;
					viewerRollDto.OutlineColor = diceDto.FontColor;
					viewerRollDto.RollStr = GetRollStr(diceRoll.DiceDtos);
					viewerRollDto.QueuePosition = queuePosition;
					queuePosition++;
					ViewerRollDto.Add(viewerRollDto);
					if (queuePosition >= INT_MaxViewersInQueueToSend)
						return;
				}
			}
		}
		string GetRollStr(List<DiceDto> diceDtos)
		{
			Dictionary<int, int> diceSidesFound = new Dictionary<int, int>();

			foreach (DiceDto diceDto in diceDtos)
				if (diceSidesFound.ContainsKey(diceDto.Sides))
					diceSidesFound[diceDto.Sides] += diceDto.Quantity;
				else
					diceSidesFound.Add(diceDto.Sides, diceDto.Quantity);
			string result = string.Empty;
			const string commaSeparator = ", ";
			foreach (int key in diceSidesFound.Keys)
			{
				string dieStr = $"{diceSidesFound[key]}d{key}";
				result += dieStr + commaSeparator;
			}
			if (result.EndsWith(commaSeparator))
				result = result.Substring(0, result.Length - commaSeparator.Length);

			return result;
		}
	}
}
