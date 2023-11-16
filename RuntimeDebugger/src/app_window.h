//
// Created by EDY on 2023/11/13.
//

#ifndef RUNTIMEDEBUGGER_APP_WINDOW_H
#define RUNTIMEDEBUGGER_APP_WINDOW_H

#include <string>
#include <vector>
#include <functional>

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
        show_ = false;
        show_first_time_ = true;
    }
    ~AppWindow()
    {}
protected:
    std::string name_;
    bool show_;
    bool show_first_time_;
    std::function<void(uint8_t,const std::string &message)> websocket_send_callback_;
    uint8_t key_;
public:
    std::string GetName() const
    {
        return name_;
    }

    bool GetShow()
    {
        return  show_;
    }

    virtual void OnShow(bool show)
    {
        printf("AppWindow::OnShow\n");
    }

    void SetShow(bool show)
    {
        show_ = show;
        this->OnShow(show);
        if (show_ && show_first_time_)
        {
            show_first_time_ = false;
        }
    }

    void BindSend( std::function<void(uint8_t,const std::string &message)> websocket_send_callback,uint8_t key)
    {
        websocket_send_callback_ = websocket_send_callback;
        key_ = key;
    }

    void DrawWindow()
    {
        if (show_)
        {
            if(ImGui::Begin(name_.c_str(), &show_))
            {
                OnDraw();
            }
            ImGui::End();

            if(!show_)
            {
                OnShow(false);
            }
        }
    }

    virtual void OnMessage(const std::string &message)
    {
    }


    virtual void Send(const std::string & message)
    {
        if (websocket_send_callback_)
        {
            websocket_send_callback_(key_,message);
        }
    }

    virtual void OnDraw()
    {

    }
};

#endif //RUNTIMEDEBUGGER_APP_WINDOW_H
