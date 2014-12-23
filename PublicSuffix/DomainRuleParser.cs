using System;
using System.Linq;

namespace Brandy.PublicSuffix
{
    public class DomainRuleParser
    {
        private readonly string[] _strings;

        public DomainRuleParser(string[] strings)
        {
            _strings = strings;
        }

        public Domain ParseDomain(string[] labels)
        {
            Array.Reverse(labels);
            var domain = GetRegisterableDomain(labels);
            var publicSuffix = GetPublicSuffix(labels);

            return new Domain
            {
                PublicSuffix = publicSuffix,
                RegisterableDomain = string.IsNullOrEmpty(domain) ? null : domain + "." + publicSuffix,
            };
        }

        private string GetRegisterableDomain(string[] labels)
        {
            if (string.IsNullOrEmpty(labels.Last())) return null;
            return labels.Skip(_strings.Length).Select(x => x.ToLower()).FirstOrDefault();
        }

        private string GetPublicSuffix(string[] labels)
        {
            return string.Join(".", labels.Take(_strings.Length).Reverse()).ToLower();
        }
    }
}