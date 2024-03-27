namespace SimpleConfigs.Core
{
    public interface IDataCorrectnessChecker
    {
        /// <summary>
        /// May throw an exception if some data in the object is unacceptable.
        /// </summary>
        public Task CheckDataCorrectnessAsync();
    }
}