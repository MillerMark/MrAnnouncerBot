---
# Full documentation: docs/features/maintenance-mode.md
# Task frequency options:
#   always        — run every maintenance session
#   daily         — run at most once per calendar day (UTC)
#   weekly        — run at most once per Monday–Sunday calendar week (UTC)
#   monthly       — run at most once per calendar month (UTC year+month)
#   after-commits — run once per new HEAD commit SHA
#   per-commit    — backward-compat alias for after-commits
#   every-N-commits — run once ≥N new commits since last run (e.g., every-5-commits)
# Set configured: true to enable maintenance mode.
# Set enabled: true on individual tasks to activate them.
# Global safety floor: per-task safety cannot be less safe than this value.
#   report-only < branch < direct
#
# INBOX REPORTING
# When a task produces a report (safety: report-only, or if_found: report),
# always deliver the findings to the user's Inbox panel by appending an
# INBOX_MESSAGE_JSON block at the very end of the response. Use from: "argus-weld".
# Example:
#
# INBOX_MESSAGE_JSON:
# {
#   "subject": "Maintenance Report: <Task Title>",
#   "from": "argus-weld",
#   "body": "## <Task Title>\n\n<Full findings in Markdown>",
#   "attachments": []
# }
#
# The body should contain the full structured report so the user can refer back
# to it without digging through the transcript. Keep the subject concise.
idle_timeout: 15
max_tasks_per_session: 5
safety: branch
enabled_on_idle: false
configured: false  # ← change to true to activate
# ─────────────────────────────────────────────
# DECOMPOSE POLICY
#
# For branch-capable tasks, the AI chooses one of two execution paths:
#
# PATH 1 — Implement directly (single pass) — use when ALL of:
#   • ≤ 5 files affected
#   • No ordering dependencies between changes
#   • Limited blast radius (no cross-cutting / public API changes)
#   • A single pass can leave the build green
#
# PATH 2 — Decompose (emit TASKS_JSON) — use when ANY of:
#   • > 5 files affected
#   • Changes have ordering dependencies
#   • High blast radius (cross-cutting changes, public API modifications)
#   • Mid-pass breakage risk (build would be red between steps)
#
# When decomposing:
#   • Do NOT make any code changes in the analysis pass
#   • Design 2–25 discrete steps, each leaving the build green when done
#   • Write fully self-contained step descriptions (no shared context assumed)
#   • Use dependsOn to enforce ordering between steps
#   • Set branch to {{branch}} (same branch for all steps)
#   • Emit TASKS_JSON as the last content in your response
# ─────────────────────────────────────────────
tasks:
  - id: architectural-practices
    enabled: true
    frequency: monthly
    safety: report-only
    has_safety_options: true
    title: Architectural Practice Review
    instructions: |
      Review the codebase for larger architectural problems: poor separation of
      concerns, leaky abstractions, god objects, circular dependencies, missing
      service boundaries, wrong layer responsibilities, etc.

      {{#if if_found == "branch"}}
      Create a maintenance branch named: {{branchName}}
      Implement the improvements you identify and commit to that branch.
      Document your reasoning in commit messages.
      {{/if}}
      {{#if if_found == "report"}}
      Do not change any code. Write a structured report of each finding:
      problem description, affected files/layers, and a recommended fix.
      Send the report to the user's Inbox using an INBOX_MESSAGE_JSON block
      (from: "argus-weld").
      {{/if}}
    options:
      if_found:
        type: radio
        label: If architectural issues are found
        tooltip: "Implement improvements or produce a report"
        value: report
        choices:
          - value: branch
            tooltip: Implement improvements on a maintenance branch
          - value: report
            tooltip: Write a report — do not change any code

  - id: code-smells
    enabled: false
    frequency: weekly-Saturday
    safety: report-only
    has_safety_options: true
    title: Code Smell Cleanup
    instructions: |
      Scan the codebase for code smells: poor readability, long methods, unclear
      naming, overly complex conditionals, dead code, unnecessary abstraction,
      inefficient patterns, missing null checks, etc.

      {{#if if_found == "fix"}}
      Address smells inline on the current branch.
      {{/if}}
      {{#if if_found == "branch"}}
      Create a maintenance branch named: {{branchName}} and address smells there.
      {{/if}}
      {{#if if_found == "report"}}
      Do not change any code. List each smell with file path, structural anchor,
      category, and a brief description of the issue and suggested fix. Send the
      report to the user's Inbox using an INBOX_MESSAGE_JSON block (from: "argus-weld").
      {{/if}}
    options:
      if_found:
        type: radio
        label: If code smells are found
        tooltip: "Fix inline, use a branch, or report only"
        value: report
        choices:
          - value: fix
            tooltip: Address smells inline on the current branch
          - value: branch
            tooltip: Create a maintenance branch and address smells there
          - value: report
            tooltip: List each smell — do not change any code

  - id: commit-review
    enabled: true
    frequency: every-10-commits
    safety: report-only
    has_safety_options: false
    title: Commit Quality Review
    instructions: |
      Review all commits since {{last_reviewed_sha}} ({{new_commit_count}} new commits since last review).
      If {{last_reviewed_sha}} is empty, review the last 10 commits.
      For each commit, review the diff and note:
      - Code quality issues introduced (smells, complexity, missing tests)
      - Missing or inadequate commit message detail
      - Potential bugs or regressions
      - Positive patterns worth reinforcing

      Write a structured review report. Do not change any code. Send the report
      to the user's Inbox using an INBOX_MESSAGE_JSON block (from: "argus-weld").

  - id: docs-review
    enabled: true
    frequency: weekly-Sunday
    safety: report-only
    has_safety_options: true
    title: Documentation Review
    instructions: |
      Review the documentation in the `docs/` folder (or the repo's primary docs
      location) for the following issues:
      
      1. **Accuracy** — Are instructions, command examples, configuration values,
         and feature descriptions still accurate relative to the current codebase?
         Flag anything that appears outdated or incorrect.
      2. **Broken internal links** — Scan all Markdown files for links to other
         pages within the docs. Check whether each target file exists. If a page
         exists but has `published: false` (or equivalent front-matter), flag any
         other page that links to it as a warning — the reader will hit an
         unpublished page.
      3. **Broken external links** — Optionally check HTTP/HTTPS links to see if
         they return a non-200 status. Flag dead external links.
      4. **Missing images** — Find image references (`![...](...)`). Check whether
         the referenced file exists on disk. Flag missing image files.
      5. **Orphaned pages** — Identify docs pages that are not reachable from any
         other page (no inbound links from within the docs tree). These may be
         forgotten or accidentally unpublished pages.
      
      {{#if if_found == "report"}}
      Do not change any files. Produce a structured report grouped by issue type,
      listing file path, structural anchor (e.g. `## Section > ### Subsection`),
      and a description of each problem found. Include a severity: Warning for
      unpublished-page links and dead links, Info for orphaned pages and accuracy
      concerns. Send the report to the user's Inbox using an `INBOX_MESSAGE_JSON`
      block (from: "argus-weld").
      {{/if}}
      {{#if if_found == "fix"}}
      Correct accuracy issues and fix broken links where possible (e.g. update a
      link target, remove a dead link). Commit changes to a maintenance branch named: {{branchName}}.
      Items that require human judgment (accuracy rewrites, missing images) should
      still be reported.
      {{/if}}
    options:
      if_found:
        type: radio
        label: If documentation issues are found
        tooltip: Produce a report or fix what can be fixed automatically
        value: report
        choices:
          - value: report
            tooltip: Write a report — do not change any files
          - value: fix
            tooltip: Fix auto-correctable issues on a maintenance branch; report the rest
  - id: eliminate-duplication
    enabled: false
    frequency: daily
    safety: report-only
    has_safety_options: true
    title: Eliminate Code Duplication
    instructions: |
      Scan the codebase for duplicated logic — identical or near-identical code
      blocks, copy-pasted utility functions, repeated patterns that should be
      extracted. Focus on meaningful duplication (not trivial one-liners).

      {{#if if_found == "fix"}}
      Refactor inline on the current branch. Extract shared logic, update all
      call sites, ensure tests still pass.
      {{/if}}
      {{#if if_found == "branch"}}
      Create a maintenance branch named: {{branchName}} and refactor there.
      {{/if}}
      {{#if if_found == "report"}}
      Do not change any code. List each duplication instance with file paths,
      structural anchors, and a brief description of the shared logic. Send the
      report to the user's Inbox using an INBOX_MESSAGE_JSON block (from: "argus-weld").
      {{/if}}
    options:
      if_found:
        type: radio
        label: If duplication is found
        tooltip: "Refactor now, create a branch, or just report"
        value: report
        choices:
          - value: fix
            tooltip: Refactor inline on the current branch
          - value: branch
            tooltip: Create a maintenance branch and refactor there
          - value: report
            tooltip: List each instance — do not change any code

  - id: error-handling-audit
    enabled: true
    frequency: weekly-Thursday
    safety: report-only
    has_safety_options: true
    title: Error Handling Audit
    instructions: |
      Audit the codebase for error-handling gaps and unsafe exception patterns.
      This task is designed to work with any language, framework, or platform —
      desktop, web, mobile, CLI, or server. Begin by identifying the tech stack
      (language, UI framework if any, async model, logging infrastructure) so you
      can apply the relevant sub-checks below accurately.

      For each finding, record: file path, structural anchor (function/class/method),
      category (from the list), severity (Critical / High / Medium / Low), and a
      concrete description of the problem and the recommended fix.

      **Categories to check:**

      1. **Silent error suppression** — catch or error-handler blocks that swallow
         failures with no logging, no rethrow, and no user notification. Examples
         across stacks:
         - C#: `catch { }` or `catch (Exception ex) { }` with empty body
         - JavaScript/TypeScript: `.catch(() => {})` or empty `catch (e) {}`
         - Python: bare `except: pass` or `except Exception: pass`
         - Go: `if err != nil { _ = err }` (error discarded)
         - Swift/Kotlin: `try? ...` or `try { } catch { }` with no handling
         Distinguish genuine best-effort suppression (e.g. cleanup during shutdown)
         from silently dropped failures that callers or users need to know about.

      2. **Catch-and-return-sentinel** — functions that catch internally and return
         a sentinel value (null, false, undefined, -1, empty string) with no log and
         no way for the caller to distinguish "legitimately absent" from "threw
         unexpectedly". Callers proceed as if nothing went wrong.

      3. **UI event/callback handlers without error guards** — handlers that run
         in response to user interaction or framework lifecycle events and are not
         wrapped in error handling. An unguarded exception here typically crashes
         or freezes the UI without a meaningful message. Applies to:
         - Desktop (WPF/WinForms/MAUI): event handlers, DispatcherTimer callbacks
         - Web front-end (React/Vue/Angular/Svelte): onClick, useEffect, lifecycle
           hooks, component error boundaries missing where subtrees can fail
         - Mobile (Android/iOS/Flutter): Activity/Fragment callbacks, lifecycle
           methods, gesture handlers, widget build methods
         - Node.js/Express: route handlers and middleware without next(err) calls
         Each handler should catch, log, and recover or degrade gracefully.

      4. **Unguarded async / concurrent entry points** — language-specific patterns
         where exceptions escape the normal error-propagation chain:
         - C#: `async void` (non-event-handler); fire-and-forget `_ = Task.Run(...)`
           or unawaited calls; lost exceptions only surface via
           `TaskScheduler.UnobservedTaskException`
         - JavaScript/TypeScript: unhandled Promise rejections (`.then()` without
           `.catch()`, `async` functions called without `await` or `.catch()`);
           missing `process.on('unhandledRejection')` / `window.onunhandledrejection`
         - Python: background threads or `asyncio` tasks whose exceptions are never
           retrieved; `asyncio.create_task()` results that are dropped
         - Go: goroutines without a deferred `recover()`; errors from goroutines
           that are never sent back over a channel
         - Swift: `Task { }` blocks where thrown errors are silently discarded
         - Kotlin: `launch { }` coroutines without a `CoroutineExceptionHandler`
         Flag each case and evaluate whether the exception needs to surface.

      5. **Missing resource cleanup on error paths** — code that acquires a resource
         (file handle, network connection, lock, database transaction, event
         subscription, native handle) without guaranteeing release on all exit paths:
         - C#: missing `using` / `IDisposable` or `finally`
         - JavaScript: missing `finally` or manual cleanup after `try`
         - Python: missing `with` / context manager or `finally`
         - Go: missing `defer` for `Close()` / `Unlock()` / `rows.Close()`
         - Java/Kotlin: missing try-with-resources or `finally`
         - Swift: missing `defer` for cleanup
         An exception partway through leaves resources leaked or state corrupted.

      6. **Overly broad exception catches** — catching the most general error type
         when a narrower type would be more appropriate. Broad catches can mask
         programming errors (null dereferences, type errors, logic bugs) that should
         propagate and be fixed. Examples:
         - C#: `catch (Exception)` instead of `catch (IOException)`
         - JavaScript: `catch (e)` on code that should only handle `NetworkError`
         - Python: `except Exception` instead of `except ValueError`
         - Go: not applicable (explicit error types), but note any pattern of
           comparing `err != nil` without inspecting the error type where type
           matters

      7. **Exception / error messages missing context** — throw or log sites where
         the message does not include enough information to diagnose the failure:
         operation name, input value, file path, object identity, or state at the
         time of failure. A bare "parsing failed" or "unexpected error" is not
         actionable. The message should answer: what was being done, on what input,
         and what went wrong specifically.

      8. **Background / worker errors not surfaced to the user** — exceptions on
         background threads, worker processes, service workers, web workers, or async
         tasks that are logged (or not) but never result in any user-visible feedback.
         Silent background failures leave the application in an inconsistent state
         with no indication to the user. Check for appropriate error-reporting
         callbacks, status indicators, or toast/notification dispatch after failure.

      9. **Global error handler gaps** — applications that do not register (or
         register incompletely) a last-resort unhandled-error handler:
         - C#: `AppDomain.UnhandledException`, `TaskScheduler.UnobservedTaskException`,
           `Dispatcher.UnhandledException` (WPF) / `Application.ThreadException` (WinForms)
         - JavaScript browser: `window.onerror`, `window.onunhandledrejection`
         - Node.js: `process.on('uncaughtException')`, `process.on('unhandledRejection')`
         - Python: `sys.excepthook`, logging of unhandled asyncio exceptions
         - Android: `Thread.setDefaultUncaughtExceptionHandler`
         - iOS: `NSSetUncaughtExceptionHandler`, signal handlers for SIGABRT/SIGSEGV
         - Flutter: `FlutterError.onError`, `PlatformDispatcher.instance.onError`
         Missing handlers mean crashes produce no diagnostic information.

      10. **Cancellation / interruption signals swallowed** — language-specific
          cancellation mechanisms that are silently consumed rather than propagated:
          - C#: `OperationCanceledException` / `TaskCanceledException` caught without
            rethrow; `CancellationToken` passed to a method but never checked
          - JavaScript: `AbortController` / `AbortSignal` ignored in fetch/async code
          - Python: `asyncio.CancelledError` caught and not re-raised (required in
            Python ≥ 3.8); `KeyboardInterrupt` swallowed in a bare `except`
          - Go: `ctx.Done()` channel never selected on in long-running goroutines
          Swallowing cancellation breaks cooperative shutdown and resource cleanup.

      {{#if if_found == "report"}}
      Do not change any code. Produce a structured report grouped by category,
      then by severity within each category. For each finding include:
      - File path and structural anchor (ClassName.MethodName or equivalent)
      - Severity: Critical / High / Medium / Low
      - Description of the problem
      - Recommended fix

      At the end, include a brief summary: total findings by severity, and the
      two or three highest-priority items to address first.

      Send the report to the user's Inbox using an INBOX_MESSAGE_JSON block
      (from: "argus-weld").
      {{/if}}
      {{#if if_found == "fix"}}
      Fix issues that are safe to patch automatically on a maintenance branch named: {{branchName}}:
      - Add logging to silent catch/error blocks where the failure is non-trivial
      - Wrap bare UI event handlers in try-catch / .catch() with appropriate logging
      - Add resource-cleanup guards (using / finally / defer / with) for obvious leaks
      Issues requiring design decisions (restructuring callers, changing return
      types, async void→Task conversions with call-site changes, adding global
      error handlers) should be reported instead in an INBOX_MESSAGE_JSON block
      (from: "argus-weld").
      {{/if}}
    options:
      if_found:
        type: radio
        label: If error-handling gaps are found
        tooltip: "Produce a report or patch safe fixes automatically"
        value: report
        choices:
          - value: report
            tooltip: Write a structured report — do not change any code
          - value: fix
            tooltip: Patch safe fixes on a maintenance branch; report the rest

  - id: magic-numbers
    enabled: false
    frequency: daily
    safety: report-only
    has_safety_options: true
    title: Extract Magic Numbers and Hardcoded Strings
    instructions: |
      Scan the codebase for magic numbers (numeric literals used in logic without
      explanation) and hardcoded strings that belong in named constants or
      configuration (connection strings, URLs, thresholds, timeouts, limits, etc.).

      {{#if if_found == "extract"}}
      Extract each magic value into a named constant or config entry. Update all
      references. Commit to a maintenance branch named: {{branchName}}.
      {{/if}}
      {{#if if_found == "report"}}
      Do not change any code. List each instance with file path, structural anchor
      (e.g. ClassName.MethodName), the literal value, and a suggested constant name.
      {{/if}}
    options:
      if_found:
        type: radio
        label: If magic numbers or hardcoded strings are found
        tooltip: "Extract to constants or report them"
        value: report
        choices:
          - value: extract
            tooltip: Extract to named constants; commit to a maintenance branch
          - value: report
            tooltip: List each instance — do not change any code

  - id: naming-conventions
    enabled: false
    frequency: daily
    safety: report-only
    has_safety_options: true
    title: Naming Convention Audit
    instructions: |
      Audit the codebase for naming inconsistencies:
      - Variables, fields, properties, methods deviating from the project's
        established convention (PascalCase, camelCase, snake_case, etc.)
      - Inconsistent pluralisation (e.g. `items` vs `itemList` vs `itemCollection`)
      - Abbreviations used in some places but not others
      - Test method naming inconsistencies

      {{#if if_found == "fix"}}
      Rename inconsistencies directly on the current branch. Update all
      references. Ensure the project still builds.
      {{/if}}
      {{#if if_found == "report"}}
      Do not change any code. List each inconsistency with file path, structural
      anchor (e.g. ClassName.MethodName), current name, and suggested name.
      Send the report to the user's Inbox using an INBOX_MESSAGE_JSON block
      (from: "argus-weld").
      {{/if}}
    options:
      if_found:
        type: radio
        label: If naming inconsistencies are found
        tooltip: "Rename inconsistencies or produce a report"
        value: report
        choices:
          - value: fix
            tooltip: Rename directly on the current branch; update all references
          - value: report
            tooltip: List each inconsistency — do not change any code

  - id: speed-improvements
    enabled: true
    frequency: daily
    safety: report-only
    has_safety_options: true
    title: Performance Improvements
    instructions: |
      Review the codebase for performance opportunities: inefficient algorithms,
      unnecessary allocations, repeated expensive operations, missing caching,
      synchronous I/O where async would improve throughput, LINQ queries that
      could be rewritten, N+1 query patterns, etc.

      {{#if if_found == "fix"}}
      Implement optimisations inline on the current branch. Add a brief comment
      explaining the change where the improvement is non-obvious.
      {{/if}}
      {{#if if_found == "branch"}}
      Create a maintenance branch named: {{branchName}} and implement improvements there.
      {{/if}}
      {{#if if_found == "report"}}
      Do not change any code. Describe each opportunity, its likely impact, and
      the recommended approach. Send the report to the user's Inbox using an
      INBOX_MESSAGE_JSON block (from: "argus-weld").
      {{/if}}
    options:
      if_found:
        type: radio
        label: If performance opportunities are found
        tooltip: "Implement optimisations or report opportunities"
        value: report
        choices:
          - value: fix
            tooltip: Implement optimisations inline on the current branch
          - value: branch
            tooltip: Create a maintenance branch for the optimisations
          - value: report
            tooltip: Describe each opportunity — do not change any code

  - id: prune-tasks
    enabled: true
    frequency: daily
    safety: direct
    has_safety_options: false
    title: Prune Completed Tasks
    instructions: |
      Open `.squad/tasks.md`. Remove all items that are marked as completed
      (`[x]`) and have no open sub-tasks. Archive removed items to
      `.squad/tasks-archive.md` (append, do not overwrite) with a timestamp.

      Do not modify any source files.

  - id: readme-currency
    enabled: true
    frequency: weekly-Wednesday
    safety: report-only
    has_safety_options: false
    title: README Currency Check
    instructions: |
      Compare README.md (and any other top-level docs) against the current state
      of the codebase. Check for:
      - Setup or build instructions that no longer match the actual commands
      - Outdated dependency versions or requirements
      - References to files, directories, or features that no longer exist
      - Missing documentation for significant new features or changed APIs

      Write a gap report. Do not change any files. Send the report to the user's
      Inbox using an INBOX_MESSAGE_JSON block (from: "argus-weld").

  - id: run-tests
    enabled: true
    frequency: daily
    safety: report-only
    has_safety_options: true
    title: Run Tests
    instructions: |
      Run all tests in the repository. Use the appropriate test runner for this
      project (e.g. `dotnet test`, `npm test`, `go test ./...`).
      
      {{#if if_failing == "fix"}}
      Diagnose each failing test. Fix the root cause in source — do not delete
      tests or weaken assertions. Commit all fixes to the branch named: {{branchName}}.
      {{/if}}
      {{#if if_failing == "report"}}
      Do not change any code. Write a summary of every failing test, the error
      message, and your diagnosis of the likely cause. Send the report to the
      user's Inbox using an INBOX_MESSAGE_JSON block (from: "argus-weld").
      {{/if}}
    options:
      if_failing:
        type: radio
        label: If failing tests are found
        tooltip: Fix failures or only report them
        value: report
        choices:
          - value: fix
            tooltip: Fix each failing test; commit fixes to the branch
          - value: report
            tooltip: Report failures only — do not change any code
  - id: security-audit
    enabled: true
    frequency: weekly-Tuesday
    safety: report-only
    has_safety_options: true
    title: Security Vulnerability Audit
    instructions: |
      Audit the codebase for security vulnerabilities and unsafe patterns.
      Focus on:
      - Injection risks (SQL, command, path traversal, format string)
      - Secrets or credentials hard-coded or logged in plain text
      - Unsafe deserialization or untrusted data passed to eval-equivalent APIs
      - Missing input validation or output encoding (XSS, open redirect)
      - Overly broad exception catches that swallow security-relevant errors
      - Insecure cryptography (MD5/SHA1 for integrity, ECB mode, short keys, hard-coded IVs)
      - Dependency or NuGet package references with known CVEs (check via `dotnet list package --vulnerable` if available)
      - File or network operations that trust caller-supplied paths without sanitisation
      - Sensitive data written to logs, temp files, or crash dumps
      {{#if if_found == "report"}}
      Do not change any code. Produce a structured report grouped by severity
      (Critical / High / Medium / Low), listing file path, structural anchor
      (e.g. ClassName.MethodName), and a description of each finding.
      Send the report to the user's Inbox using an INBOX_MESSAGE_JSON block
      (from: "argus-weld").
      {{/if}}
      {{#if if_found == "fix"}}
      Fix issues that are safe to patch automatically (remove hard-coded secrets,
      add input guards, replace deprecated crypto calls). Commit to a maintenance
      branch named: {{branchName}}. Issues requiring design decisions or external dependency updates
      should still be reported in an INBOX_MESSAGE_JSON block (from: "argus-weld").
      {{/if}}
    options:
      if_found:
        type: radio
        label: If vulnerabilities are found
        tooltip: "Produce a report or patch what can be fixed automatically"
        value: report
        choices:
          - value: report
            tooltip: Write a report — do not change any files
          - value: fix
            tooltip: Auto-patch safe fixes on a maintenance branch; report the rest

  - id: todo-fixme-scan
    enabled: false
    frequency: per-commit
    safety: direct
    has_safety_options: false
    title: TODO / FIXME / HACK Scanner
    instructions: |
      Scan all source files for TODO, FIXME, HACK, XXX, and NOTE comments.
      For each comment found, create a task entry in `.squad/tasks.md` if one
      does not already exist for that comment. Include the file path, line number,
      and the full comment text in the task description.

      Do not modify any source files. Only append to `.squad/tasks.md`.

  - id: unused-dependencies
    enabled: false
    frequency: daily
    safety: report-only
    has_safety_options: false
    title: Unused Dependency Scan
    instructions: |
      Check for NuGet packages (*.csproj), npm packages (package.json), or other
      dependency manifests in the repository. Identify packages that appear to be
      unused (not referenced in source or only referenced transitively).

      Write a report listing each potentially unused dependency, the manifest file
      it appears in, and a note on how to verify and remove it. Do not change any
      files. Send the report to the user's Inbox using an INBOX_MESSAGE_JSON block
      (from: "argus-weld").

  - id: xml-doc-coverage
    enabled: false
    frequency: daily
    safety: report-only
    has_safety_options: false
    title: XML Doc Comment Coverage
    instructions: |
      Scan all public types, methods, properties, and interfaces in the C# source
      for missing XML doc comments (`<summary>`, `<param>`, `<returns>`).
      Produce a coverage report grouped by file, listing each undocumented member.
      Do not change any code. Send the report to the user's Inbox using an
      INBOX_MESSAGE_JSON block (from: "argus-weld").

      If this is not a C# project, adapt to the equivalent docstring convention
      (JSDoc for TypeScript/JavaScript, docstrings for Python, godoc for Go).

  - id: thematic-compliance
    enabled: true
    frequency: weekly-Saturday
    safety: report-only
    has_safety_options: false
    title: Thematic Compliance Audit
    instructions: |
      Audit the codebase for UI controls that are not respecting the application's
      selected theme (dark/light). This project uses dynamic resource references
      (DynamicResource) and a named style system to support user- or system-selected
      themes. Controls that hardcode colors, use default OS styles, or skip the
      app's named styles break the theme contract.

      **Step 1 — Understand the theme system**
      Before scanning for violations, read enough of the codebase to understand
      how theming works in this project:
      - In WPF projects: look at App.xaml for named styles and brush resource keys
        (e.g., PrimaryText, InputSurface, RosterPanelSurface). Find examples of
        controls using SetResourceReference or DynamicResource correctly.
      - In web projects: look for CSS custom properties, design tokens, or a theme
        provider pattern.
      - Note which control types have custom themed styles (e.g., Button, ComboBox,
        ScrollViewer, ToolTip, ContextMenu, CheckBox) and what the correct style
        key or resource reference pattern is for each.

      **Step 2 — Scan for violations**
      Look for any of the following in C# code-behind (.cs), XAML (.xaml), CSS,
      TypeScript, or equivalent UI files:

      1. **Hardcoded colors** — `Background = Brushes.White`, `Foreground = Colors.Black`,
         `color: #ffffff`, hex/RGB color values set directly on controls rather than
         via a themed resource.
      2. **Default/unstyled controls** — Controls created with `new ToolTip()`,
         `new ComboBox()`, `new ContextMenu()` etc. without applying the app's
         named themed style. Compare against controls that do use the themed style.
      3. **StaticResource instead of DynamicResource** for theme-sensitive brushes —
         StaticResource does not update when the theme switches at runtime.
      4. **Missing style on a control type that has a themed style** — e.g., if
         ComboBoxes elsewhere use `ThemedComboBoxStyle` but one ComboBox does not.
      5. **ToolTips** that use the OS default (plain white/yellow) instead of the
         app's themed tooltip helper or style.
      6. **Inline style overrides** that re-hardcode colors already provided by
         a themed brush (redundant overrides that will break if the theme changes).

      **Step 3 — Report findings**
      Do not change any code. For each violation found, report:
      - File path and approximate line number or method/class anchor
      - Control type and property affected
      - What the current (incorrect) value is
      - What the correct themed equivalent should be (specific style key or
        resource reference)
      - Severity: High (visible in normal use), Medium (edge case or secondary UI),
        Low (cosmetic or rarely seen)

      Group findings by severity. Include a brief summary at the top: how many
      violations found, which control types are most affected, and whether any
      systematic pattern explains the violations (e.g., "all violations are in
      the Settings window, which predates the themed style system").

      Send the full report to the user's Inbox using an INBOX_MESSAGE_JSON block
      (from: "argus-weld").

      **Note:** This task applies to any project with a theme system — WPF apps
      using DynamicResource/named styles, web apps using CSS custom properties or
      design tokens, MAUI/.NET apps, etc. Adapt the scan patterns to the tech stack
      in use.

  - id: daily-issue-pr-tracker
    enabled: true
    frequency: daily
    safety: report-only
    has_safety_options: false
    title: Issue & PR Tracker
    instructions: |
      Query the GitHub API to find new issues posted to the repository in the last
      24 hours. {{#if includePullRequests}}Also include pull requests.{{/if}}{{#if !includePullRequests}}Pull requests are not included in this report.{{/if}}
      
      Use the GitHub CLI (`gh`) to query the repository. The query should:
      - Identify the current repository (use `gh repo view --json nameWithOwner`)
      - Search for issues created in the last 24 hours using GitHub's search syntax
      {{#if includePullRequests}}- Also search for pull requests created in the last 24 hours{{/if}}
      - Retrieve title, number, creation time, and URL for each result
      
      Format the findings into a clear, actionable report:
      1. **Summary**: Total count of new issues{{#if includePullRequests}} and pull requests{{/if}}
      2. **New Issues** (if any):
         - List each with: #<number> Title (created: YYYY-MM-DD HH:MM UTC)
         - Include link: https://github.com/owner/repo/issues/<number>
      {{#if includePullRequests}}
      3. **New Pull Requests** (if any):
         - List each with: #<number> Title (created: YYYY-MM-DD HH:MM UTC)
         - Include link: https://github.com/owner/repo/pull/<number>
      4. **Suggested Next Steps**: Brief recommendations for triage/review
      {{/if}}
      {{#if !includePullRequests}}
      3. **Suggested Next Steps**: Brief recommendations for issue triage
      {{/if}}
      
      Do not change any code or issues. Send the report to the user's Inbox using
      an INBOX_MESSAGE_JSON block (from: "argus-weld").
      
      **Note**: This task is designed to be distributed to other repositories. It
      queries the current workspace's repository dynamically, so it will work on
      any GitHub repository that uses SquadDash.
    options:
      includePullRequests:
        type: checkbox
        label: Include Pull Requests
        tooltip: When enabled, report will include PRs in addition to issues
        value: true
---
