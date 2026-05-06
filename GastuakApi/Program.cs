using JatetxeaApi;
using JatetxeaApi.Repositorioak;
using Microsoft.OpenApi.Models;
using NHibernate;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// --- Logging configuration ---
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Logging.AddFilter("Microsoft.AspNetCore.HttpLogging", LogLevel.Information);

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// --- Controllers ---
builder.Services.AddControllers();

// --- HTTP Logging (for endpoint tracking) ---
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPath |
                            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestMethod |
                            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseStatusCode;
});

// --- Swagger configuration (multiple documents) ---
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("inbentarioa", new OpenApiInfo
    {
        Version = "v1",
        Title = "Jatetxea API - Inbentarioa",
        Description = "Inbentarioko produktuak eta stock mugimenduak kudeatzeko endpointak."
    });

    c.SwaggerDoc("plateren_osagaiak", new OpenApiInfo
    {
        Version = "v1",
        Title = "Jatetxea API - Plateren Osagaiak",
        Description = "Plater bakoitzaren errezeta osagaiak kudeatzeko endpointak."
    });

    c.SwaggerDoc("platerak", new OpenApiInfo
    {
        Version = "v1",
        Title = "Jatetxea API - Platerak",
        Description = "Kartako platerak eta erabilgarritasuna kudeatzeko endpointak."
    });

    c.SwaggerDoc("zerbitzu_xehetasunak", new OpenApiInfo
    {
        Version = "v1",
        Title = "Jatetxea API - Zerbitzu Xehetasunak",
        Description = "Zerbitzu bateko plater lerroak eta zerbitzatuta egoera kudeatzeko endpointak."
    });

    c.SwaggerDoc("kategoria", new OpenApiInfo
    {
        Version = "v1",
        Title = "Jatetxea API - Kategoria",
        Description = "Plateren kategoriak kudeatzeko endpointak."
    });

    c.SwaggerDoc("zerbitzuak", new OpenApiInfo
    {
        Version = "v1",
        Title = "Jatetxea API - Zerbitzuak",
        Description = "Mahai edo erreserba bateko eskaerak kudeatzeko endpointak."
    });

    c.SwaggerDoc("langileak", new OpenApiInfo
    {
        Version = "v1",
        Title = "Jatetxea API - Langileak",
        Description = "Langileak eta login eragiketa kudeatzeko endpointak."
    });

    c.SwaggerDoc("rolak", new OpenApiInfo
    {
        Version = "v1",
        Title = "Jatetxea API - Rolak",
        Description = "Langileen rolak kudeatzeko endpointak."
    });

    c.SwaggerDoc("mahaiak", new OpenApiInfo
    {
        Version = "v1",
        Title = "Jatetxea API - Mahaiak",
        Description = "Jatetxeko mahaiak eta erabilgarritasuna kudeatzeko endpointak."
    });

    c.SwaggerDoc("erreserbak", new OpenApiInfo
    {
        Version = "v1",
        Title = "Jatetxea API - Erreserbak",
        Description = "Erreserbak sortu, bilatu eta kudeatzeko endpointak."
    });

    c.SwaggerDoc("jatetxeko_info", new OpenApiInfo
    {
        Version = "v1",
        Title = "Jatetxea API - Jatetxeko Info",
        Description = "Jatetxearen datu orokorrak eta kaxa totala kudeatzeko endpointak."
    });

    c.SwaggerDoc("deskontuak", new OpenApiInfo
    {
        Version = "v1",
        Title = "Jatetxea API - Deskontuak",
        Description = "Odoo datu-basetik deskontu kodeak balidatu eta aplikatzeko endpointak."
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);

    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (!apiDesc.TryGetMethodInfo(out var methodInfo))
            return false;

        var controllerName = methodInfo.DeclaringType?.Name ?? "";

        if (docName == "inbentarioa" && controllerName.Contains("InbentarioaController"))
            return true;

        if (docName == "plateren_osagaiak" && controllerName.Contains("PlaterenOsagaiakController"))
            return true;

        if (docName == "platerak" && controllerName.Contains("PlaterakController"))
            return true;

        if (docName == "zerbitzu_xehetasunak" && controllerName.Contains("ZerbitzuXehetasunakController"))
            return true;

        if (docName == "kategoria" && controllerName.Contains("KategoriaController"))
            return true;

        if (docName == "zerbitzuak" && controllerName.Contains("ZerbitzuakController"))
            return true;

        if (docName == "langileak" && controllerName.Contains("LangileakController"))
            return true;

        // Fixed: lowercase "rolak" to match doc name
        if (docName == "rolak" && controllerName.Contains("RolakController"))
            return true;

        if (docName == "mahaiak" && controllerName.Contains("MahaiakController"))
            return true;

        if (docName == "erreserbak" && controllerName.Contains("ErreserbakController"))
            return true;

        if (docName == "jatetxeko_info" && controllerName.Contains("JatetxekoInfoController"))
            return true;

        if (docName == "deskontuak" && controllerName.Contains("DeskontuakController"))
            return true;

        return false;
    });
});

