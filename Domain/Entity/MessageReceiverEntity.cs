using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity
{
    public partial class MessageReceiverEntity:BaseEntity
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// 所属用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool IsReaded { get; set; }

        /// <summary>
        /// 查看时间
        /// </summary>
        public DateTime? ReadDate { get; set; }

        /// <summary>
        /// 消息主体
        /// </summary>
        public virtual MessageEntity Message { get; set; }
    }
}
