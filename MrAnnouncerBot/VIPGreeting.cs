
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

                default:
                    GreetVip();
                    break;

            }
        }

        private void GreetVip()
        {
            switch (new Random().Next(43))
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
                case 41:
                    Greeting = $"They are no Rory Becker, but we are still glad you are here @{DisplayName}!";
                    break;
                case 42:
                    Greeting = $"They are no Wil Bennett, but we are still glad you are here @{DisplayName}!";
                    break;
            }
        }


        #region SuperVIP Greetings
        private void Greet_SurlyDev()
        {
            switch (new Random().Next(11))
            {
                case 0:
                    Greeting = $"Look out Mark!  @{DisplayName} just showed up!";
                    break;
                case 1:
                    Greeting = $"Oh no!  @{DisplayName} is here!";
                    break;
                case 2:
                    Greeting = $"Watch out, Mark! (@{DisplayName} is here)";
                    break;
                case 3:
                    Greeting = $"Look out, kids!  @{DisplayName} is here!";
                    break;
                case 4:
                    Greeting = $"Everyone to the lifeboats (me first)!  @{DisplayName} is here!";
                    break;
                case 5:
                    Greeting = $"Mark, you'd better clean up this code!  @{DisplayName} is in the house!";
                    break;
                case 6:
                    Greeting = $"Break out the drone-swatter!  @{DisplayName} just showed up!";
                    break;
                case 7:
                    Greeting = $"Drat! We almost made a show without him! But alas, no,  @{DisplayName} just sauntered in!";
                    break;
                case 8:
                    Greeting = $"He does one PR on GitHub and he thinks he's an open-source contributor.  @{DisplayName} fork off ;) Let's just hope he never clones himself.";
                    break;
                case 9:
                    Greeting = $"Drone, drone, drone. That's all  @{DisplayName} ever does. Not talking about the game; I'm sayin' that guy never shuts his trap.";
                    break;
                default:
                    GreetVip();
                    break;
            }
        }

        private void Greet_CopperBeardy()
        {
            switch (new Random().Next(2))
            {
                case 0:
                    Greeting = $"You should checkout @{DisplayName}, he has a stream too, it's not bad. I mean it's not good either, but definitely go and check it out. Not NOW! I mean LATER !!!";
                    break;
                default:
                    GreetVip();
                    break;
            }
        }

        private void Greet_JTsom()
        {
            switch (new Random().Next(3))
            {
                case 0:
                    Greeting = $"He likes to fly low and slow, it's @{DisplayName}!";
                    break;
                case 1:
                    Greeting = $"DUCK MARK! @{DisplayName} is flying low again!";
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

        private void Greet_Phrakberg()
        {
            switch (new Random().Next(2))
            {
                case 0:
                    Greeting = $"Hey Mark! here's some ice for your drink, oh no @{DisplayName} just flew into it!";
                    break;
                default:
                    GreetVip();
                    break;
            }
        }

        private void Greet_sxPositive()
        {
            switch (new Random().Next(4))
            {
                case 0:
                    Greeting = $"Hide your wallet Mark!  @{DisplayName} just showed up!";
                    break;
                case 1:
                    Greeting = $"Oh no!  @{DisplayName} is here! oh YES, I mean, OH YES !!! @{DisplayName} is here!";
                    break;
                case 2:
                    Greeting = $"Mark! Your L'il cutie is here, ready to kick ... oh wait, that's that other stream. Hi (@{DisplayName}.";
                    break;
                case 3:
                    Greeting = $"Here she is ladies and gentlemen, the lady that tries to tame this bundle of craziness, i-i-i-i-i-i-i-i-i-i-i-t-t-t-t-s-s-s-s-s-s-s (@{DisplayName}.";
                    break;
                default:
                    GreetVip();
                    break;
            }
        }

        private void Greet_BaldBeardedBuilder()
        {
            switch (new Random().Next(7))
            {
                case 0:
                    Greeting = $"Hide your wallet Mark!  @{DisplayName} just showed up!";
                    break;
                case 1:
                    Greeting = $"Us bald, bearded builders gotta stick together, and here's JUST the guy, it's @{DisplayName}";
                    break;
                case 2:
                    Greeting = $"Who would have thought there were TWO awesome bald, bearded builders on Twitch. Hi @{DisplayName}";
                    break;
                case 3:
                    Greeting = $"He's bald, he's bearded, he's a builder, he's not called Mark, I'm talking about @{DisplayName}";
                    break;
                case 4:
                    Greeting = $"You should checkout @{DisplayName}, he has a stream too, it's not bad. I mean it's not good either, but definitely go and check it out. Not NOW! I mean LATER !!!";
                    break;
                case 5:
                    Greeting = $"I always said Mark has a magnetic personality, and now he's attracted another bald, bearded builder. You could do with a little colour in yours though @{DisplayName}, and by colour I don't mean that shade of grey that is starting to make you look a little distinguished.";
                    break;
                default:
                    GreetVip();
                    break;
            }
        }

        private void Greet_CodeBaseAlpha()
        {
            switch (new Random().Next(2))
            {
                case 0:
                    Greeting = $"You should checkout @{DisplayName}, he has a stream too, it's not bad. I mean it's not good either, but definitely go and check it out. Not NOW! I mean LATER !!!";
                    break;
                default:
                    GreetVip();
                    break;
            }
        }


        private void Greet_TehPudding()
        {
            switch (new Random().Next(3))
            {
                case 0:
                    Greeting = $"You know what would go well with this sweet, sweet, stream Mark?, Some pudding, just NOT A TON OF IT @{DisplayName}!";
                    break;
                case 1:
                    Greeting = $"Hey @{DisplayName} you're right on time, your segment is just coming up now!";
                    break;
                default:
                    GreetVip();
                    break;
            }
        }

        #endregion
    }

}

