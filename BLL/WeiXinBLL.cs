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
using DAL;
using Common;

namespace BLL
{
    public class WeiXinBLL
    {
        WebHttp web = new WebHttp();
        CommonDAL dal = new CommonDAL();
        WeiXinDAL wdal = new WeiXinDAL();
        public string Get_Jsapi_Ticket(string access_token) {
            var t = FJSZ.OA.Common.CacheAccess.GetFromCache("jsapi_ticket");
            if (t==null)
            {
                string url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + access_token + "&type=jsapi";
                string result = web.Get(url);
                WxJsApi_ticket dto = JsonConvert.DeserializeObject<WxJsApi_ticket>(result);
                t = dto.ticket;
                //int timout = 3600 - DateTime.Now.Minute * 60;
                FJSZ.OA.Common.CacheAccess.InsertToCacheByTime("jsapi_ticket", dto.ticket, 7200);
            }
            return t.ToString();
        }
        public string Get_Access_Token(string appid,string secret) {
            var t = FJSZ.OA.Common.CacheAccess.GetFromCache("jsapi_token");
            if (t==null)
            {
                string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + secret;
                string result = web.Get(url);
                WxJsApi_token dto = JsonConvert.DeserializeObject<WxJsApi_token>(result);
                t = dto.access_token;
                FJSZ.OA.Common.CacheAccess.InsertToCacheByTime("jsapi_token", dto.access_token, 7200);
            }
            return t.ToString();
        }
        public string Get_signature(long timestamp, string noncestr)
        {
            string access_token = Get_Access_Token(Wx_config.appid, Wx_config.appsecret);   //access_token
            string jsapi_ticket = Get_Jsapi_Ticket(access_token);                                               //jsapi_ticket
            string url = WebHelp.GetUrl();                                                                      //url
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
        /// 取得CGI的TOKEN值,这种方式取不到用户的openid,所以需要code先取一下openid值
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public string Get_Cgi_Taoke(string appid, string secret) {
            var t = FJSZ.OA.Common.CacheAccess.GetFromCache("cgi_token");
            if (t == null)
            {
                string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + secret;
                string result = web.Get(url);
                WxJsApi_token dto = JsonConvert.DeserializeObject<WxJsApi_token>(result);
                t = dto.access_token;
                FJSZ.OA.Common.CacheAccess.InsertToCacheByTime("cgi_token", dto.access_token, 7200);
            }
            return t.ToString();
        }
        public Wx_UserInfo Get_Cgi_UserInfo(string openid,string token) {
            string url = "https://api.weixin.qq.com/cgi-bin/user/info?access_token=" + token + "&openid=" + openid + "&lang=zh_CN";
            string result = web.Get(url);
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/WxDefault_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "Get_Cgi_UserInfo：" + result);
            Wx_UserInfo dto = JsonConvert.DeserializeObject<Wx_UserInfo>(result);
            return dto;
        }
        public string Wx_Auth_Code(string appid,string backurl,string scope,string state) {
            string url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid="+appid+"&redirect_uri="+ backurl + "&response_type=code&scope="+ scope + "&state="+ state + "#wechat_redirect";
            return url;
        }
        //取得SNS网页授权token
        public WxJsApi_token Wx_Auth_AccessToken(string appid,string secret,string code) {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid="+ appid + "&secret="+ secret + "&code="+ code + "&grant_type=authorization_code";
            string result = web.Get(url);
            WxJsApi_token dto = JsonConvert.DeserializeObject<WxJsApi_token>(result);
            return dto;
        }
        /// <summary>
        /// SNS微信用户的详细信息
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Wx_UserInfo Get_SNS_UserInfo(string openid, string token) {
            string url = "https://api.weixin.qq.com/sns/userinfo?access_token="+ token + "&openid="+ openid + "&lang=zh_CN";
            string result = web.Get(url);
            Wx_UserInfo dto = JsonConvert.DeserializeObject<Wx_UserInfo>(result);
            return dto;
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


        public T_CooperConfig Get_CooperConfig(int ctype,int issue)
        {
            IList<T_CooperConfig> list = DataTableToList.ModelConvertHelper<T_CooperConfig>.ConvertToModel(dal.GetCooperConfig(ctype, issue));
            if (list.Count > 0) {
                return list[0];
            }
            return null;
        }
        public Dictionary<string, string> ParmToDic(string url)
        {
            string[] str = url.Split('?');
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (str.Length > 1)
            {
                string[] str1 = str[1].Split('|');
                foreach (string item in str1)
                {
                    string[] str2 = item.Split('=');
                    dic.Add(str2[0], str2[1]);
                }
            }
            //string lll = "http://wx.ndll800.com/home/default?ctype=1&issue=1";
            //ParmToDic(lll);
            return dic;
        }


        public int SetWxUserInfo(string appid,string openid,string nickname,int sex,string headurl, string unionid = "1") {
            int id = 0; //记录的ID
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/WeiXinBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "SetWxUserInfo: 开始");
            int result = wdal.IsExistsWxUser(appid, openid);
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/WeiXinBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "result:" + result);
            if (result > 0) {
                if(result>1) Common.Expend.LogTxtExpend.WriteLogs("/Logs/UnusualErr_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "WeiXinBLL=>SetWxUserInfo(154) 说明:    理论上这个数值不应该多于1条,一个appid对应一个openid" );
                id = wdal.GetWxUserIdByAO(appid, openid);
            }
            T_WxUserInfo dto = new T_WxUserInfo();
            dto.id = id;dto.wx_appid = appid;dto.wx_openid = openid;dto.wx_nickname = nickname;dto.wx_sex = sex;dto.wx_headurl = headurl;dto.wx_unionid = "";
            Common.Expend.LogTxtExpend.WriteLogs("/Logs/WeiXinBLL_" + DateTime.Now.ToString("yyyyMMddHH") + ".log", "dto.id:" + dto.id+ "dto.wx_appid:  "+ dto.wx_appid + "dto.wx_openid:  " + dto.wx_openid + "dto.wx_nickname:  " + dto.wx_nickname + "dto.wx_sex:  " + dto.wx_sex + "dto.headurl:  " + dto.wx_headurl + "dto.wx_unionid:  " + dto.wx_unionid);
            result = wdal.SetWxUserInfo(dto);
            return result;
        }

    }
}
