using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BLL
{
    public class ActivityBLL
    {
        public int Getprob() {
            string result = HttpContext.Current.Server.MapPath(@"/Content/Txt/prob.txt");
            Dictionary<string, double> prize = (Dictionary<string, double>)GetProbData("大转盘", result);
            List<string> list = DrawLottey(prize, 1);
            int resultnum = 0;
            foreach (KeyValuePair<string, double> item in prize)
            {
                resultnum++;
                if (item.Key == list[0])
                {
                    break;
                }
            }
            return resultnum;
        }

        private Dictionary<string, double> GetProbData(string proname, string url)
        {
            StreamReader sr = new StreamReader(url, Encoding.Default);
            Dictionary<string, double> prize = new Dictionary<string, double>();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line == proname)
                {
                    while ((line = sr.ReadLine()) != null & line != "")
                    {
                        string[] str = line.Split(new char[]
                        {
                    '\t'
                        });
                        if (str.Length > 1)
                        {
                            prize.Add(str[0].ToString(), Convert.ToDouble(str[1]));
                        }
                    }
                }
            }
            sr.Dispose();
            sr.Close();
            return prize;
        }
        private List<string> DrawLottey(Dictionary<string, double> prize, int total = 1)
        {
            long basicNumber = 0L;
            double[] array = new double[prize.Count];
            int i = 0;
            foreach (KeyValuePair<string, double> item in prize)
            {
                array[i] = item.Value;
                i++;
            }
            basicNumber = GetBaseNumber(array);
            Random random = new Random();
            List<string> list = new List<string>(total);
            for (int j = 0; j < total; j++)
            {
                long diceRoll = GetRandomNumber(random, 1L, basicNumber);
                long cumulative = 0L;
                foreach (KeyValuePair<string, double> item in prize)
                {
                    cumulative += (long)(item.Value * (double)basicNumber);
                    if (diceRoll <= cumulative)
                    {
                        list.Add(item.Key);
                        break;
                    }
                }
            }
            return list;
        }
        private long GetBaseNumber(double[] array)
        {
            long result = 0L;
            long result2;
            try
            {
                if (array == null || array.Length == 0)
                {
                    result2 = result;
                    return result2;
                }
                string targetNumber = string.Empty;
                for (int i = 0; i < array.Length; i++)
                {
                    string temp = array[i].ToString();
                    if (temp.Contains('.'))
                    {
                        temp = temp.Substring(temp.IndexOf('.')).Replace(".", "");
                        if (targetNumber.Length < temp.Length)
                        {
                            targetNumber = temp;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(targetNumber))
                {
                    int ep = targetNumber.Length;
                    result = (long)Math.Pow(10.0, (double)ep);
                }
            }
            catch
            {
            }
            result2 = result;
            return result2;
        }

        private long GetRandomNumber(Random random, long min, long max)
        {
            byte[] minArr = BitConverter.GetBytes(min);
            int hMin = BitConverter.ToInt32(minArr, 4);
            int lMin = BitConverter.ToInt32(new byte[]
            {
        minArr[0],
        minArr[1],
        minArr[2],
        minArr[3]
            }, 0);
            byte[] maxArr = BitConverter.GetBytes(max);
            int hMax = BitConverter.ToInt32(maxArr, 4);
            int lMax = BitConverter.ToInt32(new byte[]
            {
        maxArr[0],
        maxArr[1],
        maxArr[2],
        maxArr[3]
            }, 0);
            if (random == null)
            {
                random = new Random();
            }
            int h = random.Next(hMin, hMax);
            int i;
            if (h == hMin)
            {
                i = random.Next(Math.Min(lMin, lMax), Math.Max(lMin, lMax));
            }
            else
            {
                i = random.Next(0, 2147483647);
            }
            byte[] lArr = BitConverter.GetBytes(i);
            byte[] hArr = BitConverter.GetBytes(h);
            byte[] result = new byte[8];
            for (int j = 0; j < lArr.Length; j++)
            {
                result[j] = lArr[j];
                result[j + 4] = hArr[j];
            }
            return BitConverter.ToInt64(result, 0);
        }
    }
}
