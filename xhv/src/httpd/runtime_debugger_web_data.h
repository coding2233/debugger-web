//
// Created by wanderer on 2023/11/25.
//

#ifndef XHV_RUNTIME_DEBUGGER_WEB_DATA_H
#define XHV_RUNTIME_DEBUGGER_WEB_DATA_H

#include <map>
#include <string>
#include <iostream>

#include "zip.h"

class RuntimeDebuggerWebData {

public:
    RuntimeDebuggerWebData();
    ~RuntimeDebuggerWebData();

private:
    std::map<std::string,std::string> extracted_files_;
public:
    int ExtractToMap(const char* data,size_t size);

    std::string GetFileData(std::string file_name);
};


#endif //XHV_RUNTIME_DEBUGGER_WEB_DATA_H
