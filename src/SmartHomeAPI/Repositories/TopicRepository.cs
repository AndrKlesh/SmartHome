#pragma warning disable CA1515

using SmartHomeAPI.Entities;

namespace SmartHomeAPI.Repositories;

public class TopicRepository
{
	private readonly List<TopicDomain> _topics = new();

	public async Task<TopicDomain?> GetTopicByNameAsync (string topicName)
	{
		return await Task.FromResult(_topics.FirstOrDefault(t => t.Name == topicName)).ConfigureAwait(false);
	}

	public async Task AddTopicAsync (TopicDomain topic)
	{
		_topics.Add(topic);
		await Task.CompletedTask.ConfigureAwait(false);
	}

	public async Task ToggleFavouriteAsync (string topicName, bool isFavourite)
	{
		TopicDomain? topic = _topics.FirstOrDefault(t => t.Name == topicName);
		if (topic == null)
		{
			throw new ArgumentNullException($"Topic with name '{topicName}' not found.");
		}

		topic.IsFavourite = isFavourite;
		await Task.CompletedTask.ConfigureAwait(false);
	}
}
