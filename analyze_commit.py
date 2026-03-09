#!/usr/bin/env python3
"""Read git commit object and show what changed"""
import zlib
import os
import struct
import hashlib
import subprocess

os.chdir(r'D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot')

commit_hash = 'be0e1ceec39a7caf0b26b579a22762f2109a7cd8'
obj_path = '.git/objects/be/0e1ceec39a7caf0b26b579a22762f2109a7cd8'

print("=" * 70)
print("READING GIT COMMIT OBJECT")
print("=" * 70)
print(f"\nCommit hash: {commit_hash}")
print(f"Object path: {obj_path}")
print(f"File exists: {os.path.exists(obj_path)}")

if os.path.exists(obj_path):
    print("\n" + "=" * 70)
    print("DECOMPRESSED COMMIT OBJECT CONTENT")
    print("=" * 70)
    
    with open(obj_path, 'rb') as f:
        compressed = f.read()
        decompressed = zlib.decompress(compressed)
        content = decompressed.decode('utf-8', errors='replace')
        print(content)
        
        # Parse the commit object to extract parent and tree
        print("\n" + "=" * 70)
        print("PARSED COMMIT DETAILS")
        print("=" * 70)
        
        lines = content.split('\n')
        tree_hash = None
        parent_hash = None
        author = None
        committer = None
        message = None
        
        # Parse header
        for i, line in enumerate(lines):
            if line.startswith('tree '):
                tree_hash = line.replace('tree ', '').strip()
                print(f"\nTree hash: {tree_hash}")
            elif line.startswith('parent '):
                parent_hash = line.replace('parent ', '').strip()
                print(f"Parent hash: {parent_hash}")
            elif line.startswith('author '):
                author = line
                print(f"Author: {author}")
            elif line.startswith('committer '):
                committer = line
                print(f"Committer: {committer}")
            elif line == '' and i > 0:
                message = '\n'.join(lines[i+1:])
                print(f"\nCommit message:\n{message}")
                break

print("\n" + "=" * 70)
print("USING GIT COMMANDS FOR DETAILED CHANGE INFORMATION")
print("=" * 70)

# Try to use git commands to get the actual diff
try:
    print("\n--- GIT SHOW --STAT ---")
    result = subprocess.run(
        ['git', 'show', commit_hash, '--stat'],
        capture_output=True,
        text=True,
        timeout=10
    )
    print(result.stdout)
    if result.stderr:
        print("STDERR:", result.stderr)
except Exception as e:
    print(f"Error running git show --stat: {e}")

try:
    print("\n--- GIT SHOW --NAME-ONLY ---")
    result = subprocess.run(
        ['git', 'show', commit_hash, '--name-only'],
        capture_output=True,
        text=True,
        timeout=10
    )
    print(result.stdout)
    if result.stderr:
        print("STDERR:", result.stderr)
except Exception as e:
    print(f"Error running git show --name-only: {e}")

try:
    print("\n--- GIT DIFF-TREE ---")
    result = subprocess.run(
        ['git', 'diff-tree', '--no-commit-id', '-r', commit_hash],
        capture_output=True,
        text=True,
        timeout=10
    )
    print(result.stdout)
    if result.stderr:
        print("STDERR:", result.stderr)
except Exception as e:
    print(f"Error running git diff-tree: {e}")
