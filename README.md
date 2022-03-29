# MrAnnouncerBot
A Twitch bot written in C#

# Overview
This bot runs during the live twitch.tv/CodeRushed stream and its primary function is to allow people in the chat room to control Mark's electronic co-host, during the show.

# Phrases Mr. Announcer Bot Can Say
Mr. Announcer Bot has a vocabulary of over 100 phrases that include pre-rendered animations synched up to the words. To get Mr. Announcer Bot to say something, simply type the "!" symbol into the chat room followed by the shortcut command for the phrase.

For example, to get Mr. Announcer Bot to say "**Excellent!**", send the text "**!ex**" to the chat window.

## Levels
For the purposes of determining what you can say, Mr. Announcer Bot tracks viewer "levels". Levels are determined by past contributions, total time spent in the chat room (across all the episodes), and whether you are following/subscribing to the channel or not. It's a lot like one of those dungeon adventuring games. A level-72 wizard can smite you with a side-eye glance, while a level-0 wizard may be stuck fumbling with the spell casting book just to cast magic missiles.

Here is more about level qualifications and what you can say based on your level:

### Level 0

![](https://community.devexpress.com/blogs/markmiller/TwitchLevels/Level0.png)

Anyone in the chat room who is not yet following the CodeRushed channel is a Level 0 botcaster. Level 0 botcasters can say any of these phrases:

| Shortcut | Scene | Category |
|---|---|---|
| cr | CodeRush!* | Branding | 
| dx | DevExpress! | Branding | 
| ex | Excellent! | Good | 
| mark | Mark->* | Alert | 

### Level 1

![](https://community.devexpress.com/blogs/markmiller/TwitchLevels/Level1.png)

Want to jump-start your levels in the chat room? Follow us on Twitch at:

http://twitch.tv/CodeRushed

This will instantly take you to Level 1 and you gain these botcasting skills:

| Shortcut | Scene | Category |
|---|---|---|
| awe | Awesome! | Good | 
| aweCon | Awesome Contribution! | Chat | 
| b2code | Back to the Code! | Transition | 
| bfi | Brute Force and Ignorance! | Gung Ho | 
| bug | That's a BUG! | Debug | 
| bugsCode | Bugs in My Code! | Debug | 
| close1 | That was a close one! | Good | 
| coding | Coding! | Gung Ho | 
| compShip | It compiles - let's ship it! | Good | 
| constants | Constants! | Code | 
| constructors | Constructors! | Code | 
| debugger | Debugger! | Debug | 
| debugIt | Debug It! | Debug | 
| fixed | Fixed! | Good | 
| fixIt | Fix It! | Debug | 
| getClose | Getting Closer! | Good | 
| goodDesign | Good Design | Good | 
| goodQ | Good Question! | Chat | 
| greatQ | Great Question! | Chat | 
| greatSug | Great Suggestion! | Chat | 
| handlers | Event Handlers! | Code | 
| heAwe | He's Awesome! | Chat | 
| inspect | Inspect It! | Debug | 
| js | Javascript! | Code | 
| letsDoThis | Let's Do This! | Good | 
| niceClassName | Nice Class Name! | Code | 
| niceFunc | That's a nice function! | Good | 
| niceVarName | Nice Variable Name! | Good | 
| phew | Phew - That was close! | Good | 
| sadP | Sad Panda! | Sad | 
| setBreak | Set Breakpoint | Debug | 
| sheAwe | She's Awesome! | Chat | 
| theyAwe | They're Awesome! | Chat | 
| worked | That Worked! | Good | 

### Level 2

![](https://community.devexpress.com/blogs/markmiller/TwitchLevels/Level2.png)



| Shortcut | Scene | Category |
|---|---|---|
| askChat | Ask the chat room! | Chat | 
| bro | Cool Story Bro! | Bad | 
| codeDance | Let's make this code dance! | Gung Ho | 
| forgot | Forgot Something! | Bad | 
| knowWhatHappen | Does Anybody Know What's Happening? | Mystery | 
| nfn | Nice Function Name! | Code | 

### Level 3

![](https://community.devexpress.com/blogs/markmiller/TwitchLevels/Level3.png)



| Shortcut | Scene | Category |
|---|---|---|
| airbags | Check your airbags! | Gung Ho | 
| funny | That was funny! | Good | 
| hack | Hacked! | Bad | 
| notPretty | Won't be pretty! | Bad | 
| whatHappen | What happened? | Mystery | 
| wow | Wow. Just Wow. | Bad | 

### Level 4

![](https://community.devexpress.com/blogs/markmiller/TwitchLevels/Level4.png)



| Shortcut | Scene | Category |
|---|---|---|
| exactly | Exactly! | Good | 
| mop | Mother of Pearl! | Swear | 
| noSense | this doesn't make any sense | Mystery | 
| offRails | Going Off Rails! | Gung Ho | 
| toobad | Too Bad! | Sad | 
| unb | Unbelievable! | Mystery | 

### Level 5

![](https://community.devexpress.com/blogs/markmiller/TwitchLevels/Level5.png)



| Shortcut | Scene | Category |
|---|---|---|
| aye | Aye Aye Aye Yikesters! | Swear | 
| evenWork | Does This Even Work? | Mystery | 
| noo | Nooooo! | Bad | 
| ugly | Getting Ugly! | Bad | 
| wheelsOff | The Training Wheels Are Off! | Gung Ho | 

### Level 6

![](https://community.devexpress.com/blogs/markmiller/TwitchLevels/Level6.png)



| Shortcut | Scene | Category |
|---|---|---|
| buttercup | Buckle Up Buttercup! | Gung Ho | 
| danceBattle | Dance Battle! | Crazy | 
| face | Excuse your face sir! | Swear | 
| sickBurn | Sick Burn! | Bad | 
| stinks | This Stinks! | Bad | 

### Level 7

![](https://community.devexpress.com/blogs/markmiller/TwitchLevels/Level7.png)



| Shortcut | Scene | Category |
|---|---|---|
| chatRoomOnFire | Chat Room On Fire | Good | 
| fail | Fail! | Bad | 
| frogs | FrogsInCars | Good | 
| marksick | Mark is sick! | Bad | 
| mdkwhd | Disclaimer - Mark doesn't know what he's doing! | Disclaimer | 
| noJack | Disclaimer - Mark doesn't know Jack! | Disclaimer | 
| party | DH.Party1 | Good | 
| party2 | DH.Party2 | Good | 

### Level 10



| Shortcut | Scene | Category |
|---|---|---|
| party3 | DH.Party3 | Good | 
| ufo | UFO Crash | Good | 

### Level 11



| Shortcut | Scene | Category |
|---|---|---|
| sprinkles | DH.Sprinkles | Good | 

### Level 12



| Shortcut | Scene | Category |
|---|---|---|
| party4 | DH.Party4 | Good | 

### Level 14



| Shortcut | Scene | Category |
|---|---|---|
| party5 | DH.Party5 | Good | 

### Level 15



| Shortcut | Scene | Category |
|---|---|---|
| droneFail | Drone Failure | Bad | 
| oops | Oops! | Sorry | 
| sorry | Sorry-ish | Sorry | 
| urWelcome | you're welcome! | Conversation | 

### Level 16



| Shortcut | Scene | Category |
|---|---|---|
| party6 | DH.Party6 | Good | 

### Level 18



| Shortcut | Scene | Category |
|---|---|---|
| party7 | DH.Party7 | Good | 

### Level 20



| Shortcut | Scene | Category |
|---|---|---|
| party8 | DH.Party8 | Good | 

### Level 22



| Shortcut | Scene | Category |
|---|---|---|
| party9 | DH.Party9 | Good | 

### Level 24



| Shortcut | Scene | Category |
|---|---|---|
| party10 | DH.Party10 | Good | 

### Level 26



| Shortcut | Scene | Category |
|---|---|---|
| party11 | DH.Party11 | Good | 

