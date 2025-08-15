using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MizanOriginalSoft.MainClasses.OriginalClasses 
{
    public class PaymentMethod
    {
        public int ID { get; set; }
        public string? NamePaymentMethod { get; set; }

        // هذا يجعل الكمبوبوكس يعرض الاسم
        public override string? ToString()
        {
            return NamePaymentMethod;
        }
    }

}
