## REXJ Standalone Add-ins for Revit 2027 — v1.1.2

### What's Changed

**Legal Compliance Check (#09 — CheckingALVS):**
- Weave-compliant WPF dialogs for **Daylight Check**, **Smoke Exhaust Check**, and **Ventilation Check**
- Smoke and ventilation checks now use the same Weave UI as daylight (replaces legacy WinForms)
- Fixed ceiling height unit consistency for smoke calculations (room volume/area height now uses project length units)
- Smoke effective height and area calculations align with head height and smoke wall values in mm/m projects
- Improved property-line horizontal distance measurement and bulk-update behavior for daylight openings
- Imperial and mixed-unit project support for area and distance calculations

---

### Per-Function Installer (Updated)

| # | Installer | Function | Description | Version | Last Updated |
|---|-----------|----------|-------------|---------|--------------|
| **09** | [**Download**](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.1.2/REXJ_09_LegalComplianceCheck_Setup_1.1.2.exe) | **Legal compliance check (Weave-Compliant)** | **Weave WPF dialogs for Daylight, Smoke, and Ventilation Check with unit fixes** | **1.1.2** | **2026-07-13** |

---

### Installation

1. Download the installer above
2. Close Revit 2027 if running
3. Run the installer — files deploy to the Revit 2027 Addins folder automatically
4. Launch Revit 2027 — tools appear under the **REXJ Standalone** ribbon tab

### Notes

- Set **Opening Coefficient** on window/door family types for smoke and ventilation effective area calculations
- The full bundle installer has not been rebuilt for v1.1.2; install this per-function update for CheckingALVS
