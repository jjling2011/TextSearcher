### TextSearcher
文本内容搜索器

### 这个软件要解决的问题
现在很多文本搜索软件内部调用搜索引擎，秒出搜索结果。但是搜索引擎会对文本进行分词，比如“红苹果”这个词会分成“红”和“苹果”，所以搜索“红苹”的时候就搜不到。TextSearcher反其道而行，不分词不用搜索引擎，改用最原始的字符串查找，所以不会出现上面这个问题。代价就是搜得很慢。数据库是把文本文件的内容复制一次，所以如果文本内容很多就会占用大量硬盘空间。由于EF6的SQLite有BUG搜索不了中文，所以这个软件把所有数据加载到内存中搜索，如果文本内容很多会占用大量内存。所以这个软件只适用于搜索小文本文件的内容。  
