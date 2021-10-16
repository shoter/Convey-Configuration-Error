using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Net5WebProvider
{
    public class TimeService : IHostedService
    {
        private readonly IOptionsMonitor<TimeSettings> settings;

        public TimeService(IOptionsMonitor<TimeSettings> settings)
        {
            this.settings = settings;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (true)
            {

                Console.WriteLine(settings.CurrentValue.Time);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            
            
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}