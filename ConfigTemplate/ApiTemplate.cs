using System;
using System.Collections.Generic;
using Common.Engine.Model;
using log4net;

namespace Common.Engine.ConfigTemplate
{
    /// <summary>
    /// api请求参数模板引擎
    /// </summary>
    public class ApiTemplate
    {
        private static readonly Dictionary<string, ApiRequestModel> APITempleDict;
        private static readonly ILog Log = LogManager.GetLogger("fileLogger");
        
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static ApiTemplate()
        {
            try
            {
                APITempleDict = ConfigHelper.GetJsonConfigByFilePath<Dictionary<string, ApiRequestModel>>(ApiBizEngine.ApiTempPath);
            }catch(Exception e)
            {
                Log.Error(e);   
            }
        }

        /// <summary>
        /// 根据api的id获取api相关信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ApiRequestModel GetApiModelById(string id)
        {
            ApiRequestModel model;
            APITempleDict.TryGetValue(id, out model);
            return model;
        }

        /// <summary>
        /// 根据id获取api请求参数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetApiRequestParam(string id,IDictionary<string,object> obj)
        {
            return VelocityHelper.RenderString(GetApiModelById(id).Template, obj);
        }
    }
}
