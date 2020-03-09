using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        }            
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

        static List<data> GetDataListByClass(List<data> datas,int status)
        {
            List<data> labelList = new List<data>();
            foreach(data d in datas)
            {
                if (d.Label.Status == status)
                    labelList.Add(d);
            }
            return labelList;
        }
        public static void SaveToPngImage(string filePath, data d, int range)
        {

            List<float> priceList = d.GoldPrice;

            //scaling price list into range
            int scaleRange = range - 1;
            List<int> price = new List<int>();
            float max = 0;
            float min = 0;
            foreach (float p in priceList)
            {
                if (min == 0) min = p;
                else if (p < min) min = p;
                if (p > max) max = p;
            }
            //range = a*max +b;
            //0 = a*min +b;
            //range = a *(max - min);
            float alpha, beta;
            alpha = scaleRange / (max - min);
            beta = -alpha * min;
            foreach (float p in priceList)
            {
                price.Add(Convert.ToInt32(p * alpha + beta));
            }

            int[,] a = new int[range, range];
            Array.Clear(a, 0, a.Length);
            for (int i = 0; i < price.Count; i++)
            {
                int j = 0;
                while (j <= price[i])
                {
                    a[j, i] = 255;
                    j++;
                }
            }

            //convert price list of data to byte image
            byte[] data = new byte[range * range];
            int index = 0;
            for (int r = 19; r >= 0; r--)
            {
                for (int c = 0; c < 20; c++)
                {
                    data[index] = byte.Parse(a[r, c].ToString());
                    index++;
                }
            }

            int width, height;
            width = height = range;
            if (width * height != data.Length)
                throw new FormatException("Size does not match");

            Bitmap bmp = new Bitmap(width, height);

            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    byte value = data[r * width + c];
                    bmp.SetPixel(c, r, Color.FromArgb(value, value, value));
                }
            }

            bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
        }


        //there are some bugs here, updated patch is comming soon because i'm lazy
        //static void DataInitialization(string dataPath, List<data> dataList,int classes)
        //{
        //    for (int i = 1; i <= 10; i++)
        //    {
        //        List<data> processedData = new List<data>(DataProcessing(dataList, i * 10));

        //        //create folder to store each percent data

        //        string folderPath = dataPath + @"Data\" + (i * 10) + @"%";
        //        System.IO.Directory.CreateDirectory(folderPath);

        //        //debug
        //        Console.WriteLine("\ncreate folder: {0}\n", folderPath);
        //        //processing data
        //        for (int c = 0; c < classes; c++)
        //        {
        //            //create classes folder in each percent folder
        //            string classesFolderPath = folderPath + @"\" + c;
        //            System.IO.Directory.CreateDirectory(classesFolderPath);

        //            //get data list in a class
        //            List<data> dl = new List<data>(GetDataListByClass(processedData, c));
        //            int imageIndex = 0;

        //            //save data as an image in each classes folder
        //            foreach (data d in dl)
        //            {
        //                //convert data to byte
        //                string filePath = classesFolderPath + @"\" + imageIndex + ".png";
        //                SaveToPngImage(filePath, d, d.GoldPrice.Count);
        //                imageIndex++;
        //            }

        //            //debug
        //            Console.WriteLine("class: {0} --- data in class: {1}\n", c, imageIndex);
        //        }
        //    }
        //}

        static void AppendToFile(string csvFileAddress, string classId, string fileName)
        {
            using (FileStream fileStream = new FileStream(csvFileAddress, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    writer.WriteLine(20 + ", " + 20 + ", " + 0 + ", " + 0 + ", " + 20 + ", " + 20 + ", " + classId + ", "+ fileName);
                }
            }
        }
        static void DataInitializationForTest(List<data> processedData)
        {

            //create folder to store each percent data

            string folderTrainPath = @"C:\Users\trinh\OneDrive\Documents\bienbao\input\Train";
            string folderTestPath = @"C:\Users\trinh\OneDrive\Documents\bienbao\input\Test";
            string fileTestCSVPath = @"C:\Users\trinh\OneDrive\Documents\bienbao\input\test2.csv";
            //processing data
            for (int c = 0; c < 3; c++)
            {
                //create classes folder in each percent folder
                string classesFolderPath = folderTrainPath + @"\" + c;
                System.IO.Directory.CreateDirectory(classesFolderPath);

                //get data list in a class
                List<data> dl = new List<data>(GetDataListByClass(processedData, c));
                int imageIndex = 0;
                int testImgIndex = 0;
                //save data as an image in each classes folder
                foreach (data d in dl)
                {
                    //convert data to byte
                    string filePath = classesFolderPath + @"\" + imageIndex + ".png"; //file to save image
                    if (imageIndex < dl.Count * 0.9)
                        SaveToPngImage(filePath, d, d.GoldPrice.Count);
                    else
                    {
                        string[] dir = System.IO.Directory.GetFiles(folderTestPath);
                        testImgIndex = dir.Count();
                        filePath = folderTestPath + @"\" + testImgIndex + ".png"; //file to save image
                        SaveToPngImage(filePath, d, d.GoldPrice.Count);
                        AppendToFile(fileTestCSVPath, c+"","Test/" + testImgIndex+".png");
                        testImgIndex++;
                    }
                    imageIndex++;
                }

                //debug
                Console.WriteLine("class: {0} --- data in class: {1}\n", c, imageIndex);
            }

        }
        static void Main(string[] args)
        {
            string dataPath = @"C:\Users\trinh\OneDrive\Desktop\";
            string fileName = "golddata.csv";
            List<float> oData = GetData(dataPath + fileName);
            List<label> labelList = new List<label>(GetLabel(oData));
            List<data> dataList = new List<data>(LabelProcessing(labelList));

            List<data> processedData = new List<data>(DataProcessing(dataList, 10));
            DataInitializationForTest(dataList);
            //DataInitialization(dataPath, dataList, 3);
            //---------------------------------------------TESTING-HERE----------------------------------------------------------------------------------------------

            //float maxDelta = dataList[0].Label.MaxDelta;
            //List<int> price = ScalePrice(dataList[0],20);
            //int[,] a = convertImage(price);
            //for (int i = 19; i >= 0; i--)
            //{
            //    for (int k = 0; k <= 19; k++)
            //    {
            //        Console.Write("{0} ", a[i, k]);
            //    }
            //    Console.Write("\n");
            //}

        }
    }
} 
