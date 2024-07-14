
using log4net.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using YzMedia.Library.Common.BusinessObject;
using YzMedia.Library.Common.BusinessService;

var builder = WebApplication.CreateBuilder(args);

XmlConfigurator.Configure(new FileInfo("Config/log4netcore.config")); //Configure log4net
log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(Program));

try
{
	var path = Path.Combine(Directory.GetCurrentDirectory(), "Config", "appsettings.json");

	var config = new ConfigurationBuilder()
		.AddJsonFile(path, optional: true, reloadOnChange: true)
		.AddCommandLine(args)
		.Build();

	ReadAppSetting.ReadAppSettingsInfo(config);

	IConfiguration configuration = new ConfigurationBuilder()
	.AddJsonFile(path, optional: true, reloadOnChange: true)
	.Build();
	builder.Services.AddCors();

	builder.Services.AddLogging(builder =>
	{
		builder.AddConfiguration(configuration.GetSection("Logging"))
			   .AddConsole()
			   .AddDebug();
	});

	// Chạy load memory
	builder.Services.AddSingleton<IHostedService, RunBackgroundService>();

	builder.Services.Configure<FormOptions>(options =>
	{
		options.ValueCountLimit = int.MaxValue; // 200 items max
		options.ValueLengthLimit = 1024 * 1024 * 100; // 100MB max len form data
		options.MultipartBoundaryLengthLimit = int.MaxValue;
		options.MultipartBodyLengthLimit = int.MaxValue;
		options.MultipartHeadersLengthLimit = int.MaxValue;
	});

	builder.Services.Configure<IISServerOptions>(options =>
	{
		options.MaxRequestBodySize = int.MaxValue;
	});

	// web socket
	builder.Services.AddSingleton<IWebSocketProcess, WebSocketProcess>();

	builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = AppSetting.JWT_Issuer,
			ValidAudience = AppSetting.JWT_Issuer,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSetting.JWT_Key))
		};
	});

	// Add services to the container.

	builder.Services.AddControllers();
	// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();

	builder.Services.AddCors();

	var app = builder.Build();

	app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials());

	// Configure the HTTP request pipeline.
	app.UseSwagger();
	app.UseSwaggerUI();

	app.UseWebSockets(new WebSocketOptions()
	{
		KeepAliveInterval = TimeSpan.FromSeconds(120),
	});

	app.UseHttpsRedirection();

	app.UseRouting();

	app.UseAuthentication();
	app.UseAuthorization();

	app.MapControllers();

	//var _HostUrl = configuration["HostUrl"] ?? "http://*:8008";
	//app.Urls.Add(_HostUrl);
	app.Run();
}
catch (Exception ex)
{
	_logger.Error(ex.Message, ex);
	throw;
}