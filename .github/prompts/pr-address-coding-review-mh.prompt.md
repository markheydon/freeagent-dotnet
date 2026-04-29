---
name: pr-address-coding-review-mh
description: Review all open comment threads on a pull request, fix the issues raised, reply to each thread explaining what was done, and resolve the conversation.
argument-hint: PR number to address, e.g. "PR 25" or "PR 25 in owner/repo"
---

## When to use this prompt

- **When a PR has received a code review** with open comment threads.
- **Before merging** — ensures no review feedback is silently ignored.
- **Time:** 5–15 minutes depending on the number of threads and complexity of fixes.

## What you'll get

- All open review threads read and understood.
- Code changes applied for each actionable thread.
- A reply posted into each thread explaining what was changed (or why no change was made).
- Each thread marked as **Resolved**.
- A summary table of everything done.

## What comes next

After running this prompt:
- Check the PR on GitHub to confirm all threads show as resolved.
- If any threads were left unresolved (fallback path triggered), review the provided reply text and paste it manually.
- Once all threads are resolved, the PR is ready to merge.

---

## Step 1 — Identify the PR

If a PR number was provided, use it. If not, ask:
> "Which PR number should I address the review for? And which repo (owner/repo) if not the current one?"

Confirm the PR number, owner, and repo name before proceeding. You will need all three for the GraphQL queries in Step 2.

---

## Step 2 — Read all open review threads

Use `@github` to fetch all open (unresolved) review comment threads on the PR.

For each thread, extract and list:
- **Thread number** (sequential, 1-based — used as a reference throughout)
- **File and line number**
- **Reviewer comment** (full text)
- **Category** (one of: `fix-required`, `suggestion`, `question`, `nitpick`, `praise`)

Present this list as a table and confirm with the developer before making any changes:

```
| # | File | Line | Category | Summary of comment |
|---|------|------|----------|--------------------|
| 1 | src/Foo.cs | 42 | fix-required | Variable name is unclear |
```

If there are no open threads, report that and stop.

---

## Step 3 — Get thread node IDs via gh CLI

Thread replies and resolving require GraphQL mutations that need node IDs not returned
by the standard read tools. Run the following in the terminal to retrieve them:

```bash
gh api graphql -f query='
  query($owner: String!, $repo: String!, $pr: Int!) {
    repository(owner: $owner, name: $repo) {
      pullRequest(number: $pr) {
        id
        reviewThreads(first: 50) {
          nodes {
            id
            isResolved
            comments(first: 1) {
              nodes {
                id
                body
              }
            }
          }
        }
      }
    }
  }' -f owner=OWNER -f repo=REPO -F pr=PR_NUMBER
```

Replace `OWNER`, `REPO`, and `PR_NUMBER` with the actual values confirmed in Step 1.

From the output, build a mapping table of unresolved threads only:

```
| # | Thread node ID (PRRT_...) | First comment node ID (PRRC_...) |
|---|---------------------------|----------------------------------|
| 1 | PRRT_abc123               | PRRC_xyz789                      |
```

Also note the **PR node ID** (`pullRequest.id`) — you will need it for posting replies.

> If `gh` is not installed, not authenticated, or the token lacks `pull_requests: write`
> scope, stop here and report the issue. The mutations in Step 4 will fail without it.
> The user can check with: `gh auth status`

---

## Step 4 — Address each thread in order

Work through threads one at a time using the mapping from Step 3.
For each thread, first make any code changes, then post the reply, then resolve.

### If category is `fix-required` or `suggestion`:

1. **Make the code change.** If the fix is ambiguous, state your interpretation before applying it.
2. **Post an in-thread reply** using the first comment node ID as `inReplyTo`:

```bash
gh api graphql -f query='
  mutation($prId: ID!, $body: String!, $inReplyTo: ID!) {
    addPullRequestReviewComment(input: {
      pullRequestId: $prId,
      body: $body,
      inReplyTo: $inReplyTo
    }) {
      comment { id }
    }
  }' \
  -f prId="PR_NODE_ID" \
  -f body="Fixed. [One sentence describing what was changed and where.]" \
  -f inReplyTo="PRRC_FIRST_COMMENT_NODE_ID"
```

3. **Resolve the thread** using the thread node ID:

```bash
gh api graphql -f query='
  mutation($threadId: ID!) {
    resolveReviewThread(input: { threadId: $threadId }) {
      thread { isResolved }
    }
  }' \
  -f threadId="PRRT_THREAD_NODE_ID"
```

---

### If category is `question`:

1. **Post an in-thread reply** answering the question based on the code and context.
   If the question implies a code change is needed, make it first and reference it in the reply.
   Use the same `addPullRequestReviewComment` mutation as above.
2. **Resolve the thread** using `resolveReviewThread`.

---

### If category is `nitpick`:

1. Apply the change if it is trivially safe (e.g. rename, formatting, comment wording).
2. **Post an in-thread reply:**
   - If applied: `Applied. [Brief note on what was changed.]`
   - If not applied: `Noted but not changed — [brief reason].`
   Use the same `addPullRequestReviewComment` mutation as above.
3. **Resolve the thread** using `resolveReviewThread`.

---

### If category is `praise`:

1. **Post an in-thread reply:** `Thanks for the kind words!`
   Use the same `addPullRequestReviewComment` mutation as above.
2. **Resolve the thread** using `resolveReviewThread`.

---

## Step 5 — Summary

When all threads have been addressed, output a summary table:

```
| # | File | Comment summary | Action taken | Resolved |
|---|------|-----------------|--------------|----------|
| 1 | src/Foo.cs | Variable name unclear | Fixed: renamed to `customerId` | ✅ |
| 2 | src/Bar.cs | Why is this nullable? | Answered: nullable because... | ✅ |
```

Then state: `All [n] threads resolved. PR is ready for final review before merge.`

---

## Rules

- **Never resolve a thread without posting an in-thread reply first.** The reply is the audit trail.
- **Never use top-level PR comments as a substitute for thread replies.** They do not satisfy the audit requirement and will not associate with the correct thread.
- **Never silently skip a thread.** If a fix cannot be applied (out of scope, disagree), post an in-thread reply explaining why, then resolve.
- **Do not make unrequested changes** to files not referenced in a review thread.
- **One reply per thread** — do not post multiple comments on the same thread.

---

## Fallback — if gh CLI mutations fail

If the `addPullRequestReviewComment` or `resolveReviewThread` mutations fail (auth error,
scope error, or unexpected API error):

1. **Do not resolve any threads.**
2. **Do not post top-level PR comments as a substitute.**
3. Apply the code fixes as normal.
4. Output the exact reply text for each thread so the developer can paste it manually:

```
Thread #1 (PRRT_abc123)
Reply: Fixed. Renamed variable from `x` to `customerId` in src/Foo.cs line 42.
Action: Resolve manually after pasting reply.

Thread #2 (PRRT_def456)
Reply: Answered: this is nullable because the value is not available until after the user completes onboarding.
Action: Resolve manually after pasting reply.
```

5. Report the failure reason so the developer can fix authentication if needed:
   `gh auth status` to check, `gh auth refresh -s repo` to add missing scope.