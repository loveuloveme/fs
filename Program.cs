class Program{
    static int Main(){
        Image abstract12 = new Image();
        abstract12.AddFile(new Image.File("./example.txt", "YAKIDAN", "avi"));
        abstract12.AddFile(new Image.File("./example.txt", "heheunited", "zip"));

        FileSystem.Fat12 fat12Image = new FileSystem.Fat12("KIDALA");
        fat12Image.readAbstract(abstract12);
        fat12Image.createImage("test1.img");

        abstract12.AddFile(new Image.File("./example.txt", "YAKIDAN", "avi"));
        abstract12.AddFile(new Image.File("./example.txt", "heheunited", "zip"));
        FileSystem.Fat16 fat16Image = new FileSystem.Fat16("notmath");
        fat16Image.readAbstract(abstract12);
        fat16Image.createImage("test2.img");


        Image abstract32 = new Image();
        abstract32.AddFile(new Image.File("./example.txt", "heheunited", "zip"));
        abstract32.AddFile(new Image.File("./kek.zip", "mellerx", "zip"));

        FileSystem.Fat32 fat32Image = new FileSystem.Fat32("lolkek");
        fat32Image.readAbstract(abstract32);
        fat32Image.createImage("test3.img");

        return 0;
    }
}