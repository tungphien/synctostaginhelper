using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncToStaging.Helper.Models
{
    public class ServiceUrlModel
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(50)]
        public string Code { get; set; }//systemadmin
        [MaxLength(150)]
        public string Name { get; set; }//rdos API
        public int InternetType { get; set; } //1: Internal, 2 External
        public int APIKind { get; set; } //1: RDOS API, 2 1SPrincipal API, 3 Client API
        [MaxLength(350)]
        public string URL { get; set; }//gatewayurl/[code]
        [MaxLength(256)]
        public string ECRURL { get; set; }
        [MaxLength(10)]
        public string ECRVersion { get; set; }
        [MaxLength(350)]
        public string Versions { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [MaxLength(256)]
        public string CreatedBy { get; set; }
        [MaxLength(256)]
        public string UpdatedBy { get; set; }
    }
}
