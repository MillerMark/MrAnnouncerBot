
using System;


/// <summary>
/// Want to contribute your own VIP greeting to the show?   Simply follow these steps
/// 1. Create a private method for your Greeting using the following outline inside of the SuperVIP Greetings region
/// 
///             private void Greet_YourTwitchUserName()
///             {
///                 switch (new Random().Next(2))  // <-- The number here should be equal to the total number of cases listed
///                 {
///                     case 0:
///                         Greeting = $"Hello @{DisplayName}!";
///                         break;
///                         
///                     default: 
///                         GreetVip();
///                         break;
///                 }
///             }
///             
/// 2. Add your name as a case inside of the GreetSuperVIP() method following this outline.
/// 
///         case "yourtwitchname": // <-- Make sure this is all lower case or the match will not work
///             Greet_YourTwitchName();
///             break;
///         
/// 3. Submit a Pull Request to Mark for approval.
/// 
/// NOTE : If you want to pull this out and test it outside of MrAnnouncerBot, simply copy this class to a new
/// Console App and in the main method type in 
/// 
///          Console.WriteLine(new VIPGreeting("YourTwitchName").Greeting);
///         
/// </summary>


namespace MrAnnouncerBot
{
	public class VIPGreeting
	{

		public string Greeting { get; set; }
		public string DisplayName { get; set; }

		public VIPGreeting(string displayName)
		{
			DisplayName = displayName;

			GreetSuperVIP();
		}

		private void GreetSuperVIP()
		{
			switch (DisplayName.ToLower())
			{
				case "surlydev":
					Greet_SurlyDev();
					break;

				case "legendairymoooo":
					Greet_LegendairyMoooo();
					break;

				case "will_bennet":
					Greet_Will_Bennet();
					break;

				case "codeman_codes":
					Greet_Codeman_Codes();
					break;

				case "sxpositive":
					Greet_sxPositive();
					break;

				case "codebasealpha":
					Greet_CodeBaseAlpha();
					break;

				case "copperbeardy":
					Greet_CopperBeardy();
					break;

				case "jtsom":
					Greet_JTsom();
					break;

				case "baldbeardedbuilder":
					Greet_BaldBeardedBuilder();
					break;

				case "phrakberg":
					Greet_Phrakberg();
					break;

				case "tehpudding":
					Greet_TehPudding();
					break;

				case "chaelcodes":
					Greet_ChaelCodes();
					break;

				default:
					GreetVip();
					break;

			}
		}

