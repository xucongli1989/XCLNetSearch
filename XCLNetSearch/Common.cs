using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Reflection;
using System.Diagnostics;

namespace XCLNetSearch
{
    /// <summary>
    /// 查询控件helper
    /// </summary>
    public class Common
    {
        #region 私有变量
        private static byte[] val = { 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x00, 0x01,
          0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
          0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F };
        #endregion

        /// <summary>
        /// 返回枚举options
        /// </summary>
        /// <returns></returns>
        internal static string GetEnumType(Type t)
        {
            StringBuilder str = new StringBuilder();
            List<EnumObj> lst = EnumObj.GetList(t);
            if (null != lst && lst.Count > 0)
            {
                foreach (EnumObj m in lst)
                {
                    str.AppendFormat("<option value=\"{0}\">{1}</option>", m.Value, m.Text);
                }
            }
            return str.ToString();
        }

        ///<summary>
        /// 返回查询控件的查询语句
        /// </summary>
        /// <param name="Search1">查询控件</param>
        /// <param name="param">Request.QueryString["where"]</param>
        /// <returns></returns>
        internal static string GetSearchStrByUrl(Search Search1, string param)
        {
            List<string> fields = new List<string>();
            foreach (SearchFieldInfo m in Search1.TypeList)
            {
                fields.Add((m.Value.Split('|'))[0]);
            }
            StringBuilder strMsg = new StringBuilder();
            StringBuilder str = new StringBuilder();
            if (!string.IsNullOrEmpty(param))
            {
                string[] where = unescape(param).Split(',');
                for (int i = 0; i < where.Length; i++)
                {
                    string current = string.Empty;
                    string wStr = Common.NoSqlAndHtml(where[i]);
                    if (wStr.Length > 0)
                    {
                        string[] s = wStr.Split('|');
                        if (s.Length == 7)
                        {
                            #region 一：拆分数据 s0:左括号 s1:字段 s2:字段类型 s3:符号 s4:输入区 s5:右括号 s6:逻辑
                            int s0 = Common.GetInt(s[0], -1);
                            string s1 = s[1];
                            string s2 = s[2];
                            int s3 = Common.GetInt(s[3], -1);
                            string s4 = Common.No_SqlHack(unescape(s[4]));
                            int s5 = Common.GetInt(s[5], -1);
                            int s6 = Common.GetInt(s[6], -1);
                            #endregion

                            #region 二：结上诉内容进行合法性检测开始
                            if (s0 != -1 && !EnumObj.IsExistEnumValue(s0, typeof(CommonState.LeftBracket)))
                            {
                                //strMsg.Append("左括号有误；");
                                continue;
                            }
                            if (s1.Length == 0 || s1 == "-1" || !fields.Contains(s1))
                            {
                                //strMsg.Append("查询字段有误；");
                                continue;
                            }
                            if (!EnumObj.IsExistEnumValue(s3, typeof(CommonState.Symbol)))
                            {
                                //strMsg.Append("符号有误；");
                                continue;
                            }
                            if (s4.Length == 0)
                            {
                                //此处不跳走是因为如果最后一条记录s4为空，则上一条的逻辑符还是存在的
                                s4 = "";
                            }
                            if (s5 != -1 && !EnumObj.IsExistEnumValue(s5, typeof(CommonState.RightBracket)))
                            {
                                //strMsg.Append("右括号有误；");
                                continue;
                            }
                            if (!EnumObj.IsExistEnumValue(s6, typeof(CommonState.logic)))
                            {
                                //strMsg.Append("逻辑符有误；");
                                continue;
                            }
                            #endregion 检测结束

                            #region 三：针对不同数据类型，对输入的数据进行处理
                            string strS4 = "";
                            switch (s2)
                            {
                                case "number":
                                    double s4Long = -1;
                                    double.TryParse(s4, out s4Long);
                                    strS4 = s4Long.ToString();
                                    break;
                                case "string":
                                    if (s3 == (int)CommonState.Symbol.包含 || s3 == (int)CommonState.Symbol.不包含)
                                    {
                                        strS4 = string.Format(" '%{0}%' ", s4);
                                    }
                                    else if (s3 == (int)CommonState.Symbol.以某某开始 || s3 == (int)CommonState.Symbol.不以某某开始)
                                    {
                                        strS4 = string.Format(" '{0}%' ", s4);
                                    }
                                    else if (s3 == (int)CommonState.Symbol.以某某结束 || s3 == (int)CommonState.Symbol.不以某某结束)
                                    {
                                        strS4 = string.Format(" '%{0}' ", s4);
                                    }
                                    else
                                    {
                                        strS4 = string.Format(" '{0}' ", s4);
                                    }
                                    break;
                                case "dateTime":
                                    s1 = GetTimeSqlInfoByFmt(s1, (SearchDateFmt)(3));
                                    strS4 = string.Format("'{0}'", Common.GetDateTime(s4, DateTime.Now.Date));
                                    break;
                                case "dateTime0":
                                    s1 = GetTimeSqlInfoByFmt(s1, (SearchDateFmt)(0));
                                    strS4 = string.Format("'{0}'", Common.GetDateTime(s4, DateTime.Now.Date));
                                    break;
                                case "dateTime1":
                                    s1 = GetTimeSqlInfoByFmt(s1, (SearchDateFmt)(1));
                                    strS4 = string.Format("'{0}'", Common.GetDateTime(s4, DateTime.Now.Date));
                                    break;
                                case "dateTime2":
                                    s1 = GetTimeSqlInfoByFmt(s1, (SearchDateFmt)(2));
                                    strS4 = string.Format("'{0}'", Common.GetDateTime(s4, DateTime.Now.Date));
                                    break;
                                case "dateTime3":
                                    s1 = GetTimeSqlInfoByFmt(s1, (SearchDateFmt)(3));
                                    strS4 = string.Format("'{0}'", Common.GetDateTime(s4, DateTime.Now.Date));
                                    break;
                                case "dateTime4":
                                    s1 = GetTimeSqlInfoByFmt(s1, (SearchDateFmt)(4));
                                    strS4 = string.Format("'{0}'", Common.GetDateTime(s4, DateTime.Now.Date));
                                    break;
                                case "dateTime5":
                                    s1 = GetTimeSqlInfoByFmt(s1, (SearchDateFmt)(5));
                                    strS4 = string.Format("'{0}'", Common.GetDateTime(s4, DateTime.Now.Date));
                                    break;
                                case "dateTime6":
                                    s1 = GetTimeSqlInfoByFmt(s1, (SearchDateFmt)(6));
                                    strS4 = string.Format("'{0}'", Common.GetDateTime(s4, DateTime.Now.Date));
                                    break;
                                case "dateTime7":
                                    s1 = GetTimeSqlInfoByFmt(s1, (SearchDateFmt)(7));
                                    strS4 = string.Format("'{0}'", Common.GetDateTime(s4, DateTime.Now.Date));
                                    break;
                                case "dateTime8":
                                    s1 = GetTimeSqlInfoByFmt(s1, (SearchDateFmt)(8));
                                    strS4 = string.Format("'{0}'", Common.GetDateTime(s4, DateTime.Now.Date));
                                    break;
                                case "dateTime9":
                                    s1 = GetTimeSqlInfoByFmt(s1, (SearchDateFmt)(9));
                                    strS4 = string.Format("'{0}'", Common.GetDateTime(s4, DateTime.Now.Date));
                                    break;
                                case "dateTime10":
                                    s1 = GetTimeSqlInfoByFmt(s1, (SearchDateFmt)(10));
                                    strS4 = string.Format("'{0}'", Common.GetDateTime(s4, DateTime.Now.Date));
                                    break;
                                default:
                                    strS4 = string.Format("'{0}'", s4);
                                    break;
                            }

                            switch (s3)
                            {
                                case (int)CommonState.Symbol.为空:
                                    s1 = string.Format("len(isnull(ltrim(rtrim({0})),''))", s1);
                                    s3 = (int)CommonState.Symbol.等于;
                                    strS4 = "0";
                                    break;
                                case (int)CommonState.Symbol.不为空:
                                    s1 = string.Format("len(isnull(ltrim(rtrim({0})),''))", s1);
                                    s3 = (int)CommonState.Symbol.大于;
                                    strS4 = "0";
                                    break;
                            }
                            #endregion

                            #region 四：正式拼接查询SQL
                            current = string.Format(" {0} {1} {2} {3} {4} {5} ",
                                s0 == -1 ? "" : CommonState.GetLeftBracket((CommonState.LeftBracket)s0),
                                s1,
                                CommonState.GetSymbol((CommonState.Symbol)s3),
                                strS4,
                                s5 == -1 ? "" : CommonState.GetRightBracket((CommonState.RightBracket)s5),
                                i == where.Length - 1 ? "" : CommonState.GetLogic((CommonState.logic)s6)//去掉最后一个"逻辑"
                                );
                            str.Append(current + " ");
                            #endregion
                        }
                    }
                }
            }

            #region  五：检查待运行的SQL条件的正反括号是否匹配
            Regex reg = new Regex(@"^[^\(\)]*(((?<o>\()[^\(\)]*)+[^\(\)]*((?<-o>\))[^\(\)]*)+)*(?(o)(?!))$");
            if (str.Length > 0 && !reg.IsMatch(str.ToString()))
            {
                strMsg.Append("括号匹配不正确！");
                str.Remove(0, str.Length);
            }
            if (strMsg.Length > 0)
            {
                Search1.strMsg = strMsg.ToString();
            }
            #endregion

            return str.ToString();
        }

