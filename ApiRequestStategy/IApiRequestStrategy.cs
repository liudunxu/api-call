using System.Collections.Generic;
using Common.Engine.Model;

namespace Common.Engine.ApiRequestStategy
{
    /// <summary>
    /// api请求策略
    /// </summary>
    public interface IApiRequestStrategy
    {
        /// <summary>
        /// 根据规则获取api返回信息
        /// </summary>
        /// <param name="model">api请求逻辑model</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        IDictionary<string, ApiResponseModel> GetApiResponse(ApiBizModel model, Dictionary<string, object> param);
    }
}
