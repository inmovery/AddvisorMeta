using System;
using System.Collections.Generic;
using System.Linq;
using DotComServer.Domain.Entities;
using DotComServer.Domain.Repositories;

namespace DotComServer.Infrastructure.Repositories.Documents
{
	public class DocumentsFileRepository : IDocumentsFileRepository
	{
		private readonly DocumentsDbContext _documentsDbContext;

		public DocumentsFileRepository()
		{
		}

		public DocumentsFileRepository(DocumentsDbContext documentsDbContext)
		{
			_documentsDbContext = documentsDbContext ?? throw new ArgumentNullException(nameof(documentsDbContext));
		}

		public int FilesCount => _documentsDbContext.Documents.Count();

		public void Add(DocumentFile documentFile)
		{
			_documentsDbContext.Documents.Add(documentFile);
			_documentsDbContext.SaveChanges();
		}

		public List<DocumentFile> Get()
		{
			return _documentsDbContext.Documents.ToList();
		}

		public DocumentFile Get(int id)
		{
			return _documentsDbContext.Documents.Find(id);
		}

		public void Remove(DocumentFile documentFile)
		{
			_documentsDbContext.Documents.Remove(documentFile);
			_documentsDbContext.SaveChanges();
		}
	}
}
