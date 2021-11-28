using System.Collections.Generic;
using DotComServer.Domain.Entities;

namespace DotComServer.Domain.Repositories
{
	public interface IDocumentsFileRepository
	{
		void Add(DocumentFile documentFile);
		void Remove(DocumentFile documentFile);
		List<DocumentFile> Get();
		DocumentFile Get(int id);
		int FilesCount { get; }
	}
}
