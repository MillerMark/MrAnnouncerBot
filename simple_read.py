import zlib
import os

os.chdir(r'D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot')

obj_path = '.git/objects/be/0e1ceec39a7caf0b26b579a22762f2109a7cd8'

with open(obj_path, 'rb') as f:
    compressed = f.read()
    decompressed = zlib.decompress(compressed)
    content = decompressed.decode('utf-8', errors='replace')
    
    print("COMMIT OBJECT CONTENT:")
    print("=" * 80)
    print(content)
    print("=" * 80)
    
    # Also save to a file for inspection
    with open('commit_content.txt', 'w') as out:
        out.write(content)
    print("\nSaved to commit_content.txt")
