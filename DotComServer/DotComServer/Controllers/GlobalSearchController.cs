using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using DotComServer.Business;
using DotComServer.Domain.DTOs;
using DotComServer.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotComServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
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
		public ActionResult<SearchResultDto> GlobalSearch()
		{
			var searchableContent = "address";
			var searchResult = _globalSearch.DoSearch(searchableContent);

			return Ok(searchResult);
		}

		[HttpGet("{id}")]
		public ActionResult<SearchResultDto> GlobalSearch(int id)
		{
			var searchableContent = "address";
			var searchResult = _globalSearch.DoSearch(searchableContent, -1);

			return Ok(searchResult);
		}

		//[HttpGet]
		//public ActionResult<List<DocumentFileDto>> Get()
		//{
		//	return Ok(_documentsFileService.Get());
		//}

		//[HttpDelete("{id}")]
		//public ActionResult Delete(int id)
		//{
		//	_documentsFileService.Remove(id);
		//	return NoContent();
		//}

		//[HttpGet("{id}")]
		//public ActionResult Get(int id)
		//{
		//	var docxFileDto = _documentsFileService.Get(id);
		//	return File(docxFileDto.FileContent, MediaTypeNames.Application.Octet, docxFileDto.Filename);
		//}

		//[HttpPost, DisableRequestSizeLimit]
		//public ActionResult Post()
		//{
		//	_documentsFileService.Add(Request.Form.Files.ToList());
		//	return Ok();
		//}

	}
}
