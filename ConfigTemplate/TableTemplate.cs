using System;
using System.Collections.Generic;
using System.IO;
using log4net;

namespace Common.Engine.ConfigTemplate
{
    /// <summary>
    /// 表格模板工具类
    /// </summary>
    public static class TableTemplate
    {
        /// <summary>
        /// 表格模板
        /// </summary>
        private static Dictionary<string, string> _templateDict;
        private static readonly ILog Log = LogManager.GetLogger("fileLogger");
        
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static TableTemplate()
        {
            try
            {
                ReloadTemplate();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        /// <summary>
        /// 重新加载模板配置文件
        /// </summary>
        private static void ReloadTemplate()
        {
            _templateDict = new Dictionary<string, string>();
            string path = ApiBizEngine.TableTempPath;
            DirectoryInfo folerInfo = new DirectoryInfo(path);
            //遍历config/table下的所有.vm文件，加入到内存中
            foreach (FileInfo file in folerInfo.GetFiles())
            {
                if (file.Extension == ".vm")
                {
                    StreamReader objReader = new StreamReader(file.FullName);
                    _templateDict.Add(file.Name.Substring(0, file.Name.IndexOf(".vm", StringComparison.Ordinal)), objReader.ReadToEnd().Replace("\r\n"," "));
                }
            }
        }

        /// <summary>
        /// 获得渲染后的表格
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string RenderTable(string tabName, IDictionary<string,object> obj)
        {
            string content;
            _templateDict.TryGetValue(tabName, out content);
            //如果没有内容，直接返回空字符串
            if (null == content)
            {
                return "";
            }
            try
            {
                //渲染模板
                return VelocityHelper.RenderString(content, obj);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return string.Empty;
            }
        }
    }
}