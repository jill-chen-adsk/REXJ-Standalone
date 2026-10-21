# Feature Summary — View Duplicate

> **Deck type:** Final Feature Summary (REXJ Standalone **v1.1.7, planned**, Revit 2027)  
> **PPTX master (Save As first):** `Feature Summary Template - SAVEAS.pptx` (Revit org-wide Final Feature Summary template)  
> **Scope:** **View Duplicate** (`ADSK.ViewExtension.ViewDuplicate`) — main dialog + **Add rule** dialog, Weave WPF UI  
> **Vocabulary:** Ribbon button **View Copy** vs dialog title **View Duplicate**. In the dialog, **View category** = Revit view kind (Floor Plan, Section, …); **View type** = view family type (e.g. a "Mechanical Plan" type). Keep the two separate on every slide.

---

## Internal only — Customer problem (not numbered, do not circulate)

**Why View Duplicate exists**

Project teams build parallel view sets from one base set: for example, floor plans L1–L5 copied into Mechanical, Electrical, and Plumbing working views, each with its own view type, view template, and naming convention.

In Revit alone, each copy is a multi-step task:

- Duplicate the view (names come out as *"L2 Copy 1"*)
- Rename it to the office convention
- Change its view type
- Assign the discipline view template

Repeat per level and per discipline. Five levels × three disciplines is 15 new views, each needing a rename and a type/template check. Errors are easy to miss: a skipped template, an inconsistent prefix, a name clash that leaves a default name behind.

**View Duplicate turns that into one dialog:** pick the source views once, define one rule per target set (text to add, view type, view template), and create every copy in a single run with consistent names.

*(Illustrative example from the tool's workflow; no customer data on this slide.)*

---

## Slide 1 — Cover

**View Duplicate**

*Batch view duplication with naming, view type, and view template rules*  
*REXJ Standalone · Views panel · Revit 2027 · Weave-compliant UI*

`-- 1 of 7 --`

---

## Slide 2 — Intro

**View Duplicate**

**View Duplicate** creates multiple copies of selected views in one step. Users pick source views, define one or more duplication rules, and the tool creates one new view per source view per rule — named with the rule's prefix or suffix, set to the rule's **View type**, and assigned the rule's **View template**.

*(Before: duplicate, rename, change type, and assign a template view by view. Now: one dialog run for the whole set.)*

With View Duplicate, users can:

- Filter source views by **Discipline**, **View category**, and **View type**, then multi-select in **Views to duplicate** (Shift / Ctrl)
- Choose **Duplicate** or **Duplicate with detailing**
- Add several rules in the **Duplication rules** table to produce several view sets from the same sources in one run
- Get unique names automatically: if a name already exists, a number is appended, e.g. *M-L2(1)*
- Review every view that could not be duplicated in one message; other copies are kept

`-- 2 of 7 --`

---

## Slide 3 — Behavior detail — Select source views

**View Duplicate**

**Narrow the list, then multi-select**

Click **View Copy** on the **Views** panel. In the **View Duplicate** dialog, choose **Discipline** and **View category** (e.g. Floor Plan), and optionally a **View type** (or **\<All\>**). **Views to duplicate** lists matching views in alphabetical order; select one or many with Shift or Ctrl. Set **Duplicate mode** to **Duplicate** or **Duplicate with detailing**.

*(Templates and views that Revit cannot duplicate are not listed. The last filters, mode, and dialog position are remembered.)*

`[SCREENSHOT]` Ribbon **Views** panel → **View Copy**; Weave dialog with Mechanical / Floor Plan filters and L2–L5 selected.

`-- 3 of 7 --`

---

## Slide 4 — Behavior detail — Duplication rules

**View Duplicate**

**One rule per output view set**

The **Duplication rules** table shows **Prefix / suffix**, **Added text**, **View template**, and **View type** for each rule. Click **Add** to create a rule, select rows and click **Delete** to remove them. Double-click **Added text** to edit it in place; press Enter to commit.

*(Each selected view is copied once per rule: 4 views × 3 rules = 12 new views.)*

`[SCREENSHOT]` Table with three rules (e.g. M-, E-, P- prefixes with matching templates).

`-- 4 of 7 --`

---

## Slide 5 — Behavior detail — Add rule

**View Duplicate**

**Set view type, template, and naming**

**Add** opens a rule dialog. Choose the target **View type** (types in the same view family as the source views). Choose a **View template** for the same view category; if no template is available, the list is empty and the rule is shown with **\<None\>** in the table. Pick **Prefix** or **Suffix** and enter **Text to add**; the field is pre-filled with the template name and can be edited.

*(If the chosen view type assigns its own template on creation, **View template** shows that template and is locked. Prefix/Suffix and the last view type are remembered.)*

`[SCREENSHOT]` Add rule dialog: View type dropdown, View template dropdown, Prefix/Suffix panel with text.

`-- 5 of 7 --`

---

## Slide 6 — Behavior detail — Run and results

**View Duplicate**

**Checked before, reported after**

On **OK**, the tool checks that views are selected, at least one rule exists, every rule has **Added text**, and each rule's view type matches the view family of each source view. Any problem is listed in one message before anything is created.

During the run, each copy is created in its own step. If one copy fails, only that copy is undone; the rest are kept, and a single message lists the views that could not be copied.

*(All copies are part of one Revit undo step.)*

`[SCREENSHOT]` Project Browser showing new views (e.g. M-L2 … M-L5, E-L2 … E-L5); optional summary message for a failed view.

`-- 6 of 7 --`

---

## Slide 7 — Feedback & Status (always last)

**Feedback & Status**

### Development context

View Duplicate's Weave dialogs, run reporting, and naming fixes are implemented and targeted for REXJ Standalone **v1.1.7** (planned); final validation is in progress.

### Known Limitations

- Runs on the **active document**; one **Discipline** and **View category** per run.
- A rule's **View type** must belong to the same view family as the source views; mismatches are blocked before the run.
- Dependent views (**Duplicate as Dependent**) are not offered.
- Only the first selected view is remembered between sessions, not the full multi-selection.
- Shift-range selection in **Views to duplicate** has been reported as slow in some sessions; under investigation.

### Requested Feedback

- How many duplication rules do you typically need in one run, and for which disciplines?
- Should the dialog remember the full set of selected views between sessions?
- Is the default naming for duplicates, e.g. *M-L2(1)*, acceptable for your office standards, or do you need a different pattern?
- Would you use **Duplicate as Dependent** if it were offered as a third mode?
- Please let us know if you encounter any difficulties.

`-- 7 of 7 --`

---

## PPTX assembly

1. **Save As** → `Feature Summary_View Duplicate.pptx`
2. **Internal-only Customer problem** slide: add before the cover for internal reviews only; delete before sharing externally. Not counted in slide numbers.
3. **Cover** (Slide 1) + **Intro** (Feature Title + Text, Slide 2).
4. **Behavior:** duplicate the **IMAGE** slide **four** times (Slides 3–6).
5. **Feedback & Status** (Slide 7).
6. Export PDF with matching stem.

**Sources:** `ViewDuplicate` (`App.cs` ribbon **View Copy**, `FormViewDuplicateWPF`, `FormViewDuplicateItemWPF`, `ItmDuplicateRule`, `Text.en.resx`); `INSTALLATION_GUIDE.md` §17; sibling decks read for tone/structure: `Feature-Summary-Join-Adjustment.md`, `Feature-Summary-Auto-Floor.md` (v1.1.5 era).
