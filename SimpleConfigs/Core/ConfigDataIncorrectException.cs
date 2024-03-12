namespace SimpleConfigs.Core
{
    public class ConfigDataIncorrectException : Exception
    {
        public ConfigDataIncorrectException()
        {
        }

        public ConfigDataIncorrectException(string? message) : base(message)
        {
        }
    }
}