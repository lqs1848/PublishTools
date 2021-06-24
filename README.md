# PublishTools

C#写的发布工具

配合Maven 在windows下一键发布 springboot 生成的 jar 到 linux服务器

或者发布 前后端分离的 前端项目 先zip压缩 再上传 再ssh执行解压命令



使用方式:

生成 exe 并在 exe 所在目录 添加 ini 配置文件 填写服务器的地址账号密码

功能

​	上传功能		`call call.exe upload 本地路径 远程服务器路径`

​	Zip压缩(发布 前端项目) 	`call call.exe zip 需要压缩的路径 保存压缩文件的路径 压缩后zip文件的名称`

​	SSH 执行命令的功能	*这个自己改一下我只用来上传 jar 之前 kill 掉jar 上传后运行jar所以写死了*



用bat调用

配合Maven形成一键发布

```
echo off
set projectname=项目名称
set filename=%projectname%-模块名称-0.0.1-release.jar
set local_dir=%cd%\target\

call mvn clean install -Pcq1 -Dmaven.test.skip=true
echo "按任意键开始发布"
pause
cd ..\..\build\cq1\
echo "上传文件中 会耗费较长时间请耐心等待"
call call.exe upload %local_dir%%filename% /root/project/%projectname%/jar/%filename%
echo "重启服务中"
call call.exe report %filename% root/project/%projectname%/jar/
echo "发布完成 按任意键退出"
pause

```

cq1是我线上的服务器 重庆服务器1号



call call.exe upload %local_dir%%filename% /root/project/%projectname%/jar/%filename%

这里就是调用 call.exe 本工具了 第一个参数 本地文件 第二个参数 远程位置

call.exe 自己读取 自身文件夹 的call.ini

如果有多个服务器 多个 call.exe 多个 call.ini 配置就行







发布前端的bat

```
echo off
set projectname=项目名称
set CMD_HOME=%cd%
call install.bat
call report
cd ../mysd/build/cq1/
call call.exe zip %CMD_HOME%\dist\ %CMD_HOME%\ upload

echo rm -rf ../root/project/%projectname%/html >reportHtml.txt
echo mkdir ../root/project/%projectname%/html >>reportHtml.txt
call outercall.bat reportHtml.txt
del reportHtml.txt

call call.exe upload %CMD_HOME%\upload.zip /root/project/%projectname%/html/upload.zip
del %CMD_HOME%\upload.zip

echo unzip -o -d ../root/project/%projectname%/html ../root/project/%projectname%/html/upload.zip >reportHtml.txt
call outercall.bat reportHtml.txt
del reportHtml.txt

pause
```


























记录下自己发布的过程



最初是用 jenkin

SVN关联自动发布到服务器 但是很多提交不用发布 很多修改一部分 不完整提交等各种问题



然后换成我手动运维

最初发布是用 bat 调用 putty 执行命令

putty 的命令特别麻烦

只能传递 .txt的文本文件给 putty

```
::echo off
set filename=jarFileName.jar
set local_dir=%cd%\target\

call mvn clean install -Pprod -Dmaven.test.skip=true
cd ..\..\222\
echo "上传文件中 会耗费较长时间请耐心等待"
call call.exe upload %local_dir%%filename% /root/project/jar/%filename%
echo "重启服务中"
call call.exe report %filename%
pause
echo "按任意键查看日志"
echo vi ../root/project/jar/%filename%.log >%filename%Temp.txt
call outercall.bat %filename%Temp.txt
del %filename%Temp.txt
pause
```

#outercall.bat 内容为: `putty.exe -ssh root@ip -P 端口 -pw 密码 -m %1%`

putty 用bat调用真的非常麻烦

必须生成出命令的文本 用putty调用后再删除掉...

索性自己写了

