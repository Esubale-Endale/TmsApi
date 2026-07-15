# TmsApi Versioning Policy

## What counts as a breaking change:
removing a field, renaming a field, changing a status code, tightening validation, changing a default sort order.

## What counts as additive (non-breaking):
adding a new optional field, adding a new endpoint, adding a new optional query parameter.

## Sunset window:
<!-- how long V1 keeps running after V2 ships. -->
The TMS commits to 6 months minimum so rural training centres on quarterly maintenance schedules can migrate.

## Communication:
Deprecation / Sunset / Link headers from day one of V2;
a CHANGELOG entry;
an email to every team that holds an API key;
a calendar invite for the V1 shutdown date.

## Skipping versions: 
is Allowed.
<!-- V1 → V3 is allowed; clients are not forced to migrate through every intermediate version. -->