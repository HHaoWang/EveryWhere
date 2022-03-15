using EveryWhere.Database;
using EveryWhere.DTO.Settings;
using EveryWhere.MainServer.Contexts.Order;
using EveryWhere.MainServer.Contexts.Order.Representation;
using EveryWhere.MainServer.Contexts.Shop.Representation;
using EveryWhere.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


#region ��ӷ���

builder.WebHost.UseUrls("http://*:5500");

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
builder.Services.AddControllers().AddNewtonsoftJson(option => {
    option.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";

});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//������ݿ����
builder.Services.AddDbContext<Repository>(
    dbContextOptions => dbContextOptions
        .UseMySql(
            builder.Configuration.GetConnectionString("MainServerContext"),
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

//ע����Ŀ����
builder.Services.AddScoped<ShopRepresentationService, ShopRepresentationService>();
builder.Services.AddScoped<OrderApplicationService, OrderApplicationService>();
builder.Services.AddScoped<OrderRepo, OrderRepo>();
builder.Services.AddScoped<OrderRepresentationService, OrderRepresentationService>();

#endregion

var app = builder.Build();

#region ���ùܵ�

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();