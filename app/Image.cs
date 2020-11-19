using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Image{
    public class File{
        string filePath;
        string fileName;
        string fileExt;

        public File(string filePath_, string name, string ext){
            filePath = filePath_;
            fileName = name;
            fileExt = ext;
        }

        public System.Byte[] GetByte(){
            System.Byte[] rawData = System.IO.File.ReadAllBytes(filePath);
            return rawData;
        }

        public string GetName() => fileName;
        public string GetExt() => fileExt;

        public DateTime GetCreationTime(){
            return System.IO.File.GetCreationTime(@filePath);
        }
        public DateTime GetWriteTime(){
            return System.IO.File.GetLastWriteTime(@filePath);
        }
    }

    List<File> files = new List<File>();
    string name = "LOLKEK";

    public void AddFile(File file){
        files.Add(file);
    }

    public List<File> GetFiles() => files;
    public string GetName() => name;
}