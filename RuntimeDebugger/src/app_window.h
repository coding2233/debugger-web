//
// Created by EDY on 2023/11/13.
//

#ifndef RUNTIMEDEBUGGER_APP_WINDOW_H
#define RUNTIMEDEBUGGER_APP_WINDOW_H

#include <string>

#include "easywsclient.hpp"
using easywsclient::WebSocket;
#include "nlohmann/json.hpp"
using json = nlohmann::json;

class AppWindow
{
protected:
    bool show_;
    std::unique_ptr<WebSocket> ws_;
public:
    AppWindow()
    {}
    ~AppWindow()
    {}
    virtual bool CheckConnect()
    {
        if(ws_ && ws_->getReadyState() != WebSocket::CLOSED)
        {
            return true;
        }
        return false;
    }

    virtual bool Connect(std::string server_url)
    {
        if(CheckConnect())
        {
            ws_->close();
        }
        ws_ = std::unique_ptr<WebSocket>(WebSocket::from_url(server_url.c_str()));

        return  ws_ != NULL;
    }

    virtual bool OnDraw()
    {
        return CheckConnect();
    }
};

#endif //RUNTIMEDEBUGGER_APP_WINDOW_H
