using System.Linq;

namespace Brandy.PublicSuffix
{
    internal class ExceptionRule : Rule
    {
        public ExceptionRule(string rule)
            : base(rule)
        {
        }

        protected override DomainRuleParser CreateParser(string[] labels)
        {
            return new DomainRuleParser(labels.Skip(1).ToArray());
        }
    }
}