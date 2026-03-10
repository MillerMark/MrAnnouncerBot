# Squad Decisions

## Active Decisions

### Speech Bubble Fix (Trinity, 2026-03-09)
`GetNameAndPhrase` now uses the first separator (space or colon) so `!mark says: hello` correctly extracts `mark` as the character name. User-level gate (`minUserLevelForSpeechBubbles = 4`) restored in `SayOrThinkIt`. Known pre-existing issue: rejection messages for DroneCommands users go to CodeRushed channel — not fixed here.

> Source: `.squad/decisions/inbox/trinity-speech-bubble-fix.md` (resolved)

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
