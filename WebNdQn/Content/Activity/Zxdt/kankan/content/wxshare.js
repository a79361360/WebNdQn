
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
	    


function getSdk(appId,fx_title,fx_desc,fx_link,fx_imgUrl,isNextPage,isNextPageUrl,activityId,openId,remoteIp){
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