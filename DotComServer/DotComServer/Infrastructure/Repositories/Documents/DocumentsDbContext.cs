using DotComServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotComServer.Infrastructure.Repositories.Documents
{
	public sealed class DocumentsDbContext : DbContext
	{
		public DocumentsDbContext(DbContextOptions options)
			: base(options)
		{
			Database.EnsureCreated();
		}

		public DbSet<DocumentFile> Documents { get; set; }
	}
}
