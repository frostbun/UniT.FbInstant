namespace UniT.FbInstant
{
    using Newtonsoft.Json;
    using UnityEngine.Scripting;

    [Preserve]
    public sealed class Product
    {
        public string ProductId         { get; }
        public string Title             { get; }
        public string Description       { get; }
        public string ImageUri          { get; }
        public string Price             { get; }
        public float  PriceAmount       { get; }
        public string PriceCurrencyCode { get; }

        [JsonConstructor]
        internal Product(string productId, string title, string description, string imageUri, string price, float priceAmount, string priceCurrencyCode)
        {
            this.ProductId         = productId;
            this.Title             = title;
            this.Description       = description;
            this.ImageUri          = imageUri;
            this.Price             = price;
            this.PriceAmount       = priceAmount;
            this.PriceCurrencyCode = priceCurrencyCode;
        }
    }
}