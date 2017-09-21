using System.ComponentModel.DataAnnotations;

namespace ServiceMonitoringSystem.Model
{
    public class ServiceAlertContacts
    {
        public string _id { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 服务ID
        /// </summary>
        public int ServiceId { get; set; }

        /// <summary>
        /// 主服务ID
        /// </summary>
        public int PrimaryId { get; set; }


        /// <summary>
        /// 联系人Id(即:姓名)
        /// </summary>
        [Required]
        [Display(Name = "姓名")]
        public string UserName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [Required]
        [Display(Name = "手机号")]
        [RegularExpression(@"^1\d{10}$", ErrorMessage = "手机号格式错误！")]
        public string Tel { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [Required]
        [Display(Name = "邮箱")]
        [RegularExpression(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "邮箱格式错误！")]
        public string Email { get; set; }
        /// <summary>
        /// 微信企业号里的uid
        /// </summary>
        [Display(Name = "企业微信号UID")]
        public string WeiXin_UID { get; set; }
        /// <summary>
        /// 钉钉账号uid
        /// </summary>
      [Display(Name = "企业钉钉号UID")]
        public string DingTalk_UID { get; set; }
        /// <summary>
        /// 报警通知方式:255-全部方式;1-短信;2-邮件;4-微信;8-钉钉;其他-组合方式
        /// </summary>
      [Required]
      [Display(Name = "报警通知方式")]
        public byte AlarmType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }

        public bool IsAlert { set; get; }
    }
}
