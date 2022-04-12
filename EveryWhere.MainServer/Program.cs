using System.Configuration;
using System.Text;
using EveryWhere.Database;
using EveryWhere.DTO.Settings;
using EveryWhere.MainServer.Services;
using EveryWhere.MainServer.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
var sec = builder.Configuration.GetSection("tokenConfig");
var tokenSettings = ConfigurationBinder.Get<TokenSettings>(sec);

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

// Add services to the container.
builder.Services.AddScoped<UserService,UserService>();
builder.Services.AddScoped<ShopService,ShopService>();
builder.Services.AddScoped<AreaService, AreaService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

#endregion