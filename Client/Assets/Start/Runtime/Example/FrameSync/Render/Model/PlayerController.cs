using Start;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }
    
    
    public void OnDestroy()
    {
        Instance = null;
    }

    public void RenderUpdate(MatchEntity matchEntity,PlayerEntity playerEntity)
    {
        TSVector position = playerEntity.Transform.Position;
        transform.position = new Vector3(position.X.AsFloat(), position.Y.AsFloat(), position.Z.AsFloat());
    }
}
