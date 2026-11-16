# Check-in quick entry

Front-desk operators may type a bare member code/phone or a compact command such as
`checkinMEM-1001`, `check in MEM-1001`, or `checkin0901234567`.

The shared application-layer `CheckInQuickEntryParser` applies a small domain-specific
vocabulary and memoized segmentation to extract an explicit member identifier.
It does not depend on a dictionary file, call any external services, or perform
fuzzy matching. Unknown and ambiguous inputs are preserved rather than guessed.

The prototype check-in screen uses the parser **only during member lookup**.
The existing membership eligibility, tenant/branch authorization, subscription,
duplicate-check-in, and attendance recording rules remain unchanged.

## Decisions

- Keep parsing separate from business validation (single responsibility).
- Require a recognizable member-code or numeric identifier after a command.
- Preserve raw identifiers and unknown input to avoid surprising matches.
- Cap parsing at 128 input characters and memoize unsuccessful suffixes.
- This is explicit command parsing, not general natural-language understanding.

## Example

| Input | Lookup |
| --- | --- |
| `MEM-1001` | `MEM-1001` |
| `checkinMEM-1001` | `MEM-1001` |
| `check in MEM-1001` | `MEM-1001` |
| `checkin0901234567` | `0901234567` |
| `checkinUNKNOWN` | `checkinUNKNOWN` (unrecognized) |
