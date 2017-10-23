
//弹窗 
function showpop(id, id_bg) {
    document.getElementById(id).style.display = 'block';
    document.getElementById(id_bg).style.display = 'block';
}
function closepop(id, id_bg) {
    document.getElementById(id).style.display = 'none';
    document.getElementById(id_bg).style.display = 'none';
}
//弹窗 
//function showpop(id, id_bg) {
//    document.getElementById(id).style.display = 'block';
//    document.getElementById(id_bg).style.display = 'block';
//}
//function closepop(id, id_bg) {
//    document.getElementById(id).style.display = 'none';
//    document.getElementById(id_bg).style.display = 'none';
//}
$(document).ready(function(){
	//引入分享
	//var appId = $("#appId").val();
	//var shareTitl = $("#shareTitl").val();
	//var shareImgPath = $("#shareImgPath").val();
	//var shareContent = $("#shareContent").val();
	//var shareUrl = $("#shareUrl").val();
	//var appId = appId;
	//var fx_title = shareTitl;
	//var fx_desc = shareContent;
	//var fx_link = shareUrl;
	//var fx_imgUrl = shareImgPath;
	//getSdk1(appId,fx_title,fx_desc,fx_link,fx_imgUrl,"0","");
	//var openId = $("#openId").val();
	//if(openId == null || openId == "" || openId == "undefined" ){
	//	$("#message").text("不要刷新,请重新授权后进入");
	//	showpop('finish-dialog','dialog-bg');
	//}
});
//保存验证手机号
function savePhone(){
	var phone=$("#phone_input").val();
	var id=$("#id").val();
	if(phone.length!=11){
		alert("请输入正确的手机号！");
		return ;
	}
	if(!phonecheck(phone)){
		alert("请输入正确的手机号");
		return;
	}
	$("#receive").removeAttr("onclick");
	$.ajax({
		url:"savePhone.jhtml",
		data:{"id":id,"phone":phone},
		success:function(data){
			var obj=JSON.parse(data);
			var code = obj.code;
			var msg = obj.msg;
			if (code != null && code != "" && code != "undefined" && code == "1") {
				// OK
				closepop('tel-dialog','dialog-bg');
				$("#phone").val(phone);
				receive();
			} else {
				alert(msg);
				return;
			}
		}	
	})
}
//正则表达式
function phonecheck(str){
	var x = /^1[3|4|5|8][0-9]\d{4,8}$/;
	return x.test(str);
}
//验证是否已经答过题，并且验证本次是否可以答题以及今天是否可以答题
function checkMobile(){
	var joinNumber=$("#joinNumber").val();
	var joinNumberDay=$("#joinNumberDay").val();
	var receiveNumber=$("#receiveNumber").val();
	var notStart=$("#notStart").val();
	if($("#phone").val()==""){
		showpop('tel-dialog','dialog-bg');
	}else{
		closepop('tel-dialog','dialog-bg');
		if(notStart!=null && notStart!="undefined" && notStart!="" && notStart=="1"){
			showpop('notStar-dialog','dialog-bg');
		}else{
			if(joinNumber==null||joinNumber=="undefined"||joinNumber==""){
				//已经达到领取上限
				showpop('lllqw-dialog','dialog-bg');
			}else if(receiveNumber==null||receiveNumber=="undefined"||receiveNumber==""){
				//已经达到领取上限
				if($("#id").val()!="299"){
					showpop('lllqw-dialog','dialog-bg');
				}
			}else if(joinNumber=='1'&&joinNumberDay=='0'){
				//今天答过题了
				showpop('noChanceToDay-dialog','dialog-bg');
			}
		}
	}
};
//验证完手机以后调用
function receive(){
	var phone=$("#phone").val();
	var score=$("#score").val();
	var id=$("#id").val();
	$("#receive").removeAttr("onclick");
 	$.ajax({
		url:"receive.jhtml",
		data:{"phone":phone,"score":score,"id":id},
		success:function(data){
			console.log(data);
			var obj=JSON.parse(data);
			var code = obj.code;
			var msg=obj.msg;
			if (code != null && code != "" && code != "undefined" && code == "4") {
				// OK
				closepop('tel-dialog','dialog-bg');
				var flowSize=obj.data;
				$("#flowSize").text(flowSize+"M");
				showpop('dialog-success','dialog-bg');
			}else if (code != null && code != "" && code != "undefined" && code == "0") {
				closepop('tel-dialog','dialog-bg');
				$("#message").text(msg);
				showpop('finish-dialog','dialog-bg');
			}else if (code != null && code != "" && code != "undefined" && code == "1") {
				closepop('tel-dialog','dialog-bg');
				$("#message").text(msg);
				showpop('finish-dialog','dialog-bg');
			}else if (code != null && code != "" && code != "undefined" && code == "2") {
				closepop('tel-dialog','dialog-bg');
				showpop('lllqw-dialog','dialog-bg');
			}else if (code != null && code != "" && code != "undefined" && code == "3") {
				closepop('tel-dialog','dialog-bg');
				showpop('noChanceToDay-dialog','dialog-bg');
			}else if (code != null && code != "" && code != "undefined" && code == "5") {
				/*$("#message").text("很遗憾!没答对足够题数");*/
				closepop('tel-dialog','dialog-bg');
				showpop('dialog-fail','dialog-bg');
			}else if (code != null && code != "" && code != "undefined" && code == "6") {
				closepop('tel-dialog','dialog-bg');
				$("#message").text("授权失败，请重新进入");
				showpop('finish-dialog','dialog-bg');
			}else if (code != null && code != "" && code != "undefined" && code == "7") {
				closepop('tel-dialog','dialog-bg');
				$("#message").text("流量赠送已结束，如果您觉得内容不错，请动动手指转发吧！");
				showpop('finish-dialog','dialog-bg');
			}
		}
	})
}

