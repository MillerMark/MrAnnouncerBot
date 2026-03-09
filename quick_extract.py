#!/usr/bin/env python3
# Direct extraction without needing subprocess
import zipfile
import os
import sys

nupkg_path = r'C:\Users\Mark\.nuget\packages\twitchlib.client.models\3.3.1\twitchlib.client.models.3.3.1.nupkg'

if not os.path.exists(nupkg_path):
    sys.stdout.write(f"ERROR: File not found at {nupkg_path}\n")
    sys.stdout.flush()
    sys.exit(1)

try:
    z = zipfile.ZipFile(nupkg_path, 'r')
    all_files = z.namelist()
    cs_files = [f for f in all_files if f.endswith('.cs')]
    
    sys.stdout.write("FILES IN NUPKG:\n")
    sys.stdout.flush()
    for f in sorted(all_files):
        sys.stdout.write(f + "\n")
        sys.stdout.flush()
    
    sys.stdout.write("\n\nC# SOURCE FILES:\n")
    sys.stdout.flush()
    for f in sorted(cs_files):
        sys.stdout.write(f + "\n")
        sys.stdout.flush()
    
    # Find ChatCommand.cs
    chat_files = [f for f in cs_files if 'ChatCommand.cs' in f]
    if chat_files:
        sys.stdout.write(f"\n\nFOUND: {chat_files[0]}\n")
        sys.stdout.write("EXTRACTING CONTENT:\n")
        sys.stdout.flush()
        content = z.read(chat_files[0]).decode('utf-8')
        sys.stdout.write(content)
        sys.stdout.flush()
    
    z.close()
except Exception as e:
    import traceback
    sys.stdout.write(f"ERROR: {e}\n")
    sys.stdout.write(traceback.format_exc())
    sys.stdout.flush()
    sys.exit(1)
