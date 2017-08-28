var video = jQuery("#movie_player"); 

if(video){
	
	
	
	
	var ggiBtn = jQuery('<Button class="ggi-button" style="width:120px; zIndex:999; position:absolute; top:5px; left:50%; height:30px;color:#fff; background:rgb(36,169,205); border:none">Download Video</Button>');
	ggiBtn.click(function(){
		jQuery.getJSON('http://127.0.0.1:9898/?GGI=' + window.location.href  +'&GGIsize=' +0, {
  key: 'value',
  otherKey: 'otherValue'
}, function(data){
     
	 
});
		
	}); 
	jQuery(".ytp-chrome-controls").append(ggiBtn); 
		
}