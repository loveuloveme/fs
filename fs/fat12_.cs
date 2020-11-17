using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileSystem{
    class Fat12{
        class FatUnit{

        }
        class Cluster{
            int id;
            System.Byte[] rawData;

            public Cluster(System.Byte[] data, int id_){
                rawData = data;
                id = id_;
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

            int id;

            public File(string name_, List<int> clustersId, int size){
                Byte name = new Byte(0x0, name_);
                Byte attr = new Byte(0xB); 
                Byte createDate = new Byte(0x0 + 16, new byte[]{0x5, 0x5});
                Byte accessDate = new Byte(0x0 + 18, new byte[]{0x4, 0x4});
                Byte firstCluster = new Byte(0x0 + 20, new byte[]{0x3, 0x3});
                Byte writeTime = new Byte(0x0 + 22, new byte[]{0x2, 0x2});
                Byte writeDate = new Byte(0x0 + 24, new byte[]{0x1, 0x2});
                Byte cluster = new Byte(0x1A, BitConverter.GetBytes(clustersId[0]));
                Byte size = new Byte(0x1C, BitConverter.GetBytes(file.getSize())); 
            }

            public void Write(Stream fstream){
                long offset = 0;
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
        private int bytesPerCluster = 16384;

        List<File> files = new List<File>();
        List<Cluster> clusters = new List<Cluster>();
        FatUnit FatTable = new FatUnit();

        public void readAbstract(Image img){
            List<Image.File> imgFiles = img.GetFiles();

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

                    clustersId.Add(clusters.Count);
                    clusters.Add(new Cluster(rawDataSub, clusters.Count));
                }

                files.Add(new File(item.GetName(), clustersId, size));
            }
        }

        public void createImage(){
            Stream file = new FileStream("test3.img", FileMode.Create, FileAccess.ReadWrite);

        }
    }

}