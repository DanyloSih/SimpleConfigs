namespace SimpleConfigs.Utilities
{
    public static class AsyncUtilities
    {
        public static async Task ForEachAsync<TSource, TContext>(
            IEnumerable<TSource> source, TContext context, Func<TSource, TContext, ValueTask> body, bool inParallel = true)
            where TContext : ICloneable
        {
            if (inParallel)
            {
                IEnumerable<(TSource Source, TContext Context)> sourceWithContext 
                    = source.Select(x => (x, (TContext)context.Clone()));

                await Parallel.ForEachAsync(
                    sourceWithContext, async (value, token) => await body.Invoke(value.Source, value.Context));
            }
            else
            {
                foreach (var sourceValue in source)
                {
                    await body.Invoke(sourceValue, context);
                }
            }
        }
    }
}