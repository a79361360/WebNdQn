//弹窗 
function showpop(id,id_bg) {
	document.getElementById(id).style.display = 'block';
	document.getElementById(id_bg).style.display = 'block';       
}
function closepop(id,id_bg) {
	document.getElementById(id).style.display = 'none';
	document.getElementById(id_bg).style.display = 'none';
}

$(document).ready(function(){
	//引入分享
	//var appId = $("#appId").val();
	//var shareTitl = $("#shareTitl").val();
	//var shareImgPath = $("#shareImgPath").val();
	//var shareContent = $("#shareContent").val();
	//var shareUrl = $("#shareUrl").val();
	//var openId = $("#openId").val();
	//var remoteIp = $("#remoteIp").val();
	//var appId = appId;
	//var fx_title = shareTitl;
	//var fx_desc = shareContent;
	//var fx_link = shareUrl;
	//var fx_imgUrl = shareImgPath;
    //getSdk(appId,fx_title,fx_desc,fx_link,fx_imgUrl,"0","",openId,remoteIp);
    GetWxSdk();
	//if(openId == null || openId == "" || openId == "undefined" ){
	//	$("#message").text("不要刷新,请重新授权后进入");
	//	showpop('common-dialog','dialog-bg');
	//}else{
	//	checkMobile();
	//}
});

