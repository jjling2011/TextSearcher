### TextSearcher
文本内容搜索器

### 这个软件要解决的问题
虽然现在很多文本内容搜索软件内部调用搜索引擎，秒出搜索结果。
但是搜索引擎会对文本内容进行分词，比如“红苹果”这个词会分成“红”和“苹果”，所以搜索“红苹”就搜不到。
TextSearcher反其道而行，不分词不用搜索引擎，采用最原始的字符串查找算法。
所以不会出现上面的问题，但是代价就是搜得很慢。
TextSearcher的数据库会把文件内容复制一份，如果文本文件比较大就会占用很多硬盘空间。
所以这个软件只适用于搜索包含小文本文件的目录。  

### 用法
![formmain.png](https://raw.githubusercontent.com/jjling2011/TextSearcher/main/imgs/formmain.png)
输入关键词然后按回车搜索。双击不同列有不同效果。    
 * Filename 打开文件
 * Path 打开目录
 * Content 弹出消息窗口

![formconfigs.png](https://raw.githubusercontent.com/jjling2011/TextSearcher/main/imgs/formconfigs.png)
设置窗口中Extensions设定搜索的文件后缀，用空格分隔多个后缀。  
Scan表示Update DB的时候扫描这个目录。  
Search表示搜索的时候包含这个目录。  