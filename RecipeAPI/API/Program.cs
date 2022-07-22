﻿using System;
using System.Text.Json;
using Newtonsoft.Json;
using RecipeAPI.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

var recipesList = new List<ServerRecipe>();
var categoriesList = new List<string>();
var jsonPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
string jsonFile = Path.Combine(jsonPath, "Recipes.json");

using (StreamReader r = new StreamReader(jsonFile))
{
	var Data = r.ReadToEnd();
	var Json = JsonConvert.DeserializeObject<List<ServerRecipe>>(Data);
	if (Json != null)
	{
		recipesList = Json;
	}
}

app.MapGet("/recipes", () =>
{
	return Results.Ok(recipesList);
});

app.MapPost("/recipes", (ServerRecipe recipe) =>
{
	recipesList.Add(recipe);
	Save();
	return Results.Created($"/recipes/{recipe.Id}", recipe);
});

app.MapDelete("/recipes", (Guid id) =>
{
	if (recipesList.Find(recipe => recipe.Id == id) is ServerRecipe recipe)
	{
		recipesList.Remove(recipe);
		Save();
		return Results.Ok(recipe);
	}
	return Results.NotFound(); //404 not found
});

app.MapPut("/recipes", (ServerRecipe editedRecipe) =>
{
	if (recipesList.Find(recipe => recipe.Id == editedRecipe.Id) is ServerRecipe recipe)
	{
		recipesList.Remove(recipe);
		recipesList.Add(editedRecipe);
		Save();
		return Results.NoContent();
	}
	return Results.NotFound();
});

app.MapGet("/category", () =>
{
	return Results.Ok(categoriesList);
});

app.MapPost("/category", (string category) =>
{
	categoriesList.Add(category);
	Save();
	return Results.Created($"/recipes/{category}", category);
});
app.MapDelete("/category", (string category) =>
{
	for (int i = 0; i < categoriesList.Count; ++i)
	{
		if (categoriesList[i] == category)
		{
			foreach (ServerRecipe r in recipesList)
			{
				r.Categories.Remove(category);
			}
			categoriesList.Remove(category);
			Save();
			return Results.Ok(category);
		}
	}
	return Results.NotFound();
});

app.MapPut("/category", (string oldCategory, string editCategory) =>
{
	for (int i = 0; i < categoriesList.Count; ++i)
	{
		if (categoriesList[i] == oldCategory)
		{
			categoriesList.Remove(oldCategory);
			categoriesList.Add(editCategory);
			foreach (var r in recipesList)
			{
				r.Categories.Remove(oldCategory);
				r.Categories.Add(editCategory);
			}
			Save();
			return Results.NoContent();
		}
	}
	return Results.NotFound();
});
void Save()
{
	File.WriteAllText(jsonFile, JsonConvert.SerializeObject(recipesList));
}
app.Run();