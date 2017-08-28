
jQuery("video").each(function(i , e){
	
	
	var url = e.currentSrc ;
	var ggiBtn = jQuery('<Button class="ggi-button" style="width:120px; zIndex:999; position:absolute; top:0px; left:0%; height:30px;color:#fff; background:rgb(36,169,205); border:none">Download Video</Button>');
	ggiBtn.css("top" ,$(e).position().top + 25);
	ggiBtn.css("left" ,$(e).position().left);
	
	
	ggiBtn.data("url" , url);
	$("body").append(ggiBtn); 
	
	ggiBtn.click(function(){
		var url = this.data("url"); 
		
		
		$.ajax({
			type: "GET",
			url: 'http://127.0.0.1:9898/?GGI=' +url+'&GGIsize=' +0,
			contentType: "application/text; charset=utf-8",
			dataType: "text"  
		});

		
	}.bind(ggiBtn)); 
	
	
}); 