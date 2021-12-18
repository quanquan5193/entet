using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.Common.Helpers
{
    public static class CardCheckStatusCode
    {
        public static string NotExistCardInDatabase => "NotExistCardInDatabase";
        public static string ErrorCardStatus => "ErrorCardStatus";
        public static string OK => "OK";
        public static string InvalidLength => "InvalidLength";
        public static string FirstLetterNotEqual2 => "FirstLetterNotEqual2";
        public static string CardExpired => "CardExpired";
        public static string InvalidBetweenGiveAndReceiveCard_1 => "InvalidBetweenGiveAndReceiveCard_1";
        public static string InvalidBetweenGiveAndReceiveCard_2 => "InvalidBetweenGiveAndReceiveCard_2";
        public static string InvalidBetweenGiveAndReceiveCard_3 => "InvalidBetweenGiveAndReceiveCard_3";
        public static string InvalidBetweenGiveAndReceiveCard_4 => "InvalidBetweenGiveAndReceiveCard_4";
    }
}
