class Program{
    static int Main(){
        Image img = new Image();

        img.AddFile(new Image.File("./jopa.txt", "YAKIdanlo"));

        //file_.getRoot().addFile(new Img.File("lohtv", "./jopa.txt"));
        //file_.getRoot().addFile(new Img.File("lohtv2", "./jopa.txt"));
        //file_.getRoot().addFolder(new Img.Folder("YAKIDAN"));
        //file_.getRoot().addFolder(new Img.Folder("YAKIDALA"));
        //System.Console.WriteLine(file_.getRoot().getFolders().Count);
        FileSystem.Fat12 fs = new FileSystem.Fat12();
        fs.readAbstract(img);
        fs.createImage();
        return 0;
    }
}