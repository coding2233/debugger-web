using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer
{
    [System.AttributeUsage(AttributeTargets.Class)]
    public class DebuggerWindowAttribute : Attribute
    {
        public string Title { get; private set; }

        public int Priority { get; private set; }

        public DebuggerWindowAttribute(string title, int priority = 0)
        {
            Title = title;
            Priority = priority;
        }
    }

}
