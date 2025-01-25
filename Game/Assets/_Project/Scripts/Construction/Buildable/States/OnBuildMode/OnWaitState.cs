using UnityEngine;

namespace BuildState
{
    //DISABLED
    public class OnWaitState : State
    {
        protected override void OnEnter()
        {
            d.oldPos = build.transform.position;
            d.oldRotation = build.transform.rotation;
        }

        protected override void OnBeginDrag()
        {
            build.ChangeState<OnDragState>();
        }

        protected override void OnMouseButtonUp()
        {
            Drawer.TurnQuarterDegreeClockwise();
        }
    }
}
