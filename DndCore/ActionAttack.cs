namespace DndCore
{
	public class ActionAttack
	{
		private Monster Monster;
		private string constrict;

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
