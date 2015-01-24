using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XCLNetSearch
{
    /// <summary>
    /// 字段类
    /// </summary>
    public class SearchFieldInfo
    {
        /// <summary>
        /// 搜索控件字段的构造函数
        /// </summary>
        /// <param name="Text">字段显示的名字</param>
        /// <param name="Value">字段对应的值的格式字符串，如：“A|B|C”【A：数据库中对应的真实字段名、B：此字段的类型，值为"dateTime、number、ntext、string"(ntext主要是为了去掉Like)】、C：输入区控件的类型:select为下拉框，text为文本框</param>
        /// <param name="FieldChangedHtml">字段发生change事件时，输入区的option（用于输入区为select的情况）</param>
        /// <param name="DateFmt">时间字段的格式（如:yyyy-MM-dd）</param>
        public SearchFieldInfo(string Text, string Value, string FieldChangedHtml, params string[] DateFmt)
        {
            this.Text = Text;
            this.Value = Value;
            this.FieldChangedHtml = FieldChangedHtml;
            if (null != DateFmt && DateFmt.Length > 0)
            {
                this.DateFmt = DateFmt[0];
            }
        }

        /// <summary>
        /// 字段显示的名字
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// 字段对应的值的格式字符串，如：“A|B|C”
        /// A：数据库中对应的真实字段名
        /// B：此字段的类型，值为"dateTime、number、ntext、string"(ntext主要是为了去掉Like)
        /// C：输入区控件的类型:select为下拉框，text为文本框
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// 字段发生change事件时，输入区的option（用于输入区为select的情况）
        /// </summary>
        public string FieldChangedHtml
        {
            get;
            set;
        }

        /// <summary>
        /// 字段为时间时，指定时间的格式
        /// </summary>
        public string DateFmt
        {
            get;
            set;
        }
    }
}
