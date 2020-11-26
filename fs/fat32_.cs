using System;
using System.Collections.Generic;
using System.IO;

namespace FileSystem{
    class Fat32:Fat{        
        static class Config{
            public static int sectorSize = 16384;
            public static int sectorsPerCluster = 32;
            public static int clusterCount = 65200;
            public static int sectorsPerFat = 9;
        }

        class FatUnit12:FatUnit{
            public override void Write(Stream fstream, long offset){
                fstream.Seek(offset, SeekOrigin.Begin);
                fstream.Write(new byte[]{0xf0, 0xff, 0xff});

                bool isLast = false;

                for(int i = 0; i < clusterMap.Count; i++){
                    List<int> clusters = clusterMap[i];

                    isLast = false;

                    for(int j = 0; j < clusters.Count; j++){
                        if(j == clusters.Count - 1){
                            isLast = true;
                        }

                        ushort value = (ushort)(isLast ? 0x0FFF : clusters[j] + 1);
                        
                        var val = BitConverter.GetBytes(value);
                        
                        if(val.Length < 4){
                            var margin = 4 - val.Length;
                            var temp = new System.Byte[4];
                            for(int b = 0; b < 4; b++){
                                if(b < val.Length){
                                    temp[b] = val[b];
                                }else{
                                    temp[b] = 0x0;
                                }
                            }
                            val = temp;
                        }

                        fstream.Write(val);
                    }
                }
            }
        }
        
        private int bytesPerCluster = Config.sectorSize*Config.sectorsPerCluster;

        
        public Fat32(string diskName){
            string sampleDiskName = parseDiskName(diskName);

            FatTable = new FatUnit12();
            boot = new Boot(sampleDiskName, Config.sectorSize, Config.sectorSize/32, Config.clusterCount, Config.sectorsPerFat, "FAT32");
            volume = new File(Config.sectorSize*(Config.sectorsPerFat*2 + 1), sampleDiskName);
        }

        private void writeFat(Stream file){
            FatTable.Write(file, Config.sectorSize);
            FatTable.Write(file, Config.sectorSize*(Config.sectorsPerFat + 1));
        }



        public void readAbstract(Image img){
            List<Image.File> imgFiles = img.GetFiles();
            long offset = Config.sectorSize*(Config.sectorsPerFat*2 + 1) + 32;
            long bytesPerItem = 32;
            long offsetCluster = Config.sectorSize*20;

            foreach(var item in imgFiles){
                System.Byte[] rawData;

                if(!item.Dir()){
                    rawData = item.GetByte();
                }else{
                    rawData = new System.Byte[0];
                }

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

                if(!item.Dir()){
                    files.Add(new File(offset, item.GetName(), item.GetExt(), clustersId, item.GetCreationTime(), item.GetWriteTime(), size));
                    offset += bytesPerItem;
                }else{
                    continue;
                    
                    files.Add(new File(offset, item.GetName(), item.GetExt(), new List<int>(){0}, item.GetCreationTime(), item.GetWriteTime(), 0, 0x10));
                }
                
            }
        }

        public void createImage(string fileName){
            Stream file = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            file.Seek((Config.clusterCount * Config.sectorSize) - 1, SeekOrigin.Begin);
            file.WriteByte(0x00);

            writeBoot(file);
            writeFat(file);
            writeVolume(file);
            writeFiles(file);
            writeClusters(file);
        }
    }
}