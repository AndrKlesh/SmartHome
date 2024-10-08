using MQTTnet.Client;
using MQTTnet;
using System.Text;
using MQTTnet.Server;

namespace SmartHomeWeb.Services
{
  public class MeasuresReceiverService(IHostApplicationLifetime _lifetime, MeasuresStorageService _measuresStorageService) : IHostedService
  {
    public Task StartAsync(CancellationToken cancellationToken)
    {
      _lifetime.ApplicationStarted.Register(() => _ = ConfigureSubscriptions(default));
      _lifetime.ApplicationStopping.Register(() => _ = UnconfigureSubscriptions(default));
      return Task.CompletedTask;
    }


    public  Task StopAsync(CancellationToken cancellationToken)
    {
      return Task.CompletedTask;
    }

    private async Task ConfigureSubscriptions(CancellationToken cancellationToken) 
    {
      _mqttClient = _mqttFactory.CreateMqttClient();
      var mqttClientOptions = _mqttFactory.CreateClientOptionsBuilder()
                                          .WithTcpServer("localhost", 1883)
                                          .Build();

      try
      {
        await _mqttClient.ConnectAsync(mqttClientOptions, cancellationToken);


        _mqttClient.ApplicationMessageReceivedAsync += OnApplicationMessageReceivedAsync;
        var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
                                               .WithTopicFilter("home/#")
                                               .Build();
        await _mqttClient.SubscribeAsync(mqttSubscribeOptions, cancellationToken);
      }
      catch (Exception)
      {

      }
    }
    private async Task UnconfigureSubscriptions(CancellationToken cancellationToken)
    {
      var mqttClient = _mqttClient;
      _mqttClient = null;
      if (mqttClient is null)
      {
        return;
      }
      mqttClient.ApplicationMessageReceivedAsync -= OnApplicationMessageReceivedAsync;
      await mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
      mqttClient.Dispose();
    }

    private Task OnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
    {
      //TODO: конвертер из mqtt во внутренние измерения
      _measuresStorageService.PutMeasure(e.ApplicationMessage.Topic,
                                         Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment),
                                         DateTime.UtcNow);
      return Task.CompletedTask;
    }

    private IMqttClient? _mqttClient;
    private readonly MqttFactory _mqttFactory = new();
  }
}
