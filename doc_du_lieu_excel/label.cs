using System;
using System.Collections.Generic;

namespace doc_du_lieu_excel
{
    public class label
    {
        float price;
        int status;
        int times;
        float delta;
        static float maxDelta;
        static int maxTimes;
        static float maxPrice;
        static float minPrice = 0;

        
        static void setMaxPrice(float price)
        {
            if (price > maxPrice)
            {
                maxPrice = price;
            }
            if (minPrice == 0)
            {
                minPrice = price;
            }
            else
            {
                if (price < minPrice)
                {
                    minPrice = price;
                }
            }
        }

        static void setMaxDelta(float insertedDelta)
        {
            if (insertedDelta > maxDelta)
            {
                maxDelta = insertedDelta;
            }
        }
        static void setMaxTimes(int insertedTimes)
        {
            if (insertedTimes > maxTimes)
            {
                maxTimes = insertedTimes;
            }
        }

        static void setLabel(float price, float insertedDelta, int insertedTimes)
        {
            setMaxPrice(price);
            setMaxDelta(insertedDelta);
            setMaxTimes(insertedTimes);
        }
        void SetDelta(float delta)
        {
            this.delta = delta;
            setMaxDelta(delta);
        }
        void SetTimes(int times)
        {
            this.times = times;
            setMaxTimes(times);
        }
        void SetPrice(float price) 
        { 
            this.price = price;
            if (price > maxPrice)
            {
                maxPrice = price;
            }
            if (minPrice == 0)
            {
                minPrice = price;
            }
            else
            {
                if (price < minPrice)
                {
                    minPrice = price;
                }
            }
        }

        public float MaxDelta{ get => maxDelta; }
        public int MaxTimes{ get => maxTimes; }
        public float MaxPrice { get => maxPrice; }
        public float MinPrice { get => minPrice; }
        public label() { }
        public label(int status, int times, float delta)
        {
            this.status = status;
            this.times = times;
            this.delta = delta;
            setLabel(price, delta, times);
        }

        public label(float price, int status, int times, float delta)
        {
            this.price = price;
            this.status = status;
            this.times = times;
            this.delta = delta;
            setLabel(price,delta, times);
        }

        public int Status { get => status; set => status = value; }
        public int Times { get => times; set => SetTimes(value); }
        public float Delta { get => delta; set => SetDelta(value); }
        public float Price { get => price; set => SetPrice(value); }

        public string toString()
        {
            string a = this.price.ToString()+", "+ this.status.ToString() + ", " + this.times.ToString() + ", " + this.delta.ToString();
            return a;
        }
    }
} 
