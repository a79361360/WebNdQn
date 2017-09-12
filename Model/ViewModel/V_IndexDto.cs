using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class V_IndexDto
    {
        /// <summary>
        /// 关注公众号的二维码图片地址
        /// </summary>
        public string qrcodeurl { get; set; }
        /// <summary>
        /// 是否关注1为已经关注,这个后面可能还是需要加密一下
        /// </summary>
        public string gz { get; set; }
        /// <summary>
        /// 用户的微信Openid
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 背景图片的url
        /// </summary>
        public string bgurl { get; set; }
        /// <summary>
        /// 图片的按钮url
        /// </summary>
        public string btnurl { get; set; }
        /// <summary>
        /// 推广类型 分享推广|关注推广
        /// </summary>
        public string genner { get; set; }
        /// <summary>
        /// 地区1宁德地区2莆田地区
        /// </summary>
        public int areatype { get; set; }
        /// <summary>
        /// 微信分享API 时间戳
        /// </summary>
        public long timestamp { get; set; }
        /// <summary>
        /// 微信分享API 随机字符串
        /// </summary>
        public string noncestr { get; set; }
        /// <summary>
        /// 微信分享API 验签
        /// </summary>
        public string signatrue { get; set; }
        /// <summary>
        /// 微信分享API 微信公众号的appid
        /// </summary>
        public string wx_appid { get; set; }
        /// <summary>
        /// 微信分享API 显示给分享用户的标题
        /// </summary>
        public string fx_title { get; set; }
        /// <summary>
        /// 微信分享API 显示给分享用户的描述
        /// </summary>
        public string fx_descride { get; set; }
        /// <summary>
        /// 微信分享API 显示给分享用户的小图标
        /// </summary>
        public string fx_imgurl { get; set; }
        /// <summary>
        /// 微信分享API 分享用户点击所跳转的link地址
        /// </summary>
        public string fx_linkurl { get; set; }
    }
}
