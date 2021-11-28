using DotComServer.Domain.Repositories;
using DotComServer.Domain.Services;
using DotComServer.Infrastructure.Repositories.Docx;
using DotComServer.Infrastructure.Services.Docx;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotComServer
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			var connectionString = Configuration.GetConnectionString("ApplicationDbConnection");
			services.AddDbContext<DocxDbContext>(options => options.UseSqlServer(connectionString));
			services.AddTransient<IDocxFileRepository, DocxFileRepository>();
			services.AddTransient<IDocxFileService, DocxFileService>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();

			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
