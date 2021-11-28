using System;
using System.Collections.Generic;
using System.Linq;
using DotComServer.Domain.Entities;
using DotComServer.Domain.Repositories;

namespace DotComServer.Infrastructure.Repositories.Docx
{
	public class DocumentsFileRepository : IDocumentsFileRepository
	{
		private readonly DocxDbContext _docxDbContext;

		public DocumentsFileRepository()
		{
		}

		public DocumentsFileRepository(DocxDbContext docxDbContext)
		{
			_docxDbContext = docxDbContext ?? throw new ArgumentNullException(nameof(docxDbContext));
		}

		public int FilesCount => _docxDbContext.Documents.Count();

		public void Add(DocumentFile documentFile)
		{
			_docxDbContext.Documents.Add(documentFile);
			_docxDbContext.SaveChanges();
		}

		public List<DocumentFile> Get()
		{
			return _docxDbContext.Documents.ToList();
		}

		public DocumentFile Get(int id)
		{
			return _docxDbContext.Documents.Find(id);
		}

		public void Remove(DocumentFile documentFile)
		{
			_docxDbContext.Documents.Remove(documentFile);
			_docxDbContext.SaveChanges();
		}
	}
}
