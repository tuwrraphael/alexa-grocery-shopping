using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillaSkill.Models
{

    public class BillaSearchResult
    {
        public Tile[] tiles { get; set; }
        public bool showFilter { get; set; }
        public string[] resultCategories { get; set; }
        public bool noResultsFound { get; set; }
        public Facet[] facets { get; set; }
        public Paginginfo pagingInfo { get; set; }
        public Sortinfo sortInfo { get; set; }
    }

    public class Paginginfo
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public int numResults { get; set; }
        public bool isFirstPage { get; set; }
        public bool isLastPage { get; set; }
    }

    public class Sortinfo
    {
        public string currentSort { get; set; }
        public string[] availableSort { get; set; }
        public string sortParameter { get; set; }
    }

    public class Tile
    {
        public float quantity { get; set; }
        public string type { get; set; }
        public Data data { get; set; }
        public object filter { get; set; }
        public string size { get; set; }
    }

    public class Data
    {
        public object category { get; set; }
        public string url { get; set; }
        public string articleOnlyCanonicalPath { get; set; }
        public string canonicalPath { get; set; }
        public object paths { get; set; }
        public object recipeID { get; set; }
        public string articleId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public object productText { get; set; }
        public string[] articleGroupIds { get; set; }
        public string slug { get; set; }
        public string brand { get; set; }
        public string video { get; set; }
        public bool vtcOnly { get; set; }
        public bool hasSpecialDeliveryWindows { get; set; }
        public bool isDelicatessen { get; set; }
        public string[] recommendationArticleIds { get; set; }
        public string grammage { get; set; }
        public float minimalOrderQuantity { get; set; }
        public float maximalOrderQuantity { get; set; }
        public int?[] quantitySteps { get; set; }
        public float orderQuantityStepSize { get; set; }
        public float score { get; set; }
        public object articleTypeIndicator { get; set; }
        public Price price { get; set; }
        public Vtcprice vtcPrice { get; set; }
        public string vatCode { get; set; }
        public string[] storeAvalibility { get; set; }
        public string[] badges { get; set; }
        public string[] attributes { get; set; }
        public object[] properties { get; set; }
    }

    public class Price
    {
        public float normal { get; set; }
        public float sale { get; set; }
        public string unit { get; set; }
        public float final { get; set; }
        public Priceadditionalinfo priceAdditionalInfo { get; set; }
        public string[] bulkDiscountPriceTypes { get; set; }
        public string[] defaultPriceTypes { get; set; }
    }

    public class Priceadditionalinfo
    {
        public string vptxt { get; set; }
    }

    public class Vtcprice
    {
        public float normal { get; set; }
        public float sale { get; set; }
        public string unit { get; set; }
        public float final { get; set; }
        public Priceadditionalinfo1 priceAdditionalInfo { get; set; }
        public string[] bulkDiscountPriceTypes { get; set; }
        public string[] defaultPriceTypes { get; set; }
    }

    public class Priceadditionalinfo1
    {
        public string vptxt { get; set; }
    }

    public class Facet
    {
        public Value[] values { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
    }

    public class Value
    {
        public int total { get; set; }
        public string value { get; set; }
        public bool active { get; set; }
    }

}
