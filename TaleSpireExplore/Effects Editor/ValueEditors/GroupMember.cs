using TaleSpireCore;

namespace TaleSpireExplore
{
	public class GroupMember
	{
		public CreatureBoardAsset CreatureBoardAsset { get; set; }
		public string Name { get; set; }

		public GroupMember(CreatureBoardAsset creatureBoardAsset)
		{
			CreatureBoardAsset = creatureBoardAsset;
			Name = creatureBoardAsset.GetOnlyCreatureName().Trim();
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
