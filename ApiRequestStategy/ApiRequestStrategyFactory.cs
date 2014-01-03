namespace Common.Engine.ApiRequestStategy
{
    /// <summary>
    /// api请求策略工厂
    /// </summary>
    public class ApiRequestStrategyFactory
    {
        /// <summary>
        /// 获取api请求策略
        /// </summary>
        /// <param name="isSingleThread">是否为单线程</param>
        /// <returns></returns>
        public static IApiRequestStrategy GetStrategy(bool isSingleThread)
        {
            if(isSingleThread)
            {
                return new SimpleApiRequestStrategy();
            }
            return new CoApiRequestStrategy();
        }
    }
}
