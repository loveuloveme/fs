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

            public SuperBlock(){

            }
        }


        public void readAbstract(){

        }   
    }
}