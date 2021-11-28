using System;
using DotComServer.Business;
using DotComServer.Domain.DTOs;
using DotComServer.Domain.Entities;
using DotComServer.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotComServer.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class GlobalSearchController : ControllerBase
	{
		private readonly IDocumentsFileService _documentsFileService;
		private readonly GlobalSearch _globalSearch;

		public GlobalSearchController(IDocumentsFileService fileService)
		{
			_documentsFileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
			_globalSearch = new GlobalSearch(fileService);
		}

		[HttpGet]
		public ActionResult<SearchResultDto> GlobalSearch([FromBody] SearchParams searchParams)
		{
			var countFiles = _documentsFileService.FilesCount;
			var searchResult = _globalSearch.DoSearch(searchParams, countFiles);

			return Ok(searchResult);
		}
	}
}