		private void GreetVip()
		{
			switch (new Random().Next(41))
			{
				case 0:
					Greeting = $"@{DisplayName} is in the house!";
					break;
				case 1:
					Greeting = $"@{DisplayName} has arrived!";
					break;
				case 2:
					Greeting = $"Welcome @{DisplayName}!";
					break;
				case 3:
					Greeting = $"Everybody say Hi to @{DisplayName}!";
					break;
				case 4:
					Greeting = $"Greetings @{DisplayName}!";
					break;
				case 5:
					Greeting = $"Hello @{DisplayName}! So glad you are here!";
					break;
				case 6:
					Greeting = $"OMG! @{DisplayName} is here!";
					break;
				case 7:
					Greeting = $"A warm welcome to @{DisplayName}, who is NOW in the house!";
					break;
				case 8:
					Greeting = $"Hey @{DisplayName}! So happy you could make it!";
					break;
				case 9:
					Greeting = $"Look out kids! Here comes @{DisplayName}!";
					break;
				case 10:
					Greeting = $"Everybody stand back! @{DisplayName} is here!";
					break;
				case 11:
					Greeting = $"Yay! My favorite (@{DisplayName}) just got here!";
					break;
				case 12:
					Greeting = $"Fantastic! @{DisplayName} has entered the building.";
					break;
				case 13:
					Greeting = $"Hey! Say hello to our friend @{DisplayName}!";
					break;
				case 14:
					Greeting = $"Welcome! Welcome! Welcome! (talking to you, @{DisplayName})";
					break;
				case 15:
					Greeting = $"Good news, kids: @{DisplayName} is in the house!";
					break;
				case 16:
					Greeting = $"Thank God you're here, @{DisplayName}. Now we can finally write some decent code.";
					break;
				case 17:
					Greeting = $"Gentle folk of the chat room, I give you, the incredible, the amazing, the mind-blowingly fantastic, it's.... @{DisplayName}!!!!!";
					break;
				case 18:
					Greeting = $"Hey @{DisplayName}! How are you doing today?";
					break;
				case 19:
					Greeting = $"Everybody relax! @{DisplayName} is here. Everything's gonna be okay.";
					break;
				case 20:
					Greeting = $"We can finally calm down, people! @{DisplayName} is here!";
					break;
				case 21:
					Greeting = $"Wassup @{DisplayName}???";
					break;
				case 22:
					Greeting = $"I'm so psyched that @{DisplayName} is here. That means this show is gonna be awesome!";
					break;
				case 23:
					Greeting = $"Yo @{DisplayName}!";
					break;
				case 24:
					Greeting = $"Hey @{DisplayName}!";
					break;
				case 25:
					Greeting = $"Hi @{DisplayName}!";
					break;
				case 26:
					Greeting = $"Hello @{DisplayName}!";
					break;
				case 27:
					Greeting = $"Yo yo yo! It's @{DisplayName}!";
					break;
				case 28:
					Greeting = $"Yes! It's @{DisplayName}!";
					break;
				case 29:
					Greeting = $"It's @{DisplayName}!";
					break;
				case 30:
					Greeting = $"@{DisplayName}! Excellent.";
					break;
				case 31:
					Greeting = $"@{DisplayName}! Fantastic to have you here.";
					break;
				case 32:
					Greeting = $"@{DisplayName}! Yes! Exactly what we need right now.";
					break;
				case 33:
					Greeting = $"@{DisplayName}! Now the stream's gonna be awesome.";
					break;
				case 34:
					Greeting = $"@{DisplayName}! Buckle up, kids. This show is gonna rock now.";
					break;
				case 35:
					Greeting = $"Whoa kids! @{DisplayName} is in the house!";
					break;
				case 36:
					Greeting = $"Yo @{DisplayName}! How's it going?";
					break;
				case 37:
					Greeting = $"Hey @{DisplayName}! How are you doing?";
					break;
				case 38:
					Greeting = $"Hi @{DisplayName}! Glad you are here.";
					break;
				case 39:
					Greeting = $"Greetings @{DisplayName}! Great to see you.";
					break;
				case 40:
					Greeting = $"Greetings @{DisplayName}!";
					break;
			}
		}


		#region SuperVIP Greetings

		private void Greet_SurlyDev()
		{
			switch (new Random().Next(14))
			{
				case 0:
					Greeting = $"Look out Mark! @{DisplayName} just showed up!";
					break;
				case 1:
					Greeting = $"Oh no! @{DisplayName} is here!";
					break;
				case 2:
					Greeting = $"Watch out, Mark! (@{DisplayName} is here)";
					break;
				case 3:
					Greeting = $"Look out, kids! @{DisplayName} is here!";
					break;
				case 4:
					Greeting = $"Everyone to the lifeboats (me first)! @{DisplayName} is here!";
					break;
				case 5:
					Greeting = $"Mark, you'd better clean up this code! @{DisplayName} is in the house!";
					break;
				case 6:
					Greeting = $"Break out the drone-swatter! @{DisplayName} just showed up!";
					break;
				case 7:
					Greeting = $"Drat! We almost made a show without him! But alas, no, @{DisplayName} just sauntered in!";
					break;
				case 8:
					Greeting = $"He does one PR on GitHub and he thinks he's an open-source contributor. @{DisplayName} fork off ;) Let's just hope he never clones himself.";
					break;
				case 9:
					Greeting = $"Drone, drone, drone. That's all @{DisplayName} ever does. Not talking about the game; I'm sayin' that guy never shuts his trap.";
					break;
				case 10:
					Greeting = $"We're in big trouble, kids! @{DisplayName} is here.";
					break;
				case 11:
					Greeting = $"Somebody call the authorities! @{DisplayName} just burst in!";
					break;
				case 12:
					Greeting = $"Well, the show *was* going great until @{DisplayName} walked in!";
					break;
				default:
					GreetVip();
					break;
			}
		}

