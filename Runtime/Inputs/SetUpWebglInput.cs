using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpWebglInput : MonoBehaviour
{
    void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
                  WebGLInput.captureAllKeyboardInput = false;
#endif  

    }

}
