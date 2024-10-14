using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet.AspNetCore;

namespace SHMQ;

internal sealed class MqttServerSetup
{
	private static async Task Main ()
	{
		IHostBuilder builder = Host.CreateDefaultBuilder()
			.ConfigureServices((hostContext, services) =>
			{
				_ = services.AddHostedMqttServer(optionsBuilder => _ = optionsBuilder.WithDefaultEndpoint());

				_ = services.AddMqttConnectionHandler();
				_ = services.AddConnections();
			})
			.ConfigureWebHostDefaults(webBuilder =>
			{
				_ = webBuilder.ConfigureKestrel(serverOptions =>
					serverOptions.ListenAnyIP(1883, listenOptions => listenOptions.UseMqtt()));

				_ = webBuilder.Configure(app =>
					_ = app.UseMqttServer(server => Console.WriteLine("MQTT сервер запущен.")));
			});

		IHost host = builder.Build();
		await host.RunAsync().ConfigureAwait(false);
	}
}
