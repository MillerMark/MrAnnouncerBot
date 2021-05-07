namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Modes
		{
			public static void SwitchToSpectator()
			{
				LocalClient.SetLocalClientMode(ClientMode.Spectator);
			}

			public static void SwitchToGameMaster()
			{
				LocalClient.SetLocalClientMode(ClientMode.GameMaster);
			}

			public static void SwitchToPlayer()
			{
				LocalClient.SetLocalClientMode(ClientMode.Player);
			}
		}
	}
}
