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

#region ��ӷ���

//���Log����
builder.Host.UseSerilog((context, logger) =>
{
    logger.ReadFrom.Configuration(context.Configuration);
    logger.WriteTo.File(Path.Combine(FileUtil.GetLogDirectory().FullName, "MainServer", "log.txt"),
        rollingInterval: RollingInterval.Day,
        shared: true,
        retainedFileCountLimit: null);
});

//���token���÷���
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("tokenConfig"));
//ʵ����token����
var sec = builder.Configuration.GetSection("tokenConfig");
var tokenSettings = ConfigurationBinder.Get<TokenSettings>(sec);

//���jwt��֤
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.SaveToken = true;
    option.TokenValidationParameters = new TokenValidationParameters
    {
        //У����ֵ
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.SaltValue)),
        //У��ǩ����
        ValidateIssuer = true,
        ValidIssuer = tokenSettings.Issuer,
        //У����֤��
        ValidateAudience = true,
        ValidAudience = tokenSettings.Audience,
        //У�����ʱ��
        ValidateLifetime = true
    };
});

//��� ���������������
builder.Services.AddHttpClient();

//ʹ��newtonsoftJson���д���
builder.Services.AddControllers().AddNewtonsoftJson(option =>
{
    option.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
});

//������ݿ����
builder.Services.AddDbContext<Repository>(
    dbContextOptions => dbContextOptions
        .UseMySql(
        builder.Configuration.GetConnectionString("EveryWhereContext"),
            new MySqlServerVersion(new Version(5, 7))
        )
        .EnableDetailedErrors()
        .ConfigureWarnings(b =>
        {
            //��sql��������־������debug����
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

#region �������

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