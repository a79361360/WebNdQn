/** 
 * vlstat 浏览器统计脚本 
 */  

var statIdName = "vlstatId";  
var xmlHttp;
var cName="liuliangjimu";
/*window.onload=function(){
	
	setCookie(cName, cName, "123");
	getCookie(cName);
}*/
/** 
 * 设置cookieId 
 */  
function setCookie(c_name, value, expiredays) {  
    var exdate = new Date();  
    exdate.setDate(exdate.getDate() + expiredays); 
//    console.log("document.cookie="+document.cookie);
    if(getCookie(c_name)==null ||getCookie(c_name)==''){
    var value=value+exdate.getTime();
    console.log("value:"+value);
    var cookieValue = c_name + "=" + escape(value) + ((expiredays == null) ? "" : ";expires=" + exdate.toGMTString()) + ";path=/;domain=liuliangjimu.com";
    document.cookie = cookieValue;
    console.log("document.cookie:"+document.cookie);
//    domain=www.liuliangjimu.com
//    domain=localhost
    }
//    alert(document.cookie);
}  
  
/** 
 * 获取cookieId 
 */  
function getCookie(c_name) {  
    if (document.cookie.length > 0) {  
        c_start = document.cookie.indexOf(c_name + "=");  
        console.log(document.cookie);
        console.log(c_name + "=");
        if (c_start != -1) {  
            c_start = c_start + c_name.length + 1;  
            c_end = document.cookie.indexOf(";", c_start);  
            if (c_end == -1) {  
                c_end = document.cookie.length;  
            }  
            return unescape(document.cookie.substring(c_start, c_end));  
        }  
    }
    return "";  
}  
  
/** 
 * 获取当前时间戳 
 */  
function getTimestamp() {  
    var timestamp = Date.parse(new Date());  
    return timestamp;  
}  
  
/** 
 * 生成statId 
 */  
function genStatId() {  
    var cookieId = getTimestamp();  
    cookieId = "vlstat" + "-" + cookieId + "-" + Math.round(Math.random() * 3000000000); 
    return cookieId;  
}  
  
/** 
 * 设置StatId 
 */  
function setStatId() {  
    var cookieId = genStatId();  
    setCookie(statIdName, cookieId, 365);  
}  
  
/** 
 * 获取StatId 
 */  
function getStatId() {  
    var statId = getCookie(statIdName);  
    if (statId != null && statId.length > 0) {  
        return statId;  
    } else {  
        setStatId();  
        return getStatId();  
    }  
}  
  
/** 
 * 用户浏览器内核
 * 获取UA 
 */  
function getUA() {  
    var ua = navigator.userAgent;  
    if (ua.length > 250) {  
        ua = ua.substring(0, 250);  
    }  
    return ua;  
}  
  
/** 
 * 获取浏览器类型 
 */  
function getBrower() {  
    var ua = getUA();  
    if (ua.indexOf("Maxthon") != -1) {  
        return "Maxthon";  
    } else if (ua.indexOf("MSIE") != -1) {  
        return "MSIE";  
    } else if (ua.indexOf("Firefox") != -1) {  
        return "Firefox";  
    } else if (ua.indexOf("Chrome") != -1) {  
        return "Chrome";  
    } else if (ua.indexOf("Opera") != -1) {  
        return "Opera";  
    } else if (ua.indexOf("Safari") != -1) {  
        return "Safari";  
    } else {  
        return "ot";  
    }  
}  
  
/** 
 * 获取浏览器语言 
 */  
function getBrowerLanguage() {  
    var lang = navigator.browserLanguage;  
    return lang != null && lang.length > 0 ? lang : "";  
}  
  
/** 
 * 获取操作系统 
 */  
function getPlatform() {  
    return navigator.platform;  
}  
  
/** 
 * 获取页面title 
 */  
function getPageTitle() {  
    return document.title;  
}  

/**
 * 
 * @param activityId
 * @param type
 * @param resource
 */
function getVisit(vid,type,resource,rss,openId,remoteIp) {
    //用户入口
    var remoteAddr = encodeURIComponent(document.URL);
    //  用户浏览器内核
    var userAgent = encodeURIComponent(getUA());
    //  用户浏览器
    var userBrower = getBrower();
//    var resource = encodeURIComponent(document.referrer);
//    var cookie = "测试";
    
    console.log("remoteAddr==="+remoteAddr);
    console.log("userAgent==="+userAgent);
    console.log("resource===="+resource);
    console.log("vid==="+vid);
//    getCookie(cName);
    var cookie=encodeURIComponent(getCookie(cName));
    console.log("cookies==="+getCookie(cName));
    
	var myForm = {"remoteAddr":remoteAddr,"userAgent":userAgent,"userBrower":userBrower,"vid":vid,"type":type,"resource":resource,"rss":rss,"cookie":cookie,"openId":openId,"remoteIp":remoteIp};
	$.ajax({
		url : "http://www.liuliangjimu.com:8084/zjg-h5/front/visit/visitSave.jhtml",
//		url : "http://test.liuliangjimu.com/zjg-h5/front/visit/visitSave.jhtml",
//		url : "http://localhost:8081/zjg-h5/front/visit/visitSave.jhtml",
//		url : "http://test.liuliangjimu.com/zjg-wechat/front/visit/visitSave.jhtml",
		dataType : "json",
		type : "POST",
		async : false,
		data : myForm,
		success : function(data) {
			var code = data.code;
			if (code != null && code != "" && code != "undefined" && code == "1") {// OK
//				alert("ss");
				//location.reload();
				return ;
			}
		}
	});
}