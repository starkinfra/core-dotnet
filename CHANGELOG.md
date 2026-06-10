# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
and this project adheres to the following versioning pattern:

Given a version number MAJOR.MINOR.PATCH, increment:

- MAJOR version when the **API** version is incremented. This may include backwards incompatible changes;
- MINOR version when **breaking changes** are introduced OR **new functionalities** are added in a backwards compatible manner;
- PATCH version when backwards compatible bug **fixes** are implemented.

## [Unreleased]

## [0.2.1] - 2026-06-10
### Fixed
- support for apps built with InvariantGlobalization=true: replaced `new CultureInfo("en-US")` with `CultureInfo.InvariantCulture` in Request.cs (Access-Time header) and made all other request-path formatting/parsing culture-invariant (StarkDate/StarkDateTime, EndToEndId/ReturnId timestamps, Checks date parsing, Url query values), which previously threw `CultureNotFoundException` on .NET 6+ invariant-mode runtimes; output is byte-identical to the previous en-US formatting

## [0.2.0] - 2026-03-03
### Added
- Put method

## [0.1.1] - 2025-04-22
### Fixed
- PostSubResource method

## [0.1.0] - 2024-07-01
### Added
- restRaw methods
- restRaw tests

## [0.0.9] - 2024-01-05
### Added
- basic functionalities for StarkBank and StarkInfra SDKs