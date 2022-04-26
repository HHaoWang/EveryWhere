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
IConfigurationSection? sec = builder.Configuration.GetSection("tokenConfig");
TokenSettings? tokenSettings = sec.Get<TokenSettings>();
//�����Ϣ���з�������
builder.Services.Configure<MessageQueueSettings>(builder.Configuration.GetSection("MessageQueue"));

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
    option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    option.SerializerSettings.Converters.Add(new DateOnlyJsonConverter());
    option.SerializerSettings.Converters.Add(new TimeOnlyJsonConverter());
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

//��ӻ������
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
    // Ϊ Swagger JSON and UI����xml�ĵ�ע��·��
    string? basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//��ȡӦ�ó�������Ŀ¼
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

//app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseWebSockets();

app.MapControllers();

app.Run();

#endregion