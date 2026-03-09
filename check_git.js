const {execSync} = require('child_process');
const opts = {cwd: 'D:/Dropbox/DX/Twitch/CodeRushed/MrAnnouncerBot', encoding: 'utf8'};

console.log('=== git log ===');
console.log(execSync('git log --oneline -20', opts));

console.log('=== git status ===');
console.log(execSync('git status', opts));

console.log('=== git log for key files ===');
try {
  console.log(execSync('git log --oneline -10 -- BotCoreNet/AllViewers.cs BotCoreNet/Viewer.cs "MrAnnouncerBot/MrAnnouncerBot.cs"', opts));
} catch(e) {
  console.log(e.message);
}
