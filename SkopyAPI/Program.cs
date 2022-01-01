using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

var app = builder.Build();
app.UseCors(builder => 
    builder.AllowAnyHeader()
    .AllowAnyOrigin()
    .AllowAnyMethod());

var problemFileDirectory = "TestFiles";

app.MapGet("/loadFile/{filename}", (string filename) =>
{
    var skopySolver = new Skopy.SkopySolver();
    var fullPath = Path.Combine(problemFileDirectory, filename);
    var treesAndToys = Skopy.ReadProblemFile.ReadFile(fullPath);
    skopySolver.Init(treesAndToys.Item2, treesAndToys.Item1);
    skopySolver.AnswerFromAnsFile = Skopy.ReadProblemFile.ReadAnswerFile(fullPath);
    return new JsonResult(skopySolver);
});

app.MapPost("/solve", (Skopy.SkopySolver skopySolver) =>
{
    skopySolver.Solve();
    return new JsonResult(skopySolver);
});

app.Run();
