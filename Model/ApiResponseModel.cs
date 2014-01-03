using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Common.Engine.Model
{
    /// <summary>
    /// api返回结果信息
    /// </summary>
    public class ApiResponseModel
    {
        /// <summary>
        /// 是否调用成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// api返回结果
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// 返回信息格式
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// 返回信息对象
        /// </summary>
        public object ResponseObj { get; set; }

        /// <summary>
        /// 设置response信息为对象
        /// </summary>
        /// <param name="format"></param>
        public void SetResponseToObject(string format)
        {
            //没配置格式，直接返回
            if(null == format)
            {
                return;
            }
            //根据格式设置返回结果
            switch (format.ToLower())
            {
                case "json":
                    ResponseObj = JsonConvert.DeserializeObject<JContainer>(Response);
                    break;
                case "xml":
                    {
                        var xdoc = new XmlDocument();
                        xdoc.LoadXml(Response);
                        ResponseObj = xdoc;
                    }
                    break;
            }
        }

    }
}
