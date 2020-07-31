using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Domain.Entity
{
    public partial class MessageEntity
    {
        /// <summary>
        /// 设置总量
        /// </summary>
        public void SetNumber()
        {
            Total = MessageReceivers.Count;
        }

        /// <summary>
        /// 查阅
        /// </summary>
        public void Read()
        {
            ReadedNumber += 1;
        }
    }

    public partial class MessageReceiverEntity
    {
        public void Read()
        {
            if (IsReaded == false)
            {
                IsReaded = true;
                ReadDate = DateTime.Now;
                Message.Read();
            }
        }
    }
}
