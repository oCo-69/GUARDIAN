# Our Method — Archive

The initial method was to:

1. start from real problems observed in the library;
2. avoid irreversible transformations;
3. produce short prototypes;
4. verify results against a representative collection;
5. document Decisions;
6. evolve the architecture when prototypes exposed a limitation.

This approach led from PowerShell scripts to the design of a structured Windows application.

The current method preserves this spirit with additional discipline:

```text
Problem → invariant → optional ADR → test → implementation → validation
```
