#!/usr/bin/env python3
"""
Direct analysis of git commit object without subprocess calls.
This will decompress and parse the commit and tree objects manually.
"""
import zlib
import os
import sys

def read_git_object(sha):
    """Read a git object from the loose objects directory"""
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
    """Parse a commit object"""
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
    result['message'] = '\n'.join(msg_lines).strip()
    return result

def parse_tree(content):
    """Parse a tree object"""
    entries = []
    i = 0
    while i < len(content):
        try:
            space_pos = content.index(b' ', i)
            mode = content[i:space_pos].decode('ascii')
            null_pos = content.index(b'\x00', space_pos)
            name = content[space_pos+1:null_pos].decode('utf-8', errors='replace')
            sha_bytes = content[null_pos+1:null_pos+21]
            sha = sha_bytes.hex()
            entries.append((mode, name, sha))
            i = null_pos + 21
        except (ValueError, IndexError) as e:
            break
    return entries

def get_all_files(tree_sha, prefix=''):
    """Recursively get all files in a tree"""
    files = {}
    t, tc = read_git_object(tree_sha)
    if tc is None:
        return files
    entries = parse_tree(tc)
    for mode, name, sha in entries:
        full_path = f"{prefix}{name}" if not prefix else f"{prefix}/{name}"
        if mode in ('40000', '04') or mode.startswith('04'):
            # This is a directory/subtree
            sub = get_all_files(sha, full_path)
            files.update(sub)
        else:
            # This is a file
            files[full_path] = (mode, sha)
    return files

# Main analysis
os.chdir(r'D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot')

COMMIT = 'be0e1ceec39a7caf0b26b579a22762f2109a7cd8'

print("=" * 80)
print(f"GIT COMMIT ANALYSIS: {COMMIT}")
print("=" * 80)

# Read the commit object
obj_type, content = read_git_object(COMMIT)
if content is None:
    print(f"ERROR: Could not find commit object {COMMIT}")
    sys.exit(1)

print(f"\nObject type: {obj_type}")
commit_data = parse_commit(content)

print(f"Tree:       {commit_data.get('tree', 'N/A')}")
print(f"Parent:     {commit_data.get('parent', 'N/A')}")
print(f"Author:     {commit_data.get('author', 'N/A')}")
print(f"Committer:  {commit_data.get('committer', 'N/A')}")
print(f"Message:    {commit_data.get('message', 'N/A')}")

tree_sha = commit_data.get('tree')
parent_sha = commit_data.get('parent')

if not tree_sha or not parent_sha:
    print("\nERROR: Could not find tree or parent in commit")
    sys.exit(1)

# Read parent commit
print("\n" + "=" * 80)
print("PARENT COMMIT INFORMATION")
print("=" * 80)

_, parent_content = read_git_object(parent_sha)
if parent_content:
    parent_data = parse_commit(parent_content)
    parent_tree_sha = parent_data.get('tree')
    print(f"Parent tree:     {parent_tree_sha}")
    print(f"Parent message:  {parent_data.get('message', 'N/A')}")
else:
    print("ERROR: Could not read parent commit")
    sys.exit(1)

# Compare the trees
print("\n" + "=" * 80)
print("FILE CHANGES IN COMMIT")
print("=" * 80)

print(f"\nAnalyzing commit tree {tree_sha[:8]}...")
commit_files = get_all_files(tree_sha)
print(f"Found {len(commit_files)} files in commit tree")

print(f"\nAnalyzing parent tree {parent_tree_sha[:8]}...")
parent_files = get_all_files(parent_tree_sha)
print(f"Found {len(parent_files)} files in parent tree")

# Calculate differences
commit_file_paths = set(commit_files.keys())
parent_file_paths = set(parent_files.keys())

added = commit_file_paths - parent_file_paths
deleted = parent_file_paths - commit_file_paths
changed = {f for f in commit_file_paths if f in parent_file_paths and 
           commit_files[f][1] != parent_files[f][1]}

print(f"\n" + "=" * 80)
print("SUMMARY OF CHANGES")
print("=" * 80)
print(f"Files added:    {len(added)}")
print(f"Files deleted:  {len(deleted)}")
print(f"Files modified: {len(changed)}")
print(f"Total affected: {len(added) + len(deleted) + len(changed)}")

if added:
    print(f"\n{'ADDED FILES':─^80}")
    for f in sorted(added):
        print(f"  + {f}")

if deleted:
    print(f"\n{'DELETED FILES':─^80}")
    for f in sorted(deleted):
        print(f"  - {f}")

if changed:
    print(f"\n{'MODIFIED FILES':─^80}")
    for f in sorted(changed):
        print(f"  M {f}")

print("\n" + "=" * 80)
print("END OF ANALYSIS")
print("=" * 80)
