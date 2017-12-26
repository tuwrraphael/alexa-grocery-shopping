namespace BillaSkill.Models
{
    public static class BillaSearchResultExtension
    {
        public static Ware ToWare(this Tile tile)
        {
            return new Ware() {
                Marke = tile.data.brand,
                Name = tile.data.name,
                Preis = tile.data.price.final,
                Menge = tile.data.grammage
            };
        }
    }
}
