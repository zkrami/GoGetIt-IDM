





function ggi(item){
	
	chrome.downloads.cancel(item.id);	
	
	console.log(item); 
	$.getJSON('http://127.0.0.1:9898/?GGI=' + item.finalUrl  +'&GGIsize=' + item.fileSize, {
  key: 'value',
  otherKey: 'otherValue'
}, function(data){
     
	 
}).fail(function(){
	
			
			
});
	
}


chrome.downloads.onDeterminingFilename.addListener(ggi); 