# REXJ Standalone — Documentation

Product and portfolio documentation for the REXJ program. These files are meant to be edited collaboratively via pull requests.

## Insights Hub (styled review)

The portfolio review uses an **Insights Hub** layout — sticky sidebar navigation, deep links (`#section`), and scroll highlighting — similar to internal FCC Insights Hub pages.

| Link | Use |
|------|-----|
| [REXJ_Review_Done_and_Remains.html](./REXJ_Review_Done_and_Remains.html) | Local file / repo path |
| [Pages hub (after enable)](https://pages.git.autodesk.com/chenji/REXJ-Standalone/REXJ_Review_Done_and_Remains.html) | **Share with collaborators** |
| [Deep link example](https://pages.git.autodesk.com/chenji/REXJ-Standalone/REXJ_Review_Done_and_Remains.html#pillar-operating-model) | Jump to Operating model pillar |

### Enable GitHub Pages (one-time)

In **git.autodesk.com/chenji/REXJ-Standalone** → **Settings** → **Pages**:

1. **Source:** Deploy from branch `main`
2. **Folder:** `/docs`
3. Save — site publishes at `https://pages.git.autodesk.com/chenji/REXJ-Standalone/`

`docs/index.html` redirects to the review page and preserves hash links.

### Section anchors (for sharing)

| Section | Hash |
|---------|------|
| Overview | `#overview` |
| Headline | `#headline` |
| Four pillars | `#four-pillars` |
| Product catalog | `#pillar-product-catalog` |
| Adoption analytics | `#pillar-adoption-analytics` |
| Operating model | `#pillar-operating-model` |
| Recharge | `#pillar-recharge` |
| Two-year arc | `#two-year-arc` |
| Roadmap levers | `#roadmap-levers` |
| PM delivered vs blocked | `#pm-delivered` |
| Communication initiatives | `#communication-initiatives` |
| Self-review narrative | `#self-review-narrative` |

## Documents

| Document | Purpose | Edit |
|----------|---------|------|
| [REXJ_Review_Done_and_Remains.md](./REXJ_Review_Done_and_Remains.md) | Achievement vs remaining gaps (APAC PM portfolio review) | **Preferred** — edit in GitHub or your IDE |
| [REXJ_Review_Done_and_Remains.html](./REXJ_Review_Done_and_Remains.html) | Insights Hub styled view | Update after changing the `.md` |

## How to contribute

1. **Edit the Markdown file** — `REXJ_Review_Done_and_Remains.md` is the easiest file to update on GitHub (click the pencil icon on the file page).
2. **Sync the HTML view** — copy updated metrics, pillar text, and lever rows into the HTML file so the visual summary stays current.
3. **Open a pull request** — describe what changed (e.g. updated MAU, closed a gap, new communication initiative).

For local preview, open `docs/REXJ_Review_Done_and_Remains.html` in a browser.

## Related docs (repo root)

Presentation and sharing-session materials live at the repository root:

- `REXJ_Standalone_Presentation.html`
- `REXJ_Standalone_Sharing_Session.md`
- `INSTALLATION_GUIDE.md` / `INSTALLATION_GUIDE.html`
