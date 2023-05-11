using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Minimal_EF_Dapper.Business;
using Minimal_EF_Dapper.Business.Interface;
using Minimal_EF_Dapper.Domain.Database;
using Minimal_EF_Dapper.Endpoints.Segmented.Categories;
using Minimal_EF_Dapper.Endpoints.Segmented.Orders;
using Minimal_EF_Dapper.Endpoints.Segmented.Products;
using Minimal_EF_Dapper.Endpoints.Unified.Direct;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

//==================================================================================================
//Serviços
//==================================================================================================

//Adicionando o serviço do Swagger

builder.Services.AddSwaggerGen(c =>
{
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); // Resolve conflito de nomes de endpoits no swagger
    c.EnableAnnotations();
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
//Meus servicos personalizados da aplicacao
//--------------------------------------------------------------------------------------------------
builder.Services.AddScoped<IServiceAllProductsSold, ServiceAllProductsSold>();

//==================================================================================================
//Application
//==================================================================================================

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.DocExpansion(DocExpansion.None);//==> mostra os endpoints colapsados
    });
}

//==================================================================================================
// END POINTS
//==================================================================================================

// Product Endpoints -------------------------------------------------------------------------------

app.MapMethods(ProductGetAll.Template, ProductGetAll.Methods, ProductGetAll.Handle);

app.MapMethods(ProductGet.Template, ProductGet.Methods, ProductGet.Handle);

app.MapMethods(ProductPost.Template, ProductPost.Methods, ProductPost.Handle);

app.MapMethods(ProductPut.Template, ProductPut.Methods, ProductPut.Handle);

app.MapMethods(ProductDelete.Template, ProductDelete.Methods, ProductDelete.Handle);

app.MapMethods(ProductSoldGet.Template, ProductSoldGet.Methods, ProductSoldGet.Handle);

// Categorie Endpoints-------------------------------------------------------------------------------

app.MapMethods(CategoryGetAll.Template, CategoryGetAll.Methods, CategoryGetAll.Handle);

app.MapMethods(CategoryPost.Template, CategoryPost.Methods, CategoryPost.Handle);

app.MapMethods(CategoryPut.Template, CategoryPut.Methods, CategoryPut.Handle);

app.MapMethods(CategoryDelete.Template, CategoryDelete.Methods, CategoryDelete.Handle);

// Order Endpoints------------------------------------------------------------------------------------

app.MapMethods(OrderPost.Template, OrderPost.Methods, OrderPost.Handle);

app.MapMethods(OrderGet.Template, OrderGet.Methods, OrderGet.Handle);

//-----------------------------------------------------------------
// Endpoints Unificados
//-----------------------------------------------------------------
app.AddProductsEndPoints();
app.AddCategoriesEndPoints();
app.AddOrdersEndPoints();

//Alteracao de unificados para permitir teste integrado-------------
app.AddTestableProductsEndPoints();
app.AddTestableCategoriesEndPoints();
app.AddTestableOrdersEndPoints();

//--------------------------------------------------------------------------------
//GetAllSold
//(Caso especial classe estatica não aceita injecao de dependencia de interface)
//--------------------------------------------------------------------------------
app.MapGet("unified/Product/sold", async ([FromServices] IServiceAllProductsSold serviceAllProductsSold) =>
{
    var result = await serviceAllProductsSold.Execute();
    return Results.Ok(result);
}).WithTags("Unified Product");

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

app.UseHttpsRedirection();

app.Run();//<============ RUN