// --- NHibernate ---
builder.Services.AddSingleton<ISessionFactory>(NHibernateHelper.SessionFactory);

// --- Repositories ---
builder.Services.AddTransient<InbentarioaRepository>();
builder.Services.AddTransient<PlaterenOsagaiakRepository>();
builder.Services.AddTransient<PlaterakRepository>();
builder.Services.AddTransient<ZerbitzuXehetasunakRepository>();
builder.Services.AddTransient<KategoriaRepository>();
builder.Services.AddTransient<ZerbitzuakRepository>();
builder.Services.AddTransient<LangileakRepository>();
builder.Services.AddTransient<RolakRepository>();
builder.Services.AddTransient<MahaiakRepository>();
builder.Services.AddTransient<ErreserbakRepository>();
builder.Services.AddTransient<JatetxekoInfoRepository>();
builder.Services.AddTransient<DeskuntuakRepository>();

// --- Hosted Service ---
builder.Services.AddHostedService<JatetxeaApi.BackService.KaxaTotalaKalkulatu>();

var app = builder.Build();

// --- Test log to confirm console output works ---
app.Logger.LogInformation("Jatetxea API started. Console logging and HTTP logging are active.");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/inbentarioa/swagger.json", "JatetxeaAPI/Inbentarioa");
        c.SwaggerEndpoint("/swagger/plateren_osagaiak/swagger.json", "JatetxeaAPI/Plateren_Osagaiak");
        c.SwaggerEndpoint("/swagger/platerak/swagger.json", "JatetxeaAPI/Platerak");
        c.SwaggerEndpoint("/swagger/zerbitzu_xehetasunak/swagger.json", "JatetxeaAPI/Zerbitzu_Xehetasunak");
        c.SwaggerEndpoint("/swagger/kategoria/swagger.json", "JatetxeaAPI/Kategoria");
        c.SwaggerEndpoint("/swagger/zerbitzuak/swagger.json", "JatetxeaAPI/Zerbitzuak");
        c.SwaggerEndpoint("/swagger/langileak/swagger.json", "JatetxeaAPI/Langileak");
        c.SwaggerEndpoint("/swagger/rolak/swagger.json", "JatetxeaAPI/Rolak");
        c.SwaggerEndpoint("/swagger/mahaiak/swagger.json", "JatetxeaAPI/Mahaiak");
        c.SwaggerEndpoint("/swagger/erreserbak/swagger.json", "JatetxeaAPI/Erreserbak");
        c.SwaggerEndpoint("/swagger/jatetxeko_info/swagger.json", "JatetxeaAPI/Jatetxeko_Info");
        c.SwaggerEndpoint("/swagger/deskontuak/swagger.json", "JatetxeaAPI/Deskontuak");
        c.RoutePrefix = "swagger";
    });
}

// --- Custom debug middleware (placed first to log all requests) ---
app.Use(async (context, next) =>
{
    app.Logger.LogInformation("REQUEST: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next.Invoke();
    app.Logger.LogInformation("RESPONSE: {StatusCode}", context.Response.StatusCode);
});

// --- Built-in HTTP Logging middleware (placed early to capture all requests) ---
app.UseHttpLogging();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.UseMiddleware<NHibernateSessionMiddleware>();
app.MapControllers();

app.Run();
