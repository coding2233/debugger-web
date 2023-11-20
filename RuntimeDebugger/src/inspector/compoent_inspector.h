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
    std::string FullName;
    std::string ValueType;
    std::string ReflectionType;
    bool CanWrite;
    ReflectionValue Value;
    bool DrawReflectionValue()
    {
        bool result = false;
        ImGui::BeginDisabled(!CanWrite);
        result = Value.DrawValue(Name.c_str());
        ImGui::EndDisabled();
        return result;
    }
    json ToJson()
    {
        return Value.ToJson();
    }
    NLOHMANN_DEFINE_TYPE_INTRUSIVE(ReflectionInspector,Name,FullName,ValueType,ReflectionType,CanWrite);
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
    bool IsMonoBehaviour;
    std::vector<ReflectionInspector> ReflectionValues;
    std::map<std::string, std::vector<std::vector<std::string>>> MapMaterialValues;
    NLOHMANN_DEFINE_TYPE_INTRUSIVE(CompoentInspector,InstanceID,Name,Enable,IsMonoBehaviour,ReflectionValues,MapMaterialValues);
};

#endif //RUNTIMEDEBUGGER_INSPECTOR_COMPOENT_H
