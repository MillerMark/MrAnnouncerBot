@echo off
cd /d "D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot"

echo ========== COMMAND 1: git log --oneline -20 ==========
git log --oneline -20

echo.
echo ========== COMMAND 2: dotnet build BotCoreNet\BotCoreNet.csproj ==========
dotnet build BotCoreNet\BotCoreNet.csproj 2>&1

echo.
echo ========== COMMAND 3: dotnet build MrAnnouncerBot\MrAnnouncerBot.csproj ==========
dotnet build MrAnnouncerBot\MrAnnouncerBot.csproj 2>&1

echo.
echo ========== ALL COMMANDS COMPLETED ==========