        /// <summary>
        /// 查询的时间格式
        /// </summary>
        public enum SearchDateFmt
        {
            yyyy_MM_dd_HH_mm_ss = 0,
            yyyy_MM_dd_HH_mm = 1,
            yyyy_MM_dd_HH = 2,
            yyyy_MM_dd = 3,
            yyyy_MM = 4,
            yyyy = 5,
            MM = 6,
            dd = 7,
            HH = 8,
            mm = 9,
            ss = 10
        }

        /// <summary>
        /// 根据日期格式返回SQL时间语句
        /// </summary>
        /// <param name="timeField">字段名</param>
        /// <param name="fmt">时间格式</param>
        internal static string GetTimeSqlInfoByFmt(string timeField, SearchDateFmt fmt)
        {
            string str = "";
            switch (fmt)
            {
                case SearchDateFmt.yyyy_MM_dd_HH_mm_ss:
                    str = string.Format("CONVERT(DATETIME,CONVERT(varchar(100), {0}, 20))", timeField);
                    break;
                case SearchDateFmt.yyyy_MM_dd_HH_mm:
                    str = string.Format("CONVERT(DATETIME,CONVERT(varchar(16), {0}, 120)+':00')", timeField);
                    break;
                case SearchDateFmt.yyyy_MM_dd_HH:
                    str = string.Format("CONVERT(DATETIME,CONVERT(varchar(13), {0}, 120)+':00:00')", timeField);
                    break;
                case SearchDateFmt.yyyy_MM:
                    str = string.Format("CONVERT(DATETIME,CONVERT(varchar(7), {0}, 120)+'-01 00:00:00')", timeField);
                    break;
                case SearchDateFmt.yyyy:
                    str = string.Format("DATEPART(yyyy,{0})", timeField);
                    break;
                case SearchDateFmt.MM:
                    str = string.Format("DATEPART(MM,{0})", timeField);
                    break;
                case SearchDateFmt.dd:
                    str = string.Format("DATEPART(dd,{0})", timeField);
                    break;
                case SearchDateFmt.HH:
                    str = string.Format("DATEPART(HH,{0})", timeField);
                    break;
                case SearchDateFmt.mm:
                    str = string.Format("DATEPART(mm,{0})", timeField);
                    break;
                case SearchDateFmt.ss:
                    str = string.Format("DATEPART(ss,{0})", timeField);
                    break;
                default://yyyy-MM-dd
                    str = string.Format("DATEADD(day,DATEDIFF(day,0,{0}),0)", timeField);
                    break;
            }
            return str;
        }

