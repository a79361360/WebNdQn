using FJSZ.OA.Common.Web;
using Fgly.Common.Expand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.CommonModel;
using FJSZ.OA.Common.DEncrypt;
using System.Collections.Specialized;
using Common.ExHelp;
using Newtonsoft.Json;
using Model.WxModel;

namespace BLL
{
    public class WeiXinBLL
    {
        WebHttp web = new WebHttp();
        public string Get_Jsapi_Ticket(string access_token) { 
            string url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + access_token + "&type=jsapi";
            string result = web.Get(url);
            WxJsApi_ticket dto = JsonConvert.DeserializeObject<WxJsApi_ticket>(result);
            return dto.ticket;
        }
        public string Get_Access_Token(string appid,string secret) {
            string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + secret;
            string result = web.Get(url);
            WxJsApi_token dto = JsonConvert.DeserializeObject<WxJsApi_token>(result);
            return dto.access_token;
        }
        public string Get_signature(string jsapi_ticket) {
            long timestamp = 1493205225;
            //long timestamp = DateTime.Now.ToUnixTimeStamp();
            string noncestr = "Cz2dVmjsL1Lc0ygU";
            //string noncestr = TxtHelp.GetRandomString(16, true, true, true, false, "");
            string url = WebHelp.GetUrl();
            List<Parameter> listParam = new List<Parameter>();
            listParam.Add(new Parameter("noncestr", noncestr));
            listParam.Add(new Parameter("jsapi_ticket", jsapi_ticket));
            listParam.Add(new Parameter("timestamp", timestamp.ToString()));
            listParam.Add(new Parameter("url", url));
            StringBuilder str = DicSort(listParam);
            //string ll = "jsapi_ticket=sM4AOVdWfPE4DxkXGEs8VMCPGGVi4C3VM0P37wVUCFvkVAy_90u5h9nbSlYy3-Sl-HhTdfl2fzFy1AOcHKP7qg&noncestr=Wm3WZYTPz0wzccnW&timestamp=1414587457&url=http://mp.weixin.qq.com?params=value";
            //string signature = Encryptor.MD5Encrypt(ll);
            string signature = Encryptor.SHA1Encrypt(str.ToString());
            return signature.ToLower();
        }
        /// <summary>
        /// 将待签名的参数列表按ACSII码按字典排序后,再生成字符串
        /// </summary>
        /// <param name="listParam"></param>
        /// <returns></returns>
        public StringBuilder DicSort(List<Parameter> listParam)
        {
            NameValueCollection postCollection = new NameValueCollection();
            foreach (var item in listParam)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    postCollection.Add(item.Name, item.Value);
                }
            }
            var dic = ListHelp.GetSortedDictionary(postCollection, null);   //排序后到字典
            StringBuilder tmp = new StringBuilder();
            ListHelp.FillStringBuilder(tmp, dic);//将QueryString填入StringBuilder 
            return tmp;
        }
    }
}
