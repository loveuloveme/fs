using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Image{
    public class File{

        List<File> files = new List<File>();
        string filePath;
        string fileName;
        string fileExt;
        bool isDir = false;

        public File(string filePath_, string name, string ext){
            filePath = filePath_;
            fileName = name;
            fileExt = ext;
        }

        public File(string dirName){
            isDir = true;
            fileName = dirName;
            fileExt = "   ";
        }

        public System.Byte[] GetByte(){
            System.Byte[] rawData = System.IO.File.ReadAllBytes(filePath);
            return rawData;
        }

        public bool Dir() => isDir;
        public string GetName() => fileName;
        public List<File> GetFiles() => files;
        public string GetExt() => fileExt;

        public DateTime GetCreationTime(){
            if(!isDir){
                return System.IO.File.GetCreationTime(@filePath);
            }else{
                return DateTime.Now;
            }
        }
        public DateTime GetWriteTime(){
            if(!isDir){
                return System.IO.File.GetLastWriteTime(@filePath);
            }else{
                return DateTime.Now;
            }
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