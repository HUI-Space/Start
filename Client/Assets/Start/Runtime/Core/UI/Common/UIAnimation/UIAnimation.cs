

using UnityEngine;

namespace Start
{
    [RequireComponent(typeof(Animator))]
    public class UIAnimation : UIAnimationBase
    {
        public UIAnimationClip Open;
        public UIAnimationClip Close;
        public UIAnimationClip Loop;
        
        public override void Initialize()
        {
            Open.ActionType = UIActionTypes.Open;
            
            Close.ActionType = UIActionTypes.Close;
            Close.Await = true;
            
            Loop.ActionType = "Loop";
            Loop.Loop = true;
            Loop.Await = false;
            
            UIAnimationClip = new UIAnimationClip[3];
            UIAnimationClip[0] = Open;
            UIAnimationClip[1] = Close;
            UIAnimationClip[2] = Loop;
            
            base.Initialize();
        }
    }
}