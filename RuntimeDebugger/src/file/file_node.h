//
// Created by wanderer on 2023/11/25.
//

#ifndef RUNTIMEDEBUGGER_FILE_NODE_H
#define RUNTIMEDEBUGGER_FILE_NODE_H


#include <string>
#include <vector>
#include <map>

#include "nlohmann/json.hpp"
using json = nlohmann::json;

#include "imgui.h"

class FileNode
{
public:
    FileNode()
    {}
    ~FileNode()
    {}

public:
    std::string Name;
    std::string FullName;
    bool IsFile ;
    std::string CreationTime ;
    std::string LastAccessTime ;
    std::string LastWriteTime ;
    bool HasError;
    std::vector<FileNode> Children ;

    NLOHMANN_DEFINE_TYPE_INTRUSIVE(FileNode,FullName,Name,IsFile,CreationTime,LastAccessTime,LastWriteTime,HasError,Children);
};

#endif //RUNTIMEDEBUGGER_FILE_NODE_H
