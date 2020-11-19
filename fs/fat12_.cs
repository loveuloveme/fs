using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileSystem{
    class Fat12{
        class FatUnit{
            long offset;

            List<List<int>> clusterMap = new List<List<int>>(){};
            
            public void Write(Stream fstream, long offset){

                fstream.Seek(offset, SeekOrigin.Begin);
                fstream.Write(new byte[]{0xf0, 0xff, 0xff});

                bool half = false;
                bool isLast = false;
                byte[] cluster_map_pair = new byte[3];

                for(int i = 0; i < clusterMap.Count; i++){
                    List<int> clusters = clusterMap[i];

                    isLast = false;

                    for(int j = 0; j < clusters.Count; j++){
                        if(j == clusters.Count - 1){
                            isLast = true;
                        }

                        ushort value = (ushort)(isLast ? 0xff : clusters[j] + 1);

                        if (!half){
                            half = true;
                            cluster_map_pair[0] = (byte)value;
                            cluster_map_pair[1] = (byte)(value >> 8);
                        }else{
                            half = false;

                            cluster_map_pair[2] = (byte)(value >> 4);
                            value &= 0x00F;
                            cluster_map_pair[1] ^= (byte)(value << 4);

                            fstream.Write(cluster_map_pair);

                            cluster_map_pair[2] = 0;
                        }
                    }
                }

                if(half){
                    fstream.Write(cluster_map_pair);
                }
            }
            public void Add(List<int> clusters){
                clusterMap.Add(clusters);
            }
        }
        class Cluster{
            long offset;
            System.Byte[] rawData;

            public Cluster(System.Byte[] data, long offset_){
                rawData = data;
                offset = offset_;
            }

            public void Write(Stream file){
                file.Seek(offset, SeekOrigin.Begin);
                file.Write(rawData);
            }
        }

        class File{
            Byte name;
            Byte attr;
            Byte createDate;
            Byte createTime;
            Byte accessDate;
            Byte firstCluster;
            Byte writeTime;
            Byte writeDate;
            Byte cluster;
            Byte size;

            long offset;

            private string parseName(string name, string ext = ""){
                string nameSample = "";
                string extSample = "";

                for(int i = 0; i < 8; i++){
                    if(i < name.Length){
                        nameSample += name[i];
                    }else{
                        nameSample += " ";
                    }
                }

                for(int i = 0; i < 8; i++){
                    if(i < ext.Length){
                        extSample += ext[i];
                    }else{
                        extSample += " ";
                    }
                }

                return (nameSample+ext).ToUpper();
            }

            private byte[] _parseDate(DateTime time){
                ushort day = (ushort)time.Day;
                ushort month = (ushort)time.Month;
                ushort year = (ushort)(time.Year - 1980);

                string dayStr = Convert.ToString(day, 2);
                string monthStr = Convert.ToString(month, 2);
                string yearStr = Convert.ToString(year, 2);

                // byte[] res = new byte[2];

                // res[0] = (byte)((day << 3) + (month >> 1));
                // res[1] = (byte)((month >> 3) + (year));

                if(dayStr.Length < 5){
                    string temp = dayStr;
                    dayStr = "";

                    for(int i = 0; i < 5 - temp.Length; i++){
                        dayStr += "0";
                    }

                    dayStr += temp;
                }

                if(monthStr.Length < 4){
                    string temp = monthStr;
                    monthStr = "";

                    for(int i = 0; i < 4 - temp.Length; i++){
                        monthStr += "0";
                    }

                    monthStr += temp;
                }

                if(yearStr.Length < 7){
                    string temp = yearStr;
                    yearStr = "";

                    for(int i = 0; i < 7 - temp.Length; i++){
                        yearStr += "0";
                    }

                    yearStr += temp;
                }

                string binStr = yearStr+monthStr+dayStr;
                return new byte[]{(byte)Convert.ToInt32(binStr.Substring(8, 8), 2), (byte)Convert.ToInt32(binStr.Substring(0, 8), 2)};
            }

            private byte[] _parseTime(DateTime time){
                ushort second = (ushort)(Math.Ceiling((double)time.Second/2));
                ushort minute = (ushort)time.Minute;
                ushort hour = (ushort)time.Hour;

                string secondStr = Convert.ToString(second, 2);
                string minuteStr = Convert.ToString(minute, 2);
                string hourStr = Convert.ToString(hour, 2);

                // byte[] res = new byte[2];

                // res[0] = (byte)((day << 3) + (month >> 1));
                // res[1] = (byte)((month >> 3) + (year));

                if(secondStr.Length < 5){
                    string temp = secondStr;
                    secondStr = "";

                    for(int i = 0; i < 5 - temp.Length; i++){
                        secondStr += "0";
                    }

                    secondStr += temp;
                }

                if(minuteStr.Length < 6){
                    string temp = minuteStr;
                    minuteStr = "";

                    for(int i = 0; i < 6 - temp.Length; i++){
                        minuteStr += "0";
                    }

                    minuteStr += temp;
                }

                if(hourStr.Length < 5){
                    string temp = hourStr;
                    hourStr = "";

                    for(int i = 0; i < 5 - temp.Length; i++){
                        hourStr += "0";
                    }

                    hourStr += temp;
                }

                string binStr = hourStr+minuteStr+secondStr;

                return new byte[]{(byte)Convert.ToInt32(binStr.Substring(8, 8), 2), (byte)Convert.ToInt32(binStr.Substring(0, 8), 2)};
            }

            public File(long fileOffset, string name_, string ext, List<int> clustersId, DateTime createTime_, DateTime writeDate_, int size_ = 0, long attr_ = 0){
                offset = fileOffset;

                name = new Byte(0x0, parseName(name_, ext));
                attr = new Byte(0xB); 
                createTime = new Byte(0x0 + 14, _parseTime(createTime_));
                createDate = new Byte(0x0 + 16, _parseDate(createTime_));
                accessDate = new Byte(0x0 + 18);
                firstCluster = new Byte(0x0 + 20);
                writeTime = new Byte(0x0 + 22, _parseTime(writeDate_));
                writeDate = new Byte(0x0 + 24, _parseDate(writeDate_));
                cluster = new Byte(0x1A, BitConverter.GetBytes(clustersId[0]));
                size = new Byte(0x1C, BitConverter.GetBytes(size_)); 
            }

            class LongFileName{
                Byte order;
                Byte name;
                Byte attr;
                Byte type;
                Byte sum;
                Byte name2;
                Byte firstCluster;
                Byte name3;

                long offset;

                private string parseName(string name_, string ext){
                    for(int i = 0; i < 23; i++){
                        if(i < name_.Length){
                            name_ += " ";
                        }
                    }

                    name_ += ext[0];
                    name_ += ext[1];
                    name_ += ext[2];

                    return name_;
                }
                public LongFileName(long fileOffset, string name_, string ext, List<int> clustersId, int size_ = 0, long attr_ = 0){
                    name_ = parseName(name_, ext);
                    offset = fileOffset;

                    order = new Byte(0x00, 0x40); 
                    name = new Byte(0x01, name_.Substring(0, 10));
                    attr = new Byte(0xB, 0x0f);
                    type = new Byte(0xC);
                    sum = new Byte(0xD);
                    name2 = new Byte(0xE, name_.Substring(10, 12));
                    firstCluster = new Byte(0xA + 16);
                    name3 = new Byte(0xC + 16, name_.Substring(22, 4));
                }

                public void Write(Stream fstream){
                    name.write(fstream, offset);
                    attr.write(fstream, offset);
                    type.write(fstream, offset);
                    sum.write(fstream, offset);
                    name2.write(fstream, offset);
                    firstCluster.write(fstream, offset);
                    name3.write(fstream, offset);;
                }
            }
            
            public File(long fileOffset, string name_){
                offset = fileOffset;

                name = new Byte(0x0, parseName(name_));
                attr = new Byte(0xB, new byte[]{0x8}); 
                createDate = new Byte(0x0 + 16);
                createTime = new Byte(0x0 + 16);
                accessDate = new Byte(0x0 + 18);
                firstCluster = new Byte(0x0 + 20);
                writeTime = new Byte(0x0 + 22);
                writeDate = new Byte(0x0 + 24);
                cluster = new Byte(0x1A);
                size = new Byte(0x1C); 
            }

            public void Write(Stream fstream){
                name.write(fstream, offset);
                attr.write(fstream, offset);
                createDate.write(fstream, offset);
                createTime.write(fstream, offset);
                accessDate.write(fstream, offset);
                firstCluster.write(fstream, offset);
                writeTime.write(fstream, offset);
                writeDate.write(fstream, offset);
                cluster.write(fstream, offset);
                size.write(fstream, offset);
            }
        }
        
        class Boot{
            public Byte[] boot = new Byte[]{
                new Byte(0x0, new byte[] { 0xEB, 0x3C, 0x90 }),
                new Byte(0x3, Encoding.ASCII.GetBytes("lolkek")),
                new Byte(0xB, BitConverter.GetBytes((ushort)512)),
                new Byte(0xD, new byte[] { (byte)32u }),
                new Byte(0xE, BitConverter.GetBytes((ushort)1)),
                new Byte(0x10, new byte[] { (byte)2u }), 
                new Byte(0x11, BitConverter.GetBytes((ushort)16)),
                new Byte(0x13, BitConverter.GetBytes((ushort)2880)),
                new Byte(0x15, new byte[] { 0xF0 }), 
                new Byte(0x16, BitConverter.GetBytes((ushort)9)), 
                new Byte(0x26, new byte[] { 0x29 }),
                new Byte(0x27, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }), 
                new Byte(0x02B, Encoding.ASCII.GetBytes("LOLKEK     FAT12   ")) 
            };

            public void Write(Stream fstream){
                foreach(var item in boot){
                    item.write(fstream);
                }
            }
        }
        private int bytesPerCluster = 16384;

        Boot boot = new Boot();
        File volume;
        List<File> files = new List<File>();
        List<Cluster> clusters = new List<Cluster>();
        FatUnit FatTable = new FatUnit();
        bool isReady = false;

        int freeClusterCount;

        public Fat12(string diskName, int bytesPerCluster = 512, int sectorsPerCluster = 16384, int dirCount = 16, int sectorsPerFat = 9){
            string sampleDiskName = "";

            for(int i = 0; i < 6; i++){
                if(i < diskName.Length){
                    sampleDiskName += diskName[i];
                }else{
                    sampleDiskName += " ";
                }
            }

            volume = new File(0x2600, sampleDiskName);
            boot.boot[1] = new Byte(0x3, Encoding.ASCII.GetBytes(sampleDiskName));
            boot.boot[12] = new Byte(0x02B, Encoding.ASCII.GetBytes(sampleDiskName+"     FAT12   "));


        }

        private void writeBoot(Stream file){
            boot.Write(file);
        }

        private void writeFat(Stream file){
            FatTable.Write(file, 512);
            FatTable.Write(file, 5120);
        }


        private void writeVolume(Stream file){
            volume.Write(file);
        }

        private void writeFiles(Stream file){
            foreach(var item in files){
                item.Write(file);
            }
        }

        private void writeClusters(Stream file){
            foreach(var item in clusters){
                item.Write(file);
            }
        }

        public void readAbstract(Image img){
            List<Image.File> imgFiles = img.GetFiles();
            long offset = 0x2600 + 32;
            long bytesPerItem = 32;
            long offsetCluster = 0x2800;

            foreach(var item in imgFiles){
                System.Byte[] rawData = item.GetByte();
                int size = rawData.Length;
                int clusterCount = (int)Math.Ceiling((double)size/(double)bytesPerCluster);

                List<int> clustersId = new List<int>();

                for(int i = 0; i < clusterCount; i++){
                    System.Byte[] rawDataSub = new System.Byte[bytesPerCluster];

                    for(int j = bytesPerCluster*i; j < bytesPerCluster*(i+1); j++){
                        if(j == size) break;

                        rawDataSub[j - bytesPerCluster*i] = rawData[j];
                    }

                    clustersId.Add(clusters.Count + 2);
                    clusters.Add(new Cluster(rawDataSub, offsetCluster));
                    offsetCluster += bytesPerCluster;
                }

                FatTable.Add(clustersId);

                files.Add(new File(offset, item.GetName(), item.GetExt(), clustersId, item.GetCreationTime(), item.GetWriteTime(), size));
                offset += bytesPerItem;
            }

            isReady = true;
        }

        public void createImage(){
            if(!isReady) return;

            Stream file = new FileStream("test3.img", FileMode.Create, FileAccess.ReadWrite);
            file.Seek((2880 * 512) - 1, SeekOrigin.Begin);
            file.WriteByte(0x00);

            writeBoot(file);
            writeFat(file);
            writeVolume(file);
            writeFiles(file);
            writeClusters(file);
        }
    }

}
