function remReSize() {
    var w = $(window).width();
    try {
        w = $(parent.window).width();
    } catch(ex) {}
	
    $('html').css('font-size', 100 / 750 * w + 'px');
}
for(var i = 0; i < 3; i++) {
	setTimeout(remReSize, 100 * i);
};
remReSize();
$(window).resize(remReSize);
$(document).ready(function() {
    remReSize();
});
