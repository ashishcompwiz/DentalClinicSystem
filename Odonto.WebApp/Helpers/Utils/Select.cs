using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Odonto.WebApp.Helpers.Utils
{
    public static class Select
    {
        public static List<SelectListItem> CreateSelectList(IEnumerable<dynamic> list)
        {
            var selectList = new List<SelectListItem>();

            foreach (var item in list)
                selectList.Add(new SelectListItem { Text = item.Name, Value = item.ID.ToString() });

            return selectList;
        }
    }
}
