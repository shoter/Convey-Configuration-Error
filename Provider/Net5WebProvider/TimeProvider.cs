using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Net5WebProvider
{
    public class TimeProvider : ConfigurationProvider
    {
        public TimeProvider()
        {
            Task.Run(Action);

        }

        private async Task Action()
        {
            while (true)
            {
                Load();
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        public override void Load()
        {
            Data["Time"] = DateTime.Now.ToString();
            OnReload();
        }
    }
}