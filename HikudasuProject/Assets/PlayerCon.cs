using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    // Update is called once per frame
       public Transform LRA;
   void FixedUpdate()
    {
        // 毎フレーム腕を少し回転させる（例：Z軸に回転）
        LRA.Rotate(0, 0, 1000 * Time.deltaTime);

    }
}
