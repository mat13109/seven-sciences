using Xunit;

// Maths keeps its settings in static properties, so the tests must not run in parallel
[assembly: CollectionBehavior(DisableTestParallelization = true)]
