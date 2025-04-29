using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SmartHomeAPI.Entities;
using SmartHomeAPI.Repositories;

namespace SmartHomeAPI.UnitTests.Repositories;

[TestFixture]
internal sealed class SubscriptionRepositoryTests
{
	private Mock<IOptionsMonitor<List<SubscriptionDomain>>> _optionsMonitorMock;
	private List<SubscriptionDomain> _subscriptions;
	private SubscriptionRepository _repository;

	[SetUp]
	public void SetUp ()
	{
		_optionsMonitorMock = new Mock<IOptionsMonitor<List<SubscriptionDomain>>>();

		_subscriptions = new()
	{
		new SubscriptionDomain
		{
			MeasurementId = Guid.NewGuid(),
			MqttTopic = "test/temperature"
		},
		new SubscriptionDomain
		{
			MeasurementId = Guid.NewGuid(),
			MqttTopic = "test/humidity"
		}
	};

		_ = _optionsMonitorMock.Setup(x => x.CurrentValue).Returns(_subscriptions);

		_repository = new SubscriptionRepository(NullLogger<SubscriptionRepository>.Instance, _optionsMonitorMock.Object);
	}

	[Test]
	public async Task GetAllSubscriptionsAsyncShouldReturnAllSubscriptions ()
	{
		List<SubscriptionDomain> result = await _repository.GetAllSubscriptionsAsync().ConfigureAwait(false);

		Assert.Multiple(() =>
		{
			Assert.That(result, Has.Count.EqualTo(2));
			Assert.That(result, Is.EquivalentTo(_subscriptions));
		});
	}

	[Test]
	public async Task GetSubscriptionByMeasurementIdAsyncWhenExistsReturnsSubscription ()
	{
		SubscriptionDomain expected = _subscriptions [0];

		SubscriptionDomain result = await _repository.GetSubscriptionByMeasurementIdAsync(expected.MeasurementId).ConfigureAwait(false);

		Assert.Multiple(() =>
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result.MeasurementId, Is.EqualTo(expected.MeasurementId));
			Assert.That(result.MqttTopic, Is.EqualTo(expected.MqttTopic));
		});
	}

	[Test]
	public async Task GetSubscriptionByMeasurementIdAsyncWhenNotExistsReturnsNull ()
	{
		SubscriptionDomain result = await _repository.GetSubscriptionByMeasurementIdAsync(Guid.NewGuid()).ConfigureAwait(false);

		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task GetSubscriptionByMqttTopicAsyncWhenExistsReturnsSubscription ()
	{
		SubscriptionDomain expected = _subscriptions [1];

		SubscriptionDomain result = await _repository.GetSubscriptionByMqttTopicAsync(expected.MqttTopic).ConfigureAwait(false);

		Assert.Multiple(() =>
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.MqttTopic, Is.EqualTo(expected.MqttTopic));
			Assert.That(result.MeasurementId, Is.EqualTo(expected.MeasurementId));
		});
	}

	[Test]
	public async Task GetSubscriptionByMqttTopicAsyncWhenNotExistsReturnsNull ()
	{
		SubscriptionDomain result = await _repository.GetSubscriptionByMqttTopicAsync("test/unknown").ConfigureAwait(false);

		Assert.That(result, Is.Null);
	}
}
