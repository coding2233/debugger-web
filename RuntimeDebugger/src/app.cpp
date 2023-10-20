//
// Created by EDY on 2023/9/9.
//

#include "app.h"

App::App():ImplApp("",1280,800,0)
{
}

App::~App()
{
}


void App::OnImGuiDraw()
{
     static bool sss= true;
     if(ImGui::Begin("aaaa",&sss))
     {
         if(ImGui::Button("close"))
         {
         }
         ImGui::End();
     }

}


