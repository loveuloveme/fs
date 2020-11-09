using System;
using System.Collections.Generic;
using System.Text;

namespace FileSystem{
    class Fat16{
    }

    class Fat32{
    }
}

class Program{
    static int Main(){
        Img.Image file_ = new Img.Image("Kke.img");
        file_.getRoot().addFile(new Img.File("lohtv"));
        FileSystem.Fat12 fs = new FileSystem.Fat12();
        fs.createImage(file_);
        return 0;
    }
}