function checkMobile() {
	var activityUnStart=$("#activityUnStart").val();            //0不能开始
	var activityUnStartHint=$("#activityUnStartHint").val();    //不能开始的提示
	var activityEnd=$("#activityEnd").val();                    //0活动结束
	var activityEndHint=$("#activityEndHint").val();            //结束的提示
	var toDayIsUnStart=$("#toDayIsUnStart").val();              //0今天活动不能开始
	var todayUnStartHint=$("#todayUnStartHint").val();          //不能开始的原因
	var toDayIsEnd=$("#toDayIsEnd").val();                      //0今天活动已经结束
	var personEnd = $("#lotteyn").val();                        //0参与次数不够
	var personNoChance=$("#personNoChance").val();              //提示内容
	var personDayEnd = $("#personDayEnd").val();                //0今天的参与次数不够
	var openid = $("#openId").val();

	if (openId == null || openId == "" || openId == "undefined") {
	    $("#message").text("微信授权失败,请重新点击链接授权后进入");
	    showpop('common-dialog', 'dialog-bg');
	    return false;
	}
	if(activityUnStart=="0"){
		$("#message").text(activityUnStartHint);
		showpop('common-dialog','dialog-bg');
		return false;
	}
	if(activityEnd=="0"){
		$("#message").text(activityEndHint);
		showpop('common-dialog','dialog-bg');
		return false;
	}
	if(toDayIsUnStart=="0"){
		$("#message").text(todayUnStartHint);
		showpop('common-dialog','dialog-bg');
		return false;
	}
	if(toDayIsEnd=="0"){
		$("#message").text("今天活动已经结束了!请明天再来!");
		showpop('common-dialog','dialog-bg');
		return false;
	}
	if (personEnd == "0") {
	    $("#message").text(personNoChance);
	    showpop('common-dialog', 'dialog-bg');
	    return false;
	}
	if (personDayEnd == "0") {
	    $("#message").text("您今天的参与次数已经用完了!感谢您的参与!");
	    showpop('common-dialog', 'dialog-bg');
	    return false;
	}
	if($("#phone").val()==""){
	    showpop('tel-dialog', 'dialog-bg');
	    return false;
	}
	return true;
}
//区域验证
areaphone = function (phone,area) {
    var result = false;
    $.ajax({
        type: "get",
        url: "/Activity/SignPhoneArea",
        data: { phone: phone, area: area },
        async: false,
        success: function (ret) {
            if (ret.success) {
                result = ret.jsonresult;
            }
        }
    });
    return result;
}
//返回区域名称
areaname = function (v) {
    if (v == "1") return "宁德";
    else if (v == "2") return "莆田";
    else if (v == "3") return "福建";
    else return "宁德";
}
//保存验证手机号
function savePhone(){
    var phone = $("#phone_input").val();
    var area = $("#area").val();
	if(phone.length!=11){
		alert("请输入正确的手机号！");
		return ;
	}
	if(!phonecheck(phone)){
		alert("请输入正确的手机号");
		return;
	}
	var b = areaphone(phone,area);
	if (!b) {
	    var areastr = areaname(area);     //区域名称
	    alert("请输入" + areastr + "移动号码");
	    return;
	}
	$("#phone").val(phone);
	closepop('tel-dialog', 'dialog-bg');
	//$.ajax({
	//	url:"savePhone.jhtml",
	//	data:{"id":id,"phone":phone},
	//	success:function(data){
	//		var obj=JSON.parse(data);
	//		var code = obj.code;
	//		var msg = obj.msg;
	//		if (code != null && code != "" && code != "undefined" && code == "1") {
	//			// OK
	//			closepop('tel-dialog','dialog-bg');
	//			$("#phone").val(phone);
	//			alert(msg);
	//		} else {
	//			alert(msg);
	//			return;
	//		}
	//	}	
	//})
}
//正则表达式
function phonecheck(str){
	var x = /^1[3|4|5|8][0-9]\d{4,8}$/;
	return x.test(str);
}
function myrefresh(){
    window.location.reload();
}
function receive(){
	var phone=$("#phone").val();
	var score = $("#score").val();
	var tmfs = $("#tmfs").val();
	var area = $("#area").val();
    console.log(tmfs)
	var isRecordResult=$("#isRecordResult").val();
	var detailContent="";
	if(isRecordResult=="0"){
		detailContent=$("#detailContent").val();
	}
	$(".btn-tj").css("pointer-events", "none");
	var cooperid = $("#cooperid").val();
	var openid = $("#openId").val();
	var data = {
	    cooperid: cooperid,
	    openid: openid,
	    phone: phone,
	    area: area,
	    score: Number(score) * tmfs
	}
	console.log(data);
    //15859326943
	$.ajax({
	    type: 'POST',
	    url: '/Zxdt/SubmitZxdt',
	    data: data,
	    complete: function (ret) {
	        $("#qbdd-dialog").attr("onclick", "closepop('qbdd-dialog','dialog-bg');");
	        $(".one").css("display", "block"); $(".main").css("display", "none");
	        showpop('jzjl-dialog', 'dialog-bg');
	    }
	});
 	//$.ajax({
	//	url:"receive.jhtml",
	//	scriptCharset: 'utf-8',
	//	contentType: "application/x-www-form-urlencoded; charset=utf-8",
	//	data:{"phone":phone,"score":score,"id":id,"content":detailContent},
	//	success:function(data){
	//		console.log(data);
	//		var obj=JSON.parse(data);
	//		var code = obj.code;
	//		var msg=obj.msg;
	//		if (code != null && code != "" && code != "undefined" && code == "1") {
	//			var flowSize=obj.data;
	//			$("#flowSize").text(flowSize+"M");
	//			showpop('qbdd-dialog','dialog-bg');
	//		}else if (code != null && code != "" && code != "undefined" && code == "0") {
	//			$("#message").text(msg);
	//			showpop('common-dialog','dialog-bg');
	//		}else if (code != null && code != "" && code != "undefined" && code == "2") {
	//			$("#detailCloseBtn").attr("onclick","closepop('jzjl-dialog','dialog-bg');showpop('qbdc-dialog','dialog-bg');");
	//			showpop('qbdc-dialog','dialog-bg');
	//		}else if (code != null && code != "" && code != "undefined" && code == "3") {
	//			if(obj.data != null && obj.data != "" && obj.data != "undefined"){
	//				window.location.href=obj.data;
	//			}
				
	//		}
	//	}
	//})
}