#!/usr/bin/env python3
"""Script to show git diffs for recent commits focusing on C# files."""

import subprocess
import os

# Change to the repo directory
repo_dir = r"D:\Drive\DX\Twitch\CodeRushed\MrAnnouncerBot"
os.chdir(repo_dir)

def run_git_command(cmd_args, description):
    """Run a git command and print output."""
    print(f"\n{'='*80}")
    print(f"COMMAND: git {' '.join(cmd_args)}")
    print(f"DESCRIPTION: {description}")
    print('='*80)
    try:
        result = subprocess.run(
            ['git'] + cmd_args,
            capture_output=True,
            text=True,
            cwd=repo_dir
        )
        if result.stdout:
            print(result.stdout)
        if result.stderr:
            print("STDERR:", result.stderr)
        if result.returncode != 0:
            print(f"Return code: {result.returncode}")
    except Exception as e:
        print(f"Error running command: {e}")

# Run the commands
print("GIT DIFF ANALYSIS FOR MrAnnouncerBot Repository")
print("="*80)

# 1. Show last 10 commits
run_git_command(['log', '--oneline', '-10'], "Last 10 commits")

# 2. Show statistics for commit be0e1ceec39a7caf0b26b579a22762f2109a7cd8
run_git_command(
    ['show', 'be0e1ceec39a7caf0b26b579a22762f2109a7cd8', '--stat'],
    "Stats for commit be0e1ceec39a7caf0b26b579a22762f2109a7cd8"
)

# 3. Show full diff for commit be0e1ceec39a7caf0b26b579a22762f2109a7cd8
run_git_command(
    ['show', 'be0e1ceec39a7caf0b26b579a22762f2109a7cd8'],
    "Full diff for commit be0e1ceec39a7caf0b26b579a22762f2109a7cd8"
)

# 4. Show most recent commit (bac39c37f15752f79a40002c710a22c5dc019b9d)
run_git_command(
    ['show', 'bac39c37f15752f79a40002c710a22c5dc019b9d'],
    "Full diff for most recent commit bac39c37f15752f79a40002c710a22c5dc019b9d"
)

# 5. Show last 3 commits in detail
run_git_command(
    ['log', '--oneline', '-3'],
    "Last 3 commits (for context)"
)

# 6. Show diffs for C# files in last 3 commits
run_git_command(
    ['log', '--oneline', '-3', '--', '*.cs'],
    "Last 3 commits affecting C# files"
)

print("\n" + "="*80)
print("ANALYSIS COMPLETE")
print("="*80)
