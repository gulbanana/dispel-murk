# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

`dispel-murk` converts mIRC `.log` files of tabletop RPG sessions played over IRC into browsable output: a static HTML site (one page per session plus an index), a single HTML page, plain text, Obsidian Portal wiki HTML, or a "stripped" log with metadata removed.

Version control is **Jujutsu (`jj`)**, not git. There is no `.git` directory; the git remote (`origin`, GitHub `gulbanana/dispel-murk`) lives inside `.jj`. Use `jj` for status/log/diff/commit/push.

## Commands

All projects target `net8.0`; the installed SDK is newer and builds them fine.

```sh
dotnet build dispel-murk.sln
dotnet test dispel-murk.sln                                        # all 33 xUnit tests
dotnet test Dispel.Tests --filter "FullyQualifiedName~Parse.Color" # one test class
dotnet test Dispel.Tests --filter "FullyQualifiedName~CLI.SpecifySingleFile"  # one test
```

Run the CLI (assembly name is `dispel-murk`):

```sh
dotnet run --project Dispel.CommandLine -- -o site path/to/channel.log
```

- Options: `-q` quiet, `-o page|site|text|wiki|stripped|all` (default `site`), `-m` merge all input files into one log/index, `-s` write one output file next to the input (single-file formats only; `-s -m` together is `NotImplementedException`).
- With no log arguments it converts every `*.log` in the current directory.
- **Output files are written to the current working directory**, not next to the input (except `-s`). `config.json` is also read from the current directory. Run from the directory you want output in, e.g. an empty scratch dir, and pass the log path.
- Existing output is only rewritten when content differs; an `OutputFile` with `null` content means "delete this file if present" (used for filtered-out sessions).

Sample inputs with previously generated outputs live in `testcases/` (`.log` → `.html`, `.txt`, `.wiki.html`). They are **not** used by the automated tests; regenerate and diff against them for manual regression checks. `samples/config.json` documents every config key.

## Architecture

Four projects: `Dispel` (library, all the logic), `Dispel.CommandLine` (CLI), `Dispel.Tests` (xUnit), `Dispel.Web` (legacy, see below).

### Pipeline

```
mIRC log lines
  → Engine.GetSessions        reads line by line, classifies each with LineParser
  → LineParser                parser combinators (Dispel.Parse) → Parse.Node tree
  → Engine.Process*           builds the AST: Log > Session[] > Line[] > Message > Run[] > Attribute[]
  → Formats.GetGenerator      picks Text/Web/Wiki/LogGenerator for the OutputFormat
  → OutputFile[]              (filename, content) pairs; the CLI or web layer writes them
```

`Engine` is stateful (current session ident/start/end, pending lines, blank-line counter). Construct a new one per conversion, as the CLI does.

### Parser combinators (`Dispel/Parse`)

A `Parser` is `string → ParseResult`. `Combinators` builds them from regexes:

- `Term(pattern[, extract])` matches an anchored regex (`^` is prepended) and yields a `Terminal` node. Regexes use `IgnorePatternWhitespace`, so literal spaces in patterns are ignored; write `\s`.
- `Sequence` drops `Empty` nodes (from `Skip`/failed `Optional`) and **collapses to the single child if only one remains**, otherwise yields a `Production`. This affects child indices downstream.
- `Any` returns a **1-based `Tag`** saying which alternative matched. `Engine.GetSessions` switches on that tag (1 message, 2 session directive, 3 pragma, 4 server block), so the order of alternatives there is load-bearing.
- `Set` is zero-or-more (`Repetition` node), `RequiredSet` one-or-more, `Decorated(i, ...)` keeps only child `i`.

`LineParser` composes these into the grammar. `Engine.ProcessMessage` reads the `Line` tree **by position** (`Children[0]` timestamp, `[1]` username, `[2]` runs; each run is `{Attributes Repetition, Text Terminal}`). Changing the shape of `LineParser.Line`, `Run` or `Attributes` requires updating those indices.

### Log format handled

- Message lines: `[HH:MM] <nick> text`, where `<nick>` may be wrapped in mIRC colour codes and prefixed with `@`/`+`. The speaker may also be `*` (status/control lines) or `->` (outgoing PMs); both are in `Ignored` by default and dropped.
- Inline formatting uses mIRC control characters: `\x02` bold, `\x1D` italic, `\x1F` underline, `\x03[N[,M]]` colour, `\x0F` reset. Text is split into `Run`s at each attribute change; `WebGenerator` tracks toggle state across runs and emits `b`/`i`/`u`/`c<N>` CSS classes, styled by `Dispel/style.css` (an embedded resource, read as `Dispel.style.css`).
- `Session Start|Ident|Close|Time: ...` lines drive session boundaries. `Start of <ident> buffer: <date>` / `End of ... buffer:` headers (mIRC buffer saves) are also recognised.
- Pragmas `#cmd args` are engine directives; currently only `#img <url>` exists, producing a media-only `Line` rendered as `<img>`.
- A line starting with `-` opens a server-notice block; lines are swallowed until the next `-` line.
- Any other unparseable line throws, aborting the whole conversion.

### Session splitting rules (in `Engine`)

Beyond explicit `Session Start`/`Close`, a session is split when the elapsed time from session start to the latest `Session Time` exceeds `MaxLengthThreshhold` (default 24h), or when `BlankLinesThreshhold` (default 3) consecutive blank-text lines occur; those blanks are removed. A split session inherits the previous ident. A one-session file with no ident is named after the file.

### Output naming

| Format | Files |
|---|---|
| `site` | `{Ident}-{index}.html` per session, plus `index.html` and `style.css`. With `NoIndex`, `{Ident}.html`. |
| `page` | `{logname}.html`, one file with inline CSS, all sessions concatenated |
| `text` | `{Ident}-{index}.txt` per session |
| `wiki` | `{Ident}-{index}.wiki.html` per session |
| `stripped` | `{logname}-stripped.log` |

`FilterOptions` (`IncludeOneOf` / `ExcludeAllOf` by participant) and `Groups`/`Notes` (session index → index-page grouping and annotations) are honoured only by `WebGenerator`.

### Configuration

`DispelOptions` is bound from `config.json` via `Microsoft.Extensions.Configuration`, so JSON keys must match property names exactly, including the existing misspelling `BlankLinesThreshhold` / `MaxLengthThreshhold`. `GM` (default `"banana"`) is sorted first in participant lists.

## Gotchas

- In C# string literals `\x` consumes up to four hex digits, so `"\x02foo"` is **not** bold-then-`foo`. Tests write control codes as `"\x0002foo"`, `"\x0003"` etc. Follow that.
- The `CLI` test class sets `Environment.CurrentDirectory` to the temp folder and invokes `Program.Main` directly. Don't add tests elsewhere that depend on the process working directory; xUnit runs classes in parallel.
- `Dispel.Web` compiles but is bit-rotted and not functional end to end: the form posts to a nonexistent `Convert` page, offers the format value `html` which `Formats.Parse` rejects (it wants `page`), and `IndexModel` injects `IOptions<DispelOptions>` that `Startup` never registers. Treat it as legacy unless asked to revive it.