        /// <summary>
        /// 对js的escape进行解码
        /// 解码 说明：本方法保证不论参数s是否经过escape()编码，均能得到正确的“解码”结果
        /// </summary>
        /// <param name="s">解码字符串</param>
        /// <returns></returns>
        internal static String unescape(String s)
        {
            StringBuilder sbuf = new StringBuilder();
            int i = 0;
            int len = s.Length;
            while (i < len)
            {
                int ch = s.ToCharArray()[i];
                if ('A' <= ch && ch <= 'Z')
                { // 'A'..'Z' : as it was
                    sbuf.Append((char)ch);
                }
                else if ('a' <= ch && ch <= 'z')
                { // 'a'..'z' : as it was
                    sbuf.Append((char)ch);
                }
                else if ('0' <= ch && ch <= '9')
                { // '0'..'9' : as it was
                    sbuf.Append((char)ch);
                }
                else if (ch == '-' || ch == '_' // unreserved : as it was
              || ch == '.' || ch == '!' || ch == '~' || ch == '*'
              || ch == '\'' || ch == '(' || ch == ')')
                {
                    sbuf.Append((char)ch);
                }
                else if (ch == '%')
                {
                    int cint = 0;
                    if ('u' != s.ToCharArray()[i + 1])
                    { // %XX : map to ascii(XX)
                        cint = (cint << 4) | val[s.ToCharArray()[i + 1]];
                        cint = (cint << 4) | val[s.ToCharArray()[i + 2]];
                        i += 2;
                    }
                    else
                    { // %uXXXX : map to unicode(XXXX)
                        cint = (cint << 4) | val[s.ToCharArray()[i + 2]];
                        cint = (cint << 4) | val[s.ToCharArray()[i + 3]];
                        cint = (cint << 4) | val[s.ToCharArray()[i + 4]];
                        cint = (cint << 4) | val[s.ToCharArray()[i + 5]];
                        i += 5;
                    }
                    sbuf.Append((char)cint);
                }
                else
                { // 对应的字符未经过编码
                    sbuf.Append((char)ch);
                }
                i++;
            }
            return sbuf.ToString();
        }

