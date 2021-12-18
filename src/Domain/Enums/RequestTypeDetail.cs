using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Domain.Enums
{
    public enum RequestTypeDetail
    {
        [StringValue("")]
        Unset = 0,

        //request type detail for switch card
        [StringValue("ポ専＞プリカ切替")]
        SwitchCard_PointToPrepaid = 1,
        [StringValue("カード磁気不良")]
        SwitchCard_MagneticFailure = 2,
        [StringValue("有効期限更新")]
        SwitchCard_ExpirationDateRenewal = 3,
        //end switch card

        //request type detail for Lost card reissue
        [StringValue("ポイント専用カード")]
        ReIssue_PointCard = 4,
        [StringValue("ポイント&プリペイドカード")]
        ReIssue_PrepaidCard = 5,
        //end Lost card reissue
    }
}
