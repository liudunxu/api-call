using System;
using System.Net;
using System.IO;
using System.Text;
using Common.Engine.Model;
using log4net;

namespace Common.Engine
{
    /// <summary>
    ///ApiWebRequest 的摘要说明
    /// </summary>
    public class ApiWebRequest
    {
        //创建日志记录组件实例  
        private static readonly ILog Log = LogManager.GetLogger("fileLogger");  

        /// <summary>
        /// 获取api请求结果
        /// </summary>
        /// <param name="model">api请求参数格式,读取config/api/api.json</param>
        /// <param name="param">api的param</param>
        /// <returns></returns>
        public ApiResponseModel GetApiResult(ApiRequestModel model,string param)
        {
            var result = new ApiResponseModel {Format = model.ResponseFormat};
            Encoding encoding = Encoding.GetEncoding(model.Encoding);
            try
            {
                HttpWebRequest myRequest;
                //post请求
                if(model.Method == "POST")
                {
                    byte[] data = encoding.GetBytes(param);
                    myRequest = (HttpWebRequest)WebRequest.Create(model.Url);
                    myRequest.Method = "POST";
                    myRequest.Timeout = 30000;
                    myRequest.KeepAlive = false;
                    myRequest.ContentType = string.Format("application/x-www-form-urlencoded;charset={0}", model.Encoding);
                    myRequest.ContentLength = data.Length;
                    Stream newStream = myRequest.GetRequestStream();
                    // 发送数据
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                //get请求
                else
                {
                    myRequest = (HttpWebRequest)WebRequest.Create(model.Url+"?"+param);
                    myRequest.Method = "GET";
                    myRequest.Timeout = 30000;
                    myRequest.KeepAlive = false;
                    myRequest.ContentType =  "text/html";
                }

                //得到网页的原文件
                HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse();
                Stream resStream = response.GetResponseStream();
                if (resStream != null)
                {
                    //设置apiResponse属性
                    StreamReader sr = new StreamReader(resStream, encoding);
                    result.Response = sr.ReadToEnd();
                    //记录api操作日志
                    Log.InfoFormat("\nurl:{0}\n,param:{1}\n,response:{2}\n", model.Url, param, result.Response);
                    //设置返回结果
                    result.SetResponseToObject(model.ResponseFormat);
                    result.IsSuccess = true;
                    resStream.Close();
                    sr.Close();
                }
            }
            catch (Exception exp)
            {
                //标示当前api错误
                result.IsSuccess = false;
                Log.Error(string.Format("错误url:{0}\n,错误param:{1}", model.Url, param),exp);
            }

            return result;
        }
    }
}
