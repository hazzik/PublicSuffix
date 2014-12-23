using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Brandy.PublicSuffix
{
    public class DomainParser
    {
        private readonly Rule _default = Rule.Create("*");
        private readonly IEnumerable<Rule> _rules;

        private DomainParser(IEnumerable<Rule> rules)
        {
            _rules = rules;
        }

        public Domain Parse(string host)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            var labels = host.Split('.');
            var rules = _rules.Where(r => r.Match(labels))
                .OrderByDescending(x => x.Length)
                .ToArray();

            var rule = rules.OfType<ExceptionRule>().FirstOrDefault() ??
                       rules.FirstOrDefault() ??
                       _default;

            return rule.Parse(labels);
        }

        public static DomainParser FromFile(string fileName)
        {
            using (var reader = File.OpenText(fileName))
            {
                return Read(reader);
            }
        }

        public static DomainParser FromUrl(Uri uri)
        {
            using (var client = new WebClient())
            using (var stream = client.OpenRead(uri))
            {
                return FromStream(stream);
            }
        }

        public static async Task<DomainParser> FromUrlAsync(Uri uri)
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(uri))
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                return FromStream(stream);
            }
        }

        public static DomainParser FromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return Read(reader);
            }
        }

        private static DomainParser Read(TextReader reader)
        {
            var rules = new List<Rule>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var l = line.Trim();
                if (l.Length == 0 || l.StartsWith("//")) continue;

                rules.Add(Rule.Create(l));
            }
            return new DomainParser(rules);
        }
    }
}