using System.Collections.Concurrent;
using System.Collections.Generic;
using Amib.Threading;
using Common.Engine.ConfigTemplate;
using Common.Engine.Model;

namespace Common.Engine.ApiRequestStategy
{
    /// <summary>
    /// 多线程版api请求策略
    /// </summary>
    public class CoApiRequestStrategy : IApiRequestStrategy
    {
        /// <summary>
        /// 实现接口方法
        /// </summary>
        /// <param name="bizModel"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IDictionary<string, ApiResponseModel> GetApiResponse(ApiBizModel bizModel, Dictionary<string, object> param)
        {
            ConcurrentDictionary<string, ApiResponseModel> result = new ConcurrentDictionary<string, ApiResponseModel>();
            // 创建一个线程池
            SmartThreadPool smartThreadPool = new SmartThreadPool(1800000, 5);
            IWorkItemResult[] wirs = new IWorkItemResult[bizModel.RequestApiIds.Count];
            int curWirIndex = 0;
            foreach (var o in bizModel.RequestApiIds)
            {
                wirs[curWirIndex++] =smartThreadPool.QueueWorkItem(SingleApiRequest,o,param,result);
            }
            //等待所有线程执行完毕
            SmartThreadPool.WaitAll(wirs);
            //关闭线程池
            smartThreadPool.Shutdown();
            return result;
        }

        /// <summary>
        /// 每个线程实现的功能
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="param"> </param>
        /// <param name="result"> </param>
        public void SingleApiRequest(RequestApiProp prop, Dictionary<string, object> param,ConcurrentDictionary<string, ApiResponseModel> result)
        {
            var requestManager = new ApiWebRequest();
            ApiRequestModel model = ApiTemplate.GetApiModelById(prop.Id);
            string requestParam = string.IsNullOrEmpty(prop.NewTemplate) ? ApiTemplate.GetApiRequestParam(prop.Id, param) : VelocityHelper.RenderString(prop.NewTemplate, param);
            ApiResponseModel responseModel = requestManager.GetApiResult(model, requestParam);
            result.TryAdd(prop.ResultName, responseModel);
        }
    }
}
