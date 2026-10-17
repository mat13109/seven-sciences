namespace SevenScience.Tests;

/// <summary>
/// A deliberately naive scorer used to check <see cref="Maths"/>: it tries every wild card assignment and every
/// order of the end of game powers, with every possible choice for each of them (Pidgeon may even use fewer symbols
/// than it could), and adds the GroupPlus symbols one at a time to a type currently owned the most
/// </summary>
public static class ReferenceScorer
{
    private enum EPower
    {
        Aganice,
        Pidgeon,
        GroupPlus,
    }

    public static int Score(
        int compass,
        int tablet,
        int cogwheel,
        int wild,
        int groupValue,
        int groupPlus,
        int stacks,
        int[] pidgeonCards,
        bool aganice)
    {
        List<EPower> powers = new();

        if (aganice)
        {
            powers.Add(EPower.Aganice);
        }

        if (stacks > 0)
        {
            powers.Add(EPower.Pidgeon);
        }

        if (groupPlus > 0)
        {
            powers.Add(EPower.GroupPlus);
        }

        Context context = new(groupValue, groupPlus, stacks, pidgeonCards);
        int best = 0;

        for (int w1 = 0; w1 <= wild; w1++)
        {
            for (int w2 = 0; w2 <= wild - w1; w2++)
            {
                int[] owned = { compass + w1, tablet + w2, cogwheel + wild - w1 - w2 };

                foreach (List<EPower> order in Orders(powers))
                {
                    best = Math.Max(best, Resolve(context, order, 0, owned, new int[3]));
                }
            }
        }

        return best;
    }

    private static int Resolve(Context context, List<EPower> order, int index, int[] owned, int[] gained)
    {
        if (index == order.Count)
        {
            return ScoreOf(Add(owned, gained), context.GroupValue);
        }

        int best = 0;

        switch (order[index])
        {
            case EPower.Aganice:
                // Optional, and only on the symbols of the city
                best = Resolve(context, order, index + 1, owned, gained);

                for (int from = 0; from < 3; from++)
                {
                    for (int to = 0; to < 3; to++)
                    {
                        if (from == to || owned[from] == 0)
                        {
                            continue;
                        }

                        int[] replaced = (int[])owned.Clone();
                        replaced[from]--;
                        replaced[to]++;
                        best = Math.Max(best, Resolve(context, order, index + 1, replaced, gained));
                    }
                }

                break;

            case EPower.Pidgeon:
                for (int p1 = 0; p1 <= Math.Min(context.Stacks, context.PidgeonCards[0]); p1++)
                {
                    for (int p2 = 0; p2 <= Math.Min(context.Stacks - p1, context.PidgeonCards[1]); p2++)
                    {
                        for (int p3 = 0; p3 <= Math.Min(context.Stacks - p1 - p2, context.PidgeonCards[2]); p3++)
                        {
                            best = Math.Max(best, Resolve(context, order, index + 1, owned, Add(gained, new[] { p1, p2, p3 })));
                        }
                    }
                }

                break;

            case EPower.GroupPlus:
                best = AddGroupPlusSymbols(context, order, index, owned, gained, context.GroupPlus);
                break;
        }

        return best;
    }

    /// <summary>
    /// Gives the symbols of GroupPlus one by one, each to a type that is currently owned the most
    /// </summary>
    private static int AddGroupPlusSymbols(Context context, List<EPower> order, int index, int[] owned, int[] gained, int remaining)
    {
        int[] total = Add(owned, gained);
        int mostOwned = total.Max();

        if (remaining == 0 || mostOwned == 0)
        {
            return Resolve(context, order, index + 1, owned, gained);
        }

        int best = 0;

        for (int i = 0; i < 3; i++)
        {
            if (total[i] != mostOwned)
            {
                continue;
            }

            int[] next = (int[])gained.Clone();
            next[i]++;
            best = Math.Max(best, AddGroupPlusSymbols(context, order, index, owned, next, remaining - 1));
        }

        return best;
    }

    private static IEnumerable<List<EPower>> Orders(List<EPower> powers)
    {
        if (powers.Count <= 1)
        {
            yield return powers;
            yield break;
        }

        for (int i = 0; i < powers.Count; i++)
        {
            List<EPower> others = new(powers);
            others.RemoveAt(i);

            foreach (List<EPower> order in Orders(others))
            {
                yield return new List<EPower>(order.Prepend(powers[i]));
            }
        }
    }

    private static int[] Add(int[] first, int[] second) => new[] { first[0] + second[0], first[1] + second[1], first[2] + second[2] };

    private static int ScoreOf(int[] counts, int groupValue) => counts.Sum(count => count * count) + (groupValue * counts.Min());

    private sealed record Context(int GroupValue, int GroupPlus, int Stacks, int[] PidgeonCards);
}
