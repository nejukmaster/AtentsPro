using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    [ExecuteAlways]
    public class ObjectPreprocess : MonoBehaviour
    {
        static int vertexAveragePosId = Shader.PropertyToID("_VertexAveragePos");

        static MaterialPropertyBlock block;

        [SerializeField] Transform shadingCenterPos;

        private void Update()
        {
            if (block == null)
            {
                block = new MaterialPropertyBlock();
            }

            if(shadingCenterPos != null)
            {
                block.SetVector(vertexAveragePosId, shadingCenterPos.position);
            }
            GetComponent<Renderer>().SetPropertyBlock(block);
        }
    }
}