        private void Greet_ChaelCodes()
        {
            switch (new Random().Next(4))
            {
                case 0:
                    Greeting = $"Heya @{DisplayName} I hope you are sat with your feet up enjoying some CAT SNUGGLE TIME";
                    break;
                case 1:
                    Greeting = $"Hi @{DisplayName} I hope Perl and Ruby are snuggled up watching with you.";
                    break;
                case 2:
                    Greeting = $"https://clips.twitch.tv/CarefulExuberantNightingaleBleedPurple";
                    break;
                default:
                    GreetVip();
                    break;

            }
        }


		private void Greet_CopperBeardy()
		{
			switch (new Random().Next(7))
			{
				case 0:
					Greeting = $"Kids, you should checkout @{DisplayName}, he has an awesome stream!. Definitely check it out. Just not right now. Maybe later, when he's actually streaming.";
					break;
				case 1:
					Greeting = $"Hey, @{DisplayName}, is here. I hope his CodeRushed sunflower is still growing.";
					break;
				case 2:
					Greeting = $"Our day just got brighter - @{DisplayName} is here!";
					break;
				case 3:
					Greeting = $"Sunflowers and @{DisplayName}! Just what we needed.";
					break;
				case 4:
					Greeting = $"@{DisplayName} and his amazing sunflowers are in the house! Awesome!";
					break;
				case 5:
					Greeting = $"Look what's growing in our garden. It's @{DisplayName}!";
					break;
				default:
					GreetVip();
					break;
			}
		}

		private void Greet_JTsom()
		{
			switch (new Random().Next(8))
			{
				case 0:
					Greeting = $"He likes to fly low and slow, it's @{DisplayName}!";
					break;
				case 1:
					Greeting = $"Duck, Mark! @{DisplayName} is flying low again!";
					break;
				case 2:
					Greeting = $"Look out, Mark! Here comes @{DisplayName}!";
					break;
				case 3:
					Greeting = $"Look at all those white fluffy clouds... And who's flying in? It's @{DisplayName}!";
					break;
				case 4:
					Greeting = $"Look, up in the sky. It's a bird. It's a plane. It's @{DisplayName}!";
					break;
				case 5:
					Greeting = $"Somebody revoke this guy's pilot license. @{DisplayName} is in the house!";
					break;
				case 6:
					Greeting = $"This guy is flying way too low. Mark, watch out for that wing! It's @{DisplayName}!";
					break;
				default:
					GreetVip();
					break;
			}
		}


		private void Greet_Phrakberg()
		{
			switch (new Random().Next(2))
			{
				case 0:
					Greeting = $"Hey kids, here's some ice for your drink. Oh no! @{DisplayName} just crashed into it!";
					break;
				case 1:
					Greeting = $"Let's see if he can miss the iceberg this time.... Nope. @{DisplayName} hit it again!";
					break;
				case 2:
					Greeting = $"Pull up! Pull up! Pull up! Nooooo! It's @{DisplayName}!";
					break;
				case 3:
					Greeting = $"You know, it's not about trying to *miss* the iceberg. It's about trying to *hit* it. Nice aim, @{DisplayName}!";
					break;
				case 4:
					Greeting = $"Is it getting chilly in here? Look out, here comes @{DisplayName}!";
					break;
				case 5:
					Greeting = $"Watch out for that iceberg! Everyone say hello to @{DisplayName}!";
					break;
				default:
					GreetVip();
					break;
			}
		}

