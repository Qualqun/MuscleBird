using UnityEngine;

namespace BuildState
{
    public class HoverGridState : State
    {
        protected override void OnEnter()
        {
            Drawer.SetDefaultColor();
            Drawer.SetOrderLayer("BuildOnDrag");

        }

        protected override void TriggerEnter(Collider2D collision)
        {
            if (collision.tag == "Build" && collision.GetComponent<Build>().CheckCurrentState<DropedState>())
            {
                d.nbColliding++;
                build.ChangeState<CollidAnotherState>();
            }
        }

        protected override void OnDrag(Vector3 dragPos)
        {
            Vector2Int pair = new Vector2Int(Drawer.Data.nbColumns % 2, Drawer.Data.nbRows % 2);
            build.transform.position = d.collidedGrid.GetMagnetizedPosition(dragPos, pair.x == 0, pair.y == 0);

            Bounds bounds = build.GetComponent<CompositeCollider2D>().bounds;
            if (!d.collidedGrid.CheckIfIsOnGrid(bounds))
            {
                build.ChangeState<OnDragState>();
            }
        }

        protected override void OnEndDrag()
        {
            d.oldPos = build.transform.position;
            d.oldRotation = build.transform.rotation;;
            build.GetBuildsStack().AddBuildOnStack(build);

            build.ChangeState<DropedState>();
        }
    }
}
