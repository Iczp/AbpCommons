using System.ComponentModel.DataAnnotations;

namespace IczpNet.AbpCommons.Models
{
    public class ValueInput
    {
        //[DefaultValue(null)]
        //public virtual Guid? Id { get; set; }

        [Required]
        public virtual string Value { get; set; }
    }
}
