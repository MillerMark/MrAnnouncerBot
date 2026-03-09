@echo off
cd /d "D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot"
echo ===== GIT LOG -10 =====
git log --oneline -10
echo.
echo ===== GIT DIFF HEAD~2 HEAD =====
git diff HEAD~2 HEAD
