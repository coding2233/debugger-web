//
// Created by EDY on 2023/9/26.
//

#include "command_view.h"


CommandView::CommandView()
{
    text_editor_ = new TextEditor();
    text_editor_->SetReadOnly(true);
//    std::thread aa(&App::RunProgress,this);
//    aa.detach();
}

CommandView::~CommandView()
{
    if(text_editor_)
    {
        delete(text_editor_);
    }
}

void CommandView::Run(const char *command)
{
    FILE* fp;
    if( (fp = popen(command, "r")) != NULL)
    {
        memset(buf, 0, 1024);
        while (fgets(buf, 1024, fp) != NULL)
        {
            text_editor_->SetReadOnly(false);
            text_editor_->MoveBottom(false);
            text_editor_->InsertText(buf);
            text_editor_->SetReadOnly(true);
        }
        pclose(fp);
        fp = NULL;
    }
}

void CommandView::Render()
{
    text_editor_->Render("Command ImGui View",ImVec2(ImGui::GetWindowWidth(),ImGui::GetWindowHeight()));
}
