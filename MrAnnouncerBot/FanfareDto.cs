using System;

namespace MrAnnouncerBot
{
	public class FanfareDto
	{
		public string DisplayName { get; set; }
		public double Fanfare1 { get; set; }
		public double Fanfare2 { get; set; }
		public double Fanfare3 { get; set; }
		public double Fanfare4 { get; set; }
		public int Count
		{
			get
			{
				if (Fanfare1 > 0)
					if (Fanfare2 > 0)
						if (Fanfare3 > 0)
							if (Fanfare4 > 0)
								return 4;
							else
								return 3;
						else
							return 2;
					else
						return 1;
				return 0;
			}
		}
		public FanfareDto()
		{

		}
		public double GetFanfareDuration(int fanfareIndex)
		{
			if (fanfareIndex == 1)
				return Fanfare1;
			if (fanfareIndex == 2)
				return Fanfare2;
			if (fanfareIndex == 3)
				return Fanfare3;
			if (fanfareIndex == 4)
				return Fanfare4;
			return 0;
		}
	}
}