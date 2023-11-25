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
    // 创建一个zip源，从内存中读取数据
    zip_error_t error;
    zip_error_init(&error);
    zip_source_t *source = zip_source_buffer_create(data, size, 0, &error);
    if (source == NULL) {
        std::cerr << "Failed to create zip source from buffer: " << zip_error_strerror(&error) << std::endl;
        zip_error_fini(&error);
        return -1;
    }

    // 从zip源创建zip对象
    zip_t *za = zip_open_from_source(source, 0, &error);
    if (za == NULL) {
        std::cerr << "Failed to open zip from source: " << zip_error_strerror(&error) << std::endl;
        zip_source_free(source);
        zip_error_fini(&error);
        return -1;
    }
    zip_error_fini(&error);

    // 遍历zip条目
    zip_int64_t num_entries = zip_get_num_entries(za, 0);
    for (zip_uint64_t i = 0; i < num_entries; i++) {
        // 获取当前条目的名称
        const char *name = zip_get_name(za, i, 0);
        if (name == NULL) {
            std::cerr << "Failed to get name for index " << i << std::endl;
            continue;
        }

        // 打开zip条目
        zip_file_t *zf = zip_fopen_index(za, i, 0);
        if (!zf) {
            std::cerr << "Failed to open entry " << name << std::endl;
            continue;
        }

        // 读取zip条目数据
        std::string file_contents;
        char buffer[4096];
        zip_int64_t bytes_read;
        while ((bytes_read = zip_fread(zf, buffer, sizeof(buffer))) > 0) {
            file_contents.append(buffer, bytes_read);
        }

        // 关闭zip条目
        zip_fclose(zf);

        if (bytes_read < 0) {
            std::cerr << "Failed to read entry " << name << std::endl;
            continue;
        }

        // 将文件数据存储到map中
        out_files[std::string(name)] = file_contents;
    }

    // 关闭zip对象
    zip_close(za);

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