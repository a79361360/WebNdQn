(function(){
	$(".title em").html("1");
	var num = 0;
	var random = $("#isRandom").val();
	function h(){
		for(var g=parseInt(Math.random()*f),c=0;c<a.length;c++)
			if(a[c]==g)
				return;a.push(g)
	}
	var showQestionNum=$("#showQestionNum").val();
	var questionListSize=$("#questionListSize").val();
	var isRightGo=$("#isRightGo").val();
	
	if(questionListSize!=null&&questionListSize!=""&&questionListSize!=undefined){
		if(parseInt(showQestionNum)>parseInt(questionListSize)){
			showQestionNum=questionListSize;
		}
	}
	var isShowRightTip=$("#isShowRightTip").val();
	var b=$(".page");$(".dialog");
	var f=b.length,a=[],d=void 0,e=0;
	for(a.push(parseInt(Math.random()*f));showQestionNum>a.length;)
	    h();
	if (random == "0") { b.addClass("none").eq(a[0]).removeClass("none"); } else { b.addClass("none").eq(0).removeClass("none"); }
		b.find("li").click(function(){
		if("2"==isRightGo){
		if(num == 0){	
			var a=$(this),c=a.parent().find(".icon-right"),b=a.parent().find(".icon-wrong"),f=a.parent().find(".zqda"),e=a.parent().find(".zqda i");
			var rightNum=0;
			var anserList="";
			a.parent().find("li").each(function(){
				if($(this).hasClass("right")){
					rightNum+=1;
					anserList+=$(this).attr('data-value')+" ";
				}
				
			});
			if(rightNum>1){
				$(this).toggleClass("active");
			}else{
				$(this).addClass("active").siblings().removeClass("active");
			}
			
			if(rightNum==0){
				
				f.hide();
				if(isShowRightTip=='0'){
					c.removeClass("none");b.addClass("none");
				}
				d=!0;
				num=1;
			}else if(rightNum==1){
				if(a.hasClass("right")){
					f.hide();
					if(isShowRightTip=='0'){
						c.removeClass("none");b.addClass("none");
					}
					d=!0;
				}else{
					if(isShowRightTip=='0'){e.text($(this).attr('data-value'));f.show();c.addClass("none");b.removeClass("none");}
					d=!1;
				}
				if(isShowRightTip=='1'){
					
				}
				num=1;
			}else if(rightNum>1){
				var selectAnswer="";
				a.parent().find("li").each(function(){
					if($(this).hasClass("active")){
						selectAnswer+=$(this).attr('data-value')+" ";
					}
				});
				if(selectAnswer==anserList){
					f.hide();
					if(isShowRightTip=='0'){
						c.removeClass("none");b.addClass("none");
					}
					d=!0;
				}else{
					if(isShowRightTip=='0'){e.text(selectAnswer);f.show();c.addClass("none");b.removeClass("none");}
					d=!1;
				}
				num=0;
			}
			
		}
		}else{
			var a=$(this),c=a.parent().find(".icon-right"),b=a.parent().find(".icon-wrong"),f=a.parent().find(".zqda"),e=a.parent().find(".zqda i");
			var rightNum=0;
			var anserList="";
			a.parent().find("li").each(function(){
				if($(this).hasClass("right")){
					rightNum+=1;
					anserList+=$(this).attr('data-value')+" ";
				}
				
			});
			if(rightNum>1){
				$(this).toggleClass("active");
			}else{
				$(this).addClass("active").siblings().removeClass("active");
			}
			
			if(rightNum==0){
				
				f.hide();
				if(isShowRightTip=='0'){
					c.removeClass("none");b.addClass("none");
				}
				d=!0;
			}else if(rightNum==1){
				if(a.hasClass("right")){
					f.hide();
					if(isShowRightTip=='0'){
						c.removeClass("none");b.addClass("none");
					}
					d=!0;
				}else{
					if(isShowRightTip=='0'){e.text($(this).attr('data-value'));f.show();c.addClass("none");b.removeClass("none");}
					d=!1;
				}
				if(isShowRightTip=='1'){
					
				}
			}else if(rightNum>1){
				var selectAnswer="";
				a.parent().find("li").each(function(){
					if($(this).hasClass("active")){
						selectAnswer+=$(this).attr('data-value')+" ";
					}
				});
				if(selectAnswer==anserList){
					f.hide();
					if(isShowRightTip=='0'){
						c.removeClass("none");b.addClass("none");
					}
					d=!0;
				}else{
					if(isShowRightTip=='0'){e.text(selectAnswer);f.show();c.addClass("none");b.removeClass("none");}
					d=!1;
				}
			}
		}
		});
		b.find(".btn-xyt").click(function(){
			num=0;
			var picture=$("#picture3").val();
			var rightNumEm="";
			var selectListEm="";
			var selectAnwerText="";
			$(this).parent().find("li").each(function(){
				if($(this).hasClass("right")){
					rightNumEm+=$(this).attr('data-value')+" ";
				}
				if($(this).hasClass("active")){
					selectListEm+=$(this).attr('data-value')+" ";
					selectAnwerText+=$(this).text();
				}
				
			});
			
			var titleText=$(this).parent().parent().find(".title").text();
			//console.log(selectAnwerText);
			if(void 0!=d){
				if(d){
			
					if(e==a.length-1){
						$("#score").val(1+Number($("#score").val()));
						$("#rightNumEm").html($("#score").val());
						$("#wrongNumEm").html($("#wrongNum").val());
						$("#tailTable").append("<tr><td>第"+(e+1)+"题</td><td>"+rightNumEm+"</td><td>"+selectListEm+"</td></tr>");
						$("#detailContent").val($("#detailContent").val()+titleText+selectAnwerText);
						console.log($("#detailContent").val());
						return receive();
					}
					e++;
					
					$(".title em").html(e+1);
					if(random=="0"){
						b.addClass("none").eq(a[e]).removeClass("none");
					}else{
						b.addClass("none");
						b.eq(e).removeClass("none");
					}
					//
					if(e==(showQestionNum-1)){
						b.find(".btn-xyt").addClass("btn-tj");
						if(picture!=""&&picture!=undefined){
							b.find(".btn-xyt").css("background-image", 'url("' + picture + '")');
						}
						
					}
					$("#score").val(1+Number($("#score").val()));
					$("#tailTable").append("<tr><td>第"+e+"题</td><td>"+rightNumEm+"</td><td>"+selectListEm+"</td></tr>");
					$("#detailContent").val($("#detailContent").val()+titleText+selectAnwerText);
					return d=void 0
	
				}else{
					//如果开启直接进入下一题（无论对错）
				    if ("2" == isRightGo) {
							if(e==a.length-1){
								//最后一题
								$("#wrongNum").val(1+Number($("#wrongNum").val()));
								$("#wrongNumEm").html($("#wrongNum").val());
								$("#rightNumEm").html($("#score").val());
								$("#tailTable").append("<tr><td>第"+(e+1)+"题</td><td>"+rightNumEm+"</td><td>"+selectListEm+"</td></tr>");
								$("#detailContent").val($("#detailContent").val()+titleText+selectAnwerText);
								console.log($("#detailContent").val());
								return receive();
							}
							e++;
							$(".title em").html(e+1);
							if(random=="0"){
								b.addClass("none").eq(a[e]).removeClass("none");
							}else{
								b.addClass("none");
								b.eq(e).removeClass("none");
							}
							if(e==(showQestionNum-1)){
								b.find(".btn-xyt").addClass("btn-tj");
								if(picture!=""&&picture!=undefined){
									b.find(".btn-xyt").css("background-image", 'url("' + picture + '")');
								}
							}
							$("#wrongNum").val(1+Number($("#wrongNum").val()));
							
							$("#tailTable").append("<tr><td>第"+e+"题</td><td>"+rightNumEm+"</td><td>"+selectListEm+"</td></tr>");
							$("#detailContent").val($("#detailContent").val()+titleText+selectAnwerText);
							return d=void 0
						}
					else{
					if(rightNumEm==selectListEm){
						if(e==a.length-1){
							//最后一题
							$("#wrongNum").val(1+Number($("#wrongNum").val()));
							$("#wrongNumEm").html($("#wrongNum").val());
							$("#rightNumEm").html($("#score").val());
							$("#tailTable").append("<tr><td>第"+(e+1)+"题</td><td>"+rightNumEm+"</td><td>"+selectListEm+"</td></tr>");
							$("#detailContent").val($("#detailContent").val()+titleText+selectAnwerText);
							console.log($("#detailContent").val());
							return receive();
						}
						e++;
						$(".title em").html(e+1);
						if(random=="0"){
							b.addClass("none").eq(a[e]).removeClass("none");
						}else{
							b.addClass("none");
							b.eq(e).removeClass("none");
						}
						if(e==(showQestionNum-1)){
							b.find(".btn-xyt").addClass("btn-tj");
							if(picture!=""&&picture!=undefined){
								b.find(".btn-xyt").css("background-image", 'url("' + picture + '")');
							}
						}
						$("#wrongNum").val(1+Number($("#wrongNum").val()));
						
						$("#tailTable").append("<tr><td>第"+e+"题</td><td>"+rightNumEm+"</td><td>"+selectListEm+"</td></tr>");
						$("#detailContent").val($("#detailContent").val()+titleText+selectAnwerText);
						return d=void 0
					}else{
						return alert("答对才能进行下一题哦!")
					}
				}
				}
			}
			alert("完成本题后,才能继续!");
			}
		)})();



