using System.ComponentModel.DataAnnotations;

namespace SyncToStaging.Helper.Models
{
    public class ServiceUrlModel
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(50)]
        public string Code { get; set; }//systemadmin
        [MaxLength(350)]
        public string Url { get; set; }//gatewayurl/[code]
    }
}
