using System.Text;
using EveryWhere.Database;
using EveryWhere.Database.JsonConverter;
using EveryWhere.DTO.Settings;
using EveryWhere.MainServer.Entity.Setting;
using EveryWhere.MainServer.Infrastructure.MessageQueue;
using EveryWhere.MainServer.Infrastructure.Websocket;
using EveryWhere.MainServer.Services;
using EveryWhere.MainServer.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

#region 添加服务

//添加Log服务
builder.Host.UseSerilog((context, logger) =>
{
    logger.ReadFrom.Configuration(context.Configuration);
    logger.WriteTo.File(Path.Combine(FileUtil.GetLogDirectory().FullName, "MainServer", "log.txt"),
        rollingInterval: RollingInterval.Day,
        shared: true,
        retainedFileCountLimit: null);
});

//添加token配置服务
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("tokenConfig"));
//实例化token配置
IConfigurationSection? sec = builder.Configuration.GetSection("tokenConfig");
TokenSettings? tokenSettings = sec.Get<TokenSettings>();
//添加消息队列服务配置
builder.Services.Configure<MessageQueueSettings>(builder.Configuration.GetSection("MessageQueue"));

//添加jwt验证
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.SaveToken = true;
    option.TokenValidationParameters = new TokenValidationParameters
    {
        //校验盐值
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.SaltValue)),
        //校验签发者
        ValidateIssuer = true,
        ValidIssuer = tokenSettings.Issuer,
        //校验验证者
        ValidateAudience = true,
        ValidAudience = tokenSettings.Audience,
        //校验过期时间
        ValidateLifetime = true
    };
});

//添加 发送网络请求服务
builder.Services.AddHttpClient();

//使用newtonsoftJson进行处理
builder.Services.AddControllers().AddNewtonsoftJson(option =>
{
    option.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    option.SerializerSettings.Converters.Add(new DateOnlyJsonConverter());
    option.SerializerSettings.Converters.Add(new TimeOnlyJsonConverter());
});

//添加数据库服务
builder.Services.AddDbContext<Repository>(
    dbContextOptions => dbContextOptions
        .UseMySql(
        builder.Configuration.GetConnectionString("EveryWhereContext"),
            new MySqlServerVersion(new Version(5, 7))
        )
        .EnableDetailedErrors()
        .ConfigureWarnings(b =>
        {
            //把sql语句输出日志提升到debug级别
            b.Log(
                (RelationalEventId.CommandExecuted, LogLevel.Debug),
                (RelationalEventId.ConnectionOpening, LogLevel.Debug)
            );
        })
);

//添加缓存服务
builder.Services.AddMemoryCache();

// Add services to the container.
builder.Services.AddScoped<UserService,UserService>();
builder.Services.AddScoped<ShopService,ShopService>();
builder.Services.AddScoped<AreaService, AreaService>();
builder.Services.AddScoped<FileService, FileService>();
builder.Services.AddScoped<OrderService, OrderService>();
builder.Services.AddScoped<PrinterService, PrinterService>();
builder.Services.AddScoped<PrintJobService, PrintJobService>();
builder.Services.AddScoped<Publisher, Publisher>();

builder.Services.AddSingleton<Hub,Hub>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // 为 Swagger JSON and UI设置xml文档注释路径
    string? basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录
    string xmlPath = Path.Combine(basePath!, $"{typeof(Program).Assembly.GetName().Name}.xml");
    c.IncludeXmlComments(xmlPath, true);
    c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
            },
            Array.Empty<string>()
        }
    });
});

#endregion

WebApplication app = builder.Build();

#region 服务加载

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

//app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseWebSockets();

app.MapControllers();

app.Run();

#endregion