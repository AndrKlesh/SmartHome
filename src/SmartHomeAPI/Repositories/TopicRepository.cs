using SmartHomeAPI.Entities;

namespace SmartHomeAPI.Repositories;

internal class TopicRepository
{
	private readonly List<TopicDomain> _topics = new();

	internal async Task<TopicDomain?> GetTopicByNameAsync (string topicName)
	{
		return await Task.FromResult(_topics.FirstOrDefault(t => t.Name == topicName));
	}

	internal async Task AddTopicAsync (TopicDomain topic)
	{
		_topics.Add(topic);
		await Task.CompletedTask;
	}

	internal async Task ToggleFavouriteAsync (string topicName, bool isFavourite)
	{
		TopicDomain? topic = _topics.FirstOrDefault(t => t.Name == topicName);
		if (topic == null)
		{
			throw new ArgumentNullException($"Topic with name '{topicName}' not found.");
		}

		topic.IsFavourite = isFavourite;
		await Task.CompletedTask;
	}
}
