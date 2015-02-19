#region using
using RedDog.Search.Model;
using System.Collections.Generic;

#endregion

namespace BetterAdventureWorksWeb.Model.Search
{
    public sealed class SuggestViewModelBuilder
    {
        #region Build
        public List<string> Build(IEnumerable<SuggestionResultRecord> data)
        {
            List<string> suggestions = new List<string>();

            foreach (var item in data)
            {
                suggestions.Add(item.Text);
            }

            return suggestions;
        } 
        #endregion
    }
}