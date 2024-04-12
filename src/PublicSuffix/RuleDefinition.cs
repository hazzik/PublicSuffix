namespace Brandy.PublicSuffix
{
    internal struct RuleDefinition
    {
        public string[] Labels;
        public int? Length;

        public RuleDefinition(string[] labels, int length)
        {
            Labels = labels;
            Length = length;
        }
    }
}
