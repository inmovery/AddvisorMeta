using DotComServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotComServer.Infrastructure.Repositories.Docx
{
	public sealed class DocxDbContext : DbContext
	{
		public DocxDbContext(DbContextOptions options)
			: base(options)
		{
			Database.EnsureCreated();
		}

		public DbSet<DocxFile> DocxFiles { get; set; }
	}
}
