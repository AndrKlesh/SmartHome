using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SHMQ;

internal sealed class MqttServerSetup
{
	private static async Task Main ()
	{
		IHostBuilder builder = Host.CreateDefaultBuilder()
			.ConfigureServices((hostContext, services) =>
				_ = services.AddHostedService<MqttHostedService>());

		IHost host = builder.Build();
		await host.RunAsync().ConfigureAwait(false);
	}
}
