import zipfile
import json

nupkg_path = r'C:\Users\Mark\.nuget\packages\twitchlib.client.models\3.3.1\twitchlib.client.models.3.3.1.nupkg'

z = zipfile.ZipFile(nupkg_path)
files = sorted(z.namelist())

# Write files list to a temp file we can read
with open(r'D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\nupkg_files.txt', 'w') as f:
    f.write('\n'.join(files))

# Look for ChatCommand.cs
cs_files = [f for f in files if f.endswith('.cs')]

if cs_files:
    with open(r'D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\cs_files.txt', 'w') as f:
        f.write('\n'.join(cs_files))

# Find ChatCommand.cs
chat_command_files = [f for f in cs_files if 'ChatCommand' in f]

if chat_command_files:
    content = z.read(chat_command_files[0]).decode('utf-8')
    with open(r'D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\ChatCommand.cs', 'w') as f:
        f.write(content)
    print(f"Extracted: {chat_command_files[0]}")

z.close()
print("Done!")
