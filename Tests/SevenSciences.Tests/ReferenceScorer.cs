namespace SevenScience.Tests;

/// <summary>
/// A deliberately naive scorer used to check <see cref="Maths"/>: it tries every wild card assignment, every way
/// to use Pidgeon (including using fewer symbols than possible) and both orders for Pidgeon and GroupPlus,
/// and adds the GroupPlus symbols one at a time to a type currently owned the most
/// </summary>
public static class ReferenceScorer
{
    public static int Score(int compass, int tablet, int cogwheel, int wild, int groupValue, int groupPlus, int stacks, int[] pidgeonCards)
    {
        int best = 0;

        for (int w1 = 0; w1 <= wild; w1++)
        {
            for (int w2 = 0; w2 <= wild - w1; w2++)
            {
                int[] owned = { compass + w1, tablet + w2, cogwheel + wild - w1 - w2 };

                for (int p1 = 0; p1 <= Math.Min(stacks, pidgeonCards[0]); p1++)
                {
                    for (int p2 = 0; p2 <= Math.Min(stacks - p1, pidgeonCards[1]); p2++)
                    {
                        for (int p3 = 0; p3 <= Math.Min(stacks - p1 - p2, pidgeonCards[2]); p3++)
                        {
                            int[] gained = { p1, p2, p3 };
                            int[] none = new int[3];

                            best = Math.Max(best, WithGroupPlus(Add(owned, gained), groupPlus, groupValue, none));
                            best = Math.Max(best, WithGroupPlus(owned, groupPlus, groupValue, gained));
                        }
                    }
                }
            }
        }

        return best;
    }

    private static int WithGroupPlus(int[] counts, int remaining, int groupValue, int[] gainedAfter)
    {
        int mostOwned = counts.Max();

        if (remaining == 0 || mostOwned == 0)
        {
            return ScoreOf(Add(counts, gainedAfter), groupValue);
        }

        int best = 0;

        for (int i = 0; i < counts.Length; i++)
        {
            if (counts[i] != mostOwned)
            {
                continue;
            }

            int[] next = (int[])counts.Clone();
            next[i]++;
            best = Math.Max(best, WithGroupPlus(next, remaining - 1, groupValue, gainedAfter));
        }

        return best;
    }

    private static int[] Add(int[] first, int[] second) => new[] { first[0] + second[0], first[1] + second[1], first[2] + second[2] };

    private static int ScoreOf(int[] counts, int groupValue) => counts.Sum(count => count * count) + (groupValue * counts.Min());
}
