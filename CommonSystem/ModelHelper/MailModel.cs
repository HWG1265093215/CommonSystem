using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CommonSystem.ModelHelper
{
    /// <summary>
    /// 邮件结构体
    /// </summary>
    public struct MailModel
    {
        /// <summary>
        /// 收件人地址
        /// </summary>
        public string ReceiverAddress { get; set; }
        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string ReceiverName { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 发件人地址（非必填）
        /// </summary>
        public string SenderAddress { get; set; }
        /// <summary>
        /// 发件人姓名（非必填）
        /// </summary>
        public string SenderName { get; set; }
        /// <summary>
        /// 发件人密码（非必填）
        /// </summary>
        public string SenderPassword { get; set; }
    }

    public static class MailSendHelper
    {

        public static bool SendMail(MailModel model)
        {
            try
            {
                MailAddress receiver = new MailAddress(model.ReceiverAddress, model.ReceiverName);
                MailAddress sender = new MailAddress(model.SenderAddress, model.SenderName);
                MailAddress mail = new MailAddress("1847017679@qq.com", "GDX");
                MailMessage message = new MailMessage();
                message.From = sender; //发件人
                message.To.Add(receiver); //收件人
                message.CC.Add(mail); //抄送人
                message.Subject = model.Title; //标题
                message.Body = model.Content; //内容
                message.IsBodyHtml = true; //是否支持内容为HTML

                SmtpClient client = new SmtpClient();
                client.Host = "smtp.qq.com";
                //client.Port = 465;
                client.EnableSsl = true; //是否启用SSL
                client.Timeout = 10000; //超时
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(model.SenderAddress, model.SenderPassword);
                client.Send(message);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
