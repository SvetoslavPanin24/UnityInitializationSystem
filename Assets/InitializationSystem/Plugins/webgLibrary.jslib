mergeInto(LibraryManager.library, {
	load: function(keyName) {
		var keyString = UTF8ToString(keyName);
		var storedString = "";
	
		if(localStorage.getItem(keyString) !== null)
		{
			storedString = localStorage.getItem(keyString);
		
			var bufferSize = lengthBytesUTF8(storedString) + 1;
			var buffer = _malloc(bufferSize);
			stringToUTF8(storedString, buffer, bufferSize);
			
			return buffer;
		}
		
		return null;
	},
	save: function(keyName, data) {
        localStorage.setItem(UTF8ToString(keyName), UTF8ToString(data));
	},
    deleteItem: function(keyName) {
        localStorage.removeItem(UTF8ToString(keyName));
    }
});
