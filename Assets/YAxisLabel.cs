using System;

namespace Assets
{
    internal enum YAxisLabel
    {
        Popularity,
        Frequency,
        Responses,
        Occurances,
        Density,
        Magnitude
    }

    internal static class YAxisLabelExtensions
    {
        internal static string GetComparativeWord(this YAxisLabel label)
        {
            switch (label)
            {
                case YAxisLabel.Popularity:
                    return "is shorter";
                case YAxisLabel.Frequency:
                    return "is taller";
                case YAxisLabel.Responses:
                    return "is more to the left";
                case YAxisLabel.Occurances:
                    return "is more to the right";
                case YAxisLabel.Density:
                    return "has a smaller value";
                case YAxisLabel.Magnitude:
                    return "has a greater value";
                default:
                    throw new ArgumentOutOfRangeException(nameof(label), label, null);
            }
        }
    }
}
