namespace DndCore
{
	public class ActionAttack
	{
		private string constrict;
		private Monster Monster;

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
