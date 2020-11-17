using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Image{
    public class File{
        string filePath;
        string fileName;

        public File(string filePath_){
            filePath = filePath_;
        }

        public System.Byte[] GetByte(){
            System.Byte[] rawData = System.IO.File.ReadAllBytes(filePath);
            return rawData;
        }

        public string GetName() => fileName;
    }

    List<File> files = new List<File>();
    string name = "LOLKEK";

    public List<File> GetFiles() => files;
    public string GetName() => name;
}