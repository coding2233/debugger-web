//
// Created by EDY on 2023/11/17.
//

#ifndef RUNTIMEDEBUGGER_REFLECTION_VALUE_H
#define RUNTIMEDEBUGGER_REFLECTION_VALUE_H

#include <vector>
#include <string>
#include <functional>

#include "nlohmann/json.hpp"
using json = nlohmann::json;

#include "imgui.h"

class ReflectionValue
{
public:
    ReflectionValue()
    {}
    ~ReflectionValue()
    {}
private:
    std::function<void(const char *value_name)> draw_value_callback_;
public:
    std::string ReflectionString;
    int ReflectionInt32;
    unsigned int ReflectionUInt32;
    long ReflectionInt64;
    unsigned long ReflectionUInt64;
    float ReflectionSingle;
    double ReflectionDouble;
    float ReflectionVector[4];
    int ReflectionVectorInt[4];

    void ToData(std::string value_type,json value_json)
    {
        if(value_type=="String")
        {
            ReflectionString = value_json;
            draw_value_callback_ = [this](const char *value_name){
                ImGui::InputText(value_name,ReflectionString.data(),256);
            };
        }
        else if(value_type=="Int32")
        {
            ReflectionInt32 = value_json;
            draw_value_callback_ =[this](const char *value_name){
                ImGui::InputInt(value_name,&ReflectionInt32);
            };
        }
        else if(value_type=="UInt32")
        {
            ReflectionUInt32 = value_json;
            draw_value_callback_ = [this,value_type](const char *value_name){
                ImGui::Text("[unsupported] %s %s %u",value_name,value_type.c_str(),ReflectionUInt32);
            };
        }
        else if(value_type=="Int64")
        {
            ReflectionInt64 = value_json;
            draw_value_callback_ = [this,value_type](const char *value_name){
                ImGui::Text("[unsupported] %s %s %ld",value_name,value_type.c_str(),ReflectionInt64);
            };
        }
        else if(value_type=="UInt64")
        {
            ReflectionUInt64 = value_json;
            draw_value_callback_ = [this,value_type](const char *value_name){
                ImGui::Text("[unsupported] %s %s %llu",value_name,value_type.c_str(),ReflectionUInt64);
            };
        }
        else if(value_type=="Single")
        {
            ReflectionSingle = value_json;
            draw_value_callback_ = [this](const char *value_name){
                ImGui::InputFloat(value_name,&ReflectionSingle);
            };
        }
        else if(value_type=="Double")
        {
            ReflectionDouble = value_json;
            draw_value_callback_ = [this](const char *value_name){
                ImGui::InputDouble(value_name,&ReflectionDouble);
            };
        }
        else if(value_type=="Vector2")
        {
            ReflectionVector[0] = value_json["x"].template get<float>();
            ReflectionVector[1] = value_json["y"].template get<float>();
            draw_value_callback_ = [this](const char *value_name){
                ImGui::InputFloat2(value_name,ReflectionVector);
            };
        }
        else if(value_type=="Vector3")
        {
            ReflectionVector[0] = value_json["x"].template get<float>();
            ReflectionVector[1] = value_json["y"].template get<float>();
            ReflectionVector[2] = value_json["z"].template get<float>();
            draw_value_callback_ = [this](const char *value_name){
                ImGui::InputFloat3(value_name,ReflectionVector);
            };
        }
        else if(value_type=="Vector4" || value_type=="Quaternion")
        {
            ReflectionVector[0] = value_json["x"].template get<float>();
            ReflectionVector[1] = value_json["y"].template get<float>();
            ReflectionVector[2] = value_json["z"].template get<float>();
            ReflectionVector[3] = value_json["w"].template get<float>();
            draw_value_callback_ = [this](const char *value_name){
                ImGui::InputFloat4(value_name,ReflectionVector);
            };
        }
        //Color32 byte类型 0-255
        //color float类型 0-1
        else if(value_type=="Color" || value_type=="Color32")
        {
            ReflectionVector[0] = value_json["r"].template get<float>();
            ReflectionVector[1] = value_json["g"].template get<float>();
            ReflectionVector[2] = value_json["b"].template get<float>();
            ReflectionVector[3] = value_json["a"].template get<float>();
            draw_value_callback_ = [this](const char *value_name){
                ImGui::InputFloat4(value_name,ReflectionVector);
            };
        }
        else if(value_type=="Vector2Int")
        {
            ReflectionVectorInt[0] = value_json["x"].template get<int>();
            ReflectionVectorInt[1] = value_json["y"].template get<int>();
            draw_value_callback_ = [this](const char *value_name){
                ImGui::InputInt2(value_name,ReflectionVectorInt);
            };
        }
        else if(value_type=="Vector3Int")
        {
            ReflectionVectorInt[0] = value_json["x"].template get<int>();
            ReflectionVectorInt[1] = value_json["y"].template get<int>();
            ReflectionVectorInt[2] = value_json["z"].template get<int>();
            draw_value_callback_ = [this](const char *value_name){
                ImGui::InputInt3(value_name,ReflectionVectorInt);
            };
        }
        else
        {
            draw_value_callback_ = [this,value_type](const char *value_name) {
                ImGui::Text("[unsupported] %s %s",value_name,value_type.c_str());
            };
        }
    }

    void DrawValue(const char* value_name)
    {
        if (draw_value_callback_)
        {
            draw_value_callback_(value_name);
        }
    }
};



#endif //RUNTIMEDEBUGGER_REFLECTION_VALUE_H
