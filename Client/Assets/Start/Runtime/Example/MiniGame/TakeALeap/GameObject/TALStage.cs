using DG.Tweening;
using UnityEngine;

namespace Start
{
    public class TALStage : MonoBehaviour
    {
        public void PlayAnimation()
        {
            transform.localScale += new Vector3(0, -1, 0) * 0.15f * Time.deltaTime;
            transform.localPosition += new Vector3(0, -1, 0) * 0.15f * Time.deltaTime;
        }
        
        public void ResetAnimation()
        {
            //还原小人的形状
            transform.DOLocalMoveY(-0.25f, 0.2f);
            transform.DOScaleY(0.5f, 0.2f);
        }
    }
    
}