		private void Greet_sxPositive()
		{
			switch (new Random().Next(6))
			{
				case 0:
					Greeting = $"Mark, hide your wallet! @{DisplayName}'s here!";
					break;
				case 1:
					Greeting = $"Oh no! @{DisplayName} is here! Erm... Oh YES, I mean, OH YES!!! @{DisplayName} is here!";
					break;
				case 2:
					Greeting = $"Hey Mark! Your L'il Cutie is here, ready to kick some... Oh wait, that's that *other* stream (Twitch.tv/DragonHumpers). Hi (@{DisplayName}.";
					break;
				case 3:
					Greeting = $"Here she is, ladies and gentlemen, the only hottie brave enough to tame this bundle of crazy, iiiiiiiiiiiiiiiiiiiiiiiiiiiits... @{DisplayName}!";
					break;
				case 4:
					Greeting = $"She's amazing! She's brilliant! She's beautiful! Kids, say hello to @{DisplayName}!";
					break;
				default:
					GreetVip();
					break;
			}
		}

		private void Greet_BaldBeardedBuilder()
		{
			if (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday)
			{
				switch (new Random().Next(9))
				{
					case 0:
						Greeting = $"It's TACO TUESDAYS! @{DisplayName} is in the house!";
						break;
					case 1:
						Greeting = $"Tacos are for Tuesdays! And @{DisplayName} is here!";
						break;
					case 2:
						Greeting = $"Today is Tuesday. You know what that means. It's low-flying @{DisplayName}!";
						break;
					case 3:
						Greeting = $"T stands for Taco. T stands for Tuesday. And T stands for Terrific. And BBB stands for @{DisplayName}!";
						break;
					case 4:
						Greeting = $"Mmm. It's Tuesday and I'm hungry for Tacos. Good thing @{DisplayName} just flew in.";
						break;
					case 5:
						Greeting = $"Guess what day it is, kids! @{DisplayName} is in the house!";
						break;
					case 6:
						Greeting = $"Wait. Is today Tuesday? Is that a Taco? But why does it have arms and legs? Everyone say hi to our sour cream-filled friend, @{DisplayName}!";
						break;
					case 7:
						Greeting = $"Every day is Taco Tuesday at this guy's house. It's @{DisplayName}!";
						break;
					default:
						GreetVip();
						break;
				}
				return;
			}
			switch (new Random().Next(9))
			{
				case 0:
					Greeting = $"Watch out Mark - @{DisplayName} is flyin' low and he's filled with sour cream!";
					break;
				case 1:
					Greeting = $"Bald bearded builders, we gotta stick together, and speaking of sticky, here's JUST the guy, it's @{DisplayName}";
					break;
				case 2:
					Greeting = $"Who would have thought there were TWO incredibly handsome bald bearded builders on Twitch. It's @{DisplayName}!";
					break;
				case 3:
					Greeting = $"He's bald, he's bearded, and he's a builder, and I'm not talking about Mark. I'm talking about this guy --> it's @{DisplayName}!";
					break;
				case 4:
					Greeting = $"Go checkout @{DisplayName}'s stream, kids. This guy is amazing. Trust me. Mark wants to be as good as Michael when he grows up.";
					break;
				case 5:
					Greeting = $"Hey @{DisplayName}, I was thinking maybe you could use a little color in your beard. It could cover up that streak of grey that's starting to make you look a little too distinguished.";
					break;
				case 6:
					Greeting = $"@{DisplayName}, I was thinking maybe you could use a little color in your beard. It could cover up that streak of grey that's starting to make you look a little too distinguished.";
					break;
				case 7:
					Greeting = $"Look! Up in the sky! Is that a bird? A plane? No, it's @{DisplayName}!";
					break;
				default:
					GreetVip();
					break;
			}
		}

