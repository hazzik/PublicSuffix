using System;
using System.Collections.Generic;
using System.Linq;

namespace Brandy.PublicSuffix
{
    internal class Rule
    {
        private int? _length;
        private readonly IDictionary<string, Rule> _children;

        public Rule(int? length, IDictionary<string, Rule> children)
        {
            _length = length;
            _children = children;
        }

        private bool TryGetChildren(string label, out Rule entry)
        {
            return _children.TryGetValue(DomainParser.IdnMapping.GetAscii(label), out entry) ||
                   _children.TryGetValue("*", out entry);
        }

        public Domain Parse(string[] labels)
        {
            var parseLength = _length.GetValueOrDefault(1);
            var publicSuffix = GetPublicSuffix(parseLength, labels);
            var domain = GetRegisterableDomain(parseLength, labels);
            var subdomain = GetSubdomain(parseLength, labels);

            return new Domain(publicSuffix, domain, subdomain);
        }

        private static string GetSubdomain(int parseLength, string[] labels)
        {
            return String.Join(".", labels.Skip(parseLength + 1).Reverse()
#if NET35
                    .ToArray()
#endif
            ).ToLower();
        }

        private static string GetPublicSuffix(int parseLength, string[] labels)
        {
            return String.Join(".", labels.Take(parseLength).Reverse()
#if NET35
                    .ToArray()
#endif
            ).ToLower();
        }

        private static string GetRegisterableDomain(int parseLength, string[] labels)
        {
            return labels.Skip(parseLength).Select(x => x.ToLower()).FirstOrDefault();
        }

        public Rule FindMatchingRule(string[] labels)
        {
            var rule = this;
            foreach (var label in labels)
            {
                if (String.IsNullOrEmpty(label))
                    break;

                if (!rule.TryGetChildren(label, out var entry))
                    break;

                rule = entry;
            }

            return rule;
        }
    }
}