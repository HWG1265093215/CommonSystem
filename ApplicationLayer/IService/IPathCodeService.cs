using System.Collections.Generic;

namespace ApplicationLayer.IService
{
    /// <summary>
    /// 路径码服务接口
    /// </summary>
    public interface IPathCodeService
    {
        /// <summary>
        /// 获取路径码
        /// </summary>
        /// <returns></returns>
        IList<string> GetPathCodes();
    }
}
