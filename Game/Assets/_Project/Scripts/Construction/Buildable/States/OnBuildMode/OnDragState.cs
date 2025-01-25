using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BuildState
{
    public class OnDragState : State
    {
        protected override void OnEnter()
        {
            Drawer.SetDefaultColor();
            Drawer.SetColorAlpha(0.5f);
            Drawer.SetOrderLayer("BuildOnDrag");
            d.nbColliding = 0;
        }

        protected override void TriggerEnter(Collider2D collision)
        {
            if (collision.tag == "Build" && collision.GetComponent<Build>().CheckCurrentState<DropedState>())
            {
                d.nbColliding++;
            }
            else if (collision.tag == "BuildingGrid")
            {
                d.collidedGrid = collision.GetComponent<BuildingGrid>();
            }
        }

        protected override void TriggerExit(Collider2D collision)
        {
            if (collision.tag == "Build" && collision.GetComponent<Build>().CheckCurrentState<DropedState>())
            {
                d.nbColliding--;
            }
            else if (collision.tag == "BuildingGrid")
            {
                d.collidedGrid = null;
            }
        }

        protected override void OnDrag(Vector3 dragPos)
        {
            transform.position = dragPos;

            if (d.collidedGrid != null)
            {
                Bounds bounds = build.GetComponent<CompositeCollider2D>().bounds;
                if (d.collidedGrid.CheckIfIsOnGrid(bounds))
                {
                    if (d.nbColliding == 0)
                    {
                        build.ChangeState<HoverGridState>();
                    }
                    else
                    {
                        build.ChangeState<CollidAnotherState>();
                    }
                }
            }
        }

        protected override void OnEndDrag()
        {
            if (d.materialData.material == BuildMaterialData.Material.PROT)
            {//Void drop
                Debug.Log($"OnDragState EndDrag for id {d.kingId}");
                Tiling.placed[GameManager.Instance.currentPlayerIndex][d.kingId + 1] = false;
                GameManager.Instance.CurrentBuildsInfo.kingComponents[GameManager.Instance.currentPlayerIndex][d.kingId].m_active = false;
                GameManager.Instance.CurrentBuildsInfo.kingComponents[GameManager.Instance.currentPlayerIndex][d.kingId].m_waitingForActivation = false;
            }
            //build.ChangeState<OnWaitState>();
            d.slot.ReturnBuildOnSlot();
            Object.Destroy(gameObject);
        }
    }
}
