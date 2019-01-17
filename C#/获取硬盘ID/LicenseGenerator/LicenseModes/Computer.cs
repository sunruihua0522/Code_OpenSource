using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Netframe.License.Modes
{
    public class Computer
    {
        public string CpuID; //1.cpu序列号
        public string DiskVolume;//硬盘的卷标号
        public string MacAddress; //2.mac序列号
        public string DiskID; //3.硬盘id
        public string IpAddress; //4.ip地址
        public string LoginUserName; //5.登录用户名
        public string ComputerName; //6.计算机名
        public string SystemType; //7.系统类型
        public string TotalPhysicalMemory; //8.内存量，单位：M
    }
}
