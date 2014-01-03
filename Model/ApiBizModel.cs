using System.Collections.Generic;

namespace Common.Engine.Model
{
    /// <summary>
    /// api请求逻辑model
    /// </summary>
    public class ApiBizModel
    {
        /// <summary>
        /// 是否并行执行
        /// </summary>
        public bool IsParalla { get; set; }

        /// <summary>
        /// 请求api集合
        /// </summary>
        public List<RequestApiProp> RequestApiIds { get; set; }

        /// <summary>
        /// 结果模板
        /// </summary>
        public string ResultTempl{ get; set; }
    }

    /// <summary>
    /// api请求参数信息
    /// </summary>
    public class RequestApiProp
    {
        /// <summary>
        /// apiId
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// api请求结果名称
        /// </summary>
        public string ResultName { get; set; }

        /// <summary>
        /// api新请求格式
        /// </summary>
        public string NewTemplate { get; set; }
    }
}
