//
// Created by EDY on 2023/11/14.
//

#ifndef RUNTIMEDEBUGGER_INSPECTOR_COMPOENT_H
#define RUNTIMEDEBUGGER_INSPECTOR_COMPOENT_H

#include <vector>
#include <string>

#include "nlohmann/json.hpp"
using json = nlohmann::json;

#include "imgui.h"
#include "reflection_value.h"

class ReflectionInspector
{
public:
    ReflectionInspector()
    {}
    ~ReflectionInspector()
    {}
public:
    std::string Name;
    std::string ValueType;
    std::string ReflectionType;
    bool CanWrite;
    ReflectionValue Value;
    void DrawReflectionValue()
    {
        if (!CanWrite)
        {
            ImGui::BeginDisabled();
        }
        Value.DrawValue(Name.c_str());
        if (!CanWrite)
        {
            ImGui::EndDisabled();
        }
    }
    NLOHMANN_DEFINE_TYPE_INTRUSIVE(ReflectionInspector,Name,ValueType,ReflectionType,CanWrite);
};

class CompoentInspector
{
public:
    CompoentInspector()
    {}
    ~CompoentInspector()
    {}
public:
    int InstanceID;
    std::string Name;
    bool Enable;
    std::vector<ReflectionInspector> ReflectionValues;
    NLOHMANN_DEFINE_TYPE_INTRUSIVE(CompoentInspector,InstanceID,Name,Enable,ReflectionValues);
};

#endif //RUNTIMEDEBUGGER_INSPECTOR_COMPOENT_H
