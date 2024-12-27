namespace BF_Host
{
    public class Recipe : BaseData
    {
        public string name { get; set; } = string.Empty;
        
        public string product_name { get; set; } = string.Empty;

        public int product_quantity { get; set; } = 0;
    }
}
