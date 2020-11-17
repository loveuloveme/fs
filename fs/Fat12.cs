using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileSystem{
    /*
    class Fat12{
        class Cluster{
            System.Byte[] bytes;
            int id;
            public Cluster(System.Byte[] bytes_, int id_){
                bytes = bytes_;
                id = id_;
            }

            public void write(Stream fStream, long baseOffset = 0){
                fStream.Seek(0x2800 + 16384*id, SeekOrigin.Begin);
                for(int i = 0; i < bytes.Length; i++){
                    fStream.WriteByte(bytes[i]);
                }
            }
        }

        private long offsetToClusterSection = 0x2600;
        private long offsetToDirSection = 0x2600 + 512;
        private int bytesPerCluster = 16384;
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

        private Byte fatId = new Byte(0, new byte[] { 0xF0, 0xFF, 0xFF });

        private void writeLabel(string vol, Stream fileStream, long offset){
            Byte name = new Byte(0x0, vol);
            Byte extenstion = new Byte(0x0 + 8, new byte[]{0x20, 0x20, 0x20});
            Byte attr = new Byte(0xB, new byte[]{0x8});
            Byte cluster = new Byte(0x1A); 
            Byte size = new Byte(0x1C, 0); 

            name.write(fileStream, offset);
            attr.write(fileStream, offset);
            extenstion.write(fileStream, offset);
            cluster.write(fileStream, offset);
            size.write(fileStream, offset);
        }

        
        //byte[] cluster_map_pair = new byte[3];
        // void writeFAT(Stream fStream, long offset){
        //         fatId.write(fStream, offset);
        //         bool half = false;
        //         foreach (FatFile file in files)
        //         {
        //             uint last = file.clusters.Last();
        //             foreach (uint cluster in file.clusters)
        //             {
        //                 ushort mapvalue = (ushort)(cluster == last ? 0xFFF : cluster + 1);
        //                 if (!half)
        //                 {
        //                     half = true;
        //                     cluster_map_pair[0] = (byte)mapvalue;
        //                     cluster_map_pair[1] = (byte)(mapvalue >> 8);
        //                 }
        //                 else
        //                 {
        //                     half = false;

        //                     cluster_map_pair[2] = (byte)(mapvalue >> 4);
        //                     mapvalue &= 0x00F;
        //                     cluster_map_pair[1] ^= (byte)(mapvalue << 4);
        //                     fStream.Write(cluster_map_pair);
        //                     cluster_map_pair[2] = 0;
        //                 }
        //             }
        //         }
        //         if (half)
        //         {
        //             fStream.Write(cluster_map_pair);
        //         }
        // }


        private void writeCluster(Img.File file, Stream fileStream){
            Byte name = new Byte(0x0, file.getName()); 
            Byte attr = new Byte(0xB); 
            Byte cluster = new Byte(0x1A); 
            Byte size = new Byte(0x1C, 5); 
        }

        private void writeFile(Img.File file, Stream fileStream, long offset){
            Byte name = new Byte(0x0, file.getName());
            Byte attr = new Byte(0xB); 
            Byte createDate = new Byte(0x0 + 16, new byte[]{0x5, 0x5});
            Byte accessDate = new Byte(0x0 + 18, new byte[]{0x4, 0x4});
            Byte firstCluster = new Byte(0x0 + 20, new byte[]{0x3, 0x3});
            Byte writeTime = new Byte(0x0 + 22, new byte[]{0x2, 0x2});
            Byte writeDate = new Byte(0x0 + 24, new byte[]{0x1, 0x2});
            Byte cluster = new Byte(0x1A, BitConverter.GetBytes(file.getCluster()));
            Byte size = new Byte(0x1C, BitConverter.GetBytes(file.getSize())); 

            name.write(fileStream, offset);
            attr.write(fileStream, offset);
            createDate.write(fileStream, offset);
            accessDate.write(fileStream, offset);
            cluster.write(fileStream, offset);
            size.write(fileStream, offset);
        }

        private void writeFolder(Img.Folder folder, Stream fileStream, long offset){
            Byte name = new Byte(0x0, folder.getName());
            Byte attr = new Byte(0xB); 
            Byte createDate = new Byte(0x0 + 16, new byte[]{0x5, 0x5});
            Byte accessDate = new Byte(0x0 + 18, new byte[]{0x4, 0x4});
            Byte firstCluster = new Byte(0x0 + 20, new byte[]{0x3, 0x3});
            Byte writeTime = new Byte(0x0 + 22, new byte[]{0x2, 0x2});
            Byte writeDate = new Byte(0x0 + 24, new byte[]{0x1, 0x2});
            Byte cluster = new Byte(0x1A, new byte[]{0x5, 0x0});
            Byte size = new Byte(0x1C, new byte[]{0x0, 0x0, 0x0, 0x0}); 

            name.write(fileStream, offset);
            attr.write(fileStream, offset);
            createDate.write(fileStream, offset);
            accessDate.write(fileStream, offset);
            cluster.write(fileStream, offset);
            size.write(fileStream, offset);
        }

        public void createImage(Img.Image img){
            Stream file = new FileStream(img.getName(), FileMode.Create, FileAccess.ReadWrite);
            List<Cluster> clusters = new List<Cluster>();

            file.Seek((2880 * 512) - 1, SeekOrigin.Begin);
            file.WriteByte(0x00);

            for(int i = 0; i < boot.Length; i++){
                boot[i].write(file);
            }

            List<Img.File> files = img.getRoot().getFiles();

            var fileOffset = offsetToClusterSection;

            writeLabel("LOLKEK", file, fileOffset);

            fileOffset += 32;

            int currentCluster = 2;
            foreach(Img.File item in files){
                System.Byte[] rawData = item.getByte();
                item.setCluster(currentCluster);
                int clusterCount = (int)Math.Ceiling((double)rawData.Length/(double)bytesPerCluster);

                //System.Console.WriteLine(Math.Ceiling((double)rawData.Length/(double)bytesPerCluster));

                for(int i = 0; i < clusterCount; i++){
                    System.Byte[] rawDataSub = new System.Byte[bytesPerCluster];

                    for(int j = bytesPerCluster*i; j < bytesPerCluster*(i+1); j++){
                        if(j == rawData.Length) break;

                        rawDataSub[j - bytesPerCluster*i] = rawData[j];
                    }
                    clusters.Add(new Cluster(rawDataSub, clusters.Count));
                    item.clusters.Add(clusters.Count - 1);
                }

                for(int l = 0; l < 2; l++){
                    int offsetStart = 512;

                    if(l == 1) offsetStart = 10 * 512;

                    file.Seek(offsetStart, SeekOrigin.Begin);
                    file.WriteByte(0xF0);
                    for(int k = 0; k < item.clusters.Count; k++){
                        file.Seek(offsetStart + item.clusters[k] + 1, SeekOrigin.Begin);
                        if(k == item.clusters.Count - 1){
                            file.WriteByte(0xff);
                        }else{
                            byte offset = 0x000;
                            
                            for(int b = 0; b < item.clusters[k]; b++){
                                offset += 0x001;
                            }

                            file.WriteByte((byte)(0x001+offset));
                        }
                    }
                }

                

                writeFile(item, file, fileOffset);
                // fatId.write(file, 512);
                // fatId.write(file, 10*512);
                fileOffset += 32;
                currentCluster += clusterCount;
            }

            List<Img.Folder> folder = img.getRoot().getFolders();

            var folderOffset = offsetToClusterSection + 32;
            foreach(Img.Folder item in folder){
                writeFolder(item, file, fileOffset);

                fileOffset += 32;
            }

            foreach(Cluster item in clusters){
                item.write(file);
            }
            
        }
    }
    */
    class Ext2{
        
    }
}