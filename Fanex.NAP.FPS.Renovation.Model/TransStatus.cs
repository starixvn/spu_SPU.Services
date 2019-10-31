using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FPS.Renovation.Model
{
    public enum TransStatus
    {
        /// <summary>
        /// Binary: 0 - Decimal: 0
        /// </summary>
        ValidTransaction = -1,
        /// <summary>
        /// Binary: 1 - Decimal: 1 
        /// </summary>
        InvalidSubscriber = 15,
        /// <summary>
        /// Binary: 10 - Decimal: 2
        /// </summary>
        InvalidActionResult = 14,
        /// <summary>
        /// Binary: 100 - Decimal 4
        /// </summary>
        DeviceCodeIsEmpty = 13,
        /// <summary>
        /// Binary: 1000 - Decimal: 8
        /// </summary>
        InvalidLoginName = 12,
        /// <summary>
        /// Binary: 10000 - Decimal: 16
        /// </summary>
        InvalidDI = 11
    }
}
