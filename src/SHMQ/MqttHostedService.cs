#pragma warning disable CA1515

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Server;

namespace SHMQ;

public class MqttHostedService (IConfiguration configuration) : IHostedService
{
	private readonly IConfiguration _configuration = configuration;
	private MqttServer? _mqttServer;

	public async Task StartAsync (CancellationToken cancellationToken)
	{
		int port = _configuration.GetValue<int>("MqttSettings:Port");

		MqttServerOptions options = new MqttServerOptionsBuilder()
			.WithDefaultEndpoint()
			.WithDefaultEndpointPort(port)
			.Build();

		_mqttServer = new MqttServerFactory().CreateMqttServer(options);

		await _mqttServer.StartAsync().ConfigureAwait(false);
	}

	public async Task StopAsync (CancellationToken cancellationToken)
	{
		if (_mqttServer is not null)
		{
			await _mqttServer.StopAsync().ConfigureAwait(false);
		}
	}
}
