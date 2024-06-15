using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddHttpClient();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors();

const string url =
    "https://api.z-api.io/instances/3D0D4E6E823FF08863F672B70F2FFCF9/token/AA2A36974D53AF768D41DC4F/send-text";

app.MapPost("/send-text", async (HttpClient client, SendTextRequest request) =>
{
    var json = request.ToJson();
    client.DefaultRequestHeaders.Add("Client-Token", "F4917100691124675b3422040300fffa2S");
    var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
    
    var responseContent = await response.Content.ReadAsStringAsync();
    
    //preciso retornar o status code da resposta 200 caso sucesso e 400 caso erro
    if (response.StatusCode == System.Net.HttpStatusCode.OK)
        return Results.Ok();
    
    return Results.Json(responseContent, AppJsonSerializerContext.Default.HttpResponseMessage);
});

app.Run();

internal record SendTextRequest
{
    public string phone { get; init; }
    public string message { get; init; }
    public string ToJson()
        => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.SendTextRequest);
}

[JsonSerializable(typeof(SendTextRequest))]
[JsonSerializable(typeof(HttpResponseMessage))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}