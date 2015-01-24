using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;

namespace XCLNetSearch
{
    /// <summary>
    /// 查询控件主体(此控件基于JS)
    /// 需要引用外部JS：dynamicCon.js、jquery-1.5.2.min.js、My97DatePicker
    /// 原理：将条件拼接为SQL查询中的WHERE后面的条件语句。使用form提交表单
    /// by:xcl @2012.8  qq:80213876  http://blog.csdn.net/luoyeyu1989 （如需修改此控件，请保留此行信息即可，谢谢）
    /// </summary>
    public class Search
    {
        private static string getGuid = string.Format("_{0}", Common.GenerateStringId());//作为JS的全局变量名
        private string _strParamName = "where";
        private string selOptions = "";
        private string selInputOptionsEventJs = "";//字段区change时执行响应输入区option的语句
        private List<SearchFieldInfo> _typeList = null;
        private int _maxLine = 10;
        private bool _isOnLoadShow = true;
        private bool _isCompressCode = true;

        /// <summary>
        /// 说明信息
        /// </summary>
        private static string GetRemark = new Func<string>(() =>
        {
            StringBuilder str = new StringBuilder();
            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
            str.AppendFormat("<!--*****************{0}  {1}，{2} *****************-->", fvi.FileDescription, fvi.FileVersion, fvi.LegalCopyright);
            return str.ToString();
        }).Invoke();

