#!/usr/bin/env python3
import zlib
import os
import struct
import sys

os.chdir(r'D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot')

commit_hash = 'be0e1ceec39a7caf0b26b579a22762f2109a7cd8'

# Read the git commit object
obj_path = f'.git/objects/be/0e1ceec39a7caf0b26b579a22762f2109a7cd8'
print(f"Looking for commit object at: {obj_path}")
print(f"File exists: {os.path.exists(obj_path)}")

if os.path.exists(obj_path):
    with open(obj_path, 'rb') as f:
        compressed = f.read()
        try:
            decompressed = zlib.decompress(compressed)
            content = decompressed.decode('utf-8', errors='replace')
            print("\n=== COMMIT OBJECT CONTENT ===\n")
            print(content)
            
            # Parse the commit object
            lines = content.split('\n')
            print("\n=== PARSED COMMIT ===")
            for line in lines[:10]:
                print(line)
        except Exception as e:
            print(f'Error decompressing: {e}')
            print(f'Raw bytes (first 100): {compressed[:100]}')
else:
    print(f'File not found!')
    # List what's in the objects directory
    obj_dir = '.git/objects/be'
    if os.path.exists(obj_dir):
        print(f"\nFiles in {obj_dir}:")
        for f in os.listdir(obj_dir):
            print(f"  {f}")
