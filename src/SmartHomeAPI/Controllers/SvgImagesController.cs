#pragma warning disable CA1515

using System.Text;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SvgImagesController (SvgImagesService svgImageService) : Controller
{
	/// <summary>
	/// Получить SVG-изображение по названию
	/// </summary>
	/// <param name="name">Название изображения</param>
	/// <returns>SVG-картинка</returns>
	[HttpGet("{name}")]
	public IActionResult GetSvgImage (string name)
	{
		string? svgContent = svgImageService.GetSvgImage(name);
		if (svgContent != null)
		{
			return File(Encoding.UTF8.GetBytes(svgContent), "image/svg+xml");
		}

		return NotFound();
	}
}
