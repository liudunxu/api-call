namespace Common.Engine.Model
{
    /// <summary>
    /// api请求参数格式
    /// </summary>
    public class ApiRequestModel
    {
        /// <summary>
        /// api标示
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// api的url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// api的请求参数模板
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// api请求方法get或者post
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// api编码
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// api返回格式
        /// </summary>
        public string ResponseFormat { get; set; }

        /// <summary>
        /// 每秒最多调用次数
        /// </summary>
        public int PerSecNum { get; set; }
    }
}
