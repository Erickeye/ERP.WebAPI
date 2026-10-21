# Issue tracker: Local Markdown

Issues and specs for this repo live as markdown files in `.scratch/`.

## Conventions

- One feature per directory: `.scratch/<feature-slug>/`
- The spec is `.scratch/<feature-slug>/spec.md`
- Implementation issues are one file per ticket at `.scratch/<feature-slug>/issues/<NN>-<slug>.md`, numbered from `01`, never a single combined tickets file
- Triage state is recorded as a `Status:` line near the top of each issue file (values and when to change them: see "Status values" and "On completion" below)
- Comments and conversation history append to the bottom of the file under a `## Comments` heading

## Status values

- `ready-for-agent`: the spec or ticket is confirmed and implementation can start.
- `claimed`: someone (an agent) has started it (used by wayfinding).
- `resolved`: done.
- `deferred`: deliberately postponed, not being done now. The reason must be recorded.

## On completion

When implementation is finished (after `implement` and `code-review`) or the work is deliberately postponed, come back and update the records. No skill does this for you.

**One-commit rule.** Updates to the ticket and spec (status, ticked criteria, completion record) go in the **same commit** as the work they describe. Never make a separate "docs: update ticket status" commit. Order: finish implementation and tests → update the records → commit once → `code-review`. If the review leads to fixes, update the records again and include them in the fix commit.

1. Set the ticket's `Status:` to `resolved`, or to `deferred` if it is postponed.
2. Tick acceptance criteria only when there is evidence: passing tests, a review confirmation, or hands-on verification. Leave unverified ones unticked and say why in Comments (for example "frontend not verified manually in a browser").
3. Append a completion record to the ticket's `## Comments`: date, how it was verified and the result, and why any criteria remain unticked. Do not write the commit hash: the record is committed together with the code, so the hash does not exist yet (find it later with `git log -- <ticket path>`). For a postponed ticket, write a status record instead: who decided, why, and where to resume.
4. When every ticket under a spec is `resolved` or `deferred`, set the spec's `Status:` to `resolved` and note which tickets were deferred.
5. For a feature with only a spec and no tickets, set the spec's `Status:` to `resolved` when it is done.

## When a skill says "publish to the issue tracker"

Create a new file under `.scratch/<feature-slug>/` (creating the directory if needed).

## When a skill says "fetch the relevant ticket"

Read the file at the referenced path. The user will normally pass the path or the issue number directly.

## Wayfinding operations

Used by `/wayfinder`. The **map** is a file with one **child** file per ticket.

- **Map**: `.scratch/<effort>/map.md` (the Notes / Decisions-so-far / Fog body).
- **Child ticket**: `.scratch/<effort>/issues/NN-<slug>.md`, numbered from `01`, with the question in the body. A `Type:` line records the ticket type (`research`/`prototype`/`grilling`/`task`); a `Status:` line records `claimed`/`resolved`.
- **Blocking**: a `Blocked by: NN, NN` line near the top. A ticket is unblocked when every file it lists is `resolved`.
- **Frontier**: scan `.scratch/<effort>/issues/` for files that are open, unblocked, and unclaimed; first by number wins.
- **Claim**: set `Status: claimed` and save before any work.
- **Resolve**: append the answer under an `## Answer` heading, set `Status: resolved`, then append a context pointer (gist + link) to the map's Decisions-so-far in `map.md`.
