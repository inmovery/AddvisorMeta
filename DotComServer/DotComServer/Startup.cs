using DotComServer.Domain.Repositories;
using DotComServer.Domain.Services;
using DotComServer.Infrastructure.Repositories.Documents;
using DotComServer.Infrastructure.Services.Documents;
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

			services.Configure<IISServerOptions>(options =>
			{
				options.AllowSynchronousIO = true;
			});

			var connectionString = Configuration.GetConnectionString("ApplicationDbConnection");
			services.AddDbContext<DocumentsDbContext>(options => options.UseSqlServer(connectionString));
			services.AddTransient<IDocumentsFileRepository, DocumentsFileRepository>();
			services.AddTransient<IDocumentsFileService, DocumentsFileService>();
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
