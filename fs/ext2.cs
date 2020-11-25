using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileSystem{
    class Ext2{
        class SuperBlock{
            Byte s_inodes_count; 
            Byte s_blocks_count; 
            Byte s_r_blocks_count;
            Byte s_free_blocks_count; 
            Byte s_free_inodes_count; 
            Byte s_first_data_block;  
            Byte s_log_block_size;    
            Byte s_log_frag_size;     
            Byte s_blocks_per_group;
            Byte s_frags_per_group;
            Byte s_inodes_per_group;
            Byte s_mtime; 
            Byte s_wtime; 
            Byte s_mnt_count;
            Byte s_max_mnt_count;
            Byte s_magic;
            Byte s_state;
            Byte s_pad; 
            Byte s_minor_rev_level;
            Byte s_lastcheck;
            Byte s_checkinterval;
            Byte s_creator_os;
            Byte s_rev_level;
            Byte s_def_resuid;
            Byte s_def_regid; 
            Byte s_first_ino;
            Byte s_inode_size;
            Byte s_block_group_nr;
            Byte s_feature_compat;
            Byte s_feature_incompat;
            Byte s_feature_ro_compat;
            Byte s_uuid; 
            Byte s_volume_name; 
            Byte s_last_mounted; 
            Byte s_algorithm_usage_bitmap;
            Byte s_prealloc_blocks;
            Byte s_prealloc_dir_blocks; 
            Byte s_padding1;
            Byte s_reserved;

            public SuperBlock(int inodesCount, int blocksCount){
                s_inodes_count = new Byte(0x0, BitConverter.GetBytes(inodesCount)); 
                s_blocks_count = new Byte(0x0 + 4, BitConverter.GetBytes(blocksCount)); 

                s_free_blocks_count = new Byte(0x0 + 12);
                s_free_inodes_count = new Byte(0x0 + 16);
                s_first_data_block = new Byte(0x0 + 20);
                s_log_block_size = new Byte(0x0 + 24, BitConverter.GetBytes(0));
                s_log_frag_size = new Byte(0x0 + 28, BitConverter.GetBytes(0));

                s_blocks_per_group = new Byte(0x0 + 28, BitConverter.GetBytes(1));
            }
        }

        class GroupDesc{
            long offset;

            Byte bg_block_bitmap;
            Byte bg_inode_bitmap;
            Byte bg_inode_table;
            Byte bg_free_blocks_count;
            Byte bg_free_inodes_count;
            Byte bg_used_dirs_count;    
            Byte bg_pad;                       
            Byte bg_reserved;

            public GroupDesc(){
                bg_block_bitmap = new Byte(0x0);
                bg_inode_bitmap = new Byte(0x0 + 4);
                bg_inode_table = new Byte(0x0 + 8);
                bg_free_blocks_count = new Byte(0x0 + 12);
                bg_free_inodes_count = new Byte(0x0 + 14);
                bg_used_dirs_count = new Byte(0x0 + 16);
                bg_pad = new Byte(0x0 + 18);
                bg_reserved = new Byte(0x0 + 20); 

                /* 32 bytes */
            }
        }

        class Inode{
            Byte i_mode;                        /* File mode */
            Byte i_uid;                /* Low 16 bits of Owner Uid */
            Byte i_size;               /* Size in bytes */
            Byte i_atime;                        /* Access time */
            Byte i_ctime;                        /* Creation time */
            Byte i_mtime;                       /* Modification time */
            Byte i_dtime;                        /* Deletion Time */
            Byte i_gid;                /* Low 16 bits of Group Id */
            Byte i_links_count;   /* Links count */
            Byte i_blocks;                      /* Blocks count */
            Byte i_flags;              /* File flags */
            Byte osd1;                                     /* OS dependent 1 */
            Byte i_block; //[EXT2_N_BLOCKS]; /* Pointers to blocks */
            Byte i_generation;     /* File version (for NFS) */
            Byte i_file_acl;                      /* File ACL */
            Byte i_dir_acl;                      /* Directory ACL */
            Byte i_faddr;                        /* Fragment address */
            Byte l_i_frag;            /* Fragment number */
            Byte l_i_fsize;           /* Fragment size */
            Byte i_pad1;
            Byte l_i_uid_high;     /* these 2 fields    */
            Byte l_i_gid_high;     /* were reserved2[0] */
            Byte l_i_reserved2;

            public Inode(){
                Byte i_mode = new Byte(0x0);                        /* File mode */
                Byte i_uid = new Byte(0x0 + 2);                /* Low 16 bits of Owner Uid */
                Byte i_size = new Byte(0x0 + 4);               /* Size in bytes */
                Byte i_atime = new Byte(0x0 + 8);                        /* Access time */
                Byte i_ctime = new Byte(0x0 + 12);                        /* Creation time */
                Byte i_mtime = new Byte(0x0 + 16);                       /* Modification time */
                Byte i_dtime = new Byte(0x0 + 20);                        /* Deletion Time */
                Byte i_gid = new Byte(0x0 + 24);                /* Low 16 bits of Group Id */
                Byte i_links_count = new Byte(0x0 + 26);   /* Links count */
                Byte i_blocks = new Byte(0x0 + 28);                      /* Blocks count */
                Byte i_flags = new Byte(0x0 + 32);              /* File flags */
                Byte osd1 = new Byte(0x0 + 36);                                     /* OS dependent 1 */
                Byte i_block = new Byte(0x0 + 40); //[EXT2_N_BLOCKS]; /* Pointers to blocks */
                Byte i_generation = new Byte(0x0 + 44);     /* File version (for NFS) */
                Byte i_file_acl = new Byte(0x0 + 48);                      /* File ACL */
                Byte i_dir_acl = new Byte(0x0 + 52);                      /* Directory ACL */
                Byte i_faddr = new Byte(0x0 + 56);                        /* Fragment address */
                Byte l_i_frag = new Byte(0x0 + 60);            /* Fragment number */
                Byte l_i_fsize = new Byte(0x0 + 61);           /* Fragment size */
                Byte i_pad1 = new Byte(0x0 + 62);
                Byte l_i_uid_high = new Byte(0x0 + 64);     /* these 2 fields    */
                Byte l_i_gid_high = new Byte(0x0 + 66);     /* were reserved2[0] */
                Byte l_i_reserved2 = new Byte(0x0 +  68);
            }
        }

        class Block{
            long offset;
            System.Byte[] rawData;

            public Block(System.Byte[] data, long offset_){
                rawData = data;
                offset = offset_;
            }

            public void Write(Stream file){
                file.Seek(offset, SeekOrigin.Begin);
                file.Write(rawData);
            }
        }

        class File{
            Byte inode; /* Inode number */
            Byte rec_len;/* Directory entry length */
            Byte name_len;/* Name length */
            Byte name;//[EXT2_NAME_LEN]; /* File name */

            public File(int inodeNum, int size, int nameSize, string name_){
                inode = new Byte(0x0, BitConverter.GetBytes(inodeNum));             /* Inode number */
                rec_len = new Byte(0x0 + 4, BitConverter.GetBytes(size));             /* Directory entry length */
                name_len = new Byte(0x0 + 6, BitConverter.GetBytes(nameSize));        /* Name length */
                name_len = new Byte(0x0 + 7, BitConverter.GetBytes(1));
                name = new Byte(0x0 + 8, name_); //[EXT2_NAME_LEN]; /* File name */
            }
        }

        List<int> blockMap = new List<int>();
        List<int> inodeMap = new List<int>();

        List<Block> blocks = new List<Block>();
        List<Inode> inodes = new List<Inode>();

        int blockSize = 1024;

        public Ext2(){

        }

        public void createImage(){

        }

        public void readAbstract(Image img){
            List<Image.File> imgFiles = img.GetFiles();
            
            long offset = 0x2600 + 32;
            long bytesPerItem = 32;
            long offsetCluster = 0x2800;

            foreach(var item in imgFiles){
                System.Byte[] rawData;

                if(!item.Dir()){
                    rawData = item.GetByte();
                }else{
                    rawData = new System.Byte[0];
                }

                Inode inodeFile = new Inode();
                File file = new File(0, rawData.Length, item.GetName().Length, item.GetName());

                int size = rawData.Length;
                int clusterCount = (int)Math.Ceiling((double)size/(double)blockSize);

                List<int> clustersId = new List<int>();

                for(int i = 0; i < clusterCount; i++){
                    blockMap.Add(1);

                    System.Byte[] rawDataSub = new System.Byte[blockSize];

                    for(int j = blockSize*i; j < blockSize*(i+1); j++){
                        if(j == size) break;

                        rawDataSub[j - blockSize*i] = rawData[j];
                    }

                    clustersId.Add(blocks.Count);
                    blocks.Add(new Block(rawDataSub, offsetCluster));
                    offsetCluster += blockSize;
                }
            }
        }
    }
}