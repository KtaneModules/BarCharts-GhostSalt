namespace Assets
{
    internal enum BarRule : int
    {
        Leftmost = 0,
        SecondLeftmost,
        SecondRightmost,
        Rightmost,
        Shortest,
        SecondShortest,
        SecondLongest,
        Longest,
        FirstInOrder
    }

    internal static class BarRuleExtensions
    {
        internal static string ToLogString(this BarRule e)
        {
            switch (e)
            {
                case BarRule.SecondLeftmost:
                    return "second-leftmost bar";
                case BarRule.SecondRightmost:
                    return "second-rightmost bar";
                case BarRule.SecondShortest:
                    return "second-shortest bar";
                case BarRule.SecondLongest:
                    return "second-longest bar";
                case BarRule.FirstInOrder:
                    return "first bar in the order";
                default:
                    return e.ToString().ToLower() + " bar";
            }
        }
    }
}
