#pragma warning disable CA1515

using SmartHomeAPI.Repositories;

namespace SmartHomeAPI.Services;

public sealed class SvgImagesService (SvgImagesRepository svgImageRepository)
{
	public string? GetSvgImage (string name)
	{
		return svgImageRepository.GetSvgImage(name);
	}
}
