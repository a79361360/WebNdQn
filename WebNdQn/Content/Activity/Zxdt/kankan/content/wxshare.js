
function getTimeStamp(){
	        var timestamp=new Date().getTime();
	        var timestampstring = timestamp.toString();//一定要转换字符串
	        time_stamp=timestampstring;
	        return timestampstring;
}
// 获取随机串
function getNonceStr(){
	        //var $chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
	        var $chars = 'abcdefghijklmnopqrstuvwxyz0123456789';
	        var maxPos = $chars.length;
	        var noceStr = "";
	        for (i = 0; i < 32; i++) {
	            noceStr += $chars.charAt(Math.floor(Math.random() * maxPos));
	        }
	        nonce_str=noceStr;
	        return noceStr;
}

function setVisi(activityId,openId,remoteIp){
	var rss= "fromUrl";
	var resource= "vote";
	var types = "0"
	if(rss == null){
		rss = "fromUrl"
	}
	if(resource == null){
		resource = "vote"
	}
	if(types == null){
		types = "0"
	}
	getVisit(activityId,types,resource,rss,openId,remoteIp);
}
	    
function GetWxSdk() {
    var appid = $("#appId").val();                  //appid
    var timestamp = $("#timestamp").val();          //时间戳
    var noncestr = $("#noncestr").val();            //串
    var signatrue = $("#signatrue").val();          //
    var wx_title = $("#shareTitl").val();           //标题
    var wx_desc = $("#shareContent").val();         //描述
    var wx_shareUrl = $("#shareUrl").val();         //分享LINK
    var shareImgPath = $("#shareImgPath").val();    //小图标地址
    wx.config({
        debug: false,
        appId: appid,
        timestamp: timestamp,
        nonceStr: noncestr,
        signature: signatrue,
        jsApiList: ['onMenuShareAppMessage','onMenuShareTimeline']
    });
    wx.ready(function () {
        // 2. 分享接口
        // 2.1 监听“分享给朋友”，按钮点击、自定义分享内容及分享结果接口
        wx.onMenuShareAppMessage({
            title: wx_title,
            desc: wx_desc,
            link: wx_shareUrl,
            imgUrl: shareImgPath,
            trigger: function (res) {
                // 不要尝试在trigger中使用ajax异步请求修改本次分享的内容，因为客户端分享操作是一个同步操作，这时候使用ajax的回包会还没有返回
                //alert('用户点击发送给朋友');
            },
            success: function (res) {
                activityshare(1);   //调用分享后添加摇奖次数的方法
                //alert('已分享');
            },
            cancel: function (res) {
                //alert('已取消');
            },
            fail: function (res) {
                alert(JSON.stringify(res));
            }
        });
        // 2.2 监听“分享到朋友圈”按钮点击、自定义分享内容及分享结果接口
        wx.onMenuShareTimeline({
            title: wx_title,
            link: wx_shareUrl,
            imgUrl: shareImgPath,
            trigger: function (res) {
                // 不要尝试在trigger中使用ajax异步请求修改本次分享的内容，因为客户端分享操作是一个同步操作，这时候使用ajax的回包会还没有返回
                //alert('用户点击分享到朋友圈');
            },
            success: function (res) {
                activityshare(2);   //调用分享后添加摇奖次数的方法
                //alert('已分享');
            },
            cancel: function (res) {
                //alert('已取消');
            },
            fail: function (res) {
                alert(JSON.stringify(res));
            }
        });
    });
}

function getSdk1(appId,fx_title,fx_desc,fx_link,fx_imgUrl,isNextPage,isNextPageUrl,activityId,openId,remoteIp){
			if(fx_link == null || fx_link == undefined || fx_link == '' || fx_link.lengh <=0){
				
				fx_link = location.href
			}
			if(fx_link.indexOf('&code') !=-1){
				fx_link = fx_link.split("&code")[0];
			}
			console.log(fx_link);
			var url=location.href;
			var noncestr=getNonceStr();
			var timestamp=getTimeStamp();
			console.log(document.domain);
			//var openid=$("#openid").val();
				$.ajax({
				
                 url:"/zjg-wechat/front/flowPay/ticket.jhtml",
					//url:"../flowPay/ticket.jhtml",
                   dataType: 'json',
                   success:function(data){
//                     console.log(data.data.ticket);
                     var sign_str=
						"jsapi_ticket="+data.data.ticket+
						"&noncestr="+noncestr+
						"&timestamp="+timestamp+
						"&url="+url;
						//签名
			             var signature=CryptoJS.SHA1(sign_str).toString();
					     wx.config({
						    debug: false,
						    appId: appId,
						    timestamp:timestamp ,
						    nonceStr: noncestr,
						    signature: signature,
						    jsApiList: ['onMenuShareAppMessage','onMenuShareTimeline']
						});
						wx.ready(function () {
//							  var fx_title="牵手漫步宅中乡，不负春光不负卿";
//							  var fx_desc="牵手漫步宅中乡，不负春光不负卿，快来分享领流量吧！";
//							  var fx_link="<%=basePath%>/weixin/flowReceive/goZzc.do";
//							  var fx_imgUrl="<%=basePath%>/activitytpl/ninde/zzc/images/banner.jpg";
							  wx.onMenuShareAppMessage({
								title:fx_title,
								desc:fx_desc,
								link:fx_link,
						        imgUrl:fx_imgUrl,
						       
					            success: function () {
					            	alert("分享成功");
					            	//保存分享记录
					            	console.log(activityId);
					            	console.log(openId);
					               	console.log(remoteIp);
					            	setVisi(activityId,openId,remoteIp);
					               if( isNextPage =='1'){
					            	   if(isNextPageUrl.indexOf("?") == -1){
						            	   window.location.href=isNextPageUrl;
						               }else{
						            	   window.location.href=isNextPageUrl;
						               }
					               }
					            },
					            cancel: function () {
					                
					            }
					   		 });
							wx.onMenuShareTimeline({
								title:fx_title,
								link:fx_link, // 分享链接					
								imgUrl:fx_imgUrl, // 分享图标
						        success: function () {
						            alert("分享成功");
						          //保存分享记录
						            console.log(openId);
						           	console.log(remoteIp);
						           	setVisi(activityId,openId,remoteIp);
						              //window.location.href="<%=path%>/activitytpl/ninde/zzc/page_02.jsp?openid=${openid}";
						            if( isNextPage =='1'){
							            if(isNextPageUrl.indexOf("?") == -1){
							            	   window.location.href=isNextPageUrl;
							               }else{
							            	   window.location.href=isNextPageUrl;
							               }
						            }
						        },
							    cancel: function () { 
							        // 用户取消分享后执行的回调函数
							    }
					     });
						});
                   }
                })
	    }