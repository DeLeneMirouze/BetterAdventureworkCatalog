#region using

#endregion

namespace BetterAdventureWorksWeb.Model.Search
{
    public sealed class FacetItemViewModel
    {
        public long Count { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
    }
}