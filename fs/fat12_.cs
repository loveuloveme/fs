using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileSystem{
    class Fat12{
        class FatUnit{
            long offset;

            List<List<int>> clusterMap = new List<List<int>>();
            
            public void Write(Stream fstream, long offset){
                fstream.Seek(offset, SeekOrigin.Begin);

                bool half = false;
                bool isLast = false;
                byte[] cluster_map_pair = new byte[3];

                for(int i = 0; i < clusterMap.Count; i++){
                    List<int> clusters = clusterMap[i];
                    System.Console.WriteLine("File "+i+": ");

                    isLast = false;

                    for(int j = 0; j < clusters.Count; j++){
                        System.Console.WriteLine(j);

                        if(j == clusters.Count - 1){
                            isLast = true;
                        }else{
                            if(clusters[j] != clusters[j + 1]) isLast = true;
                        }

                        ushort value = (ushort)(isLast ? 0xfff : clusters[j] + 1);

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
            Byte accessDate;
            Byte firstCluster;
            Byte writeTime;
            Byte writeDate;
            Byte cluster;
            Byte size;

            long offset;

            public File(long fileOffset, string name_, List<int> clustersId, int size_ = 0, long attr_ = 0){
                offset = fileOffset;

                name = new Byte(0x0, name_);
                attr = new Byte(0xB); 
                createDate = new Byte(0x0 + 16, new byte[]{0x5, 0x5});
                accessDate = new Byte(0x0 + 18, new byte[]{0x4, 0x4});
                firstCluster = new Byte(0x0 + 20, new byte[]{0x3, 0x3});
                writeTime = new Byte(0x0 + 22, new byte[]{0x2, 0x2});
                writeDate = new Byte(0x0 + 24, new byte[]{0x1, 0x2});
                cluster = new Byte(0x1A, BitConverter.GetBytes(clustersId[0]));
                size = new Byte(0x1C, BitConverter.GetBytes(size_)); 
            }

            public File(long fileOffset, string name_){
                offset = fileOffset;

                name = new Byte(0x0, name_);
                attr = new Byte(0xB, new byte[]{0x8}); 
                createDate = new Byte(0x0 + 16);
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
                accessDate.write(fstream, offset);
                firstCluster.write(fstream, offset);
                writeTime.write(fstream, offset);
                writeDate.write(fstream, offset);
                cluster.write(fstream, offset);
                size.write(fstream, offset);
            }
        }
        
        class Boot{
            private Byte[] boot = new Byte[]{
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
        File volume = new File(0x2600, "LOLKEK");
        List<File> files = new List<File>();
        List<Cluster> clusters = new List<Cluster>();
        FatUnit FatTable = new FatUnit();
        bool isReady = false;

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

                files.Add(new File(offset, item.GetName(), clustersId, size));
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