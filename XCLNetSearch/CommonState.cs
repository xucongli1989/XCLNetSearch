using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XCLNetSearch
{
    public class CommonState
    {
        #region 搜索控件枚举
        /// <summary>
        /// 左括号
        /// </summary>
        public enum LeftBracket
        {
            左括号 = 0
        }
        /// <summary>
        /// 获取实际左括号
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string GetLeftBracket(LeftBracket m)
        {
            string str = string.Empty;
            switch (m)
            {
                case LeftBracket.左括号:
                    str = "(";
                    break;
                default:
                    str = "(";
                    break;
            }
            return str;
        }
        /// <summary>
        /// 右括号
        /// </summary>
        public enum RightBracket
        {
            右括号 = 0
        }
        /// <summary>
        /// 获取实际右括号
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string GetRightBracket(RightBracket m)
        {
            string str = string.Empty;
            switch (m)
            {
                case RightBracket.右括号:
                    str = ")";
                    break;
                default:
                    str = ")";
                    break;
            }
            return str;
        }
        /// <summary>
        /// 逻辑
        /// </summary>
        public enum logic
        {
            并且 = 0,
            或者 = 1
        }
        /// <summary>
        /// 获取实际逻辑
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string GetLogic(logic m)
        {
            string str = string.Empty;
            switch (m)
            {
                case logic.并且:
                    str = "and";
                    break;
                case logic.或者:
                    str = "or";
                    break;
                default:
                    str = "and";
                    break;
            }
            return str;
        }
        /// <summary>
        /// 符号enum
        /// </summary>
        public enum Symbol
        {
            包含 = 0,
            不包含 = 10,
            等于 = 1,
            不等于 = 2,
            大于 = 3,
            大于等于 = 4,
            小于 = 5,
            小于等于 = 6,
            为空 = 7,
            不为空 = 8,
            以某某开始 = 9,
            不以某某开始 = 12,
            以某某结束 = 11,
            不以某某结束 = 13,
        }
        /// <summary>
        /// 根据符号枚举返回实际符号
        /// </summary>
        public static string GetSymbol(Symbol m)
        {
            string str = string.Empty;
            switch (m)
            {
                case Symbol.包含:
                case Symbol.以某某开始:
                case Symbol.以某某结束:
                    str = "like";
                    break;
                case Symbol.不包含:
                case Symbol.不以某某结束:
                case Symbol.不以某某开始:
                    str = "not like";
                    break;
                case Symbol.等于:
                    str = "=";
                    break;
                case Symbol.不等于:
                    str = "<>";
                    break;
                case Symbol.大于:
                    str = ">";
                    break;
                case Symbol.大于等于:
                    str = ">=";
                    break;
                case Symbol.小于:
                    str = "<";
                    break;
                case Symbol.小于等于:
                    str = "<=";
                    break;
                case Symbol.为空:
                    str = "isNull";
                    break;
                case Symbol.不为空:
                    str = "isNotNull";
                    break;
                default:
                    str = "=";
                    break;
            }
            return str;
        }
        #endregion
    }
}
