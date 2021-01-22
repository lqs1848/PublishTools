using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace call
{
    class Program
    {
        static void Main(string[] args)
        {
            IniFiles iniFile = new IniFiles("call.ini");
            //iniFile.WriteString("server", "host", "192.168.99.91");
            //iniFile.WriteInteger("server", "port", 22);
            //iniFile.WriteString("server", "user", "root");
            //iniFile.WriteString("server", "pwd", "root");

            string host = iniFile.ReadString("server", "host", "");
            int port = iniFile.ReadInteger("server", "port", 22);
            string user = iniFile.ReadString("server", "user", "");
            string pwd = iniFile.ReadString("server", "pwd", "");

            string act = args[0];
            if ("upload".Equals(act)) //上传文件
            {
                string lfile = args[1];//本地文件
                string rfile = args[2];//远程文件
                Console.WriteLine("上传文件--start");
                using (var sftpClient = new SftpClient(host, port, user, pwd))
                {
                    sftpClient.Connect();
                    sftpClient.UploadFile(File.Open(lfile, FileMode.Open), rfile);
                }
                Console.WriteLine("文件上传--end");
            }

            if ("report".Equals(act)) //ssh 重新运行上传的文件
            {
                string filename = args[1];//文件名称
                string cPath = args.Length == 3 ? args[2] : "";//路径
                using (var sshClient = new SshClient(host, port, user, pwd))
                {
                    sshClient.Connect();
                    Console.WriteLine("重启服务中--start");
                    using (SshCommand cmd = sshClient.CreateCommand("source /etc/profile"))
                    {
                        var res = cmd.Execute();
                        Console.Write(res);
                        res = cmd.Execute("ps -ef|grep "+filename+"|grep -v grep|awk '{print $2}'|xargs kill -9");
                        Console.Write(res);
                        if (!"".Equals(cPath)) {
                            res = cmd.Execute("source /etc/profile; nohup java -jar ../" + cPath + filename + " > ../" + cPath + filename + ".log 2>&1 &");
                        }
                        else
                        {
                            //不加 source /etc/profile 会找不到 java 环境变量 坑爹 每条都要加
                            res = cmd.Execute("source /etc/profile; nohup java -jar ../root/project/jar/" + filename + " > ../root/project/jar/" + filename + ".log 2>&1 &");
                        }
                        Console.Write(res);
                    }
                    Console.WriteLine("重启服务中--end");
                }

            }

            if ("zip".Equals(act)) 
            {
                string lpath = args[1];//本地路径
                string toPath = args[2];//本地路径
                string fileName = args[3];//压缩名称
                Console.WriteLine("压缩中--start");
                try
                {
                    ZipHelper.ZipDirectory(lpath, toPath, fileName);
                    Console.WriteLine("压缩中--end");
                } 
                catch (Exception e) 
                {
                    Console.WriteLine("压缩中--error");
                    Console.Write(e);
                }
                
            }

            //string param1 = args[0];
            //string acc = args[1];
            //string pwd = args[2];
            /*
                using (var sshClient = new SshClient("ip", 22, "root", "password"))
                {
                    sshClient.Connect();
                    using (var cmd = sshClient.CreateCommand("java -version"))
                    {
                        var res = cmd.Execute();
                        Console.Write(res);
                    }
                }
            */
            //让窗体保存接受外部参数的状态来达到不退出的效果
            /*
            string path = System.IO.Directory.GetCurrentDirectory();
            string fileStr = path + "/temp.txt";


            if (System.IO.File.Exists(fileStr)) {
                File.Delete(fileStr);
            }

            string txt = "ps -ef|grep " + param1 + "|grep -v grep|awk '{print $2}'|xargs kill -9\r\n";
            txt += "cd project/jar/\r\n";
            txt += "nohup java -jar "+ param1 + " >"+ param1 + ".log &";

            FileStream fs1 = new FileStream(fileStr, FileMode.Create, FileAccess.Write);//创建写入文件 
            StreamWriter sw = new StreamWriter(fs1);
            sw.WriteLine(txt);//开始写入值
            sw.Close();
            fs1.Close();

            string exefile = path+ "/putty.exe";
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo(exefile, "-ssh "+ acc + " -pw "+ pwd + " -m temp.txt");
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit(2000);
            string output = process.StandardOutput.ReadToEnd();
            process.Close();

            File.Delete(fileStr);
            */
        }
    }
}
