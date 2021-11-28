using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotComServer.Domain.DTOs;
using DotComServer.Domain.Entities;
using DotComServer.Domain.Repositories;
using DotComServer.Domain.Services;
using Microsoft.AspNetCore.Http;

namespace DotComServer.Infrastructure.Services.Docx
{
	public class DocxFileService : IDocxFileService
	{
		private readonly IDocxFileRepository _fileRepository;

		public DocxFileService(IDocxFileRepository fileRepository)
		{
			_fileRepository = fileRepository ?? throw new ArgumentNullException(nameof(fileRepository));
		}

		public void Add(List<IFormFile> docxFiles)
		{
			foreach (var file in docxFiles)
			{
				using MemoryStream memoryStream = new();
				file.OpenReadStream().CopyTo(memoryStream);

				_fileRepository.Add(new DocxFile
				{
					Filename = file.FileName,
					FileContent = Convert.ToBase64String(memoryStream.ToArray()),
					Size = file.Length
				});
			}
		}

		public List<DocxFileDto> Get()
		{
			return _fileRepository.Get().Select(file => new DocxFileDto
			{
				Size = file.Size,
				FileContent = Convert.FromBase64String(file.FileContent),
				Filename = file.Filename,
				Id = file.Id

			}).ToList();
		}

		public DocxFileDto Get(int id)
		{
			var docxFile = _fileRepository.Get(id);

			var docxFileDto = new DocxFileDto
			{
				FileContent = Convert.FromBase64String(docxFile.FileContent),
				Filename = docxFile.Filename,
				Id = docxFile.Id,
				Size = docxFile.Size
			};

			return docxFileDto;
		}

		public void Remove(int id)
		{
			_fileRepository.Remove(_fileRepository.Get(id));
		}
	}
}
