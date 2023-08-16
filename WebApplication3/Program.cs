using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var produtos = new List<Atividade01.API.Model.Produto>
{
    new Atividade01.API.Model.Produto { Id = 1, Nome = "Azeitona", Preco = 16.5m, Quantidade = 10 },
    new Atividade01.API.Model.Produto { Id = 2, Nome = "Abacate", Preco = 2.5m, Quantidade = 5 },
    new Atividade01.API.Model.Produto { Id = 3, Nome = "Pera", Preco = 3.8m, Quantidade = 6 }
};

builder.Services.AddSingleton(produtos);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Atividade01.API", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/getProdutos", () =>
{
    var produtoService = app.Services.GetRequiredService<List<Atividade01.API.Model.Produto>>();
    return Results.Ok(produtoService);
});

app.MapGet("/getProdutos/{id}", (int id, HttpRequest request) =>
{
    var produtoService = app.Services.GetRequiredService<List<Atividade01.API.Model.Produto>>();
    var produto = produtoService.FirstOrDefault(t => t.Id == id);

    if (produto == null)
    {
        return Results.NotFound("O produto solicitado não existe!");
    }

    return Results.Ok(produto);
});


app.MapPost("/adicionarProduto", (Atividade01.API.Model.Produto produto) =>
{
    var produtoService = app.Services.GetRequiredService<List<Atividade01.API.Model.Produto>>();
    produto.Id = produtoService.Max(t => t.Id) + 1;

    if (produto.Nome == null || produto.Nome == "string" || produto.Nome == "" || produto.Quantidade == 0 || produto.Preco == 0m)
    {
        return Results.BadRequest("Existem dados faltantes e/ou inválidos no produto a ser adicionado. Corrija e tente novamente!");
    }

    produtoService.Add(produto);
    return Results.Created($"/produtos/{produto.Id}", produto);
});

app.MapPut("/editarProduto/{id}", (int id, Atividade01.API.Model.Produto produto) =>
{
    var produtoService = app.Services.GetRequiredService<List<Atividade01.API.Model.Produto>>();
    var existingProduto = produtoService.FirstOrDefault(t => t.Id == id);

    if (existingProduto == null)
    {
        return Results.NotFound();
    }

    existingProduto.Nome = produto.Nome;
    existingProduto.Preco = produto.Preco;
    existingProduto.Quantidade = produto.Quantidade;

    return Results.NoContent();
});

app.MapDelete("/removerProduto/{id}", (int id) =>
{
    var produtoService = app.Services.GetRequiredService<List<Atividade01.API.Model.Produto>>();
    var existingProduto = produtoService.FirstOrDefault(t => t.Id == id);

    if (existingProduto == null)
    {
        return Results.NotFound();
    }

    produtoService.Remove(existingProduto);

    return Results.NoContent();
});
app.Run();
