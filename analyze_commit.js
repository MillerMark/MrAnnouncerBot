const fs = require('fs');
const zlib = require('zlib');
const path = require('path');

const baseDir = r`D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot`;
process.chdir(baseDir);

const COMMIT = 'be0e1ceec39a7caf0b26b579a22762f2109a7cd8';

function readGitObject(sha) {
    const objPath = path.join('.git', 'objects', sha.substring(0, 2), sha.substring(2));
    if (!fs.existsSync(objPath)) {
        return [null, null];
    }
    const compressed = fs.readFileSync(objPath);
    const decompressed = zlib.uncompressSync(compressed);
    const nullIdx = decompressed.indexOf(0);
    const header = decompressed.toString('ascii', 0, nullIdx);
    const objType = header.split(' ')[0];
    const content = decompressed.subarray(nullIdx + 1);
    return [objType, content];
}

function parseCommit(content) {
    const text = content.toString('utf-8', { errors: 'replace' });
    const lines = text.split('\n');
    const result = {};
    let msgLines = [];
    let inMsg = false;
    
    for (const line of lines) {
        if (inMsg) {
            msgLines.push(line);
        } else if (line === '') {
            inMsg = true;
        } else if (line.startsWith('tree ')) {
            result.tree = line.substring(5).trim();
        } else if (line.startsWith('parent ')) {
            result.parent = line.substring(7).trim();
        } else if (line.startsWith('author ')) {
            result.author = line.substring(7).trim();
        } else if (line.startsWith('committer ')) {
            result.committer = line.substring(10).trim();
        }
    }
    result.message = msgLines.join('\n').trim();
    return result;
}

console.log('='.repeat(80));
console.log(`GIT COMMIT ANALYSIS: ${COMMIT}`);
console.log('='.repeat(80));

const [objType, content] = readGitObject(COMMIT);
if (content === null) {
    console.log(`ERROR: Could not find commit object ${COMMIT}`);
    process.exit(1);
}

console.log(`\nObject type: ${objType}`);
const commitData = parseCommit(content);

console.log(`Tree:       ${commitData.tree || 'N/A'}`);
console.log(`Parent:     ${commitData.parent || 'N/A'}`);
console.log(`Author:     ${commitData.author || 'N/A'}`);
console.log(`Committer:  ${commitData.committer || 'N/A'}`);
console.log(`Message:    ${commitData.message || 'N/A'}`);
