using System.Collections.Generic;
using DotComServer.Domain.DTOs;
using IFormFile = Microsoft.AspNetCore.Http.IFormFile;

namespace DotComServer.Domain.Services
{
	public interface IDocumentsFileService
	{
		void Add(List<IFormFile> docxFiles);
		void Remove(int id);
		List<DocumentFileDto> Get();
		DocumentFileDto Get(int id);
		int FilesCount { get; }
	}
}
