using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.injector.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer
{
    public class ResourceUpdateContext : MVCSContext
    {
        public ResourceUpdateContext(ContextView contextView, ContextStartupFlags flags) : base(contextView, flags)
        {
            Log.Info("ResourceUpdateContext");
        }

        protected override void mapBindings()
        {
            base.mapBindings();

            commandBinder.Bind(ContextEvent.START).To<ResourceUpdateStarCommand>().Once();
        }

        public override void Launch()
        {
            base.Launch();
        }

    }

}