//分享微信封装
//function getSdk1(appId,fx_title,fx_desc,fx_link,fx_imgUrl,isNextPage,isNextPageUrl,activityId,type){
//	if(fx_link == null || fx_link == undefined || fx_link == '' || fx_link.lengh <=0){
//		fx_link = location.href
//	}
//	console.log(fx_link);
//	var url=location.href;
//	var noncestr=getNonceStr();
//	var timestamp=getTimeStamp();
//	//var openid=$("#openid").val();
//		$.ajax({
		
//         url:"/zjg-wechat/front/flowPay/ticket.jhtml",
//			//url:"../flowPay/ticket.jhtml",
//           dataType: 'json',
//           success:function(data){
////             console.log(data.data.ticket);
//             var sign_str=
//				"jsapi_ticket="+data.data.ticket+
//				"&noncestr="+noncestr+
//				"&timestamp="+timestamp+
//				"&url="+url;
//				//签名
//	             var signature=CryptoJS.SHA1(sign_str).toString();
//			     wx.config({
//				    debug: false,
//				    appId: appId,
//				    timestamp:timestamp ,
//				    nonceStr: noncestr,
//				    signature: signature,
//				    jsApiList: ['onMenuShareAppMessage','onMenuShareTimeline']
//				});
//				wx.ready(function () {
//					  wx.onMenuShareAppMessage({
//						title:fx_title,
//						desc:fx_desc,
//						link:fx_link,
//				        imgUrl:fx_imgUrl,
				       
//			            success: function () {
//			            	alert("分享成功");
				            
//			            },
//			            cancel: function () {
			                
//			            }
//			   		 });
//					wx.onMenuShareTimeline({
//						title:fx_title,
//						link:fx_link, // 分享链接					
//						imgUrl:fx_imgUrl, // 分享图标
//				        success: function () {
//				            alert("分享成功");
				           
//				        },
//					    cancel: function () { 
//					        // 用户取消分享后执行的回调函数
//					    }
//			     });
//				});
//           }
//        })
//}

