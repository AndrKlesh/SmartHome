#pragma warning disable CA1515

using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Models;
using SmartHomeAPI.Services;

namespace SmartHomeAPI.Controllers;

/// <summary>
/// ���������� Dashboard'��
/// </summary>
/// <param name="measuresStorageService"></param>
[ApiController]
[Route("api/[controller]")]
public class DashboardController (MeasuresStorageService measuresStorageService) : ControllerBase
{
	/// <summary>
	/// �������� ��������� ���������
	/// </summary>
	/// <param name="mask">����� ������ �� ���������
	/// ��������, �������� ��������� �� ������ �����: mask = "�����/*"</param>
	/// <returns></returns>
	[HttpGet("latest/{mask}")]
	public async Task<ActionResult<IReadOnlyList<MeasureDTO>>> GetLatestMeasurements (string mask)
	{

		IReadOnlyList<MeasureDTO> latestMeasurements = await _measuresStorageService.GetLatestMeasurementsAsync(mask).ConfigureAwait(false);
		return Ok(latestMeasurements);
	}
	private readonly MeasuresStorageService _measuresStorageService = measuresStorageService;
}

