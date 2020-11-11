using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Img{
    public class Folder{
        private List<File> files = new List<File>();
        private List<Folder> folders = new List<Folder>();
        private string name;
        public string getName() => name;
        public List<File> getFiles() => files;
        public List<Folder> getFolders() => folders;

        public void addFile(File file){
            files.Add(file);
        }

        public void addFolder(Folder folder){
            folders.Add(folder);
        }

        public Folder(string name_){
            name = name_;
        }
    }

    public class File{
        private string name;
        private int size;
        private int cluster;
        public List<int> clusters = new  List<int>();
        private string extenstion;
        private string filePath;

        public string getName() => name;
        public string getExtenstion() => extenstion;
        public int getSize() => size;
        public int getCluster() => cluster;
        public void setCluster(int cluster_){
            cluster = cluster_;
        }

        public System.Byte[] getByte(){
            System.Byte[] rawData = System.IO.File.ReadAllBytes(filePath);
            size = rawData.Length;
            return rawData;
        }

        public File(string name_, string filePath_){
            name = name_;
            filePath = filePath_;
        }
        public File(string name_){
            name = name_;
        }
    }

    public class Image{
        private Folder root = new Folder("root");
        private string name;

        // public List<File> getFolders(){
        //     //root.getRoot()
        // }
        public string getName() => name;
        public Folder getRoot() => root;

        public Image(string name_){
            name = name_;
        }
    }
}