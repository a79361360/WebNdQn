// JavaScript Document
;(function (doc, win) {
	var max = 640,
	docEl = doc.documentElement,
	resizeEvt = 'orientationchange' in window ? 'orientationchange' : 'resize',
	fsize;
	recalc = function () {
		var clientWidth = docEl.clientWidth;
		var clientHeight = docEl.clientHeight;
		if (!clientWidth) return;
		if (clientWidth >= max) {
			fsize = 100;
			docEl.style.fontSize = '100px';
		} else {
			fsize = 100 * clientWidth / max;
			docEl.style.fontSize = 100 * clientWidth / max + 'px';
		}
		var wrap=document.getElementsByClassName('wrap');
		var wh=fsize*10.72;
		if(wh<clientHeight){wh=clientHeight}
		document.getElementById("wrap").style.height=wh-1+"px";
	};
	if (!doc.addEventListener) return;
	win.addEventListener(resizeEvt, recalc, false);
	doc.addEventListener('DOMContentLoaded', recalc, false);
})(document, window);

/*var maxw=640,cw=document.body.clientWidth,ch=window.innerHeight,fsize;
var wrap=document.getElementsByClassName('wrap');
if(cw>=maxw){fsize=100}
else{fsize=100*cw/maxw}
var wh=fsize*10.72;
if(wh<ch){wh=ch}
document.getElementById("wrap").style.height=wh-1+"px";
*/