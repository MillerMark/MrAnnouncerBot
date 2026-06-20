---
configured: true
interval: 1
timeout: 60
options:
  behavior_header:
    type: group
    label: "After Fix:"
  build_verify:
    value: true
    type: bool
    label: "Verify build"
    hint: "Run the auto-detected build command after implementing and confirm it passes"
  commit_after_task:
    value: ask
    type: enum
    choices: [always, never, ask]
    label: "Commit:"
    hint: "When to automatically commit completed work"
description: "Interactive Repair — find the next filtered task, diagnose the root cause, propose a fix, implement on approval, repeat"
commands: [stop_loop]
---

# Interactive Fix Loop

You are running as part of a SquadDash **interactive fix loop**. Each cycle has two phases:

- **Analyze Phase** — find the next failing task, diagnose the root cause, write a proposed fix, and pause for human approval.
- **Implement Phase** — apply the approved fix, verify, commit, and mark done.

You never implement without prior human approval. The file `.squad/pending-fix.md` is the handoff between phases: its presence means a fix has been approved and is ready to implement.

> Iteration: {{iteration}}

---

## Phase Detection

**First action every iteration:** check whether `.squad/pending-fix.md` exists.

- If it **exists** → skip to [Implement Phase](#implement-phase)
- If it **does not exist** → proceed with [Analyze Phase](#analyze-phase)

---

## Analyze Phase

### Step 1 — Find the next task

Read `.squad/tasks.md`. Find the **first unchecked (`- [ ]`) task** that:
- Is NOT owned by `*(Owner: User)*`
- Matches the filter below

Work top-to-bottom; higher sections (🔴 High, 🟡 Mid) take priority over lower ones (🟢 Low).

The active filter from the Tasks panel is injected below. Match only tasks that satisfy it.

[**FILTER**]

If **no matching task** is found:

> No open tasks matching the filter remain. Stopping the loop.

```
HOST_COMMAND_JSON:
[
  { "command": "stop_loop" }
]
```

Do not proceed further this iteration.

---

### Step 2 — Identify the owner

Check the task line for an owner tag: `*(Owner: agent-handle)*`

- If an owner is listed on the task, become that agent for this iteration.
- If no owner is listed, read `.squad/routing.md` to identify the appropriate specialist based on the task type and affected area.

---

### Step 3 — Analyze the failure

Read the files relevant to the task to understand the root cause:

- **Test failures** — look for test result files in `.squad/test-results/`, `TestResults/`, or wherever the workspace stores them. Read the failure message and trace to the specific source file and line.
- **Build errors** — read the compiler output and trace to source.
- **Runtime errors** — read stack traces and trace to source.
- **Other failures** — read the task description and any linked files; understand what is broken and why.

---

### Step 4 — Write proposed fix to `.squad/pending-fix.md`

Create or overwrite `.squad/pending-fix.md` with the following structure:

```markdown
# Pending Fix

**Task:** [TASK-ID] — one-line description
**Owner:** agent-handle (from task line or routing.md)

## Root Cause

Specific file(s), line(s), and the reason the failure occurs.

## Proposed Fix

Concrete description of the change — specific enough to implement without re-reading
the source. Include exact method names, variable names, or logic changes.

## Files to Change

- path/to/file.ext — what changes and why

## Confidence

high / medium / low — with one-line rationale

## Risk

low / medium / high — with one-line rationale
```

---

### Step 5 — Present the diagnosis and pause

After writing `pending-fix.md`, present the full diagnosis to the user. Then offer options:

```
QUICK_REPLIES_JSON:
[
  { "label": "▶ Apply fix", "routeMode": "continue_current_agent", "reason": "Restart the loop to implement the fix in pending-fix.md" },
  { "label": "⏭ Skip this task", "routeMode": "continue_current_agent", "reason": "Abandon this fix and move to the next task" },
  { "label": "✏️ Suggest alternative", "routeMode": "continue_current_agent", "reason": "Discuss a different approach before approving" }
]
```

---

## Implement Phase

### Step 1 — Read the approved fix plan

Read `.squad/pending-fix.md` in full. Then read the source files listed in it to confirm you understand the scope.

---

### Step 2 — Adopt the owner role and apply the fix

Become the agent named in `pending-fix.md` as Owner. Implement the change described there. Keep the change **minimal and focused**: fix the specific root cause, do not refactor adjacent code, do not add unrelated improvements.

If the task involves a decision or architectural choice rather than a code change, document the decision in `.squad/decisions.md` (create if missing) and update any relevant architecture docs.

---

### Step 3 — Verify

If `build_verify` is `true` and `{{build_command}}` is non-empty, run `{{build_command}}` and confirm the build passes. If the build fails for any reason related to your change, fix it before continuing — do not skip or work around the build check.

Run the tests relevant to the changed files and confirm the specific failure described in `pending-fix.md` is resolved. If the test command is not obvious, check `package.json`, `*.sln`, a `Makefile`, or `README.md` for how to run tests in this workspace.

---

### Step 4 — Commit

{{#if commit_after_task == "always"}}
Commit immediately with a clear, descriptive message. Include the trailer: `{{copilot_trailer}}`
{{/if}}
{{#if commit_after_task == "ask"}}
Offer a quick reply: **"Commit changes"** / **"Leave uncommitted"** and wait for confirmation before committing.
{{/if}}
{{#if commit_after_task == "never"}}
Do not commit. Describe the diff in your response instead.
{{/if}}

---

### Step 5 — Mark done and clean up

1. Mark the task `[x]` in `.squad/tasks.md` and move it to the "Recently Completed" section at the bottom. Add a brief note: `✅ Fixed [TASK-ID]: one-line summary`
2. Delete `.squad/pending-fix.md`.
3. Report: **"Fixed [TASK-ID]: [what was done]"**
4. **End the iteration.** Do NOT offer quick replies to continue to the next task. Do NOT proceed to analyze the next task. The loop will automatically start the next iteration — your job ends here.

---

## Reference

- `.squad/tasks.md` — task backlog; source of truth for what needs fixing
- `.squad/routing.md` — who owns what; use to identify the right specialist
- `.squad/team.md` — squad roster
- `.squad/decisions.md` — architectural decisions log
- `.squad/pending-fix.md` — inter-phase handoff file (created during Analyze, deleted after Implement)
