//
// Created by EDY on 2023/11/17.
//

#ifndef RUNTIMEDEBUGGER_REFLECTION_VALUE_H
#define RUNTIMEDEBUGGER_REFLECTION_VALUE_H

#include <vector>
#include <string>
#include <functional>
#include <iostream>
#include <exception>

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
    std::function<bool(const char *value_name)> draw_value_callback_;
    std::function<json()> to_json_callback_;
public:
    bool ReflectionBoolean;
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
        if (value_json.empty())
        {
            printf("value_json.empty %s",value_type.c_str());
            return;
        }

        printf("value_type: %s value_json: %s\n",value_type.c_str(),value_json.dump().c_str());
        if (value_type == "Boolean")
        {
            ReflectionBoolean = value_json;
            draw_value_callback_ = [this](const char *value_name){
                return ImGui::Checkbox(value_name,&ReflectionBoolean);
            };
            to_json_callback_=[this](){
                return ReflectionBoolean;
            };
        }
        else if(value_type=="String"|| value_type=="Texture")
        {
            ReflectionString = value_json;
            draw_value_callback_ = [this](const char *value_name){
                return ImGui::InputText(value_name,ReflectionString.data(),256);
            };
            to_json_callback_=[this](){
                return ReflectionString;
            };
        }
        else if(value_type=="Int32"||value_type=="Int")
        {
            ReflectionInt32 = value_json;
            draw_value_callback_ =[this](const char *value_name){
                return ImGui::DragInt(value_name,&ReflectionInt32,1);
            };
            to_json_callback_=[this](){
                return ReflectionInt32;
            };
        }
        else if(value_type=="UInt32")
        {
            ReflectionUInt32 = value_json;
            draw_value_callback_ = [this,value_type](const char *value_name){
                ImGui::Text("[unsupported] %s %s %u",value_name,value_type.c_str(),ReflectionUInt32);
                return false;
            };
            to_json_callback_=[this](){
                return ReflectionUInt32;
            };
        }
        else if(value_type=="Int64")
        {
            ReflectionInt64 = value_json;
            draw_value_callback_ = [this,value_type](const char *value_name){
                ImGui::Text("[unsupported] %s %s %ld",value_name,value_type.c_str(),ReflectionInt64);
                return false;
            };
            to_json_callback_=[this](){
                return ReflectionInt64;
            };
        }
        else if(value_type=="UInt64")
        {
            ReflectionUInt64 = value_json;
            draw_value_callback_ = [this,value_type](const char *value_name){
                ImGui::Text("[unsupported] %s %s %llu",value_name,value_type.c_str(),ReflectionUInt64);
                return false;
            };
            to_json_callback_=[this](){
                return ReflectionUInt64;
            };
        }
        else if(value_type=="Single" || value_type=="Float" || value_type=="Range")
        {
            ReflectionSingle = value_json;
            draw_value_callback_ = [this](const char *value_name){
                return ImGui::DragFloat(value_name,&ReflectionSingle,0.1f);
            };
            to_json_callback_=[this](){
                return ReflectionSingle;
            };
        }
        else if(value_type=="Double")
        {
            ReflectionDouble = value_json;
            draw_value_callback_ = [this](const char *value_name){
                return ImGui::InputDouble(value_name,&ReflectionDouble);
            };
            to_json_callback_=[this](){
                return ReflectionDouble;
            };
        }
        else if(value_type=="Vector2")
        {
            ReflectionVector[0] = value_json["x"].template get<float>();
            ReflectionVector[1] = value_json["y"].template get<float>();
            draw_value_callback_ = [this](const char *value_name){
                return ImGui::DragFloat2(value_name,ReflectionVector,0.1f);
            };
            to_json_callback_=[this](){
                std::map<std::string,float> vector_to_json = {{"x",ReflectionVector[0]},{"y",ReflectionVector[1]}};
                return vector_to_json;
            };
        }
        else if(value_type=="Vector3")
        {
            ReflectionVector[0] = value_json["x"].template get<float>();
            ReflectionVector[1] = value_json["y"].template get<float>();
            ReflectionVector[2] = value_json["z"].template get<float>();
            draw_value_callback_ = [this](const char *value_name){
                return ImGui::DragFloat3(value_name,ReflectionVector,0.1f);
            };
            to_json_callback_=[this](){
                std::map<std::string,float> vector_to_json = {{"x",ReflectionVector[0]},{"y",ReflectionVector[1]},{"z",ReflectionVector[2]}};
                return vector_to_json;
            };
        }
        else if(value_type=="Vector4" || value_type=="Quaternion" || value_type=="Vector")
        {
            ReflectionVector[0] = value_json["x"].template get<float>();
            ReflectionVector[1] = value_json["y"].template get<float>();
            ReflectionVector[2] = value_json["z"].template get<float>();
            ReflectionVector[3] = value_json["w"].template get<float>();
            draw_value_callback_ = [this](const char *value_name){
                return ImGui::DragFloat4(value_name,ReflectionVector,0.1f);
            };
            to_json_callback_=[this](){
                std::map<std::string,float> vector_to_json = {{"x",ReflectionVector[0]},{"y",ReflectionVector[1]},{"z",ReflectionVector[2]},{"w",ReflectionVector[3]}};
                return vector_to_json;
            };
        }
        //color float类型 0-1
        //Color32 byte类型 0-255
        else if(value_type=="Color" || value_type=="Color32")
        {
            ReflectionVector[0] = value_json["r"].template get<float>();
            ReflectionVector[1] = value_json["g"].template get<float>();
            ReflectionVector[2] = value_json["b"].template get<float>();
            ReflectionVector[3] = value_json["a"].template get<float>();
            draw_value_callback_ = [this](const char *value_name){
                return ImGui::ColorEdit4(value_name,ReflectionVector);
            };
            to_json_callback_=[this](){
                std::map<std::string,float> vector_to_json = {{"r",ReflectionVector[0]},{"g",ReflectionVector[1]},{"b",ReflectionVector[2]},{"a",ReflectionVector[3]}};
                return vector_to_json;
            };
        }
        else if(value_type=="Vector2Int")
        {
            ReflectionVectorInt[0] = value_json["x"].template get<int>();
            ReflectionVectorInt[1] = value_json["y"].template get<int>();
            draw_value_callback_ = [this](const char *value_name){
                return ImGui::DragInt2(value_name,ReflectionVectorInt,1);
            };
            to_json_callback_=[this](){
                std::map<std::string,float> vector_to_json = {{"x",ReflectionVectorInt[0]},{"y",ReflectionVectorInt[1]}};
                return vector_to_json;
            };
        }
        else if(value_type=="Vector3Int")
        {
            ReflectionVectorInt[0] = value_json["x"].template get<int>();
            ReflectionVectorInt[1] = value_json["y"].template get<int>();
            ReflectionVectorInt[2] = value_json["z"].template get<int>();
            draw_value_callback_ = [this](const char *value_name){
                return ImGui::DragInt3(value_name,ReflectionVectorInt);
            };
            to_json_callback_=[this](){
                std::map<std::string,float> vector_to_json = {{"x",ReflectionVectorInt[0]},{"y",ReflectionVectorInt[1]},{"z",ReflectionVectorInt[2]}};
                return vector_to_json;
            };
        }
        else
        {
            draw_value_callback_ = [this,value_type](const char *value_name) {
                ImGui::Text("[unsupported] %s %s",value_name,value_type.c_str());
                return false;
            };
            to_json_callback_=[this](){
                return "";
            };
        }
    }

    json ToJson()
    {
        try
        {
            if(to_json_callback_)
            {
                return to_json_callback_();
            }
        }
        catch(std::exception& e)
        {
            std::cout << "Standard exception: " << e.what() << std::endl;
        }

        return NULL;
    }

    bool DrawValue(const char* value_name)
    {
        if (draw_value_callback_)
        {
            return draw_value_callback_(value_name);
        }
        return false;
    }
};



#endif //RUNTIMEDEBUGGER_REFLECTION_VALUE_H
