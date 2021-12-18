using mrs.Domain.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mrs.Domain.Entities
{
    public class PICStore : AuditableEntity
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(6)]
        public string PICCode { get; set; }
        public string PICName { get; set; }
        public IList<RequestsPending> RequestsPendings { get; set; }
        public IList<Member> Members { get; set; }
    }
}
