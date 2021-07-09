using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TaleSpireCore
{
	public static class TaleSpireUtils
	{
		public static string GetName(CreatureBoardAsset creatureAsset)
		{
			if (creatureAsset.Creature != null)
				return creatureAsset.Creature.Name;
			return creatureAsset.name;
		}

		public static float HoursToNormalizedTime(double totalHours)
		{
			float normalizedTime = (float)(totalHours / 24.0 + 0.25);
			if (normalizedTime > 1)
				normalizedTime -= 1;
			return normalizedTime;
		}

		public static byte[] ReceiveAll(this Socket socket)
		{
			List<byte> buffer = new List<byte>();
			int sleeps = 0;
			while (socket.Available == 0 && sleeps < 3000)
			{
				System.Threading.Thread.Sleep(1);
				sleeps++;
			}
			while (socket.Available > 0)
			{
				byte[] currByte = new byte[1];
				int byteCounter = socket.Receive(currByte, currByte.Length, SocketFlags.None);

				if (byteCounter.Equals(1))
					buffer.Add(currByte[0]);
			}

			return buffer.ToArray();
		}
	}
}
