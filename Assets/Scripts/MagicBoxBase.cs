using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class MagicBoxBase : NetworkBehaviour{

    public enum BoxType
    {
        none = 0,
        moveSpeed = 1,
        shootSpeed = 2,
        radar = 3
    }

    public BoxType buffType = BoxType.none;
}
