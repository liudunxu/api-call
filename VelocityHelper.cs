using System;
using System.Collections.Generic;
using System.IO;
using NVelocity;
using NVelocity.App;
using log4net;

namespace Common.Engine
{
    /// <summary>
    /// 模板引擎工具类
    /// </summary>
    public static class VelocityHelper
    {
        //创建日志记录组件实例  
        private static readonly ILog Log = LogManager.GetLogger("fileLogger");  
        private static readonly VelocityContext BaseContext = new VelocityContext();
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static VelocityHelper()
        {
            //nvelocity引擎初始化
            Velocity.Init();
            //模板上下文初始化，全局变量
            BaseContext.Put("tmpDouble", 0.0);
        }

        /// <summary>
        /// 渲染模板
        /// </summary>
        /// <param name="templ">模板字符串，如"$!a.getName"</param>
        /// <param name="obj">模板传入参数，模板可直接调用，如$!param1</param>
        /// <returns></returns>
        public static string RenderString<T>(string templ, IDictionary<string, T> obj)
        {
            //模板为空，直接返回
            if (string.IsNullOrEmpty(templ))
            {
                return string.Empty;
            }
            try
            {
                //添加模板绑定数据
                var vltContext = new VelocityContext(BaseContext);
                foreach (var pair in obj)
                {
                    vltContext.Put(pair.Key, pair.Value);
                }

                //返回模板渲染结果
                StringWriter vltWriter = new StringWriter();
                Velocity.Evaluate(vltContext, vltWriter, null, templ);
                return vltWriter.GetStringBuilder().ToString();

            }
            catch (Exception e)
            {
                Log.Error(e);
                return string.Empty;
            }
        }
    }
}
