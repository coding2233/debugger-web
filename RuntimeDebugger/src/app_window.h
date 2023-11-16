//
// Created by EDY on 2023/11/13.
//

#ifndef RUNTIMEDEBUGGER_APP_WINDOW_H
#define RUNTIMEDEBUGGER_APP_WINDOW_H

#include <string>
#include <vector>

#include "imgui.h"
#include "easywsclient.hpp"
using easywsclient::WebSocket;
#include "nlohmann/json.hpp"
using json = nlohmann::json;


class AppWindow
{
//    const std::unique_ptr<WebSocket> ws_;
public:
    AppWindow()
    {
        show_ = true;
    }
    ~AppWindow()
    {}
protected:
    std::string name_;
    bool show_;
public:
    std::string GetName() const
    {
        return name_;
    }

    bool  GetShow()
    {
        return  show_;
    }

    void SetShow(bool  show)
    {
        show_ = show;
    }

    void DrawWindow()
    {
        if (show_)
        {
            ImGui::Begin(name_.c_str(), &show_);
            ImGui::End();
        }
    }

    virtual void OnMessage(const std::string &message)
    {
    }


    virtual void Send(const std::string & message)
    {
//        ws_->send(message);
//        if(CheckConnect())
//        {
//            ws_->send(message);
//        }
    }

    virtual bool OnDraw()
    {
//        return CheckConnect();
        return  true;
    }
};

#endif //RUNTIMEDEBUGGER_APP_WINDOW_H
