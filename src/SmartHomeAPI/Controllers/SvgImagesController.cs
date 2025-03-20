#pragma warning disable CA1515

using System.Text;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SvgImagesController (SvgImagesService svgImageService, ILogger<SvgImagesController> logger) : Controller
{
	/// <summary>
	/// Получить SVG-изображение по названию
	/// </summary>
	/// <param name="name">Название изображения</param>
	/// <returns>SVG-картинка</returns>
	[HttpGet("{name}")]
	public IActionResult GetSvgImage (string name)
	{
		logger.LogInformation("Запрос на получение SVG-изображения: '{ImageName}'...", name);
		string? svgContent = svgImageService.GetSvgImage(name);

		if (svgContent == null)
		{
			logger.LogWarning("SVG-изображение '{ImageName}' не найдено", name);
			return NotFound(new { message = $"SVG-Изображение '{name}' не найдено" });
		}
		else
		{
			logger.LogInformation("SVG-изображение '{ImageName}' найдено", name);
			return File(Encoding.UTF8.GetBytes(svgContent), "image/svg+xml");
		}
	}
}
