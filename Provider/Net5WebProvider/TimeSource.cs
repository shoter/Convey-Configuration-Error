using Microsoft.Extensions.Configuration;

namespace Net5WebProvider
{
    public class TimeSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new TimeProvider();
        }
    }
}