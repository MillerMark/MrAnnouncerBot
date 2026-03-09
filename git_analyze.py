#!/usr/bin/env python3
"""Analyze git commit be0e1ceec39a7caf0b26b579a22762f2109a7cd8"""
import zlib
import os
import subprocess
import sys

os.chdir(r'D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot')

COMMIT = 'be0e1ceec39a7caf0b26b579a22762f2109a7cd8'

def read_object(sha):
    path = f'.git/objects/{sha[:2]}/{sha[2:]}'
    if not os.path.exists(path):
        return None, None
    with open(path, 'rb') as f:
        data = zlib.decompress(f.read())
    null_pos = data.index(b'\x00')
    header = data[:null_pos].decode('ascii')
    obj_type = header.split(' ')[0]
    content = data[null_pos+1:]
    return obj_type, content

def parse_commit(content):
    text = content.decode('utf-8', errors='replace')
    lines = text.split('\n')
    result = {}
    msg_lines = []
    in_msg = False
    for line in lines:
        if in_msg:
            msg_lines.append(line)
        elif line == '':
            in_msg = True
        elif line.startswith('tree '):
            result['tree'] = line[5:].strip()
        elif line.startswith('parent '):
            result['parent'] = line[7:].strip()
        elif line.startswith('author '):
            result['author'] = line[7:].strip()
        elif line.startswith('committer '):
            result['committer'] = line[10:].strip()
    result['message'] = '\n'.join(msg_lines)
    return result

def parse_tree(content):
    entries = []
    i = 0
    while i < len(content):
        space_pos = content.index(b' ', i)
        mode = content[i:space_pos].decode('ascii')
        null_pos = content.index(b'\x00', space_pos)
        name = content[space_pos+1:null_pos].decode('utf-8', errors='replace')
        sha_bytes = content[null_pos+1:null_pos+21]
        sha = sha_bytes.hex()
        entries.append((mode, name, sha))
        i = null_pos + 21
    return entries

print("=" * 70)
print(f"READING COMMIT: {COMMIT}")
print("=" * 70)

obj_type, content = read_object(COMMIT)
if content is None:
    print("ERROR: Commit object not found as loose object!")
    print("Trying git cat-file...")
else:
    print(f"Object type: {obj_type}")
    commit_data = parse_commit(content)
    print(f"\nTree:      {commit_data.get('tree', 'N/A')}")
    print(f"Parent:    {commit_data.get('parent', 'N/A')}")
    print(f"Author:    {commit_data.get('author', 'N/A')}")
    print(f"Committer: {commit_data.get('committer', 'N/A')}")
    print(f"Message:   {commit_data.get('message', 'N/A')}")
    
    tree_sha = commit_data.get('tree')
    parent_sha = commit_data.get('parent')
    
    if tree_sha and parent_sha:
        print(f"\n{'=' * 70}")
        print("READING PARENT COMMIT")
        print("=" * 70)
        
        _, parent_content = read_object(parent_sha)
        if parent_content:
            parent_data = parse_commit(parent_content)
            parent_tree_sha = parent_data.get('tree')
            print(f"Parent tree: {parent_tree_sha}")
            print(f"Parent msg:  {parent_data.get('message', 'N/A')}")
            
            print(f"\n{'=' * 70}")
            print("COMPARING TREES")
            print("=" * 70)
            
            def get_all_files(tree_sha, prefix=''):
                files = {}
                t, tc = read_object(tree_sha)
                if tc is None:
                    print(f"  [PACKED] Tree {tree_sha[:8]} - need git cat-file")
                    return files
                entries = parse_tree(tc)
                for mode, name, sha in entries:
                    full_path = f"{prefix}{name}" if not prefix else f"{prefix}/{name}"
                    if mode == '40000' or mode.startswith('04'):
                        sub = get_all_files(sha, full_path)
                        files.update(sub)
                    else:
                        files[full_path] = sha
                return files
            
            print(f"Getting files for commit tree: {tree_sha[:8]}")
            commit_files = get_all_files(tree_sha)
            print(f"Getting files for parent tree: {parent_tree_sha[:8]}")
            parent_files = get_all_files(parent_tree_sha)
            
            print(f"\nCommit has {len(commit_files)} files, parent has {len(parent_files)} files")
            
            added = set(commit_files) - set(parent_files)
            deleted = set(parent_files) - set(commit_files)
            changed = {f for f in commit_files if f in parent_files and commit_files[f] != parent_files[f]}
            
            if added:
                print(f"\nADDED ({len(added)}):")
                for f in sorted(added): print(f"  + {f}")
            if deleted:
                print(f"\nDELETED ({len(deleted)}):")
                for f in sorted(deleted): print(f"  - {f}")
            if changed:
                print(f"\nMODIFIED ({len(changed)}):")
                for f in sorted(changed): print(f"  M {f}")
        else:
            print("Parent commit not found as loose object - in pack file")

# Also try git commands
print(f"\n{'=' * 70}")
print("TRYING GIT COMMANDS")
print("=" * 70)

for cmd in [
    ['git', '--no-pager', 'show', COMMIT, '--stat', '--no-color'],
    ['git', '--no-pager', 'show', COMMIT, '--name-only', '--no-color'],
    ['git', '--no-pager', 'diff-tree', '--no-commit-id', '-r', COMMIT],
]:
    print(f"\nRunning: {' '.join(cmd)}")
    try:
        r = subprocess.run(cmd, capture_output=True, text=True, timeout=15)
        if r.stdout: print(r.stdout)
        if r.stderr: print("STDERR:", r.stderr[:200])
    except Exception as e:
        print(f"Error: {e}")
