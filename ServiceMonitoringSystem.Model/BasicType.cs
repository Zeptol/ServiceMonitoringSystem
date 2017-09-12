using System.ComponentModel.DataAnnotations;

namespace ServiceMonitoringSystem.Model
{
    public class BasicType
    {
        public int _id { get; set; }
        public int Rid { get; set; }
        [Required]
        [Display(Name = "类型")]
        public int? TypeId { get; set; }
        [Required]
        [Display(Name = "编号")]
        public int? Num { get; set; }
        [Required]
        [Display(Name = "名称")]
        public string Name { get; set; }
    }
}
