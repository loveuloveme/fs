using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace FileSystem{
    public class Byte{
        public uint offset;
        public byte[] data;

        public Byte(uint offset, byte[] data){
            this.offset = offset;
            this.data = data;
        }

        public Byte(uint offset, string data){
            this.offset = offset;
            this.data = Encoding.ASCII.GetBytes(data);
        }

        public Byte(uint offset, int data){
            this.offset = offset;
            this.data = BitConverter.GetBytes(data);
        }

        public Byte(uint offset){
            this.offset = offset;
        }

        public void write(Stream fStream, long baseOffset = 0){
            if (data == null){
                return;
            }

            fStream.Seek(baseOffset + offset, SeekOrigin.Begin);
            fStream.Write(data);
        }
    }
}