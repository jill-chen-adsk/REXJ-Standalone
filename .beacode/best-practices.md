# REXJ Standalone Code Review Guidelines

These guidelines apply to the REXJ Standalone Revit 2027 add-in suite in this repository.

## Architecture

- Each tool is a standalone Revit add-in in its own folder (for example `ExportExcel/`, `AreaSchedule/`).
- Tools must remain self-contained. Do not reintroduce `JExtCom` or other legacy Japanese-framework dependencies.
- Build with configuration `Release 2027` and platform `x64`.
- Solution entry point: `REXJ-Standalone.sln`.

## Revit API

- All model mutations must occur inside a Revit `Transaction`.
- WPF dialogs must be parented to Revit's main window. Follow existing `IWeaveChromeWindow` / ownership patterns.
- Use `ExternalEvent` and `IdlingHandler` patterns for UI-thread to Revit-thread communication.
- Shared parameter binding and other persistence operations must run inside transactions.

## UI / Weave Design System

- Prefer `@weave-mui/*` components over stock `@mui/material` when a Weave equivalent exists.
- Do not override fonts, padding, spacing, or colors on Weave components.
- Use `theme.palette.*` or semantic design tokens. Never hardcode hex colors.
- Include `CssBaseline` in Weave-themed UIs.
- Consult Weave design system documentation before introducing new UI patterns.

## Localization

- User-facing strings belong in `.resx` resource files (for example `Text.resx`, `Text.en.resx`), not hardcoded in source.
- Preserve Japanese originals when adding or updating English strings.
- Use satellite assemblies for localization; do not break existing Japanese support.

## Security

- Never hardcode secrets, API keys, tokens, or credentials. Use environment variables or secure configuration.
- Do not use user input directly in file paths, shell commands, or dynamic `require()`/`import` calls.
- Validate and sanitize external inputs before use in logic, queries, or file access.

## Build and Deploy

- Build output lands in `bin\x64\Release 2027\`.
- Deploy via `.claude/skills/run-rexj/deploy.ps1` to `C:\REXJ\Standalone\Released\<Tool>\`.
- Revit must be closed before deploying; DLLs are locked while Revit is running.
- Configuration name must be exactly `"Release 2027"` (includes a space).

## Testing Expectations

- Verify changed tools load in Revit 2027 after deploy.
- Check both English UI strings and dialog behavior (visibility, modal ownership, theme).
- For geometry or calculation changes, validate with representative Revit model inputs.
