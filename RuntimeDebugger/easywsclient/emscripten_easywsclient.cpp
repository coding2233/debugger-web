#include <vector>
#include <string>
#include <emscripten.h>
#include <emscripten/websocket.h>

#include "easywsclient.hpp"

using easywsclient::Callback_Imp;
using easywsclient::BytesCallback_Imp;


namespace 
{
    class _EMScriptenWebSocket : public easywsclient::WebSocket
    {
        public:
            _EMScriptenWebSocket(const std::string& url);
            EMSCRIPTEN_WEBSOCKET_T websocket_;
        private:
            std::vector<std::vector<uint8_t>> received_buf_;
            bool received_buf_lock_;
            bool deal_buf_lock_;
            easywsclient::WebSocket::readyStateValues ready_state_;
        public:
            void poll(int timeout);
            void send(const std::string& message) ;
            void sendBinary(const std::string& message) ;
            void sendBinary(const std::vector<uint8_t>& message);
            void sendPing() ;
            void close();
            easywsclient::WebSocket::readyStateValues getReadyState() const;
            void _dispatch(Callback_Imp & callable);
            void _dispatchBinary(BytesCallback_Imp& callable);

            void AddReceiveBuf(uint8_t *data,uint32_t numBytes);
    };

    EM_BOOL onopen(int eventType, const EmscriptenWebSocketOpenEvent* websocketEvent, void* userData) 
    {
        puts("onopen");
        if(userData)
        {
            _EMScriptenWebSocket* ems_websocket = (_EMScriptenWebSocket*)userData;
            if(ems_websocket)
            {
                ems_websocket->websocket_ =  websocketEvent->socket;
            }
        }
        return EM_TRUE;
    }
    EM_BOOL onerror(int eventType, const EmscriptenWebSocketErrorEvent* websocketEvent, void* userData)
    {
        printf("error(eventType=%d, userData=%ld)\n", eventType, (long)userData);
        return EM_TRUE;
    }
    EM_BOOL onclose(int eventType, const EmscriptenWebSocketCloseEvent* websocketEvent, void* userData)
    {
        puts("onclose");
        return EM_TRUE;
    }
    EM_BOOL onmessage(int eventType, const EmscriptenWebSocketMessageEvent* websocketEvent, void* userData)
    {
        // puts("onmessage");
        if(userData)
        {
            _EMScriptenWebSocket* ems_websocket = (_EMScriptenWebSocket*)userData;
            if(ems_websocket)
            {
                ems_websocket->AddReceiveBuf(websocketEvent->data,websocketEvent->numBytes);
            }
        }
        return EM_TRUE;
    }

    _EMScriptenWebSocket::_EMScriptenWebSocket(const std::string& url)
    :ready_state_(OPEN)
    {
        if (!emscripten_websocket_is_supported()) 
        {
            printf("emscripten_websocket_is_supported error");
            return;
        }
        EmscriptenWebSocketCreateAttributes ws_attrs = 
        {
            url.c_str(),
            NULL,
            EM_TRUE
        };

        EMSCRIPTEN_WEBSOCKET_T ws = emscripten_websocket_new(&ws_attrs);
        emscripten_websocket_set_onopen_callback(ws, this, onopen);
        emscripten_websocket_set_onerror_callback(ws, this, onerror);
        emscripten_websocket_set_onclose_callback(ws, this, onclose);
        emscripten_websocket_set_onmessage_callback(ws, this, onmessage);
    }

    easywsclient::WebSocket::readyStateValues _EMScriptenWebSocket::getReadyState() const
    {
        return  ready_state_;
    }

    void _EMScriptenWebSocket::poll(int timeout) { }
    void _EMScriptenWebSocket::send(const std::string& message) 
    { 
        EMSCRIPTEN_RESULT result;
        result = emscripten_websocket_send_utf8_text(websocket_,message.c_str());
    }
    void _EMScriptenWebSocket::sendBinary(const std::string& message) 
    {
        EMSCRIPTEN_RESULT result;
        result = emscripten_websocket_send_utf8_text(websocket_,message.c_str());
    }
    void _EMScriptenWebSocket::sendBinary(const std::vector<uint8_t>& message)
    {
        emscripten_websocket_send_binary(websocket_, (uint8_t*)message.data(), message.size());
    }
    void _EMScriptenWebSocket::sendPing() { }
    void _EMScriptenWebSocket::close() 
    {
        ready_state_ = CLOSED;
        EMSCRIPTEN_RESULT result;
        result = emscripten_websocket_close(websocket_, 1000, "no reason");
        if (result) 
        {
            printf("Failed to emscripten_websocket_close(): %d\n", result);
        }
    }
    void _EMScriptenWebSocket::_dispatch(Callback_Imp & callable) 
    {
        struct CallbackAdapter : public BytesCallback_Imp
            // Adapt void(const std::string<uint8_t>&) to void(const std::string&)
        {
            Callback_Imp& callable;
            CallbackAdapter(Callback_Imp& callable) : callable(callable) { }
            void operator()(const std::vector<uint8_t>& message) {
                std::string stringMessage(message.begin(), message.end());
                callable(stringMessage);
            }
        };
        CallbackAdapter bytesCallback(callable);
        _dispatchBinary(bytesCallback);
    }
    void _EMScriptenWebSocket::_dispatchBinary(BytesCallback_Imp& callable) 
    {
         if(received_buf_lock_)
        {
            return;
        }

        deal_buf_lock_ = true;
        for(int i = 0;i< received_buf_.size();i++)
        {
            std::vector<uint8_t> buffer = received_buf_[i];
            callable((const std::vector<uint8_t>)buffer);

            buffer.erase(buffer.begin(), buffer.end());
            std::vector<uint8_t>().swap(buffer);// free memory
        }
        received_buf_.erase(received_buf_.begin(), received_buf_.end());
        std::vector<std::vector<uint8_t>>().swap(received_buf_);
        deal_buf_lock_ = false;
    }

    void _EMScriptenWebSocket::AddReceiveBuf(uint8_t *data,uint32_t numBytes)
    {
        while(deal_buf_lock_)
        {
        }

        received_buf_lock_ = true;
        if(numBytes > 0)
        {
            std::vector<uint8_t> buffer;
            buffer.insert(buffer.end(),data,data+numBytes);
            received_buf_.push_back(buffer);
        }
        received_buf_lock_ = false;
    }

    easywsclient::WebSocket::pointer from_url(const std::string& url) 
    {
        return easywsclient::WebSocket::pointer(new _EMScriptenWebSocket(url));
    }
}

namespace easywsclient 
{
    WebSocket::pointer WebSocket::from_url(const std::string& url, const std::string& origin) 
    {
        return ::from_url(url);
    }

    WebSocket::pointer WebSocket::from_url_no_mask(const std::string& url, const std::string& origin) 
    {
        return ::from_url(url);
    }
}
