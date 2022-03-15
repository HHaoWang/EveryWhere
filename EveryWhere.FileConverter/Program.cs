using EveryWhere.Database;
using EveryWhere.FileConverter;
using EveryWhere.FileConverter.DTO;
using EveryWhere.FileConverter.Workers;
using EveryWhere.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(configHost => { })
    .UseSystemd()
    .UseWindowsService()
    .UseSerilog((context, logger) =>
    {
        logger.ReadFrom.Configuration(context.Configuration);
        logger.WriteTo.File(Path.Combine(FileUtil.GetLogDirectory().FullName, "FileConverter", "log.txt"),
            rollingInterval: RollingInterval.Day,
            shared: true,
            retainedFileCountLimit: null);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<Settings>(hostContext.Configuration.GetSection("MessageQueue"));
        services.AddHostedService<Worker>();
        //������ݿ����
        //������ݿ����
        services.AddDbContext<Repository>(
            dbContextOptions => dbContextOptions
                .UseMySql(
                    hostContext.Configuration.GetConnectionString("FileServerContext"),
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
                }),
            ServiceLifetime.Singleton
        );
        services.AddSingleton<Converter, Converter>();
    })
    .Build();

await host.RunAsync();
