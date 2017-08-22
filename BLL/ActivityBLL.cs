using Common;
using DAL;
using Fgly.Common.Expand;
using FJSZ.OA.Common.Web;
using Model.ViewModel;
using Model.WxModel;
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
        CommonDAL dal = new CommonDAL();
        ActivityDAL adal = new ActivityDAL();
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

        /// <summary>
        /// 获取概率的基数
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private long GetBaseNumber(double[] array)
        {
            long result = 0;

            try
            {
                if (array == null || array.Length == 0)
                {
                    return result;
                }

                string targetNumber = string.Empty;

                foreach (double item in array)
                {
                    string temp = item.ToString();

                    if (!temp.Contains('.'))
                    {
                        continue;
                    }

                    temp = temp.Substring(temp.IndexOf('.')).Replace(".", "");

                    if (targetNumber.Length < temp.Length)
                    {
                        targetNumber = temp;
                    }
                }

                if (!string.IsNullOrEmpty(targetNumber))
                {
                    int ep = targetNumber.Length;

                    result = (long)Math.Pow(10, ep);
                }
            }
            catch { }

            return result;
        }
        private long GetRandomNumber(Random random, long min, long max)
        {
            byte[] minArr = BitConverter.GetBytes(min);

            int hMin = BitConverter.ToInt32(minArr, 4);

            int lMin = BitConverter.ToInt32(new byte[] { minArr[0], minArr[1], minArr[2], minArr[3] }, 0);

            byte[] maxArr = BitConverter.GetBytes(max);

            int hMax = BitConverter.ToInt32(maxArr, 4);

            int lMax = BitConverter.ToInt32(new byte[] { maxArr[0], maxArr[1], maxArr[2], maxArr[3] }, 0);

            if (random == null)
            {
                random = new Random();
            }

            int h = random.Next(hMin, hMax);

            int l = 0;

            if (h == hMin)
            {
                l = random.Next(Math.Min(lMin, lMax), Math.Max(lMin, lMax));
            }
            else
            {
                l = random.Next(0, Int32.MaxValue);
            }

            byte[] lArr = BitConverter.GetBytes(l);

            byte[] hArr = BitConverter.GetBytes(h);

            byte[] result = new byte[8];

            for (int i = 0; i < lArr.Length; i++)
            {
                result[i] = lArr[i];
                result[i + 4] = hArr[i];
            }

            return BitConverter.ToInt64(result, 0);
        }

        public int Getprob(int cooperid,string openid,string phone)
        {
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/ActivityBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "方法Getprob开始：" + " cooperid=" + cooperid + " openid" + openid);
            Dictionary<string, string> prize = (Dictionary<string, string>)GetProbData(cooperid);
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/ActivityBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "1");
            List<string> list = DrawLottey(prize, 1);
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/ActivityBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "2: list=" + list.Count.ToString());
            int resultnum = 0;
            foreach (KeyValuePair<string, string> item in prize)
            {
                resultnum++;
                if (item.Key == list[0])
                {
                    int result = adal.HandleActivity(1, cooperid, 1, openid, phone, Convert.ToInt32(item.Key));
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/ActivityBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "方法Getprob，插入大转盘中奖记录:result=" + result.ToString() + " cooperid=" + cooperid + " openid" + openid + " phone" + phone);
                    break;
                }
            }
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/ActivityBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "3 list[0]: " + list[0]+ "  resultnum="+ resultnum);
            return resultnum;
        }
        public Dictionary<string, string> GetProbData(int cooperid)
        {
            int configid = adal.GetActivityConfigId(cooperid, 1);    //取得大转盘的配置主表configid
            if (configid == 0) return null;
            IList<probdto> list1 = DataTableToList.ModelConvertHelper<probdto>.ConvertToModel(adal.GetActivityConfigList(configid));
            if (list1.Count > 0)
            {
                Dictionary<string, string> prize = new Dictionary<string, string>();
                foreach (var item in list1)
                {
                    //prize.Add(item.prizename + "|" + item.count, item.winprob);
                    prize.Add(item.id.ToString(), item.winprob);
                }
                return prize;
            }
            return null;
        }
        //奖品的名称列表
        public Dictionary<string, string> GetProbNameData(int cooperid)
        {
            int configid = adal.GetActivityConfigId(cooperid, 1);    //取得大转盘的配置主表configid
            if (configid == 0) return null;
            IList<probdto> list1 = DataTableToList.ModelConvertHelper<probdto>.ConvertToModel(adal.GetActivityConfigList(configid));
            if (list1.Count > 0)
            {
                Dictionary<string, string> prize = new Dictionary<string, string>();
                foreach (var item in list1)
                {
                    prize.Add(item.prizename, item.winprob);
                }
                return prize;
            }
            return null;
        }
        private List<string> DrawLottey(Dictionary<string, string> prize, int total = 1)
        {
            long basicNumber = 0L;
            double[] array = new double[prize.Count];
            int i = 0;
            foreach (KeyValuePair<string, string> item in prize)
            {
                array[i] = Convert.ToDouble(item.Value);
                i++;
            }
            basicNumber = GetBaseNumber(array);
            Random random = new Random();
            List<string> list = new List<string>(total);
            for (int j = 0; j < total; j++)
            {
                long diceRoll = GetRandomNumber(random, 1L, basicNumber);
                long cumulative = 0L;
                foreach (KeyValuePair<string, string> item in prize)
                {
                    cumulative += (long)(Convert.ToDouble(item.Value) * (double)basicNumber);
                    if (diceRoll <= cumulative)
                    {
                        list.Add(item.Key);
                        break;
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 取得当前用户可摇奖次数
        /// </summary>
        /// <param name="cooperid">T_CooperConfig的ID值</param>
        /// <param name="type">1为大转盘</param>
        /// <param name="openid">微信openid</param>
        /// <returns></returns>
        public int GetOpenidCount(int cooperid, int type, string openid)
        {
            int result = adal.HandleActivity(3, cooperid, type, openid, "", 0);
            return result;
        }
        public int UpdateActivityPhone(int cooperid, int type, string openid,string phone)
        {
            int result = adal.HandleActivity(2, cooperid, type, openid, phone, 0);
            return result;
        }
        /// <summary>
        /// 把用户分享到朋友圈的记录
        /// </summary>
        /// <param name="cooperid">T_CooperConfig的ID值</param>
        /// <param name="type">1为大转盘</param>
        /// <param name="openid">微信openid</param>
        /// <param name="sharetype">1分享给朋友，2分享到朋友圈</param>
        /// <returns></returns>
        public int ActivityeShare(int cooperid, int type, string openid, int sharetype)
        {
            int result = adal.HandleActivity(4, cooperid, type, openid, "", sharetype);
            return result;
        }
        public T_ActivityConfig FindActivityConfigByCooperid(int cooperid) {
            IList<T_ActivityConfig> list = DataTableToList.ModelConvertHelper<T_ActivityConfig>.ConvertToModel(adal.GetActivityZb(cooperid));
            T_ActivityConfig dto = new T_ActivityConfig();
            if (list.Count > 0){
                dto = list[0];
            }
            return dto;
        }
        /// <summary>
        /// 更新背景图片
        /// </summary>
        /// <param name="cooperid"></param>
        /// <param name="fileid">前端file的文件名</param>
        /// <returns></returns>
        public string DzpUploadBgUrl(int cooperid,string fileid) {
            string pathx = "/Content/Activity/Dzp/images/";                                        //图片地址
            string filename = "body_bg"+ cooperid + ".jpg";                 //图片名称 
            string vtime = "?v=" + DateTime.Now.ToUnixTimeStamp().ToString();           //用时间戳来做版本号
            string path = WebHelp.HttpUploadFile(pathx, filename, fileid); //返回完整的上传地址 
            if (!string.IsNullOrEmpty(path))
            {
                return pathx + filename + vtime;
                //int result = adal.UpdateDzpBgUrlByCooperid(cooperid, pathx + filename+ vtime);
                //if (result > 0) return pathx + filename + vtime;
            }
            return "";
        }
        /// <summary>
        /// 取得配置对象列表
        /// </summary>
        /// <param name="configid"></param>
        /// <returns></returns>
        public IList<T_ActivityConfigList> FindConfigList(int configid)
        {
            IList<T_ActivityConfigList> list = DataTableToList.ModelConvertHelper<T_ActivityConfigList>.ConvertToModel(adal.GetActivityConfigList(configid));
            return list;
        }
        public int SetDzpConfig(int configid,int cooperid,string title,int share,string explain,string bgurl,IList<T_ActivityConfigList> list) {
            int result = 0;
            int resultnum = 0;
            //新增
            if (configid == 0)
            {
                configid = adal.AddConfig(cooperid, 1, title, share, explain, bgurl); //主表ID
                result = configid;
            }
            else
                result = adal.UpdateConfig(configid, cooperid, 1, title, share, explain, bgurl);
            foreach (var item in list)
            {
                result = adal.SetConfigList(item.id, configid, item.prizename, item.count, item.number, item.winprob);
                if (result > 0)
                    resultnum++;
            }
            return result;
        }
        public int RemoveActivitys(IList<IdListDto> ids)
        {
            int sresult = 0;    //成功的数量
            if (ids.Count == 0)
                throw new ArgumentNullException();
            else
            {
                try
                {
                    foreach (var item in ids)
                    {
                        int gid = item.id;        //ID
                        int result = adal.ActivityRemoveById(gid);
                        sresult = sresult + result;
                    }
                }
                catch
                {
                    Common.Expend.LogTxtExpend.WriteLogs("/Logs/ActivityBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "方法 RemoveActivitys 异常：" + " 成功执行=" + sresult);
                    return -1000;
                }
            }
            return sresult;
        }
        /// <summary>
        /// 取得大转盘机率合值
        /// </summary>
        /// <param name="configid"></param>
        /// <returns></returns>
        public float GetWinProb(int configid) {
            float result = adal.GetWinProb(configid);
            return result;
        }
        /// <summary>
        /// 当前这个用户中奖信息中是否有号码
        /// </summary>
        /// <param name="cooperid"></param>
        /// <param name="type"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public string GetActivityPhone(int cooperid, int type, string openid) {
            string phone = adal.GetActivityPhone(cooperid, type, openid);
            return phone;
        }
    }

    public class probdto {
        public int id { get; set; }
        public string prizename { get; set; }
        public int count { get; set; }
        public int number { get; set; }
        public string winprob { get; set; }
    }
}
