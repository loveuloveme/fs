class Program{
    static int Main(){
        Img.Image file_ = new Img.Image("Kke.img");
        file_.getRoot().addFile(new Img.File("lohtv", "./jopa.txt"));
        file_.getRoot().addFile(new Img.File("lohtv2", "./jopa.txt"));
        //file_.getRoot().addFolder(new Img.Folder("YAKIDAN"));
        //file_.getRoot().addFolder(new Img.Folder("YAKIDALA"));
        //System.Console.WriteLine(file_.getRoot().getFolders().Count);
        FileSystem.Fat12 fs = new FileSystem.Fat12();
        fs.createImage(file_);
        return 0;
    }
}