---
configured: true
options:
  filter_header:
    type: group
    label: "Task Filter:"
  task_tag:
    value: "[TF-*]"
    label: "Tag filter"
    hint: "Process tasks whose ID or text matches this tag. [TF-*] = test failures, [TEST-*] = any test task."
  test_header:
    type: group
    label: "Test Runner:"
  test_command:
    value: ""
    label: "Test command"
    hint: "Command to run tests (e.g. 'dotnet test SquadDash.Tests\\SquadDash.Tests.csproj -c Debug'). Leave blank to auto-detect from project files."
  build_verify:
    value: true
    type: bool
    label: "Verify build after fix"
    hint: "Run the auto-detected build command after implementing and confirm it passes"
  behavior_header:
    type: group
    label: "After Fix:"
  commit_after_task:
    value: ask
    type: enum
    choices: [always, never, ask]
    label: "Commit:"
    hint: "When to automatically commit completed work"
description: "Test-Failure Fix Loop — find a test-failure task, reproduce the failure, diagnose root cause, propose a fix, implement on approval, repeat"
commands: [stop_loop]
---

# Test-Failure Fix Loop

You are running as part of a SquadDash **test-failure fix loop**. Each cycle has two phases:

- **Analyze Phase** — find the next test-failure task, **reproduce the failure by actually running the tests**, diagnose the root cause, write a proposed fix with full failure evidence, and pause for human approval.
- **Implement Phase** — apply the approved fix, verify the specific test(s) now pass, commit, and mark done.

You never implement without prior human approval. The file `.squad/pending-fix.md` is the handoff between phases: its presence means a fix has been approved and is ready to implement.

> Iteration: {{iteration}}

---

## Phase Detection

**First action every iteration:** check whether `.squad/pending-fix.md` exists.

