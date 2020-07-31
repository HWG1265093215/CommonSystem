namespace CommonSystem.ModelHelper
{
    /// <summary>
    ///     通用返回Json结果
    /// </summary>
    public class JsonResult<T>
    {
        public JsonResult()
        {
            flag = false;
            data = default;
            msg = string.Empty;
        }

        public JsonResult(bool flag, T data, string msg)
        {
            this.flag = flag;
            this.data = data;
            this.msg = msg;
        }

        /// <summary>
        ///     操作结果标识  True ？ False
        /// </summary>
        public bool flag { get; set; }

        /// <summary>
        ///     返回结果
        /// </summary>
        public T data { get; set; }

        /// <summary>
        ///     返回消息
        /// </summary>
        public string msg { get; set; }
    }
}