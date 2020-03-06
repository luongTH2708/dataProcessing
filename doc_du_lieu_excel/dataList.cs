using System;
using System.Collections.Generic;
using System.Text;

namespace doc_du_lieu_excel
{
    class dataList
    {
        data[] datas;

        internal data[] Datas { get => datas; set => datas = value; }

        static void add()
        {

        }
        public dataList(data[] datas)
        {
            this.Datas = datas;
        }



    }
}