- If it **exists** → skip to [Implement Phase](#implement-phase)
- If it **does not exist** → proceed with [Analyze Phase](#analyze-phase)

---

## Analyze Phase

### Step 1 — Find the next test-failure task

Read `.squad/tasks.md`. Find the **first unchecked (`- [ ]`) task** that:
- Is NOT owned by `*(Owner: User)*`
- Matches the filter below

Work top-to-bottom; higher sections (🔴 High, 🟡 Mid) take priority over lower ones (🟢 Low).

**Filter:** If a filter was injected by Shift+clicking "Do these" in the Tasks panel, that filter applies and overrides the `{{task_tag}}` setting. If no filter was injected, fall back to matching `{{task_tag}}` (default: `[TF-*]`).

[**FILTER**]

If **no matching task** is found:

> No open test-failure tasks matching the filter remain. Stopping the loop.

```
HOST_COMMAND_JSON:
[
  { "command": "stop_loop" }
]
```

Do not proceed further this iteration.

---

### Step 2 — Reproduce the failure

**Before reading any source code**, run the tests to confirm the failure exists and capture its exact output.

**Determine the test command:**

1. If `{{test_command}}` is non-empty, use it exactly as written.
2. Otherwise, auto-detect by searching the workspace root in this order:
   - `*.sln` → use `dotnet test <solution-file>`
   - `*.csproj` containing `NUnit`, `xUnit`, or `MSTest` → use `dotnet test <project-file>`
   - `package.json` with a `"test"` script → use `npm test`
   - `Makefile` with a `test` target → use `make test`
   - Use the **first match** found.

Run the detected command with output fully captured. Record the exact command as the **Reproduction command** for use in `pending-fix.md`.

**Parse the output:**
- Identify FAILED tests by name. If the task description mentions a specific test name, filter results to that test.
- If the output references XML result files (e.g. `TestResults/*.xml`, `TestResults/*.trx`), read those files for the full failure detail: failure message, stack trace, expected value, actual value.
- Note the total number of passing and failing tests.

**If all tests pass (failure not reproduced):**
- Record this fact: "Tests passed — failure not reproduced at this time."
- Continue to Step 3 anyway. The task may still be valid (flaky test, logic error not yet caught, or environment issue). Analyze the source based on the task description.

---

### Step 3 — Identify the fix owner

Two owners apply to every test failure:

**Triage owner:** The **Testing & quality specialist** listed in `.squad/routing.md`. Read that file and find the agent responsible for testing, QA, and verification. That agent is responsible for diagnosing the failure and ensuring test quality standards are met.

**Fix owner:** Determined by the affected source code area. Read `.squad/routing.md` and find the agent responsible for the files/areas the failure implicates. Use the routing table in that file — do not guess based on file type alone.

If the failing test is in the test project itself (e.g. a bad assertion or missing setup) rather than in production code, both triage and fix owner are the Testing & quality specialist from `.squad/routing.md`.

---

### Step 4 — Write `.squad/pending-fix.md`

Create or overwrite `.squad/pending-fix.md` with the following structure:

```markdown
# Pending Fix

**Task:** [TASK-ID] — one-line description
**Triage owner:** Testing & quality specialist (from `.squad/routing.md`)
**Fix owner:** agent-handle (from routing.md, based on affected code area)

## Test Failure Evidence

**Failing test(s):** TestClassName.MethodName (or list if multiple)
**Reproduction command:** `exact command that was run`
**Failure message:**
```
Paste the exact failure message from test output or XML/TRX result file
```
**Expected:** what the test expected
**Actual:** what was returned
**Stack trace (if available):**
```
Paste the relevant portion of the stack trace
```

## Root Cause

Specific file(s), line(s), and the reason the test fails. Trace from the stack trace / failure message to the actual source defect.

## Proposed Fix

Concrete description of the change — specific enough to implement without re-reading the source. Include exact method names, variable names, or logic changes.

## Files to Change

- path/to/file.ext — what changes and why

## Confidence

high / medium / low — with one-line rationale

## Risk

low / medium / high — with one-line rationale
```

---

### Step 5 — Present the diagnosis and pause

After writing `pending-fix.md`, present the full contents to the user. Then offer options:

```
QUICK_REPLIES_JSON:
[
  { "label": "▶ Apply fix", "routeMode": "continue_current_agent", "reason": "Restart the loop to implement the fix in pending-fix.md" },
  { "label": "⏭ Skip this task", "routeMode": "continue_current_agent", "reason": "Abandon this fix and move to the next failing test" },
  { "label": "✏️ Suggest alternative", "routeMode": "continue_current_agent", "reason": "Discuss a different approach before approving" }
]
```

**To apply the fix:** restart the loop. It will detect `pending-fix.md` and go directly to the Implement Phase.

**To skip:** say "Skip this task." The loop will delete `.squad/pending-fix.md`, mark the task as skipped, and the next restart will move to the next matching task.

**To discuss an alternative:** reply in the conversation. Edit `.squad/pending-fix.md` manually if you want to adjust the approach before restarting.

Stop the loop and wait:

```
HOST_COMMAND_JSON:
[
  { "command": "stop_loop" }
]
```

---

## Implement Phase

### Step 1 — Read the approved fix plan

Read `.squad/pending-fix.md` in full. Note the **Reproduction command**, the **Fix owner**, and the **Files to Change**. Then read those source files to confirm you understand the scope before touching anything.

---

### Step 2 — Adopt the fix owner role and apply the fix

Become the agent named as **Fix owner** in `pending-fix.md`. Implement the change described there. Keep the change **minimal and focused**: fix the specific root cause identified in the failure evidence; do not refactor adjacent code, do not add unrelated improvements.

If the root cause turns out to differ from what was described in `pending-fix.md`, note the discrepancy in your response and apply the correct fix — but do not expand scope.

---

### Step 3 — Verify

Re-run the **exact Reproduction command** recorded in `pending-fix.md`. The specific failing test(s) named in the "Test Failure Evidence" section **must now pass**. If they do not pass, debug and fix before continuing — do not skip the verification step.

If `build_verify` is `true`, also run the appropriate build command for this project (e.g. `dotnet build`, `npm run build`, `make build`, or whatever the project uses) and confirm the build passes cleanly.

If any related tests that were previously passing now fail (regression), fix those too before proceeding.

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
3. Report: **"Fixed [TASK-ID]: [what was done] — [test name] now passes"**

Then stop the loop so you can review the result before the next task is analyzed:

```
HOST_COMMAND_JSON:
[
  { "command": "stop_loop" }
]
```

**To continue to the next task:** restart the loop. With `pending-fix.md` gone, it will start a fresh Analyze Phase for the next matching task.

---

## Reference

- `.squad/tasks.md` — task backlog; source of truth for what needs fixing
- `.squad/routing.md` — who owns what; use to identify triage and fix owners
- `.squad/team.md` — squad roster
- `.squad/pending-fix.md` — inter-phase handoff file (created during Analyze, deleted after Implement)
- Test result XML/TRX files — workspace-relative, auto-detected from test runner output (e.g. `TestResults/*.xml`, `TestResults/*.trx`)
