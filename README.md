PublicSuffix
============
PublicSuffix - Highly optimized Domain Name parser using Mozilla's Public Suffix List (https://publicsuffix.org/)

Installation
============
Download the latest release from the [NuGet repository](https://www.nuget.org/packages/Brandy.PublicSuffix/)

```PowerShell
Install-Package Brandy.PublicSuffix
```

Examples
========
```csharp
var parser = DomainParser.Default; // Loads rules from https://publicsuffix.org/list/effective_tld_names.dat
var domain = parser.Parse("xxx.www.takahama.aichi.jp");

Assert.IsTrue(domain.IsValid);
Assert.AreEqual("www.takahama.aichi.jp", domain.RegistrableDomain);
Assert.AreEqual("xxx", domain.Subdomain);
Assert.AreEqual("takahama.aichi.jp", domain.PublicSuffix);
```
