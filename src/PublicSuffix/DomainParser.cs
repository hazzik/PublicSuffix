using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;

#if !NET35
using System.Threading.Tasks;
#endif

#if !NET40 && !NET35
using System.Net.Http;
#endif

namespace Brandy.PublicSuffix
{
    public class DomainParser
    {
        internal static readonly IdnMapping IdnMapping = new IdnMapping();

        private readonly Rule _rule;

        private DomainParser(Rule rule)
        {
            _rule = rule;
        }

        public Domain Parse(string host)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            var labels = host.Split('.');

            if (labels.Any(String.IsNullOrEmpty))
                throw new ArgumentException("host");

            Array.Reverse(labels);

            return Parse(labels);
        }

        private Domain Parse(string[] labels)
        {
            var rule = _rule.FindMatchingRule(labels);

            return rule.Parse(labels);
        }

        public static DomainParser FromFile(string fileName)
        {
            using (var reader = File.OpenText(fileName))
            {
                return Read(reader);
            }
        }

#if !NET40 && !NET35
        public static async Task<DomainParser> FromFileAsync(string fileName)
        {
            using (var reader = File.OpenText(fileName))
            {
                return await ReadAsync(reader);
            }
        }
#endif

        public static DomainParser Default
        {
#pragma warning disable CS0618 // Type or member is obsolete
            get { return FromUrl(new Uri("https://publicsuffix.org/list/effective_tld_names.dat")); }
#pragma warning restore CS0618 // Type or member is obsolete
        }

#if !NET35
        [Obsolete("Please use FromUrlAsync(Uri) instead")]
#endif
        public static DomainParser FromUrl(Uri uri)
        {
#if !NETSTANDARD1_3
            using (var client = new WebClient())
            using (var stream = client.OpenRead(uri))
            {
                return FromStream(stream);
            }  
#else
            return Task.Run(() => FromUrlAsync(uri)).GetAwaiter().GetResult();
#endif
        }

#if !NET40 && !NET35
        public static async Task<DomainParser> FromUrlAsync(Uri uri)
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(uri))
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                return await FromStreamAsync(stream);
            }
        }
#endif

        public static DomainParser FromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return Read(reader);
            }
        }

#if !NET40 && !NET35
        public static async Task<DomainParser> FromStreamAsync(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return await ReadAsync(reader);
            }
        }
#endif

        private static DomainParser Read(TextReader reader)
        {
            string line;
            var lines = new List<string>();
            while ((line = reader.ReadLine()) != null)
            {
                var l = line.Trim();
                if (l.Length == 0 || l.StartsWith("//")) continue;

                lines.Add(l);
            }
            
            return CreateDomainParser(lines);
        }

#if !NET40 && !NET35
        private static async Task<DomainParser> ReadAsync(TextReader reader)
        {
            string line;
            var lines = new List<string>();
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var l = line.Trim();
                if (l.Length == 0 || l.StartsWith("//")) continue;

                lines.Add(l);
            }

            return CreateDomainParser(lines);
        }
#endif
        
        private static DomainParser CreateDomainParser(IEnumerable<string> lines)
        {
            return new DomainParser(new Rule(1, ToRuleMap(lines.Select(ParseRuleDefinition), 0)));
        }

        private static IDictionary<string, Rule> ToRuleMap(IEnumerable<RuleDefinition> rules, int index)
        {
            return rules.GroupBy(r => IdnMapping.GetAscii(r.Labels[index]))
                .ToDictionary(
                    g => g.Key,
                    g =>
                        new Rule(
                            g.Where(r => r.Labels.Length == index + 1).Select(r => r.Length).SingleOrDefault(),
                            ToRuleMap(g.Where(r => r.Labels.Length > index + 1), index + 1)),
                    StringComparer.OrdinalIgnoreCase);
        }

        private static string[] GetLabels(string name)
        {
#if !NETSTANDARD1_3
            var labels = Array.ConvertAll(name.Split('.'), String.Intern);
            Array.Reverse(labels);
            return labels;
#else
            return name.Split('.').Reverse().ToArray();
#endif
        }

        private static RuleDefinition ParseRuleDefinition(string rule)
        {
            if (!rule.StartsWith("!"))
            {
                var labels = GetLabels(rule);
                return new RuleDefinition(labels, labels.Length);
            }
            else
            {
                var labels = GetLabels(rule.Substring(1));
                return new RuleDefinition(labels, labels.Length - 1);
            }
        }
    }
}
