namespace Brandy.PublicSuffix
{
    internal class RuleDefinition
    {
        public readonly string[] Labels;
        public readonly int? Length;

        public RuleDefinition(string[] labels, int length)
        {
            Labels = labels;
            Length = length;
        }
    }
}