using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Main.Features.Healthchecks.Models
{
    public class NotificationModel
    {
        public NotificationModel(string message, int errorCode)
        {
            this.Message = message;
            this.ErrorCode = errorCode;
        }

        public string Message { get; set; }

        public int ErrorCode { get; set; }
    }
}
