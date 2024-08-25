using Umbraco.Cms.Persistence.Sqlite;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebOptimizer(pipeline => pipeline.MinifyCssFiles("css/**/*.css"));

builder.CreateUmbracoBuilder()
	.AddUmbracoSqliteSupport()
	.AddBackOffice()
	.AddWebsite()
	.AddDeliveryApi()
	.AddComposers()
	.Build();

WebApplication app = builder.Build();

await app.BootUmbracoAsync();

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
