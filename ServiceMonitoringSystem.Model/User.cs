namespace ServiceMonitoringSystem.Model
{
    public class User
    {
        public int _id { set; get; }

        public int DefaultRoleId { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string LoginName { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string UserName { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string EnglishName { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string UserMark { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string dept { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string dept_New { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string UserPwd { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string PassWord { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string Remark { set; get; }

        /// <summary>
        /// 失败次数
        /// </summary>
        public int FailTimes { set; get; }
    }
}
