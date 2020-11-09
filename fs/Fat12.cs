using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileSystem{
    class Fat12{
        private long offsetToClusterSection = 0x2600;
        private long offsetToDirSection = 0x2600 + 512;
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

        private void writeCluster(Img.File file, Stream fileStream){
            Byte name = new Byte(0x0, file.getName()); // File name + ext
            Byte attr = new Byte(0xB); // File attributes
            Byte cluster = new Byte(0x1A); // First cluster
            Byte size = new Byte(0x1C, 5); 
        }

        private void writeFile(Img.File file, Stream fileStream){
            Byte name = new Byte(0x0, file.getName()); // File name + ext
            Byte attr = new Byte(0xB); // File attributes
            Byte cluster = new Byte(0x1A); // First cluster
            Byte size = new Byte(0x1C, 5); 

            name.write(fileStream);
            attr.write(fileStream);
            cluster.write(fileStream);
            size.write(fileStream);
        }

        private void writeFolder(Img.Folder folder, Stream fileStream){
            Byte name = new Byte(0x0, folder.getName()); // File name + ext
            Byte attr = new Byte(0xB); // File attributes
            Byte cluster = new Byte(0x1A); // First cluster
            Byte size = new Byte(0x1C, 5); 
        }

        public void createImage(Img.Image img){
            Stream file = new FileStream(img.getName(), FileMode.Create, FileAccess.ReadWrite);

            for(int i = 0; i < boot.Length; i++){
                boot[i].write(file);
            }


            file.Seek(offsetToDirSection, SeekOrigin.Begin);

            List<Img.File> files = img.getRoot().getFiles();

            foreach(Img.File item in files){
                writeFile(item, file);
            }

            file.Seek(offsetToClusterSection + 32, SeekOrigin.Begin);

            List<Img.Folder> folder = img.getRoot().getFolders();

            foreach(Img.Folder item in folder){
                writeFolder(item, file);
            }
        }
    }

}