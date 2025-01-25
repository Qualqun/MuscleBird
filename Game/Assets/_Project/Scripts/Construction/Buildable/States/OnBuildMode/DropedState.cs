using UnityEngine;

namespace BuildState
{
    public class DropedState : State
    {
        protected override void OnEnter()
        {
            Drawer.SetOrderLayer("BuildSegments");
        }

        protected override void OnBeginDrag()
        {
            build.GetBuildsStack().RemoveBuildOnStack(build);
            build.ChangeState<HoverGridState>();
        }
    }
}

