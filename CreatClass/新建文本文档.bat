echo    1.链接数据库，生成csv文件+csv存放路径  2.链接数据库，生成客户端代码 +代码存放路径  3.将csv文件提交到数据库  4.生成lua文件+lua文件存放路径 5.生成Lua对应的c#文件+生成路径
set pathOne="D:\Work\Test\LostTemple\GitShoot\ShootGame\Assets\StreamingAssets\table"
set pathTwo="D:\Work\Test\LostTemple\GitShoot\ShootGame\Assets\Scripts\Table\Meta"
FTGTableCreater.exe 1 %pathOne%
FTGTableCreater.exe 2 %pathTwo%


pause