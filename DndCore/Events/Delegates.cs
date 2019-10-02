using System;
using System.Linq;

namespace DndCore
{
	public delegate void FeatureEventHandler(object sender, FeatureEventArgs ea);
	public delegate void AskEventHandler(object sender, AskEventArgs ea);
	public delegate void DndCoreExceptionEventHandler(object sender, DndCoreExceptionEventArgs ea);
	public delegate void RollDiceEventHandler(object sender, RollDiceEventArgs ea);
	public delegate void PlayerRollRequestEventHandler(object sender, PlayerRollRequestEventArgs ea);
	public delegate void DndSpellEventHandler(object sender, DndSpellEventArgs ea);
	public delegate void StateChangedEventHandler(object sender, StateChangedEventArgs ea);
	public delegate void TimeClockEventHandler(object sender, TimeClockEventArgs ea);
	public delegate void DndGameEventHandler(object sender, DndGameEventArgs ea);
	public delegate void DndCharacterEventHandler(object sender, DndCharacterEventArgs ea);
	public delegate void DndCreatureEventHandler(object sender, DndCreatureEventArgs ea);
	public delegate void ConditionsChangedEventHandler(object sender, ConditionsChangedEventArgs ea);
	public delegate void PlayerStateChangedEventHandler(object sender, PlayerStateEventArgs ea);
	public delegate void LevelChangedEventHandler(object sender, LevelChangedEventArgs ea);

}
