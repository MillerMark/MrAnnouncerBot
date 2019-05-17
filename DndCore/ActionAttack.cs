namespace DndCore
{
	public class ActionAttack
	{
		string constrict;
		Monster Monster;

		public ActionAttack()
		{
		}

		public ActionAttack(Monster monster, string constrict)
		{
			Monster = monster;
			this.constrict = constrict;
		}
	}
}
