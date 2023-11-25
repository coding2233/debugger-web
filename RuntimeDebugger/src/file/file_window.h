//
// Created by wanderer on 2023/11/14.
//

#ifndef RUNTIMEDEBUGGER_FILE_WINDOW_H
#define RUNTIMEDEBUGGER_FILE_WINDOW_H

#include <map>
#include <string>

#include "app_window.h"
#include "file_node.h"

class FileWindow : public AppWindow
{
public:
    FileWindow();
    ~FileWindow();
private:
    std::map<std::string,FileNode> file_nodes_;
    std::string select_file_full_path_;
    std::map<std::string,std::string> select_file_;

    void DrawFileNode(const FileNode * file_node);
public:
    void OnDraw() override;
    void OnMessage(const std::string & message) override;
    void OnShow(bool  show) override;

};


#endif //RUNTIMEDEBUGGER_FILE_WINDOW_H
