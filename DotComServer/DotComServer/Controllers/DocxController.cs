using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using DotComServer.Domain.DTOs;
using DotComServer.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotComServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DocxController : ControllerBase
	{
		private readonly IDocxFileService _docxFileService;

		public DocxController(IDocxFileService fileService, ILogger<DocxController> logger)
		{
			_docxFileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
		}

		[HttpGet]
		public ActionResult<List<DocxFileDto>> Get()
		{
			return Ok(_docxFileService.Get());
		}

		[HttpDelete("{id}")]
		public ActionResult Delete(int id)
		{
			_docxFileService.Remove(id);
			return NoContent();
		}

		[HttpGet("{id}")]
		public ActionResult Get(int id)
		{
			var docxFileDto = _docxFileService.Get(id);
			return File(docxFileDto.FileContent, MediaTypeNames.Application.Octet, docxFileDto.Filename);
		}

		[HttpPost, DisableRequestSizeLimit]
		public ActionResult Post()
		{
			_docxFileService.Add(Request.Form.Files.ToList());
			return Ok();
		}

	}
}
