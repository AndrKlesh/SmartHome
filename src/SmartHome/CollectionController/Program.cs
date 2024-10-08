using MQTTnet.Client;
using MQTTnet;

var mqttFactory = new MqttFactory();

using (var mqttClient = mqttFactory.CreateMqttClient())
{
  var mqttClientOptions = new MqttClientOptionsBuilder()
      .WithTcpServer("localhost", 1883)
      .Build();

  await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

  var random = new Random();
  while(!Console.KeyAvailable)
  {
    var applicationMessage = new MqttApplicationMessageBuilder()
      .WithTopic("home/door")
      .WithPayload(random.Next(0, 2).ToString())
      .Build();
    await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

    applicationMessage = new MqttApplicationMessageBuilder()
      .WithTopic("home/outside/temperature")
      .WithPayload(random.Next(5, 12).ToString())
      .Build();
    await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

    applicationMessage = new MqttApplicationMessageBuilder()
      .WithTopic("home/living_room/temperature")
      .WithPayload(random.Next(14, 26).ToString())
      .Build();
    await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

    applicationMessage = new MqttApplicationMessageBuilder()
    .WithTopic("home/living_room/lighting")
    .WithPayload(random.Next(0, 2).ToString())
    .Build();
    await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

    applicationMessage = new MqttApplicationMessageBuilder()
      .WithTopic("home/bathroom/cold_water_temp")
      .WithPayload(random.Next(5, 18).ToString())
      .Build();
    await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

    applicationMessage = new MqttApplicationMessageBuilder()
      .WithTopic("home/bathroom/hot_water_temp")
      .WithPayload(random.Next(30, 65).ToString())
      .Build();
    await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

    var airHumidity = random.Next(25, 96);
    applicationMessage = new MqttApplicationMessageBuilder()
      .WithTopic("home/bathroom/air_humidity")
      .WithPayload(airHumidity.ToString())
      .Build();
    await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

    applicationMessage = new MqttApplicationMessageBuilder()
      .WithTopic("home/bathroom/venting")
      .WithPayload((airHumidity > 85 ? 1 : 0).ToString())
      .Build();
    await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

    await Task.Delay(500);
  }
  await mqttClient.DisconnectAsync();
}
