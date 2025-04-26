using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SmartHomeAPI.Repositories;

namespace SmartHomeAPI.UnitTests.Repositories;

[TestFixture]
internal sealed class MeasuresLinksRepositoryTests
{
	private Mock<ILogger<MeasuresLinksRepository>> _loggerMock;
	private Mock<IOptionsMonitor<Dictionary<string, Guid>>> _optionsMonitorMock;
	private MeasuresLinksRepository _repository;
	private Dictionary<string, Guid> _links;

	[SetUp]
	public void SetUp ()
	{
		_loggerMock = new Mock<ILogger<MeasuresLinksRepository>>();
		_optionsMonitorMock = new Mock<IOptionsMonitor<Dictionary<string, Guid>>>();

		_links = new Dictionary<string, Guid>
		{
			{ "Ванная комната/Температура горячей воды", Guid.Parse("462f9446-adff-4ea4-8ca1-f1665268520f") },
			{ "Общие/Вентиляция", Guid.Parse("40eac794-65e5-432d-84e6-f1b04b14db8a") },
			{ "Общие/Входная дверь", Guid.Parse("421673e7-95ef-478c-912a-71f3158ff613") },
			{ "Общие/Температура воздуха", Guid.Parse("24fe134b-4cbf-4eb9-a811-2720d4315146") },
			{ "Спальня/Температура воздуха", Guid.Parse("21274707-c7ca-4436-b191-9bac91c473f5") }
		};

		_ = _optionsMonitorMock.Setup(m => m.CurrentValue).Returns(_links);

		_repository = new MeasuresLinksRepository(_loggerMock.Object, _optionsMonitorMock.Object);
	}

	[Test]
	public async Task GetMeasurementIdAsync_ReturnsCorrectId_WhenPathExists ()
	{
		string path = "Ванная комната/Температура горячей воды";
		Guid expectedId = _links [path];

		Guid result = await _repository.GetMeasurementIdAsync(path).ConfigureAwait(false);

		Assert.That(result, Is.EqualTo(expectedId));
	}

	[Test]
	public async Task GetMeasurementIdAsync_ReturnsEmptyGuid_WhenPathDoesNotExist ()
	{
		Guid result = await _repository.GetMeasurementIdAsync("Несуществующий/Путь").ConfigureAwait(false);

		Assert.That(result, Is.Empty);
	}

	[Test]
	public async Task FindLinksByMaskAsync_ReturnsMatchingLinks ()
	{
		string mask = ".*Температура.*";

		IReadOnlyList<KeyValuePair<string, Guid>> results = await _repository.FindLinksByMaskAsync(mask).ConfigureAwait(false);

		Assert.That(results, Has.Count.EqualTo(3));
		Assert.That(results, Has.All.Matches<KeyValuePair<string, Guid>>(kvp => kvp.Key.Contains("Температура")));
	}

	[Test]
	public async Task FindLinksByMaskAsync_ReturnsEmpty_WhenNoMatch ()
	{
		string mask = "Несуществующая/Маска";

		IReadOnlyList<KeyValuePair<string, Guid>> results = await _repository.FindLinksByMaskAsync(mask).ConfigureAwait(false);

		Assert.That(results, Is.Empty);
	}

	[Test]
	public async Task FindLinksByMaskAsync_FindsLinksWithPathContainingSpalnya ()
	{
		string mask = "Спальня/*";

		IReadOnlyList<KeyValuePair<string, Guid>> results = await _repository.FindLinksByMaskAsync(mask).ConfigureAwait(false);

		Assert.That(results, Has.Count.EqualTo(1));
		Assert.That(results [0].Key, Is.EqualTo("Спальня/Температура воздуха"));
	}

	[Test]
	public async Task FindLinksByMaskAsync_FindsLinksWithMultipleMatches ()
	{
		string mask = "Общие/*";

		IReadOnlyList<KeyValuePair<string, Guid>> results = await _repository.FindLinksByMaskAsync(mask).ConfigureAwait(false);

		Assert.That(results, Has.Count.EqualTo(3));
		Assert.That(results, Has.All.Matches<KeyValuePair<string, Guid>>(kvp => kvp.Key.StartsWith("Общие")));
	}
}
