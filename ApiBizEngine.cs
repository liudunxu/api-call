using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Common.Engine.ApiRequestStategy;
using Common.Engine.ConfigTemplate;
using Common.Engine.Model;

namespace Common.Engine
{
    /// <summary>
    /// api引擎结果
    /// </summary>
    public class EngineResult
    {
        /// <summary>
        /// api返回结果集合
        /// </summary>
        public IDictionary<string, ApiResponseModel> ApiResultDict { get; set; }

        /// <summary>
        /// api逻辑结果
        /// </summary>
        public string Result { get; set; }
    }

    /// <summary>
    /// api请求逻辑引擎
    /// </summary>
    public class ApiBizEngine
    {
        /// <summary>
        /// api配置文件路径
        /// </summary>
        public static string ApiTempPath = HttpContext.Current.Server.MapPath("~/config/api/api.json");

        /// <summary>
        /// velocity配置路径
        /// </summary>
        public static string VelocityCorePath = HttpContext.Current.Server.MapPath("~/config/velocity/nvelocity.properties");
        public static string VelocityDirectPath = HttpContext.Current.Server.MapPath("~/config/velocity/directive.properties");

        /// <summary>
        /// api逻辑配置文件路径
        /// </summary>
        public static string ApiBizPath = HttpContext.Current.Server.MapPath("~/config/apiBiz");

        /// <summary>
        /// table模板配置文件路径
        /// </summary>
        public static string TableTempPath = HttpContext.Current.Server.MapPath("~/config/table");

        /// <summary>
        /// 执行逻辑
        /// </summary>
        /// <param name="bizId">api请求逻辑id，对应/config/apiBiz文件夹下的*.json文件</param>
        /// <param name="param">传入到模板的参数</param>
        /// <returns></returns>
        public static EngineResult ExecuteBiz(string bizId, Dictionary<string, object> param)
        {
            var result = new EngineResult();
            //获取api请求逻辑配置实体
            ApiBizModel bizModel = ApiBizTemplate.GetBizModelById(bizId);
            //获取api请求策略，目前支持同步和异步两种方式
            IApiRequestStrategy strategy = ApiRequestStrategyFactory.GetStrategy(!bizModel.IsParalla);
            //返回调用结果
            result.ApiResultDict = strategy.GetApiResponse(bizModel, param);
            result.Result = VelocityHelper.RenderString(bizModel.ResultTempl, result.ApiResultDict);
            return result;
        }

        /// <summary>
        /// 获取当前逻辑下所有api请求参数
        /// </summary>
        /// <param name="bizId"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetApiRequests(string bizId, Dictionary<string, object> param)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            //获取api请求逻辑配置实体
            ApiBizModel bizModel = ApiBizTemplate.GetBizModelById(bizId);
            foreach (var o in bizModel.RequestApiIds)
            {
                string requestParam = string.IsNullOrEmpty(o.NewTemplate) ? ApiTemplate.GetApiRequestParam(o.Id, param) : VelocityHelper.RenderString(o.NewTemplate, param);
                result.Add(o.Id, requestParam);
            }
            return result;
        }

        /// <summary>
        /// 获取url参数中键值为key的参数
        /// </summary>
        /// <param name="param"></param>
        /// <param name="key"> </param>
        /// <returns></returns>
        public static string GetRequestParam(string param, string key)
        {
            if (string.IsNullOrEmpty(param) || string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }
            string[] paramList = param.Split("&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in paramList)
            {
                if (string.IsNullOrEmpty(s)) continue;
                int equalIndex = s.IndexOf("=", StringComparison.Ordinal);
                if (equalIndex == -1 || equalIndex >= s.Length - 1) continue;
                string title = s.Substring(0, equalIndex);
                if (title == key)
                {
                    return s.Substring(equalIndex + 1);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取url参数中键值为key的参数
        /// </summary>
        /// <param name="param"></param>
        /// <param name="encodingKey"> </param>
        /// <param name="encoding"> </param>
        /// <returns></returns>
        public static string GetEncodingParam(string param, string encodingKey, Encoding encoding)
        {
            if (string.IsNullOrEmpty(param))
            {
                return string.Empty;
            }
            StringBuilder result = new StringBuilder();
            string[] paramList = param.Split("&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in paramList)
            {
                if (string.IsNullOrEmpty(s)) continue;
                int equalIndex = s.IndexOf("=", StringComparison.Ordinal);
                if (equalIndex == -1 || equalIndex >= s.Length - 1) continue;
                string title = s.Substring(0, equalIndex);
                result.Append(s.Substring(0, equalIndex) + "=");
                if (title == encodingKey)
                {
                    result.Append(HttpUtility.UrlEncode(s.Substring(equalIndex + 1), encoding));
                }
                else
                {
                    result.Append(s.Substring(equalIndex + 1));
                }
                result.Append("&");
            }
            return result.Length > 0 ? result.ToString(0,result.Length-1) : string.Empty;
        }

        /// <summary>
        /// 执行单个api的批量请求
        /// </summary>
        /// <param name="apiId">api请求id，对应/config/api文件夹下的api.json文件</param>
        /// <param name="param">请求参数</param>
        /// <returns></returns>
        public static EngineResult ExecuteSingleMulti(string apiId, Dictionary<string, Dictionary<string, object>> param)
        {
            EngineResult result = new EngineResult();
            //获取api详情
            ApiRequestModel model = ApiTemplate.GetApiModelById(apiId);
            ApiWebRequest requestManager = new ApiWebRequest();
            //流量控制工具
            Throttler throttler = new Throttler(model.PerSecNum);
            //批量执行调用
            foreach (KeyValuePair<string, Dictionary<string, object>> singleParam in param)
            {
                string requestParam = ApiTemplate.GetApiRequestParam(apiId, singleParam.Value);
                //如果配置了每秒的最大请求量，执行流量控制
                if (model.PerSecNum != 0)
                {
                    throttler.Throttle(1);
                }
                ApiResponseModel responseModel = requestManager.GetApiResult(model, requestParam);
                result.ApiResultDict.Add(singleParam.Key, responseModel);
            }
            return result;
        }
    }
}
