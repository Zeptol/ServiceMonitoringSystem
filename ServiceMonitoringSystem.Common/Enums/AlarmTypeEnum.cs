using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceMonitoringSystem.Common.Enums
{
    [Flags]
    public enum AlarmTypeEnum
    {
        [Display(Name = "短信")] Message = 1,
        [Display(Name = "邮件")] Mail = 2,
        [Display(Name = "微信")] WeChat = 4,
        [Display(Name = "钉钉")] DingDing = 8,
        [Display(Name = "全部")] All = 255
    }
}
