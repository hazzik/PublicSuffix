using System;

namespace Brandy.PublicSuffix
{
    public sealed class Domain
    {
        private readonly string _domain;
        private readonly string _toString;

        public Domain(string publicSuffix, string domain, string subdomain)
        {
            PublicSuffix = publicSuffix ?? throw new ArgumentNullException(nameof(publicSuffix));
            var registrableDomain = string.IsNullOrEmpty(domain) ? null : domain + "." + publicSuffix;
            _domain = domain;
            Subdomain = subdomain;
            RegistrableDomain = registrableDomain;
            _toString = subdomain == null
                ? registrableDomain
                : subdomain + "." + registrableDomain;
        }

        public bool IsValid => !string.IsNullOrEmpty(_domain);

        public string PublicSuffix { get; }

        public string RegistrableDomain { get; }

        public string Subdomain { get; }

        public override string ToString()
        {
            return _toString;
        }
    }
}