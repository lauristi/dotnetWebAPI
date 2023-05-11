using AutoMapper;
using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.UnitOfWork;
using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.UnitOfWork.Interface;
using Controller_EF_Dapper_Repository_UnityOfWork.Business;
using Controller_EF_Dapper_Repository_UnityOfWork.Business.Interface;
using Controller_EF_Dapper_Repository_UnityOfWork.Domain.Database;
using Controller_EF_Dapper_Repository_UnityOfWork.Mapping;
using Controller_EF_Dapper_Repository_UnityOfWork.Repository.Repositories;
using Controller_EF_Dapper_Repository_UnityOfWork.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Text.Json;

//==================================================================================================
//Serviços
//==================================================================================================

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//Adicionando o serviço do Swagger

builder.Services.AddSwaggerGen(c =>
{
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); // Resolve conflito de nomes de endpoits no swagger
});

//--------------------------------------------------------------------------------------------------
//Log de aplicaçõa com SeriLog
//--------------------------------------------------------------------------------------------------

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .WriteTo.Console()
        .WriteTo.MSSqlServer(
            context.Configuration["Database:SQlServer"],
            sinkOptions: new MSSqlServerSinkOptions()
            {
                AutoCreateSqlTable = true,
                TableName = "ApplicationLog"
            });
});

//--------------------------------------------------------------------------------------------------
//DBContext
//--------------------------------------------------------------------------------------------------
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SQlServer"]);
builder.Services.AddMvc();

builder.Services.AddEndpointsApiExplorer();

//--------------------------------------------------------------------------------------------------
//Auto Mappper
//--------------------------------------------------------------------------------------------------

// Auto Mapper Configurations
var mapperConfig = new MapperConfiguration(c =>
{
    c.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//--------------------------------------------------------------------------------------------------
//Meus servicos personalizados da aplicacao
//--------------------------------------------------------------------------------------------------
builder.Services.AddScoped<IServiceProductSold, ServiceProductSold>();
builder.Services.AddScoped<IServiceOrderDetailed, ServiceOrderDetailed>();

//Repositorios (Repository DDD)
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

//==================================================================================================
//Application
//==================================================================================================

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//-----------------------------------------------------------------
//filtro de erros
//-----------------------------------------------------------------

app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext http) =>
{
    var error = http.Features?.Get<IExceptionHandlerFeature>()?.Error;

    if (error != null)
    {
        if (error is SqlException)
            return Results.Problem(title: "DataBase Out!!!", statusCode: 500);
        else if (error is FormatException)
            return Results.Problem(title: "Error to convert data to other type format", statusCode: 500);
        else if (error is JsonException)
            return Results.Problem(title: "Format error on current Json", statusCode: 500);
    }

    return Results.Problem(title: "An error ocurred", statusCode: 500);
});

app.Run(); //<============ RUN