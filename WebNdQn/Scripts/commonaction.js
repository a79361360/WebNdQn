//千位符,6位加万
function cc(num) {
    var num = (num || 0).toString(), result = '';
    if (num.length > 7) {
        result = num.slice(0, -4);
        result = result.substr(0, result.length - 3) + "," + result.substr(result.length - 3, 3) + "万";
        return result;
    }
    while (num.length > 3) {
        result = ',' + num.slice(-3) + result;
        num = num.slice(0, num.length - 3);
    }
    if (num) { result = num + result; }
    return result
}
//向服务器发送消息
SendSocket = function (url, type, content) {
    if (url == undefined || url == "" || url == null) {
        url = "/Handler/Member.ashx";
    }
    if (type == undefined || type == "" || type == null) {
        return;
    }
    $.post(url, { action: "sendsocket", type: type, content: content }, function (ret) { });
}
//当前对象是否为空
ObjEmpty = function (obj) {
    if (obj) return true; else return false;
}
//显示弹窗
function showiframe(obj, url) {
    if (!ObjEmpty($("#iframeindex"))) return;
    $("#iframeindex").css('display', 'inline');
    setframeurl(url);
}
//关闭弹窗
function hideiframe() {
    if (!ObjEmpty($("#iframeindex"))) return;
    $("#iframeindex").hide();
}
//弹窗显示的页面
function setframeurl(url) {
    if (!ObjEmpty($("#iframeindex"))) return;
    SendSocket(null, 102, "");//关闭客户端弹窗
    $("#iframeindex").attr("src", url);
}
//显示弹窗(专门用于任务完成后跳转回去)
showwindow = function () {
    if (window.parent != window) {
        window.parent.setframeurl("MembersCenter/Myjobs.aspx");
    } else {
        $.post("/Handler/Member.ashx", { action: "sendsocket", type: 112, content: "" }, function (ret) { });
    }
}
//关闭弹窗
closeiframe = function () {
    if (window.parent != window) {
        window.parent.hideiframe();
    } else {
        $.post("/Handler/Member.ashx", { action: "sendsocket", type: 102, content: "" }, function (ret) { });
    }
}
//取得字符char长度,中文算2,数字算1
function getStrLength(pStr) {
    // 原字符串长度  
    var _strLen = pStr.length;
    var _lenCount = 0;
    for (var i = 0; i < _strLen; i++) {
        if (isChar(pStr.charAt(i))) {
            _lenCount += 2;
        } else {
            _lenCount += 1;
        }
    }
    return _lenCount;
}
/* 
* 处理过长的字符串，截取并添加省略号 
* 注：半角长度为1，全角长度为2 
*  
* pStr:字符串 
* pLen:截取长度 
*  
* return: 截取后的字符串 
*/
function autoAddEllipsis(pStr, pLen) {
    if (pStr == null) { pStr = ""; }
    var _ret = cutString(pStr, pLen);
    var _cutFlag = _ret.cutflag;
    var _cutStringn = _ret.cutstring;

    if ("1" == _cutFlag) {
        return _cutStringn + "...";
    } else {
        return _cutStringn;
    }
} 
/* 
* 判断是否为全角 
*  
* pChar:长度为1的字符串 
* return: true:全角 
*          false:半角 
*/
function isChar(pChar) {
    if ((pChar.charCodeAt(0) > 128)) {
        return true;
    } else {
        return false;
    }
}
/* 
* 取得指定长度的字符串 
* 注：半角长度为1，全角长度为2 
*  
* pStr:字符串 
* pLen:截取长度 
*  
* return: 截取后的字符串 
*/
function cutString(pStr, pLen) {

    // 原字符串长度  
    var _strLen = pStr.length;

    var _tmpCode;

    var _cutString;

    // 默认情况下，返回的字符串是原字符串的一部分  
    var _cutFlag = "1";

    var _lenCount = 0;

    var _ret = false;

    if (_strLen <= pLen / 2) {
        _cutString = pStr;
        _ret = true;
    }

    if (!_ret) {
        for (var i = 0; i < _strLen; i++) {
            if (isChar(pStr.charAt(i))) {
                _lenCount += 2;
            } else {
                _lenCount += 1;
            }
            if (_lenCount > pLen) {
                _cutString = pStr.substring(0, i);
                _ret = true;
                break;
            } else if (_lenCount == pLen) {
                _cutString = pStr.substring(0, i + 1);
                _ret = true;
                break;
            }
        }
    }
    if (!_ret) {
        _cutString = pStr;
        _ret = true;
    }
    if (_cutString.length == _strLen) {
        _cutFlag = "0";
    }
    return { "cutstring": _cutString, "cutflag": _cutFlag };
}  
    //设置默认当前时间
    setinputdate = function (ids) {
        var d = new Date();
        $.each(ids, function (i, k) {
            function addzero(v) { if (v < 10) return '0' + v; return v.toString(); }
            var s = d.getFullYear().toString() + "-" + addzero(d.getMonth() + 1) + "-" + addzero(d.getDate());
            $("#" + k).val(s);
        });
    }
    //设置tr行的变色
    trchcolor = function (strobj) {
        $(strobj).mouseover(function () {
            $(this).css("background-color", "yellow");
        });
        $(strobj).mouseout(function () {
            $(this).css("background-color", "");
        }); 
    }
    //jquery验证手机号码 
    function checkSubmitMobil(obj) {
        if ($(obj).val() == "") {
            alert("手机号码不能为空！");
            $(obj).focus();
            return false;
        }

        if (!$(obj).val().match(/^1(3|4|5|7|8)\d{9}$/)) {
            alert("手机号码格式不正确！");
            $(obj).focus();
            return false;
        }
        return true;
    }
//返回区域名称
areaname = function (v) {
    if (v == "1") return "宁德";
    else if (v == "2") return "莆田";
    else return "宁德";
}