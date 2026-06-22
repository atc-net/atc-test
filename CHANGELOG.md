# Changelog

## [3.0.0](https://github.com/atc-net/atc-test/compare/v2.0.17...v3.0.0) (2026-06-22)


### ⚠ BREAKING CHANGES

* drop netstandard2.1/net8.0/net9.0; target net10.0 exclusively
* **deps:** the assertion package and namespace changed from FluentAssertions to AwesomeAssertions. Consumers must replace `using FluentAssertions;` with `using AwesomeAssertions;` (and the .Equivalency/.Primitives/.Execution/.Extensions sub-namespaces) and remove any direct FluentAssertions package reference.

### Features

* **deps:** replace FluentAssertions with AwesomeAssertions 9.x ([f5b721b](https://github.com/atc-net/atc-test/commit/f5b721b2272ef3aa8a9899ee1a66f9d16ce78858))
* drop netstandard2.1/net8.0/net9.0; target net10.0 exclusively ([57e22ef](https://github.com/atc-net/atc-test/commit/57e22efea159248a404e98e31d9173d104592f7b))
