#pragma warning disable CA1515
namespace SmartHomeAPI.Models;

/// <summary>
/// DTO ссылки
/// </summary>
public class LinkDTO
{
	/// <summary>
	/// Путь (полный или относительный) ссылки
	/// </summary>
	public string Path { get; set; }

	/// <summary>
	/// Модификатор
	/// r-w
	/// rwx
	/// drwx
	/// </summary>
	public string Mode { get; set; }
}
