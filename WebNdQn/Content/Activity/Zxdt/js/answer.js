$(document).ready(function(){ 
	var input = $("input");
	var label = $("label");
	var inputArray = input.length;
	var labelArray = label.length;
	for (i = 0; i < inputArray; i++) {
		input[i].value = label[i].innerText;
	}
}) 
/**CSS3 答题卡翻页效果**/
$.fn.answerSheet = function(options) {
	var defaults = {
		mold: 'card',
	};
	var opts = $.extend({},
	defaults, options);
	return $(this).each(function() {
		var obj = $(this).find('.card_cont');
		var _length = obj.length,
		_b = _length - 1,
		_len = _length - 1,
		_cont = '.card_cont';
		for (var a = 1; a <= _length; a++) {
			obj.eq(_b).css({
				'z-index': a
			});
			_b -= 1;
		}
		$(this).show();
		if (opts.mold == 'card') {
			$('.card_bottom').find('.next').click(function() {
				var b  = $(this).closest(".card").find("input:checked").length;
				if(b == 0) {
					alert("请至少选择一个答案再进入下一题!");
				}else{
					var _idx = $(this).parents(_cont).index(),
					_cards = obj,
					_cardcont = $(this).parents(_cont);
					if (_idx == _len) {
						return;
					} else {
						setTimeout(function() {
							_cardcont.addClass('cardn');
							setTimeout(function() {
								_cards.eq(_idx + 3).addClass('card3');
								_cards.eq(_idx + 2).removeClass('card3').addClass('card2');
								_cards.eq(_idx + 1).removeClass('card2').addClass('card1');
								_cardcont.removeClass('card1');
							},
							200);
						},
						100);
					}
				}
			});
			$('.card_bottom').find('.prev').click(function() {
				var _idx = $(this).parents(_cont).index(),
				_cardcont = $(this).parents(_cont);
				obj.eq(_idx + 2).removeClass('card3').removeClass('cardn');
				obj.eq(_idx + 1).removeClass('card2').removeClass('cardn').addClass('card3');
				obj.eq(_idx).removeClass('card1').removeClass('cardn').addClass('card2');
				setTimeout(function() {
					obj.eq(_idx - 1).addClass('card1').removeClass('cardn');
				},
				200);
			});
			$('.card_bottom').find('.ok').click(function() {
				var b  =$(this).closest(".card").find("input:checked").length;
				if(b == 0) {
					alert("请至少选择一个答案再提交!");
				}else{
					var chestr = "";
					var question = $(".question")
					var cardArray = $(".card").length;
					for (i = 1; i <= cardArray; i++) {
						//chestr += $(".question").eq(i-1).text();
						chestr += "Q" + i+ ".";
						$("input[name='r-group-" + i + "']").each(function() {
							if($(this).is(":checked")){
								chestr += $(this).val()+",";
							}
						});/**/
						//chestr += "<br>";
						chestr += "...";
					}
					//console.log(chestr);
					$("#txta").html(chestr)
					document.location.href ='last.html?mydata1=' + $("#txta").html();
				}
			});
		}
	});
};
