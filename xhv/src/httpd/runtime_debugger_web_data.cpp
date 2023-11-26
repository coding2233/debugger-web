//
// Created by wanderer on 2023/11/25.
//

#include "runtime_debugger_web_data.h"


RuntimeDebuggerWebData::RuntimeDebuggerWebData()
{}

RuntimeDebuggerWebData::~RuntimeDebuggerWebData()
{}

int RuntimeDebuggerWebData::ExtractToMap(const char* data,size_t size)
{
    std::map<std::string,std::string>& out_files = extracted_files_;
    out_files.clear();

    // 创建一个ZIP读取器
    mz_zip_archive zip_archive;
    memset(&zip_archive, 0, sizeof(zip_archive));

    mz_bool status = mz_zip_reader_init_mem(&zip_archive, data, size, 0);
    if (!status) {
        std::cerr << "Failed to initialize zip reader!" << std::endl;
        return -1;
    }

    // 遍历ZIP文件中的所有文件和目录
    for (int i = 0; i < (int)mz_zip_reader_get_num_files(&zip_archive); i++) {
        mz_zip_archive_file_stat file_stat;
        if (!mz_zip_reader_file_stat(&zip_archive, i, &file_stat)) {
            std::cerr << "Failed to get file stat for file " << i << std::endl;
            mz_zip_reader_end(&zip_archive);
            return -1;
        }

        // 只处理文件，跳过目录
        if (!file_stat.m_is_directory) {
            std::cout << "Extracting " << file_stat.m_filename << std::endl;

            // 解压文件到内存
            size_t uncompressed_size = (size_t)file_stat.m_uncomp_size;
            void *p = mz_zip_reader_extract_file_to_heap(&zip_archive, file_stat.m_filename, &uncompressed_size, 0);
            if (!p) {
                std::cerr << "Failed to extract file " << i << std::endl;
                mz_zip_reader_end(&zip_archive);
                return -1;
            }

            // 将解压缩的数据转换为std::string，并存储在map中
            out_files[file_stat.m_filename] = std::string((const char*)p, uncompressed_size);

            // 释放提取的文件数据
            mz_free(p);
        }
    }

    // 关闭ZIP读取器
    mz_zip_reader_end(&zip_archive);

    return 0;
}

std::string RuntimeDebuggerWebData::GetFileData(std::string file_name)
{
    std::string file_data;
    std::map<std::string,std::string>& extracted_files = extracted_files_;
    auto find_iter = extracted_files.find(file_name);
    if (find_iter != extracted_files.end())
    {
        file_data =find_iter->second;
    }
    return file_data;
}