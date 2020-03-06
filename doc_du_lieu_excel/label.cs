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

        static void setMaxLabel(float insertedDelta, int insertedTimes)
        {
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
        public float MaxDelta
        {
            get => maxDelta;
        }
        public int MaxTimes
        {
            get => maxTimes;
        }
        public label() { }
        public label(int status, int times, float delta)
        {
            this.status = status;
            this.times = times;
            this.delta = delta;
            setMaxLabel(delta, times);
        }

        public label(float price, int status, int times, float delta)
        {
            this.price = price;
            this.status = status;
            this.times = times;
            this.delta = delta;
            setMaxLabel(delta, times);
        }

        public int Status { get => status; set => status = value; }
        public int Times { get => times; set => SetTimes(value); }
        public float Delta { get => delta; set => SetDelta(value); }
        public float Price { get => price; set => price = value; }

        public string toString()
        {
            string a = this.price.ToString()+", "+ this.status.ToString() + ", " + this.times.ToString() + ", " + this.delta.ToString();
            return a;
        }
    }
} 
