using strange.extensions.command.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer
{
    public class ResourceUpdateStarCommand : EventCommand
    {
        [Inject("UICanvasRoot")]
        public GameObject canvasRoot { get; set; }


        [Inject]
        public IAssetsHelper assetHelper { get; set; }

        public override void Execute()
        {
            Log.Info("ResourceUpdateStarCommand");

           var load = assetHelper.LoadAsset<GameObject>("Assets/ResourceUpdate/Prefab/load.prefab");
            GameObject.Instantiate(load, canvasRoot.transform);
        }
    }
}