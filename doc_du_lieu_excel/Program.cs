using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace doc_du_lieu_excel
{
    class Program
    {
        //doc du lieu
        //khai bao reader doc du lieu
        static List<float> GetData(string csvFileAddress)
        {
            using (var reader = new StreamReader(csvFileAddress))
            {

                List<string> listA = new List<string>();
                //doc tung dong toan bo file
                //can them 1 dong co gia tri bang dong cuoi
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    listA.Add(line);
                }

                List<float> oData = new List<float>();
                //chuyen list string thanh list float cho de tinh
                oData = listA.Select(x => float.Parse(x)).ToList();

                return oData;
            }
        }            //them 1 label vao label list
        static bool isIncrease(float delta)
        {
            //neu delta > 0 thi du lieu tang status = 2 va nguoc lai status = 1
            if (delta > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //dua vao list data kieu du lieu float, tra ve mot label list
        static List<label> GetLabel(List<float> oData)
        {
            List<label> listLabel = new List<label>();
            //duyet toan bo list oData
            for (int i = 0; i < oData.Count - 1; i++)
            {
                int j = i;
                List<float> stack = new List<float>();
                //cho du lieu dau tien vao stack
                stack.Add(oData[j]);
                //kiem tra trang thai cua du lieu hien tai
                //neu du lieu trang thai dang tang thi duyet tiep den khi trang thai thay doi
                if (oData[j] < oData[j + 1])
                {
                    while (oData[j] < oData[j + 1])
                    {
                        j++;
                        if (j == oData.Count - 1)
                        {
                            break;
                        }
                        stack.Add(oData[j]);
                    }
                }
                else
                {
                    while (oData[j] >= oData[j + 1])
                    {
                        j++;
                        if (j == oData.Count - 1)
                        {
                            break;
                        }
                        stack.Add(oData[j]);
                    }
                }
                //tinh do lech cua gia tri dau va gia tri cuoi
                float delta = stack[stack.Count - 1] - stack[0];
                if (isIncrease(delta))
                    listLabel.Add(new label(oData[i], 2, oData.Count, delta));
                else
                    listLabel.Add(new label(oData[i], 1, oData.Count, delta));
            }
            return listLabel;
        }


        static List<data> LabelProcessing(List<label> labels)
        {
            List<float> priceList = new List<float>();
            List<data> d = new List<data>();
            for (int i = 0; i < labels.Count; i++)
            {
                priceList.Add(labels[i].Price);
                if (priceList.Count == 20)
                {
                    List<float> temp = new List<float>(priceList);
                    data tempdata = new data(temp, labels[i]);
                    d.Add(tempdata);
                    priceList.RemoveAt(0);
                }
            }
            return d;
        }
        static List<data> DataProcessing(List<data> dataList,int percent)
        {
            List<data> dl = new List<data>();
            float maxDelta = dataList[0].Label.MaxDelta;
            if (percent > 0 && percent <= 100)
            {
                foreach (data d in dataList)
                {
                    if (Math.Abs(d.Label.Delta) <= (maxDelta * percent /100))
                    {
                        d.Label.Status = 0;
                    }
                    dl.Add(d);
                }
            }
            return dl;
        }
        //get label 0
        static List<data> GetLabelListByStatus(List<data> datas,int status)
        {
            List<data> labelList = new List<data>();
            foreach(data d in datas)
            {
                if (d.Label.Status == status)
                    labelList.Add(d);
            }
            return labelList;
        }

        static void createFile(string pathString)
        {
            // Check that the file doesn't already exist. If it doesn't exist, create
            if (!System.IO.File.Exists(pathString))
            {
                System.IO.File.Create(pathString);
            }
            else
            {
                Console.WriteLine("File \"{0}\" already exists.", pathString);
                return;
            }
        }        //ghi label: dua vao listLabel, ghi ra file CSV
        static void ghiLabel(List<label> listLabel)
        {
            using (var writer = new StreamWriter(@"C:\Users\trinh\OneDrive\Desktop\goldlabel.csv"))
            {
                List<string> listA = new List<string>();
                listA = listLabel.Select(x => x.toString()).ToList();
                for (int i = 0; i < listA.Count; i++)
                {
                    writer.Write(listA[i] + "\n");
                }
            }
        }
        static void createFileData(string folderName,List<data> dataProcess,int classes)
        {
            System.IO.Directory.CreateDirectory(folderName);
            for(int i = 0; i < classes; i++)
            {
                string fileName = folderName + i + ".csv";
                createFile(fileName);
                List<data> dl = new List<data>(GetLabelListByStatus(dataProcess, i));
                using (var writer = new StreamWriter(fileName))
                {
                    foreach (data d in dl)
                    {
                        string str = string.Join("; ", d.GoldPrice);
                        writer.Write(str + "\n");
                    }
                }
            } 
                
        }
        static void Main(string[] args)
        {
            string dataPath = @"C:\Users\trinh\OneDrive\Desktop\";
            string fileName = "golddata.csv";
            List<float> oData = GetData(dataPath+fileName);
            List<label> labelList = new List<label>(GetLabel(oData));
            List<data> dataList = new List<data>(LabelProcessing(labelList));
            float maxDelta = dataList[0].Label.MaxDelta;
            for (int i = 1; i <= 10; i++)
            {
                List<data> dataProcess = new List<data>(DataProcessing(dataList, i * 10));
                createFileData(dataPath + @"Data\" + i + @"\", dataProcess,3);
                //Console.WriteLine("test percent: {0}% of max: {4}\nN0: {1}\nN1:{2}\nN2:{3}\n", i * 10, data0.Count, data1.Count, data2.Count, maxDelta * (i * 0.1));
            }
        }
    }
} 
