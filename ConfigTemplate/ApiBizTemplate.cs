using System;
using System.Collections.Generic;
using System.IO;
using Common.Engine.Model;
using log4net;

namespace Common.Engine.ConfigTemplate
{
    /// <summary>
    /// api逻辑模板工具
    /// </summary>
    public class ApiBizTemplate
    {
           /// <summary>
        /// 表格模板
        /// </summary>
        private static Dictionary<string, ApiBizModel> _apiBizDict;
        private static readonly ILog Log = LogManager.GetLogger("fileLogger");

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static ApiBizTemplate()
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
            _apiBizDict = new Dictionary<string, ApiBizModel>();
            string path = ApiBizEngine.ApiBizPath;
            DirectoryInfo folerInfo = new DirectoryInfo(path);
            foreach (FileInfo file in folerInfo.GetFiles())
            {
                if (file.Extension == ".json")
                {
                    var dict = ConfigHelper.GetJsonConfigByFilePath<Dictionary<string, ApiBizModel>>(file.FullName);
                    foreach (var apiBizModel in dict)
                    {
                        _apiBizDict.Add(apiBizModel.Key,apiBizModel.Value);
                    }
                }
            }
        }

        /// <summary>
        /// 获取api逻辑处理实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ApiBizModel GetBizModelById(string id)
        {
            ApiBizModel model;
            _apiBizDict.TryGetValue(id, out model);
            return model;
        }
    }
}
