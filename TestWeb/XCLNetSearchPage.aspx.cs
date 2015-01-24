using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCLNetSearch;

namespace TestWeb
{
    /// <summary>
    /// XCLNetSearch Demo
    /// </summary>
    public partial class XCLNetSearchPage : System.Web.UI.Page
    {
        protected XCLNetSearch.Search search{get;set;}

        protected void Page_Load(object sender, EventArgs e)
        {
            this.InitSearch();
        }
        protected void InitSearch()
        {
            List<SearchFieldInfo> lstItem = new List<SearchFieldInfo>() { 
                new SearchFieldInfo("系统ID","id|number|text",""),
                new SearchFieldInfo("类型","type|string|text",""),
                new SearchFieldInfo("地区","area|string|select","<option value='杭州'>杭州</option><option value='武汉'>武汉</option>"),
                new SearchFieldInfo("开始时间",string.Format("StartTime|dateTime{0}|text",(int)XCLNetSearch.Common.SearchDateFmt.yyyy_MM),"","yyyy-MM"),
                new SearchFieldInfo("结束时间","EndTime|dateTime|text","")
            };
            this.search = new Search();
            this.search.TypeList= lstItem;
            if (!string.IsNullOrEmpty(this.search.StrSQL))
            {
                this.lbSql.Text = this.search.StrSQL;
            }
        }
    }
}