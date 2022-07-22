namespace RecipeAPI.API
{
    public class ServerRecipe
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<string> Ingredients { get; set; } = new List<string>();
        public List<string> Instructions { get; set; } = new List<string>();
        public List<string> Categories { get; set; } = new List<string>();

        public ServerRecipe()
        {
            Id = Guid.NewGuid();
            this.Ingredients = new List<string>();
            this.Title = " ";
            this.Instructions = new List<string>();
            this.Categories = new List<string>();
        }
        public ServerRecipe(string title, List<string> ingredients, List<string> instructions, List<string> categories)
        {
            Id = Guid.NewGuid();
            this.Ingredients = ingredients;
            this.Title = title;
            this.Instructions = instructions;
            this.Categories = categories;
        }
    }
}
