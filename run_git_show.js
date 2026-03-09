const {execSync} = require('child_process');

try {
  const output = execSync('git show be0e1ceec39a7caf0b26b579a22762f2109a7cd8', {
    cwd: 'D:\\Drive\\DX\\Twitch\\CodeRushed\\MrAnnouncerBot',
    maxBuffer: 500000,
    encoding: 'utf-8'
  });
  console.log(output);
} catch (error) {
  console.error('Error:', error.message);
  if (error.stdout) {
    console.log('STDOUT:', error.stdout.toString());
  }
  if (error.stderr) {
    console.log('STDERR:', error.stderr.toString());
  }
}
