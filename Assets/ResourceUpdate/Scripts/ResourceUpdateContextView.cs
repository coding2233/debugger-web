using strange.extensions.context.api;
using strange.extensions.context.impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

namespace Wanderer
{
    public class ResourceUpdateContextView : ContextView
    {
        private void Start()
        {
            context = new ResourceUpdateContext(this, ContextStartupFlags.MANUAL_LAUNCH);
            context.Launch();
        }
    }
}