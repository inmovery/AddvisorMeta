using System.Collections.Generic;
using DotComServer.Domain.Entities;

namespace DotComServer.Domain.Repositories
{
	public interface IDocxFileRepository
	{
		void Add(DocxFile docxFile);
		void Remove(DocxFile docxFile);
		List<DocxFile> Get();
		DocxFile Get(int id);
	}
}
