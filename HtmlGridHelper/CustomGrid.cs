using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CodeForApp.Core.Helpers
{
    public static class CustomGridGenerator 
    {
        private static GridProperties _gridProperties = new GridProperties();

        public static MvcHtmlString GridHtml(this HtmlHelper html, Action<GridProperties> method)
        {
            var mySettings = new GridProperties();

            method(mySettings);
           
            _gridProperties = mySettings;

            return new MvcHtmlString("");
        }

        public static MvcHtmlString Bind<T>(this MvcHtmlString html, IEnumerable<T> source) where T : class
        {
            return HtmlGridHelper.Grid(_gridProperties.GridName, source, _gridProperties.GridMenu,
                                       _gridProperties.RowChangedFunction, _gridProperties.KeyFieldNames);

        }
    }
}