		private void Greet_CodeBaseAlpha()
		{
			switch (new Random().Next(6))
			{
				case 0:
					Greeting = $"@{DisplayName} has arrived from hyperspace!";
					break;
				case 1:
					Greeting = $"I always like watching Mark try to dance during @{DisplayName}'s fanfare.";
					break;
				case 2:
					Greeting = $"Kids, @{DisplayName} is in the house. Be sure to check out his live coding stream here on Twitch.";
					break;
				case 3:
					Greeting = $"@{DisplayName} has teleported into the building.";
					break;
				case 4:
					Greeting = $"I like SciFi and I like this guy. It's @{DisplayName} in the house!";
					break;
				default:
					GreetVip();
					break;
			}
		}


		private void Greet_TehPudding()
		{
			switch (new Random().Next(8))
			{
				case 0:
					Greeting = $"You know what would go well with this sweet, sweet, stream?, Some pudding. Just NOT A TON OF IT @{DisplayName}!";
					break;
				case 1:
					Greeting = $"Hey @{DisplayName} you're right on time, cause your segment is starting now!";
					break;
				case 2:
					Greeting = $"You know what we need right about now? It's @{DisplayName}!";
					break;
				case 3:
					Greeting = $"It's his stream now. @{DisplayName}!";
					break;
				case 4:
					Greeting = $"Look who just took over Mark's show. @{DisplayName} is in the house!";
					break;
				case 5:
					Greeting = $"Really? You think dumping a vat of pudding on Mark is the right thing to do right now? Just look at that mess you made, @{DisplayName}!";
					break;
				case 6:
					Greeting = $"Uh oh. @{DisplayName} is here to hijack the show!";
					break;
				default:
					GreetVip();
					break;
			}
		}

		private void Greet_LegendairyMoooo()
		{
			switch (new Random().Next(4))
			{
				case 0:
					Greeting = $"That's right little guy, you better run...  It's @{DisplayName}!";
					break;
				case 1:
					Greeting = $"To err is Human. To moo, Bovine.  @{DisplayName} is in the house!";
					break;
				case 2:
					Greeting = $"Things are about to get Legen ...  wait for it...  DAIRY!  That's right boys and girls, grab your sippy cups and prepare for some wholesome coding fun.  @{DisplayName} is in da house";
					break;
				default:
					GreetVip();
					break;
			}
		}

		private void Greet_Will_Bennet()
		{
			switch (new Random().Next(3))
			{
				case 0:
					Greeting = $"You can trust his code. His principles are so SOLID he has DRY dreams at night... @{DisplayName} is in da house";
					break;
				case 1:
					Greeting = $"Oh hi @{DisplayName} nice of you to join us today";
					break;
				default:
					GreetVip();
					break;

			}
		}

		private void Greet_Codeman_Codes()
		{
			switch (new Random().Next(10))
			{
				case 0:
					Greeting = $"Look! Behind the desk! It's an int! It's a string! It's @{DisplayName}!";
					break;
				case 1:
					Greeting = $"This is his gift, his curse. Who am he? @{DisplayName}";
					break;
				case 2:
					Greeting = $"Someone must have ripped the 'Q' section out of his dictionary, 'cause He don't know the meaning of the word 'quit'. Here comes @{DisplayName}!";
					break;
				case 3:
					Greeting = $"For there must always, always be a @{DisplayName}. And some day, when he's needed, we will see him again.";
					break;
				case 4:
					Greeting = $"Coders Compile! It's @{DisplayName}!";
					break;
				case 5:
					Greeting = $"Never fear! @{DisplayName} is here!";
					break;
				case 6:
					Greeting = $"Some say he speaks gibberish. But he actually speaks in code. @{DisplayName} is in da house!";
					break;
				case 7:
					Greeting = $"Not all heroes wear capes, but all heroes know index starts at 0. Welcome @{DisplayName}!";
					break;
				case 8:
					Greeting = $"His IntelliSense must be tingling! I see @{DisplayName}!";
					break;
				default:
					GreetVip();
					break;

			}
		}

		#endregion
	}

}

