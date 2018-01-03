// JavaScript Document
;(function (doc, win) {
	var max = 750,
	docEl = doc.documentElement,
	resizeEvt = 'orientationchange' in window ? 'orientationchange' : 'resize',
	recalc = function () {
		var clientWidth = docEl.clientWidth;
		var clientHeight = docEl.clientHeight;
		if (!clientWidth) return;
		if (clientWidth >= max) {
			docEl.style.fontSize = '100px';
		} else {
			docEl.style.fontSize = 100 * clientWidth / max + 'px';
		}
		/*if ((navigator.userAgent.match(/(phone|pad|pod|iPhone|iPod|ios|iPad|Android|Mobile|BlackBerry|IEMobile|MQQBrowser|JUC|Fennec|wOSBrowser|BrowserNG|WebOS|Symbian|Windows Phone)/i))) {
			if(clientWidth > clientHeight){alert("竖屏浏览视觉效果更佳！");}
		}*/
	};
	if (!doc.addEventListener) return;
	win.addEventListener(resizeEvt, recalc, false);
	doc.addEventListener('DOMContentLoaded', recalc, false);
})(document, window);