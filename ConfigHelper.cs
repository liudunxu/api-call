using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Common.Engine
{
    /// <summary>
    /// 配置文件工具
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// 读取json格式配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T GetJsonConfigByFilePath<T>(string path)
        {
            using(var objReader = new StreamReader(path))
            {
                var content = GetEscapeString(objReader.ReadToEnd()).Replace("\r\n", " ").Replace("\t", "");
                objReader.Close();
                return JsonConvert.DeserializeObject<T>(content);
            }
        }

        /// <summary>
        /// json注释字符串去除
        /// 目前仅支持单行的//注释
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string GetEscapeString(string str)
        {
            const RegexOptions option = ((RegexOptions.Multiline) | RegexOptions.IgnoreCase|RegexOptions.Compiled);
            Regex regObj = new Regex(@"^\s*//.*$", option);
            string result = regObj.Replace(str, "");
            return result;
        }
    }
}
