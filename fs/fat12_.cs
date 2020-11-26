using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileSystem{
    class Fat12:Fat{        
        static class Config{
            public static int sectorSize = 512;
            public static int sectorsPerCluster = 32;
            public static int clusterCount = 2880;
            public static int sectorsPerFat = 9;
        }

        class FatUnit12:FatUnit{
            public override void Write(Stream fstream, long offset){
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
        }
        
        private int bytesPerCluster = Config.sectorSize*Config.sectorsPerCluster;

        
        public Fat12(string diskName){
            string sampleDiskName = parseDiskName(diskName);

            FatTable = new FatUnit12();
            boot = new Boot(sampleDiskName, Config.sectorSize, Config.sectorSize/32, Config.clusterCount, Config.sectorsPerFat, "FAT12");
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
                    if(item.GetName().Length > 8){
                        List<string> parts = parseLongName(item.GetName()+"."+item.GetExt());
                        for(int i = parts.Count - 1; i >= 0; i--){
                            files.Add(new LongFileName(offset, parts[i], item.GetExt(), clustersId, i + 1, i == parts.Count - 1));

                            offset += bytesPerItem; 
                        }
                    }
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
