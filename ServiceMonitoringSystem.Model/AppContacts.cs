namespace ServiceMonitoringSystem.Model
{
    public class AppContacts
    {
        /// <summary>
        /// 联系人Id(即:姓名)
        /// </summary>
        public string _id { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 微信企业号里的uid
        /// </summary>
        public string WeiXin_UID { get; set; }
        /// <summary>
        /// 钉钉账号uid
        /// </summary>
        public string DingTalk_UID { get; set; }
        /// <summary>
        /// 报警通知方式:255-全部方式;1-短信;2-邮件;4-微信;8-钉钉;其他-组合方式
        /// </summary>
        public byte AlarmType = byte.MaxValue;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