        /// <summary>
        /// guid作为容器ID
        /// </summary>
        public string GetGuid
        {
            get
            {
                return getGuid;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected void InitData()
        {
            #region 生成字段select的option
            StringBuilder strJs = new StringBuilder();
            StringBuilder str = new StringBuilder("<option value=\"\" type=\"string\" inputType=\"text\">--重置此条件--</option>");
            if (null != this.TypeList && this.TypeList.Count > 0)
            {
                string[] strFieldType = { };
                foreach (SearchFieldInfo m in this.TypeList)
                {
                    strFieldType = m.Value.Replace(" ", "").Split('|');
                    if (strFieldType.Length == 3)
                    {
                        str.AppendFormat("<option value=\"{0}\" type=\"{1}\" inputType=\"{2}\" dateFmt=\"{4}\">{3}</option>", strFieldType[0], strFieldType[1], strFieldType[2], m.Text, string.IsNullOrEmpty(m.DateFmt) ? "yyyy-MM-dd" : m.DateFmt);
                        if (!string.IsNullOrEmpty(m.FieldChangedHtml))
                        {
                            strJs.AppendFormat(@"case ""{0}"":selHtml=""{1}"";break;", strFieldType[0], m.FieldChangedHtml.Replace("\"", "'"));
                        }
                    }
                }
                this.selOptions = str.ToString();
                this.selInputOptionsEventJs = strJs.ToString();
            }
            #endregion

            this.StrSQL = Common.GetSearchStrByUrl(this, this.StrParamValue);
        }

        /// <summary>
        /// 页面加载时，查询框是否可见
        /// </summary>
        public bool IsOnLoadShow
        {
            get { return this._isOnLoadShow; }
            set { this._isOnLoadShow = value; }
        }

        /// <summary>
        /// 最多几行条件
        /// </summary>
        public int MaxLine
        {
            get { return this._maxLine; }
            set { this._maxLine = value; }
        }

        /// <summary>
        /// 此控件生成的查询后的参数名，也是隐藏hidden的name
        /// </summary>
        public string StrParamName
        {
            get { return this._strParamName; }
            set { this._strParamName = value; }
        }

        /// <summary>
        /// 此控件查询参数的值，也是隐藏hidden的value
        /// </summary>
        public string StrParamValue
        {
            get
            {
                if (null != HttpContext.Current.Request.QueryString[this.StrParamName])
                {
                    return HttpContext.Current.Request.QueryString[this.StrParamName] ?? "";
                }
                else
                {
                    return HttpContext.Current.Request.Form[this.StrParamName] ?? "";
                }
            }
        }

        /// <summary>
        /// 要查询的字段list（给此字段赋值时即进行此控件数据的初始化）
        /// </summary>
        public List<SearchFieldInfo> TypeList
        {
            get { return this._typeList; }
            set
            {
                this._typeList = value;
                this.InitData();
            }
        }

        /// <summary>
        /// 错误信息提示
        /// </summary>
        public string strMsg
        {
            get;
            set;
        }

        /// <summary>
        /// 最终拼接的SQL条件语句
        /// </summary>
        public string StrSQL
        {
            get;
            set;
        }

        /// <summary>
        /// 是否压缩生成的脚本代码
        /// </summary>
        public bool IsCompressCode
        {
            get
            {
                return this._isCompressCode;
            }
            set
            {
                this._isCompressCode = value;
            }
        }

        #region  所有控件的NAME属性
        /// <summary>
        /// 左括号
        /// </summary>
        public string LeftBracketName
        {
            get
            {
                return string.Format("{0}_leftBracket", this.GetGuid);
            }
        }

        /// <summary>
        /// 搜索的字段
        /// </summary>
        public string SelSearchTypeName
        {
            get
            {
                return string.Format("{0}_selSearchType", this.GetGuid);
            }
        }

        /// <summary>
        /// 比较符号
        /// </summary>
        public string SymbolName
        {
            get
            {
                return string.Format("{0}_symbol", this.GetGuid);
            }
        }

        /// <summary>
        /// 输入区
        /// </summary>
        public string TxtSearchValueName
        {
            get
            {
                return string.Format("{0}_txtSearchValue", this.GetGuid);
            }
        }

        /// <summary>
        /// 右括号
        /// </summary>
        public string RightBracketName
        {
            get
            {
                return string.Format("{0}_rightBracket", this.GetGuid);
            }
        }

        /// <summary>
        /// 逻辑符
        /// </summary>
        public string LogicName
        {
            get
            {
                return string.Format("{0}_logic", this.GetGuid);
            }
        }

        #endregion


        public string XCLNetSearchHTML()
        {
            StringBuilder str = new StringBuilder();
            str.AppendFormat(@"<table  width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"" class=""{0}"">
            <tr>
                <td align=""center"">
                    <a href=""javascript:void(0);"" class=""openImg""><img src=""{21}"" border=""0"" alt=""展开搜索""/></a>
                    <input type=""hidden"" IsSubmit=""1"" name=""{14}"" value=""{15}""/>
                </td>
            </tr>
            <tr>
                <td align=""center"">
                    <div  class=""divSearch"">
                        <table border=""0"" cellspacing=""0"" cellpadding=""0"">
                            <tr>
                                <td>
                                        <table border=""0"" cellspacing=""0"" cellpadding=""0"">
                                                <tr class=""items"">
                                                <td align=""center"" style=""padding-top:3px;padding-bottom:3px"">
                                                        <select name=""{1}"">
                                                            <option value=""-1""></option>
                                                            {2}
                                                        </select>
                                                        —
                                                        <select name=""{3}"">
                                                            {4}
                                                        </select>
                                                        —
                                                        <select name=""{5}"" style=""width:100px;"">
                                                            {6}
                                                        </select>
                                                        —
                                                        <input type=""text"" name=""{7}"" style=""width:120px;""/>
                                                        —
                                                        <select name=""{8}"">
                                                            <option value=""-1""></option>
                                                            {9}
                                                        </select>
                                                        —
                                                        <select name=""{10}"">
                                                            {11}
                                                        </select>
                                                </td>
                                                <td align=""left"">
                                                    <a href=""javascript:void(0);"" class=""addBtn"" title=""增加搜索条件""><img src=""{17}"" border=""0"" /></a>
                                                    <a href=""javascript:void(0);""  class=""delBtn""  title=""删除搜索条件""><img src=""{18}"" border=""0"" /></a>
                                                </td>
                                            </tr>
                                                <tr class=""temp"">
                                                <td align=""center"" style=""padding-top:3px;padding-bottom:3px"">
                                                        <select name=""{1}"">
                                                            <option value=""-1""></option>
                                                            {2}
                                                        </select>
                                                        —
                                                        <select name=""{3}"">
                                                            {4}
                                                        </select>
                                                        —
                                                        <select name=""{5}"" style=""width:100px;"">
                                                            {6}
                                                        </select>
                                                        —
                                                        <input type=""text"" name=""{7}"" style=""width:120px;""/>
                                                        —
                                                        <select name=""{8}"">
                                                            <option value=""-1""></option>
                                                            {9}
                                                        </select>
                                                        —
                                                        <select name=""{10}"">
                                                            {11}
                                                        </select>
                                                </td>
                                                <td align=""left"">
                                                    <a href=""javascript:void(0);"" class=""addBtn"" title=""增加搜索条件""><img src=""{17}"" border=""0"" /></a>
                                                    <a href=""javascript:void(0);""  class=""delBtn""  title=""删除搜索条件""><img src=""{18}"" border=""0"" /></a>
                                                </td>
                                            </tr>
                                    </table>
                                </td>
                                <td align=""left"" style=""padding-left:10px;"">
                                    <a href=""javascript:void(0);"" class=""btnSearch""><img src=""{19}"" border=""0"" /></a>
                                    {23}
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
        <script src=""{22}"" type=""text/javascript""></script>
        <script type=""text/javascript"">
            var {0}={{
                /*字段change时，若输入区为select,则响应*/
                GetInputOption:function(obj){{
                    var $inputObj = $(obj).closest(""tr"").find(""[name='{7}']""); /*要输入的地方*/
                    var selHtml = """";
                    switch (obj.value) {{
                        {25}
                    }};
                    if (selHtml != """") {{
                        $inputObj.html(selHtml);

                        /*ie6 bug*/
                        var ie6=!-[1,]&&!window.XMLHttpRequest;
                        if(ie6){{
                            var len=$inputObj.find(""option"").length;
                            $inputObj[0].options.length=len+1;
                            $inputObj.find(""option"").last().remove();
                        }}
                    }}
                }},
                /*字段Change事件*/
                FieldChange:function(obj){{
                                var dateFmt=$.trim($(obj).find(""option:selected"").attr(""dateFmt"")); /*日期字段的格式*/
                                var typeValue = $.trim($(obj).find(""option:selected"").attr(""type"")); /*字段的类型*/
                                var inputType=$.trim($(obj).find(""option:selected"").attr(""inputType"")); /*要输入的控件类型（select 、input。。。）*/

                                var $txtInput = $(obj).closest(""tr"").find(""[name='{7}']"");/*手动输入区（更新控件类型前）*/

                                /*设置手动输入区的控件类型*/
                                switch(inputType)
                                {{
                                    case ""text"":
                                    $txtInput.after(""<input type=\""text\"" name=\""{7}\"" style=\""width:120px;\""/>"");
                                    $txtInput.remove();
                                    break;
                                    case ""select"":
                                    $txtInput.after(""<select name=\""{7}\"" style=\""width:120px;\""/></select>"");
                                    $txtInput.remove();
                                    break;
                                }}
                                $txtInput = $(obj).closest(""tr"").find(""[name='{7}']"");/*手动输入区（更新控件类型后）*/
                        
                                $txtInput.removeAttr(""readonly"").unbind(""click"");

                                var $symbolObj=$(obj).closest(""tr"").find(""[name='{5}']"");/*符号*/
                                $symbolObj.html('{6}');


                                if(typeValue.indexOf(""dateTime"")>=0){{
                                        $symbolObj.find(""option[value='{13}'],option[value='{30}'],option[value='{31}'],option[value='{32}'],option[value='{33}'],option[value='{34}']"").remove();
                                        $txtInput.bind(""click"",function(){{
                                            WdatePicker({{dateFmt:dateFmt,readOnly:true}});
                                        }});
                                }}
                                switch (typeValue) {{
                                    case ""ntext"":/*ntext只能使用is null和like，不能使用比较符*/
                                        $symbolObj.find(""option"").not(""option[value='{13}'],option[value='{30}'],option[value='{27}'],option[value='{28}'],option[value='{31}'],option[value='{32}'],option[value='{33}'],option[value='{34}']"").remove();
                                        break;
                                   case ""number"":/*数字把like去掉(不去掉sql也不报错)*/
                                        $symbolObj.find(""option[value='{13}'],option[value='{30}'],option[value='{31}'],option[value='{32}'],option[value='{33}'],option[value='{34}']"").remove();
                                        break;
                                }}

                                $symbolObj.bind(""change"",function(){{
                                    {0}.SymbolObjChange(this);/*调用符号选择下拉事件*/            
                                }})

                }},
                /**符号选择框change事件**/
                SymbolObjChange:function(obj){{
                        var $selectFieldObj=$(obj).closest(""tr"").find(""select[name='{3}']"");
                        var $inputObj = $(obj).closest(""tr"").find(""[name='{7}']"");
                        var inputObjType=$inputObj[0].type;                  

                        /*【为空】和【不为空】时处理*/
                        if($(obj).val()==""{27}""||$(obj).val()==""{28}""){{
                            $inputObj.prop({{""disabled"":true}});

                            switch(inputObjType){{
                                case ""text"":
                                    $inputObj.val(""."");
                                break;
                                case ""select-one"":
                                    $inputObj.html('<option value=""."">.</option>');
                                break;
                            }}

                        }}else{{

                            if($inputObj.prop(""disabled"")){{
                                $inputObj.removeAttr(""disabled"");
                                switch(inputObjType){{
                                    case ""text"":
                                        $inputObj.val("""");
                                    break;
                                    case ""select-one"":
                                        {0}.GetInputOption($selectFieldObj[0]);
                                    break;
                                }} 
                            }}
                        }}
                }},
                Init:function(){{
                        var $con = $(""table.{0}"");
                        var $searchCon = $con.find("".divSearch"");
                        var $imgSwitch = $con.find("".openImg"");
                        $searchCon.hide();
                        $(document).on(""click"",$imgSwitch.selector,function () {{
                            $searchCon.slideToggle(""fast"", function () {{
                                if ($(this).css(""display"") == ""none"") {{
                                    $imgSwitch.find(""img"").attr({{ ""src"": ""{20}"",""alt"":""展开搜索"" }});
                                }}
                                else {{
                                    $imgSwitch.find(""img"").attr({{ ""src"": ""{21}"",""alt"":""隐藏搜索"" }});
                                }}
                            }});
                        }});
                        /*动态增删行*/
                        $.DynamicCon({{ container: "".{0}"", items: "".items"", maxCount: {26} }});
                        /*通用搜索中字段下拉框*/
                        var $fieldSel =$con.find(""select[name='{3}']""); /*要搜索的字段下拉框*/

                        $(document).on(""change"",$fieldSel.selector, function () {{
                            {0}.FieldChange(this);
                        }});

                        /*回车提交查询*/
                        $(document).on(""keypress"",$con.find(""[name='{7}']"").selector,function(event){{
                            if(event.keyCode==""13"")
                            {{
                                $con.find("".btnSearch"").click();
                                return false;
                            }}
                        }});

                        var $btnSearchObj=$con.find("".btnSearch"");
                        $btnSearchObj.hover(function(){{$(this).find(""img"").attr({{""src"":""{24}""}});}},function(){{$(this).find(""img"").attr({{""src"":""{19}""}});}});
                        /*搜索：绑定提交事件*/
                        $(document).on(""click"",$btnSearchObj.selector,function(){{
                            $con.closest(""form"").submit();
                        }});
                        $con.closest(""form"").submit( function () {{
                            /*1：拼接所有搜索框中的条件为URL*/
                            var param=[];
                            var leftBracket,selectValue,selectDataType,symbol,inputValue,rightBracket,logic;/*左括号、字段、字段数据类型、运算符、输入区、右括号、逻辑符*/
                            $con.find(""tr.items"").each(function(){{
                                var $selectField=$(this).find(""select[name='{3}']"");/*当前的下拉字段*/

                                inputValue=escape($.trim($(this).find(""[name='{7}']"").val()));/*输入区*/

                                selectDataType=$selectField.find(""option:selected"").attr(""type"");/*字段数据类型*/
                                selectValue=$selectField.val();/*当前下拉字段的值（字段）*/
                                if(selectValue==""""||selectValue==""-1""||inputValue=="""")
                                {{
                                    return true;
                                }}
                        
                                leftBracket=$(this).find(""[name='{1}']"").val();/*左括号*/
                                symbol=$(this).find(""[name='{5}']"").val();/*运算符*/
                        
                                rightBracket=$(this).find(""[name='{8}']"").val();/*右括号*/
                                logic=$(this).find(""[name='{10}']"").val();/*逻辑符*/
                        
                                param.push(leftBracket+""|""+selectValue+""|""+selectDataType+""|""+symbol+""|""+escape(inputValue.replace(/[|]/,'').replace(/[,]/g,''))+""|""+rightBracket+""|""+logic);
                            }});
                    
                            /*2：将生成值放入隐藏域以便Get表单*/
                            $con.find("":hidden[name='{14}']"").val(param.toString());
                            $con.find(""input,select"").not(""[IsSubmit='1']"").prop({{""disabled"":true}});/*排除无关信息随表单提交*/                          
                        }});
                        /*页面加载时对搜索框的初始化*/
                        var currentUrl= {{""{14}"":""{15}""}};/*json*/
                        if(currentUrl[""{14}""]!=undefined&&currentUrl[""{14}""]!="""")
                        {{
                                $con.find("".openImg"").click();
                                var strWhere=unescape(currentUrl[""{14}""]);
                                var wp=strWhere.split(',');
                                if(wp.length>1)/*刚开始本来有一行*/
                                {{
                                    for(var m=0;m<wp.length-1;m++)
                                    {{
                                        $con.find("".addBtn:eq(0)"").click();
                                    }}
                                }}
                        
                                /*给搜索框赋默认值*/
                                var $trs=$con.find(""tr.items"");
                                if(wp.length==$trs.length)
                                {{
                                    var values=[];
                                    $trs.each(function(i){{
                                        var $leftBracketObj=$(this).find(""[name='{1}']"");/*左括号*/
                                        var $fieldObj=$(this).find(""[name='{3}']"");/*字段*/
                                        /*where中有个字段类型（在字段对象的option中的type属性中）*/
                                        var $symbolObj=$(this).find(""[name='{5}']"");/*运算符*/
                                        var $inputValueObj=$(this).find(""[name='{7}']"");/*输入区(旧)*/
                                        var $rightBracketObj=$(this).find(""[name='{8}']"");/*右括号*/
                                       var  $logicObj=$(this).find(""[name='{10}']"");/*逻辑符*/
                                
                                        values=wp[i].split('|');/*具体值*/
                                        if(values.length==7)/*上面共有6个设置区(左括号、字段...)*/
                                        {{
                                            XCLNetSearch_CommonJs.SelectedObj($leftBracketObj[0],values[0]);
                                            XCLNetSearch_CommonJs.SelectedObj($fieldObj[0],values[1]);
                                            {0}.FieldChange($fieldObj[0]);/*调用字段下拉事件*/

                                            {16}
                                    
                                            /*values[2]为字段类型*/
                                            XCLNetSearch_CommonJs.SelectedObj($symbolObj[0],values[3]);

                                           var  $inputValueObj=$(this).find(""[name='{7}']"");/*输入区(新)上面调用了FieldChange事件后，此对象又重新生成了。*/
                                            switch($fieldObj.find(""option:selected"").attr(""inputType""))
                                            {{
                                                case ""text"":
                                                $inputValueObj.val(unescape(unescape(values[4])));
                                                break;
                                                case ""select"":
                                                XCLNetSearch_CommonJs.SelectedObj($inputValueObj[0],unescape(unescape(values[4])));
                                                break;
                                            }}

                                            XCLNetSearch_CommonJs.SelectedObj($rightBracketObj[0],values[5]);
                                            XCLNetSearch_CommonJs.SelectedObj($logicObj[0],values[6]);
                                            {0}.SymbolObjChange($symbolObj[0]);/*调用符号选择下拉事件*/
                                        }}
                                    }});
                                }}
                        }};
                }}
            }};
            $(function(){{
                {0}.Init();

                $(document).on(""change"",$("".{0}"").find(""select[name='{3}']"").selector, function () {{
                    {12}
                }});

                if({29}){{
                        $(""table.{0}"").find("".divSearch"").show();
                }}
            }});
        </script>",
                /*0*/this.GetGuid,
                /*1*/this.LeftBracketName,
                /*2*/Common.GetEnumType(typeof(CommonState.LeftBracket)),
                /*3*/this.SelSearchTypeName,
                /*4*/this.selOptions,
                /*5*/this.SymbolName,
                /*6*/Common.GetEnumType(typeof(CommonState.Symbol)),
                /*7*/this.TxtSearchValueName,
                /*8*/this.RightBracketName,
                /*9*/Common.GetEnumType(typeof(CommonState.RightBracket)),
                /*10*/this.LogicName,
                /*11*/Common.GetEnumType(typeof(CommonState.logic)),
                /*12*/string.Format("{0}.GetInputOption(this);", this.GetGuid),
                /*13*/(int)CommonState.Symbol.包含,
                /*14*/this.StrParamName,
                /*15*/this.StrParamValue,
                /*16*/string.Format("{0}.GetInputOption($fieldObj[0]);", this.GetGuid),
                /*17*/Common.GetWebResourceUrl(this.GetType(), "XCLNetSearch.Style.Images.add_search.gif"),
                /*18*/Common.GetWebResourceUrl(this.GetType(), "XCLNetSearch.Style.Images.del_search.gif"),
                /*19*/Common.GetWebResourceUrl(this.GetType(), "XCLNetSearch.Style.Images.search.gif"),
                /*20*/Common.GetWebResourceUrl(this.GetType(), "XCLNetSearch.Style.Images.up.gif"),
                /*21*/Common.GetWebResourceUrl(this.GetType(), "XCLNetSearch.Style.Images.down.gif"),
                /*22*/Common.GetWebResourceUrl(this.GetType(), "XCLNetSearch.Style.Js.CommonJs.js"),
                /*23*/string.IsNullOrEmpty(this.strMsg) ? "" : string.Format(@"<br/><span style=""color:#f00;font-size:12px;"">{0}</span>", this.strMsg),
                /*24*/Common.GetWebResourceUrl(this.GetType(), "XCLNetSearch.Style.Images.search2.gif"),
                /*25*/this.selInputOptionsEventJs,
                /*26*/this.MaxLine,
                /*27*/(int)CommonState.Symbol.为空,
                /*28*/(int)CommonState.Symbol.不为空,
                /*29*/Convert.ToString(this.IsOnLoadShow).ToLower(),
                /*30*/(int)CommonState.Symbol.不包含,
                /*31*/(int)CommonState.Symbol.以某某开始,
                /*32*/(int)CommonState.Symbol.以某某结束,
                /*33*/(int)CommonState.Symbol.不以某某开始,
                /*34*/(int)CommonState.Symbol.不以某某结束
                  );

            string html = string.Empty;
            if (this.IsCompressCode)
            {
                html = string.Format("{0}{1}{0}", GetRemark, Regex.Replace(str.ToString(), @"(\s+)|(/\*.*\*/)", " "));
            }
            else
            {
                html = string.Format("{0}{1}{0}", GetRemark, str.ToString());
            }
            return html;
        }


    }
}