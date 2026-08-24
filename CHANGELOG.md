# Changelog

## [3.1.1](https://github.com/atc-net/atc-test/compare/v3.1.0...v3.1.1) (2026-08-24)


### Bug Fixes

* **deps:** downgrade NSubstitute to 5.3 again - wating on AutoFixture v5.0 to be released to avoid NU1608 issues ([9d35c3a](https://github.com/atc-net/atc-test/commit/9d35c3ab4395b08856c7fd103324056884d9d429))

## [3.1.0](https://github.com/atc-net/atc-test/compare/v3.0.0...v3.1.0) (2026-08-24)


### Features

* Add TimeOnly, Uri and TimeProvider specimen generators ([3882e07](https://github.com/atc-net/atc-test/commit/3882e073de0c16ebd6f277227fe80eb9f10096a3))
* **deps:** Upgrade xunit packages to v4.0.0 ([47c25af](https://github.com/atc-net/atc-test/commit/47c25af0108ea8e590804198a5899ef877cc352d))

## [3.0.0](https://github.com/atc-net/atc-test/compare/v2.0.17...v3.0.0) (2026-06-22)


### ⚠ BREAKING CHANGES

* drop netstandard2.1/net8.0/net9.0; target net10.0 exclusively
* **deps:** the assertion package and namespace changed from FluentAssertions to AwesomeAssertions. Consumers must replace `using FluentAssertions;` with `using AwesomeAssertions;` (and the .Equivalency/.Primitives/.Execution/.Extensions sub-namespaces) and remove any direct FluentAssertions package reference.

### Features

* **deps:** replace FluentAssertions with AwesomeAssertions 9.x ([f5b721b](https://github.com/atc-net/atc-test/commit/f5b721b2272ef3aa8a9899ee1a66f9d16ce78858))
* drop netstandard2.1/net8.0/net9.0; target net10.0 exclusively ([57e22ef](https://github.com/atc-net/atc-test/commit/57e22efea159248a404e98e31d9173d104592f7b))
