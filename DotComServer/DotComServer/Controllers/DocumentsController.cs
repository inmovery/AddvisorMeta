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
	public class DocumentsController : ControllerBase
	{
		private readonly IDocumentsFileService _documentsFileService;

		public DocumentsController(IDocumentsFileService fileService, ILogger<DocumentsController> logger)
		{
			_documentsFileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
		}

		[HttpGet]
		public ActionResult<List<DocumentFileDto>> Get()
		{
			return Ok(_documentsFileService.Get());
		}

		[HttpDelete("{id}")]
		public ActionResult Delete(int id)
		{
			_documentsFileService.Remove(id);
			return NoContent();
		}

		[HttpGet("{id}")]
		public ActionResult Get(int id)
		{
			var documentFileDto = _documentsFileService.Get(id);
			return File(documentFileDto.FileContent, MediaTypeNames.Application.Octet, documentFileDto.Filename);
		}

		[HttpPost, DisableRequestSizeLimit]
		public ActionResult Post()
		{
			_documentsFileService.Add(Request.Form.Files.ToList());
			return Ok();
		}

	}
}
