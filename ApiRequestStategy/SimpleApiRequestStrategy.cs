using System.Collections.Generic;
using Common.Engine.ConfigTemplate;
using Common.Engine.Model;

namespace Common.Engine.ApiRequestStategy
{
    /// <summary>
    /// 单线程版api请求策略
    /// </summary>
    public class SimpleApiRequestStrategy:IApiRequestStrategy
    {
        /// <summary>
        /// 实现接口方法
        /// </summary>
        /// <param name="bizModel">api请求逻辑model</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public IDictionary<string, ApiResponseModel> GetApiResponse(ApiBizModel bizModel, Dictionary<string, object> param)
        {
            var paraMap = new Dictionary<string, ApiResponseModel>();
            var requestManager = new ApiWebRequest();

            foreach (var o in bizModel.RequestApiIds)
            {   
                //获取api请求模板
                ApiRequestModel model = ApiTemplate.GetApiModelById(o.Id);
                string requestParam = string.IsNullOrEmpty(o.NewTemplate) ? ApiTemplate.GetApiRequestParam(o.Id, param) : VelocityHelper.RenderString(o.NewTemplate, param);
                ApiResponseModel responseModel = requestManager.GetApiResult(model, requestParam);
                paraMap.Add(o.ResultName, responseModel);
                //添加原始参数，支持链式执行
                param.Add(o.ResultName,responseModel);
            }
            return paraMap;
        }
    }
}
