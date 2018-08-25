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

Anyone in the chat room who is not yet following the CodeRushed channel is a Level 0 botcaster. Level 0 botcasters can say any of these phrases:

| Shortcut | Scene | Category |
|---|---|---|
| cr | CodeRush!* | Branding | 
| dx | DevExpress! | Branding | 
| ex | Excellent! | Good | 
| mark | Mark->* | Alert | 

### Level 1

Want to jump start your levels in the chat room? Follow us on Twitch at:

http://twitch.tv/CodeRushed

This will instantly take you to Level 1 and you gain these botcasting skills:

| Shortcut | Scene | Category |
|---|---|---|
| aweCon | Awesome Contribution! | Chat | 
| goodQ | Good Question! | Chat | 
| greatQ | Great Question! | Chat | 
| greatSug | Great Suggestion! | Chat | 
| heAwe | He's Awesome! | Chat | 
| sheAwe | She's Awesome! | Chat | 
| theyAwe | They're Awesome! | Chat | 
| constants | Constants! | Code | 
| constructors | Constructors! | Code | 
| handlers | Event Handlers! | Code | 
| js | Javascript! | Code | 
| niceClassName | Nice Class Name! | Code | 
| bugsCode | Bugs in My Code! | Debug | 
| debugIt | Debug It! | Debug | 
| debugger | Debugger! | !c | 
| fixIt | Fix It! | Debug | 
| inspect | Inspect It! | Debug | 
| setBreak | Set Breakpoint | Debug | 
| bug | That's a BUG! | Debug | 
| awe | Awesome! | Good | 
| fixed | Fixed! | Good | 
| getClose | Getting Closer! | Good | 
| goodDesign | Good Design | Good | 
| compShip | It compiles - let's ship it! | Good | 
| letsDoThis | Let's Do This! | Good | 
| niceVarName | Nice Variable Name! | Good | 
| phew | Phew - That was close! | Good | 
| close1 | That was a close one! | Good | 
| worked | That Worked! | Good | 
| niceFunc | That's a nice function! | Good | 
| bfi | Brute Force and Ignorance! | Gung Ho | 
| coding | Coding! | Gung Ho | 
| bilsimser | Bil Simser rocks! | Rocks | 
| wilbennett | wilbennettRocks!.avi | Rocks | 
| sadP | Sad Panda! | Sad | 
| b2code | Back to the Code! | Transition | 

### Level 2



| Shortcut | Scene | Category |
|---|---|---|
| bro | Cool Story Bro! | Bad | 
| forgot | Forgot Something! | Bad | 
| askChat | Ask the chat room! | Chat | 
| codeDance | Let's make this code dance! | Gung Ho | 
| knowWhatHappen | Does Anybody Know What's Happening? | Mystery | 

### Level 3



| Shortcut | Scene | Category |
|---|---|---|
| hack | Hacked! | Bad | 
| notPretty | Won't be pretty! | Bad | 
| wow | Wow. Just Wow. | Bad | 
| airbags | Check your airbags! | Gung Ho | 
| whatHappen | What happened? | Mystery | 
| funny | That was funny! | Good | 

### Level 4



| Shortcut | Scene | Category |
|---|---|---|
| exactly | Exactly! | Good | 
| offRails | Going Off Rails! | Gung Ho | 
| noSense | this doesn't make any sense | Mystery | 
| unb | Unbelievable! | Mystery | 
| mop | Mother of Pearl! | Swear | 
| toobad | Too Bad! | Sad | 

### Level 5



| Shortcut | Scene | Category |
|---|---|---|
| ugly | Getting Ugly! | Bad | 
| noo | Nooooo! | Bad | 
| wheelsOff | The Training Wheels Are Off! | Gung Ho | 
| evenWork | Does This Even Work? | Mystery | 
| aye | Aye Aye Aye Yikesters! | Swear | 

### Level 6



| Shortcut | Scene | Category |
|---|---|---|
| sickBurn | Sick Burn! | Bad | 
| stinks | This Stinks! | Bad | 
| danceBattle | Dance Battle! | Crazy | 
| buttercup | Buckle Up Buttercup! | Gung Ho | 
| face | Excuse your face sir! | Swear | 

### Level 7



| Shortcut | Scene | Category |
|---|---|---|
| fail | Fail! | Bad | 
| marksick | Mark is sick! | Bad | 

### Level 15



| Shortcut | Scene | Category |
|---|---|---|
| noJack | Disclaimer - Mark doesn't know Jack! | Disclaimer | 
| mdkwhd | Disclaimer - Mark doesn't know what he's doing! | Disclaimer | 

