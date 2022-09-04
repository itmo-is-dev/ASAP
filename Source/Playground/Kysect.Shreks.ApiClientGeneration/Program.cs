﻿using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.OperationNameGenerators;

string filePath = "ShreksApiClient.cs";

var httpClient = new HttpClient();

var spec = await httpClient.GetStringAsync("https://localhost:7188/swagger/v1/swagger.json");

var document = await OpenApiDocument.FromJsonAsync(spec);

var settings = new CSharpClientGeneratorSettings
{
    ClassName = "{controller}Client",
    CSharpGeneratorSettings =
    {
        Namespace = "Shreks.ApiClient",
        TimeSpanType = "System.TimeSpan",
        DateType = "System.DateOnly",
    },
    GenerateClientInterfaces = true,
    ClientClassAccessModifier = "public",
    OperationNameGenerator = new MultipleClientsFromFirstTagAndPathSegmentsOperationNameGenerator(),
};


var generator = new CSharpClientGenerator(document, settings);
var code = generator.GenerateFile();

File.WriteAllText(filePath, code);