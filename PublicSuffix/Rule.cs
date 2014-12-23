using System;
using System.Globalization;

namespace Brandy.PublicSuffix
{
    public class Rule
    {
        private static readonly IdnMapping IdnMapping = new IdnMapping();
        private readonly string[] _labels;
        private readonly DomainRuleParser _parser;

        protected Rule(string name)
        {
            var labels = name.Split('.');
            Array.Reverse(labels);
            _labels = labels;
            _parser = CreateParser(labels);
        }

        protected virtual DomainRuleParser CreateParser(string[] labels)
        {
            return new DomainRuleParser(labels);
        }

        public int Length
        {
            get { return _labels.Length; }
        }

        public bool Match(string[] labels)
        {
            for (var i = 0; i < _labels.Length; i++)
            {
                var ruleLabel = _labels[i];
                if (i == labels.Length)
                    return false;

                if (ruleLabel != "*")
                {
                    var label = labels[labels.Length - 1 - i];
                    if (!Match(label, ruleLabel))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool Match(string label, string ruleLabel)
        {
            if (string.IsNullOrEmpty(label)) return false;
            if (string.Equals(ruleLabel, label, StringComparison.OrdinalIgnoreCase)) return true;
            return string.Equals(IdnMapping.GetAscii(ruleLabel), IdnMapping.GetAscii(label), StringComparison.OrdinalIgnoreCase);
        }

        public Domain Parse(string[] labels)
        {
            return _parser.ParseDomain(labels);
        }

        public override string ToString()
        {
            return String.Join(".", _labels);
        }

        public static Rule Create(string rule)
        {
            return !rule.StartsWith("!")
                ? new Rule(rule)
                : new ExceptionRule(rule.Substring(1));
        }
    }
}