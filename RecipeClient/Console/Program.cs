using Spectre.Console;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using RecipeClient.Console;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;


IConfiguration config = new ConfigurationBuilder()
.AddJsonFile("appSettings.json")
.AddEnvironmentVariables()
.Build();
var url = config.GetRequiredSection("BaseUrl").Get<string>();

//Create HttpClient and add base address
HttpClient client = new HttpClient();
client.BaseAddress = new Uri(url);

client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
while (true)
{
	AnsiConsole.Write(
	new FigletText("A Winning Dish")
	.Centered()
	.Color(Color.DarkTurquoise));

	var command = AnsiConsole.Prompt(
	new SelectionPrompt<string>()
   .Title("What would you like to do?")
   .AddChoices(new[]
   {
		   "Add","Edit","Delete","List"
   }));
	AnsiConsole.Clear();
	switch (command)
	{
		case "Add":
			{
				var addChoice = AnsiConsole.Prompt(
				new SelectionPrompt<string>()
			   .Title("What would you like to add?")
			   .AddChoices(new[]
				{
					"Add a Recipe","Add a Category"
				}));
				switch (addChoice)
				{
					case "Add a Recipe":
						{
							Recipe recipe = ConsoleUi.AddRecipe(await listCategoriesAsync());
							await postRecipeAsync(recipe);
							break;
						}
					case "Add a Category":
						{
							string category = ConsoleUi.AddCategory();
							await postCategoryAsync(ConsoleUi.AddCategory());
							break;
						}

				}
				break;
			}
		case "Edit":
			{
				var editChoice = AnsiConsole.Prompt(
				new SelectionPrompt<string>()
			   .Title("What would you like to edit?")
			   .AddChoices(new[]
				{
					"Edit a Recipe","Edit a Category"
			   }));
				switch (editChoice)
				{
					case "Edit a Recipe":
						{
							Recipe recipe = ConsoleUi.EditRecipe(await listRecipesAsync(), await listCategoriesAsync());
							if (recipe != null)
								await putRecipeAsync(recipe);
							break;
						}
					case "Edit a Category":
						{
							var oldCategory = ConsoleUi.EditCategory(await listRecipesAsync(), await listCategoriesAsync());
							var newCategory = ConsoleUi.AddCategory();
							await putCategoryAsync(oldCategory, newCategory);
							break;
						}
				}
				break;
			}
		case "Delete":
			{
				var deleteChoice = AnsiConsole.Prompt(
				new SelectionPrompt<string>()
				.Title("What would you like to do?")
				.AddChoices(new[]
				{
					"Delete a Recipe","Delete a Category"
				}));
				switch (deleteChoice)
				{
					case "Delete a Recipe":
						{
							var selectedRecipes = ConsoleUi.ChooseRecipes(await listRecipesAsync());
							await deleteRecipesAsync(selectedRecipes);
							break;
						}

					case "Delete a Category":
						{
							var selectedCategories = ConsoleUi.SelectCategory(await listCategoriesAsync());
							await deleteCategoriesAsync(selectedCategories);
							break;
						}
				}

				break;
			}
		case "List":
			{
				ConsoleUi.ListRecipes(await listRecipesAsync());
				break;
			}

	}


	async Task<List<Recipe>> listRecipesAsync()
	{
		var recipeList = await client.GetFromJsonAsync<List<Recipe>>("recipes");
		if (recipeList != null)
			return recipeList;
		return new List<Recipe>();
	}

	async Task<List<string>> listCategoriesAsync()
	{
		var categoriesList = await client.GetFromJsonAsync<List<string>>("category");
		if (categoriesList != null)
			return categoriesList;
		return new List<string>();
	}

	async Task postRecipeAsync(Recipe recipe)
	{
		var result = await client.PostAsJsonAsync("recipes", recipe, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
	}

	async Task deleteRecipesAsync(List<Recipe> recipesList)
	{
		var deleteTasks = new List<Task>();
		foreach (var recipe in recipesList)
			deleteTasks.Add(client.DeleteAsync("recipes?id=" + recipe.Id));
		await Task.WhenAll(deleteTasks);
	}

	async Task putRecipeAsync(Recipe recipe)
	{
		await client.PutAsJsonAsync("recipes", recipe);
	}

	async Task postCategoryAsync(string category)
	{
		var result = await client.PostAsJsonAsync("category", category, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
	}

	async Task deleteCategoriesAsync(List<string> categoriesList)
	{
		var deleteTasks = new List<Task>();
		foreach (var category in categoriesList)
			deleteTasks.Add(client.DeleteAsync("category?category=" + category));
		await Task.WhenAll(deleteTasks);
	}

	async Task putCategoryAsync(string oldCategory, String editedCategory)
	{
		await client.PutAsync($"categories?oldcategory={oldCategory}&editedcategory={editedCategory}", null);
	}
}