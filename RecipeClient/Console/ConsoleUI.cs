using System.Text;
using Spectre.Console;

namespace RecipeClient.Console;

internal class ConsoleUi
{
    static List<string> CategoryList = new List<string>();
    // Adding a Recipe
    public static Recipe AddRecipe(List<string> categoryList)
    {
        Recipe recipe = new Recipe();
        var title = AnsiConsole.Ask<string>("Title:");
        recipe.Title = title;
        var ingredients = new List<string>();
        var instructions = new List<string>();
        categoryList = CategoryList;

        AnsiConsole.MarkupLine("[gray] after you're done of writing instructions press space to move to next step [/]");
        var ingredient = AnsiConsole.Ask<string>("Enter recipe ingredient: ");
        while (ingredient != "")
        {
            recipe.Ingredients.Add(ingredient);
            ingredient = AnsiConsole.Prompt(new TextPrompt<string>("Enter recipe ingredients: ").AllowEmpty());
        };

        AnsiConsole.MarkupLine("[gray] after you're done of writing ingredients press space to move to next step [/]");
        var instruction = AnsiConsole.Ask<string>("Enter recipe instructions: ");
        while (instruction != "")
        {
            recipe.Instructions.Add(instruction);
            instruction = AnsiConsole.Prompt(new TextPrompt<string>("Enter recipe instructions: ").AllowEmpty());

        };

        if (CategoryList.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]There are no categories[/]");
            recipe.Categories.Add("[red]not assigned to specific Category yet[/]");
            return recipe;
        }
        else
        {
            var selectedcategories = AnsiConsole.Prompt(
            new MultiSelectionPrompt<String>()
           .PageSize(10)
           .Title(" Which categories does this recipe belong to?")
           .MoreChoicesText("[gray](Move up and down to reveal more categories)[/]")
           .InstructionsText("[gray](Press space to toggle a category, Enter to accept)[/]")
           .AddChoices(CategoryList));

            recipe.Categories = selectedcategories;
            return recipe;
        }
        return recipe;
    }
    // Listing a Recipe
    public static void ListRecipes(List<Recipe> recipesList)
    {
        var table = new Table();
        table.AddColumn("Recipe Name");
        table.AddColumn("Ingredients");
        table.AddColumn("Instructions");
        table.AddColumn("Categories");

        foreach (var recipe in recipesList)
        {
            int ingredientCounter = 0;
            var ingredients = new StringBuilder();
            foreach (String ingredient in recipe.Ingredients)
            {
                ingredientCounter++;
                ingredients.Append(ingredientCounter + "-" + "[gray]" + ingredient + "[/]" + "\n");
            }

            int instructionCounter = 0;
            var instructions = new StringBuilder();
            foreach (String instruction in recipe.Instructions)
            {
                instructionCounter++;
                instructions.Append(instructionCounter + "-" + "[gray]" + instruction + "[/]" + "\n");
            }

            var categories = new StringBuilder();
            foreach (String category in recipe.Categories)
            {
                categories.Append("-" + "[gray]" + category + "[/]" + "\n");
            }

            table.AddRow(recipe.Title, ingredients.ToString(), instructions.ToString(), categories.ToString());
        }

        AnsiConsole.Write(table);
    }
    // Editing a Recipe
    public static Recipe EditRecipe(List<Recipe> recipesList, List<string> categoriesList)
    {
        if (recipesList.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]There are no recipes yet [/]");
            return null;
        }

        var chosenRecipe = AnsiConsole.Prompt(
        new SelectionPrompt<Recipe>()
        .Title("Which Recipe would you like to edit?")
        .AddChoices(recipesList));

        var command = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .Title("What would you like to edit?")
        .AddChoices(new[]
        {
           "Edit title",
           "Edit Ingredients",
           "Edit Instructions",
           "Edit Categories"
        }));

        AnsiConsole.Clear();
        switch (command)
        {
            case "Edit title":
                chosenRecipe.Title = AnsiConsole.Ask<string>("What is the new name?");
                break;
            case "Edit Ingredients":
                chosenRecipe.Ingredients.Clear();
                AnsiConsole.MarkupLine("[gray] after you're done of writing ingredients press space to move to next step [/]");
                var ingredient = AnsiConsole.Ask<string>("Enter ingredient: ");
                while (ingredient != "")
                {
                    chosenRecipe.Ingredients.Add(ingredient);
                    ingredient = AnsiConsole.Prompt(new TextPrompt<string>("Enter ingredient: ").AllowEmpty());
                };
                break;
            case "Edit Instructions":
                chosenRecipe.Instructions.Clear();
                AnsiConsole.MarkupLine("[gray] after you're done of writing instructions press space to move to next step [/]");
                var instruction = AnsiConsole.Ask<string>("Enter instruction: ");
                while (instruction != "")
                {
                    chosenRecipe.Instructions.Add(instruction);
                    instruction = AnsiConsole.Prompt(new TextPrompt<string>("Enter instruction: ").AllowEmpty());
                };
                break;
            case "Edit Category":
                var selectedcategories = AnsiConsole.Prompt(
                new MultiSelectionPrompt<String>()
                .PageSize(10)
                .Title("Which category does this recipe belong to?")
                .MoreChoicesText("[grey](Move up and down to reveal more categories)[/]")
                .InstructionsText("[grey](Press Space to toggle a category, Enter to choose the category you toggeled)")
                .AddChoices(categoriesList));

                chosenRecipe.Categories = selectedcategories;
                break;
        }
        AnsiConsole.Write("[green] Successfully edited[/]");
        return chosenRecipe;
    }
    // Adding a Category
    public static string AddCategory()
    {
        AnsiConsole.MarkupLine("[gray] after you're done of writing categories press space to move to next step [/]");
        string category = AnsiConsole.Ask<string>("What is the category called?");
        CategoryList.Add(category);
        return category;
    }
    public static List<string> SelectCategory(List<string> categoriesList)
    {
        categoriesList = CategoryList;
        if (categoriesList.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]There are no categories [/]");
            Environment.Exit(0);
        }
        var selectedCategory = AnsiConsole.Prompt(
        new MultiSelectionPrompt<String>()
        .PageSize(10)
        .Title(" Which [white]categories[/] does this recipe belong to?")
        .MoreChoicesText("[grey](Move up and down to reveal more categories)[/]")
        .InstructionsText("[grey](Press [blue]Space[/] to toggle a category, [green]Enter[/] to accept)[/]")
        .AddChoices(categoriesList));
        int i = 0;
        foreach (var category in CategoryList.ToList())
        {
            if (category == selectedCategory[i])
            {
                CategoryList.Remove(category);
            }
        }

        return CategoryList;
    }
    // Editing a category
    public static string EditCategory(List<Recipe> recipesList, List<string> categoriesList)
    {
        categoriesList = CategoryList;
        if (categoriesList.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]There are no Categories to be edited[/]");
            return null;
        }

        var chosenCategory = AnsiConsole.Prompt(
        new SelectionPrompt<string>().Title("Which Category would you like to edit?").AddChoices(categoriesList));

        String newCategoryName = AnsiConsole.Prompt(new TextPrompt<string>("What is the new name?"));

        CategoryList.Remove(chosenCategory);
        CategoryList.Add(newCategoryName);
        int i = 0;
        foreach (var r in recipesList)
        {
            if (i < recipesList.Count && i < recipesList[i].Categories.Count && r.Categories[i] == recipesList[i].Categories[i])
            {
                r.Categories[i] = newCategoryName;
            }
            i++;
        }
        return chosenCategory;
    }
    public static List<Recipe> ChooseRecipes(List<Recipe> recipesList)
    {
        if (recipesList.Count == 0)
        {
            AnsiConsole.MarkupLine("There are no Recipes");
            Environment.Exit(0);
        }
        var selectedRecipes = AnsiConsole.Prompt(
        new MultiSelectionPrompt<Recipe>()
        .PageSize(10)
        .Title("Which recipes does this recipe belong to?")
        .InstructionsText("[grey](Press Space to toggle a recipe, Enter to accept)[/]")
        .AddChoices(recipesList));

        return selectedRecipes;
    }
    public static string ChooseCategory(List<string> categoriesList)
    {
        if (categoriesList.Count == 0)
        {
            AnsiConsole.WriteLine("There are no categories!");
            Environment.Exit(0);
        }
        var chosenCategory = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .Title("Which Category would you like to edit?")
        .AddChoices(categoriesList));

        return chosenCategory;
    }
}