        /// <summary>
        /// 过滤HTML 和SQL 
        /// </summary>
        /// <param name="str"></param>
        internal static string NoSqlAndHtml(string str)
        {
            string s = string.Empty;
            if (!string.IsNullOrEmpty(str))
            {
                s = NoHTML(No_SqlHack(str));
            }
            return s;
        }

        /// <summary>
        /// 去除HTML标记    
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        internal static string NoHTML(string Htmlstring)
        {
            if (string.IsNullOrEmpty(Htmlstring))
            {
                return "";
            }
            //删除脚本    
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML    
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("\r\n", "");
            Htmlstring.Replace("'", "");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;
            //if (!string.IsNullOrEmpty(Htmlstring))
            //{
            //    Htmlstring = Microsoft.Security.Application.Encoder.HtmlEncode(Htmlstring);
            //}
            //return Htmlstring;
        }

        /// <summary>
        /// 防止SQL注入
        /// </summary>
        /// <param name="inputStr">輸入的sql語句</param>
        /// <returns>過濾後的語句</returns>
        internal static string No_SqlHack(string inputStr)
        {
            //要過濾掉的關鍵字集合
            string NoSqlHack_AllStr = "|;|and|chr(|exec|insert|select|delete|from|update|mid(|master.|";
            string SqlHackGet = inputStr;
            string[] AllStr = NoSqlHack_AllStr.Split('|');

            //分離關鍵字
            string[] GetStr = SqlHackGet.Split(' ');
            if (SqlHackGet != "")
            {
                for (int j = 0; j < GetStr.Length; j++)
                {
                    for (int i = 0; i < AllStr.Length; i++)
                    {
                        if (GetStr[j].ToLower() == AllStr[i].ToLower())
                        {
                            GetStr[j] = "";
                            break;
                        }
                    }
                }
                SqlHackGet = "";
                for (int i = 0; i < GetStr.Length; i++)
                {
                    SqlHackGet += GetStr[i].ToString() + " ";
                }
                return SqlHackGet.TrimEnd(' ').Replace("'", "");//.Replace(",", "");//.Replace("<", "&lt").Replace(">", "&gt");
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 将字符串转化为整数若值不是数字返回defaultValue
        /// </summary>
        /// <returns></returns>
        internal static int GetInt(string key, int defaultValue)
        {
            int result = 0;
            bool b = Int32.TryParse(key, out result);
            return b ? result : defaultValue;
        }

        /// <summary>
        ///  将字符串转化为浮点数 若值不是浮点数返回defaultValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        internal static decimal GetDecimal(string key, decimal defaultValue)
        {
            decimal i;
            bool b = Decimal.TryParse(key, out i);
            return b ? i : defaultValue;
        }

        /// <summary>
        /// 转换时间 转换失败则为默认值 
        /// </summary>
        /// <returns></returns>
        internal static DateTime GetDateTime(string key, DateTime defaultValue)
        {
            DateTime dt;
            bool b = DateTime.TryParse(key, out dt);
            return b ? dt : defaultValue;
        }

        /// <summary>
        /// 根据guid生成字符串(16位)
        /// 例如：aded0a2611f8aa4a
        /// </summary>
        internal static string GenerateStringId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        /// <summary>
        /// 程序集内部资源访问
        /// </summary>
        internal static string GetWebResourceUrl(Type type, string resourceId)
        {
            string resourceUrl = null;
            List<MemberInfo> methodCandidates = typeof(System.Web.Handlers.AssemblyResourceLoader).GetMember("GetWebResourceUrlInternal", BindingFlags.NonPublic | BindingFlags.Static).ToList();
            foreach (var methodCandidate in methodCandidates)
            {
                var method = methodCandidate as MethodInfo;
                if (method == null || method.GetParameters().Length != 5) continue;

                resourceUrl = string.Format("{0}", method.Invoke
                (
                    null,
                    new object[] { Assembly.GetAssembly(type), resourceId, false, false, null })
                );
                break;
            }
            return resourceUrl;
        }
    }
}
