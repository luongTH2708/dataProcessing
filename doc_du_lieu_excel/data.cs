using System;
using System.Collections.Generic;
using System.Text;

namespace doc_du_lieu_excel
{
    class data
    {
        List<float> goldPrice;
        label label;

        public data() { }
        public data(List<float> goldPrice, label label)
        {
            this.goldPrice = goldPrice;
            this.label = label;
        }
        public List<float> GoldPrice { get => goldPrice; set => goldPrice = value; }
        public label Label { get => label; set => label = value; }
    }
}
