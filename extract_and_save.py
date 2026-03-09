import zipfile
import sys

nupkg_path = r'C:\Users\Mark\.nuget\packages\twitchlib.client.models\3.3.1\twitchlib.client.models.3.3.1.nupkg'
output_file = r'D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\extraction_output.txt'

try:
    with zipfile.ZipFile(nupkg_path, 'r') as z:
        files = sorted(z.namelist())
        
        with open(output_file, 'w', encoding='utf-8') as out:
            out.write("=" * 80 + "\n")
            out.write("ALL FILES IN NUPKG:\n")
            out.write("=" * 80 + "\n")
            for f in files:
                out.write(f + "\n")
            
            # Find C# files
            cs_files = [f for f in files if f.endswith('.cs')]
            
            out.write("\n" + "=" * 80 + "\n")
            out.write("C# SOURCE FILES:\n")
            out.write("=" * 80 + "\n")
            for f in cs_files:
                out.write(f + "\n")
            
            # Find ChatCommand.cs
            chat_command_files = [f for f in cs_files if 'ChatCommand.cs' in f]
            
            if chat_command_files:
                chat_cmd_path = chat_command_files[0]
                out.write("\n" + "=" * 80 + "\n")
                out.write(f"CONTENTS OF: {chat_cmd_path}\n")
                out.write("=" * 80 + "\n")
                content = z.read(chat_cmd_path).decode('utf-8')
                out.write(content)
                out.write("\n")
            else:
                out.write("\nChatCommand.cs not found in nupkg\n")
                
                # Try to list all cs files
                out.write("\nAvailable C# files:\n")
                for f in cs_files:
                    out.write(f"  {f}\n")
        
        print(f"Output written to {output_file}")
except Exception as e:
    print(f"Error: {e}")
    with open(output_file, 'w') as out:
        out.write(f"Error: {e}\n")
    sys.exit(1)
