using System.Collections.Generic;
using DotComServer.Domain.DTOs;
using IFormFile = Microsoft.AspNetCore.Http.IFormFile;

namespace DotComServer.Domain.Services
{
	public interface IDocxFileService
	{
		void Add(List<IFormFile> docxFiles);
		void Remove(int id);
		List<DocxFileDto> Get();
		DocxFileDto Get(int id);
	}
}
