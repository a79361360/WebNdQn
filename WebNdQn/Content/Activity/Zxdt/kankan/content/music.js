//播放器控制
var audio = document.getElementById('mp3Btn');
$('#musicIcon').click(function(e){
if(audio.paused)
{
$('#musicIcon').stop(true, true).addClass("musicIconRotate");
audio.play();//播放
return;
}
//当前是播放状态
$('#musicIcon').stop(true, true).removeClass("musicIconRotate");
audio.pause(); //暂停
e.stopPropagation();
});