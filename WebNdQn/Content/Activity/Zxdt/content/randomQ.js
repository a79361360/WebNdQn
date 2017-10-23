(function(){
	function h(){
		for(var g=parseInt(Math.random()*f),c=0;c<a.length;c++)
			if(a[c]==g)
				return;a.push(g)
	}
	var b=$(".page");$(".dialog");
	var f=b.length,a=[],d=void 0,e=0;
	for(a.push(parseInt(Math.random()*f));5>a.length;)
		
	h();
	b.addClass("none").eq(a[0]).removeClass("none");
	b.find("li").click(function(){
		var a=$(this),c=a.parent().find(".icon-right"),b=a.parent().find(".icon-wrong"),f=a.parent().find(".zqda"),e=a.parent().find(".zqda i");
		a.hasClass("right")?(f.hide(),c.removeClass("none"),b.addClass("none"),d=!0):(e.text($(this).attr('data-value')),f.show(),c.addClass("none"),b.removeClass("none"),d=!1)
	});
	b.find(".btn-xyt").click(function(){
		if(void 0!=d){
			if (d) {
                console.log(d)
				$(".titleEm").text((e+2)+"、");
				if(e==a.length-1){
						
					$("#score").val(1+Number($("#score").val()));
					//最后一题
					return $("#tel-dialog,#dialog-bg").removeClass("dialog-none");
				}
				e++;
					
				b.addClass("none").eq(a[e]).removeClass("none");
				if(e==4){
					b.find(".btn-xyt").addClass("btn-tj");
				}
				$("#score").val(1+Number($("#score").val()));
				return d=void 0
			}
		return alert("答对才能进行下一题哦!")
		}
		alert("完成本题后,才能继续!");
		}
	)})();
