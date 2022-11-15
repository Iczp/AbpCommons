using IczpNet.AbpCommons.Extensions;
using Microsoft.International.Converters.PinYinConverter;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IczpNet.AbpCommons.PinYin
{
    /// <summary>
    /// 
    /// </summary>
    public static class PinYinConverterExtensions
    {

        /// <summary>
        /// 是否中文（汉语）
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsChinese(this char ch)
        {
            return Regex.IsMatch(ch.ToString(), @"[\u4e00-\u9fa5]");
        }
        /// <summary>
        /// 拼音
        /// </summary>
        /// <param name="vs"></param>
        /// <param name="joinSeperater"></param>
        /// <returns></returns>
        public static string ConvertToPinyin(this IEnumerable<IEnumerable<string>> vs, string joinSeperater = ",")
        {
            var sentences = vs.Select(x => string.Join("", x.ToArray()));
            return string.Join(joinSeperater, sentences.ToArray());
        }
        /// <summary>
        /// 拼音
        /// </summary>
        /// <param name="str"></param>
        /// <param name="joinSeperater"></param>
        /// <returns></returns>
        public static string ConvertToPinyin(this string str, string joinSeperater = ",")
        {
            return str.ToCartesian().ConvertToPinyin(joinSeperater);
        }
        /// <summary>
        /// 简写
        /// </summary>
        /// <param name="vs"></param>
        /// <returns></returns>
        public static string ConvertToPY(this IEnumerable<IEnumerable<string>> vs)
        {
            var abbreviations = vs.Select(x => string.Join("", x.Select(w => w.ToArray()[0].ToString()).ToArray()));
            return string.Join("", abbreviations.ToArray());
        }
        /// <summary>
        /// 简写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertToPY(this string str)
        {
            return str.ToCartesian().ConvertToPY();
        }
        /// <summary>
        /// 多音字的笛卡尔积集合
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static IEnumerable<IEnumerable<string>> ToCartesian(this string str)
        {
            var strList = new List<List<string>>();
            //str = "中国人";
            //strList = new List<List<string>>
            //{
            //    new List<string>(){"ZHONG"},
            //    new List<string>(){"GUO"},
            //    new List<string>(){"REN"}
            //};

            str.ToCharArray().ToList().ForEach(item =>
            {
                if (IsChinese(item))
                {
                    var chineseChar = new ChineseChar(item);

                    if (chineseChar.PinyinCount > 0)
                    {
                        var pinyinList = chineseChar.Pinyins
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Select(x => string.Concat(x.Substring(0, 1), x.Substring(1, x.Length - 2)))
                            .Distinct()
                            .ToList();
                        strList.Add(pinyinList);
                    }
                }
                else
                {
                    strList.Add(new List<string>() { item.ToString() });
                }
            });

            return strList.Cartesian();
        }
    }
}
