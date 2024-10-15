using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Server;

namespace SHMQ;
public class MqttHostedService : IHostedService
{
	private MqttServer? _mqttServer;

	public async Task StartAsync (CancellationToken cancellationToken)
	{
		MqttServerOptions options = new MqttServerOptionsBuilder()
			.WithDefaultEndpoint()
			.WithDefaultEndpointPort(1883)
			.Build();

		_mqttServer = new MqttFactory().CreateMqttServer(options);

		_mqttServer.ClientConnectedAsync += _ => Task.CompletedTask;
		_mqttServer.ClientDisconnectedAsync += _ => Task.CompletedTask;

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
