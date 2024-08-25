using Microsoft.AspNetCore.ResponseCompression;
using System.Data.Common;
using System.IO.Compression;
using Umbraco.Cms.Persistence.Sqlite;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebOptimizer(pipeline => pipeline.MinifyCssFiles("css/**/*.css"));

builder.Services.AddResponseCompression(options =>
{
	options.EnableForHttps = true;
	options.Providers.Add<BrotliCompressionProvider>();
	options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
	options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
	options.Level = CompressionLevel.SmallestSize;
});


builder.CreateUmbracoBuilder()
	.AddUmbracoSqliteSupport()
	.AddBackOffice()
	.AddWebsite()
	.AddDeliveryApi()
	.AddComposers()
	.Build();

WebApplication app = builder.Build();

await app.BootUmbracoAsync();

app.UseResponseCompression();

app.UseWebOptimizer();
app.UseUmbraco()
	.WithMiddleware(u =>
	{
		u.UseBackOffice();
		u.UseWebsite();
	})
	.WithEndpoints(u =>
	{
		u.UseBackOfficeEndpoints();
		u.UseWebsiteEndpoints();
	});


await app.RunAsync();
