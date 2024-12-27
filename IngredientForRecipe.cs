namespace BF_Host
{
    public class IngredientForRecipe : BaseData
    {
        public string recipe_name {  get; set; } = string.Empty;
        
        public string ingredient_name { get; set; } = string.Empty;
        
        public int ingredient_quantity { get; set; } = 0;
    }
}

