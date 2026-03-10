#!/usr/bin/env python3
import subprocess
import os
import sys

os.chdir(r'D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot')

print("=" * 70)
print("COMMAND 1: git log --oneline -20")
print("=" * 70)
try:
    result = subprocess.run(['git', 'log', '--oneline', '-20'], 
                          capture_output=True, text=True, timeout=30)
    print(result.stdout)
    if result.stderr:
        print("STDERR:", result.stderr)
    print(f"Return code: {result.returncode}\n")
except Exception as e:
    print(f"Error: {e}\n")

print("=" * 70)
print("COMMAND 2: dotnet build BotCoreNet\\BotCoreNet.csproj")
print("=" * 70)
try:
    result = subprocess.run(['dotnet', 'build', 'BotCoreNet\\BotCoreNet.csproj'], 
                          capture_output=True, text=True, timeout=300)
    print(result.stdout)
    if result.stderr:
        print("STDERR:", result.stderr)
    print(f"Return code: {result.returncode}\n")
except Exception as e:
    print(f"Error: {e}\n")

print("=" * 70)
print("COMMAND 3: dotnet build MrAnnouncerBot\\MrAnnouncerBot.csproj")
print("=" * 70)
try:
    result = subprocess.run(['dotnet', 'build', 'MrAnnouncerBot\\MrAnnouncerBot.csproj'], 
                          capture_output=True, text=True, timeout=300)
    print(result.stdout)
    if result.stderr:
        print("STDERR:", result.stderr)
    print(f"Return code: {result.returncode}\n")
except Exception as e:
    print(f"Error: {e}\n")

print("=" * 70)
print("ALL COMMANDS COMPLETED")
print("=" * 70)
