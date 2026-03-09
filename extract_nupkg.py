#!/usr/bin/env python3
import zipfile
import os

nupkg_path = r'C:\Users\Mark\.nuget\packages\twitchlib.client.models\3.3.1\twitchlib.client.models.3.3.1.nupkg'

if not os.path.exists(nupkg_path):
    print(f"File not found: {nupkg_path}")
    exit(1)

z = zipfile.ZipFile(nupkg_path)
files = sorted(z.namelist())

print("=" * 80)
print("FILES IN NUPKG:")
print("=" * 80)
for f in files:
    print(f)

# Look for ChatCommand.cs
cs_files = [f for f in files if f.endswith('.cs')]

print("\n" + "=" * 80)
print("C# SOURCE FILES FOUND:")
print("=" * 80)
for f in cs_files:
    print(f)

# Try to find and extract ChatCommand.cs
chat_command_files = [f for f in cs_files if 'ChatCommand' in f]

if chat_command_files:
    print("\n" + "=" * 80)
    print(f"EXTRACTING: {chat_command_files[0]}")
    print("=" * 80)
    content = z.read(chat_command_files[0]).decode('utf-8')
    print(content)

z.close()
