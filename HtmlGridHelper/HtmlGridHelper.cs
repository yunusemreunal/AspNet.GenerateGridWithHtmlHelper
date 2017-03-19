using System.Web.Mvc;
using Derindere.Core.Attributes;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeForApp.Core.Helpers
{
    /// <summary>
    /// Bootstrap grid helper class
    /// </summary>
    public static class HtmlGridHelper
    {

        /// <summary>
        /// Grid oluşturmaya yardımcı olan metod
        /// </summary>
        /// <param name="gridName">Grid id'sini temsil eder. </param>
        /// <param name="modelValues">Gridde kullanılacak modeli temsil eder.</param>
        /// <param name="actions">İşlemler menüsünü temsil eder.</param>
        /// <param name="keyFields">Gride eklenecek keyfield alanlarını temsil eder.</param>
        /// <param name="rowChangeFunction">Gridde herhangi bir satıra tıklandığında tetiklenecek fonksiyon ismini temsil eder.</param>
        /// <typeparam name="T">Gridde görüntülenecek generic tipi temsil eder</typeparam>
        /// <returns></returns>
        public static MvcHtmlString Grid<T>(string gridName, IEnumerable<T> modelValues, string actions, string rowChangeFunction = "", params string[] keyFields) where T : class
        {

            var sb = new StringBuilder();

            var dataFunction = "";

            if (!string.IsNullOrEmpty(rowChangeFunction))
            {
                dataFunction = string.Format("data-func = \"{0}();\"", rowChangeFunction);
            }

            var gridHeader = string.Format("<table id=\"example\" {1} class=\"table table-hover table-condensed \">", gridName, dataFunction);

            sb.Append(gridHeader);

            sb.Append("<thead>");

            sb.Append("<tr>");

            var dto = typeof(T);

            var gridAttributeList = new List<GridAttribute>();

            foreach (var property in dto.GetProperties())
            {
                var attributeList = property.GetCustomAttributes(typeof(GridAttribute), true);

                foreach (var gridAttribute in attributeList)
                {
                    if (!(gridAttribute is GridAttribute)) continue;

                    var attr = gridAttribute as GridAttribute;

                    gridAttributeList.Add(attr);
                }
            }

            foreach (var gridAttribute in gridAttributeList.OrderBy(p => p.ColumnIndex))
            {
                sb.Append("<th style=\"width:" + gridAttribute.ColumnWidth + "%;\">" + gridAttribute.Title + "</th>");
            }

            if (!string.IsNullOrEmpty(actions))
            {
                sb.Append("<th style=\"width:130px !important;\">İşlemler</th>");
            }

            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append("<tbody id=\"grid\">");

            foreach (var model in modelValues)
            {
                var rowAttrListAndProperty = new List<KeyValuePair<GridAttribute, PropertyInfo>>();

                foreach (var m in model.GetType().GetProperties())
                {
                    var attributeList = m.GetCustomAttributes(typeof(GridAttribute), true);

                    foreach (var gridAttribute in attributeList)
                    {
                        if (gridAttribute is GridAttribute)
                        {
                            var attr = gridAttribute as GridAttribute;

                            var newDictinory = new KeyValuePair<GridAttribute, PropertyInfo>(attr, m);

                            rowAttrListAndProperty.Add(newDictinory);
                        }
                    }
                }

                var dataRows = rowAttrListAndProperty.OrderBy(p => p.Key.ColumnIndex).ToList();

                //var trString = string.Format("<tr onclick =\"{0}();\">", rowChangeFunction);

                sb.Append("<tr>");

                foreach (var data in dataRows)
                {
                    sb.Append("<td>");
                    sb.Append(data.Value.GetValue(model, null));
                    sb.Append("</td>");
                }

                #region Adding Key Fields

                sb.Append("<td>");

                var index = 0;
                var actionsString = actions;

                foreach (var keyField in keyFields)
                {
                    var prop = model.GetType().GetProperty(keyField);

                    string propertyName = prop.Name,
                           propertyValue = prop.GetValue(model, null).ToString();

                    if (keyField != prop.Name) continue;

                    var hiddenKeyField = string.Format("<input type=\"hidden\" id=\"{0}\" value=\"{1}\" />",
                                                       propertyName, propertyValue);

                    sb.Append(hiddenKeyField);

                    if (!string.IsNullOrEmpty(actions))
                    {
                        actionsString = actionsString.Replace("{", "").Replace("}", "");

                        actionsString = actionsString.Replace(index.ToString(CultureInfo.InvariantCulture), propertyValue);
                    }

                    index++;
                }

                sb.Append(actionsString);

                sb.Append("</td>");

                #endregion

                sb.Append("</tr>");
            }

            sb.Append("</tbody>");
            sb.Append("</table>");

            var htmlString = new MvcHtmlString(sb.ToString());

            return htmlString;
        }
    }
}
