using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BuildState
{
    public class CollidAnotherState : State
    {
        protected override void OnEnter()
        {
            Drawer.SetColor(1f, 0f, 0f, 0.3f);
        }

        protected override void TriggerEnter(Collider2D collision)
        {
            if (collision.tag == "Build" && collision.GetComponent<Build>().CheckCurrentState<DropedState>())
            {
                d.nbColliding++;
            }
        }

        protected override void TriggerExit(Collider2D collision)
        {
            if (collision.tag == "Build" && collision.GetComponent<Build>().CheckCurrentState<DropedState>())
            {
                d.nbColliding--;

                if (d.nbColliding == 0)
                {
                    build.ChangeState<HoverGridState>();
                }
            }
            else if (collision.tag == "BuildingGrid")
            {
                d.collidedGrid = null;
                build.ChangeState<OnDragState>();
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
            if (d.oldPos == Vector3.zero)
            {
                if (d.materialData.material == BuildMaterialData.Material.PROT)
                {
                    //Debug.Log($"CollidAnotherState EndDrag for id {d.kingId}");

                    Tiling.placed[GameManager.Instance.currentPlayerIndex][d.kingId + 1] = false;
                    GameManager.Instance.CurrentBuildsInfo.kingComponents[GameManager.Instance.currentPlayerIndex][d.kingId].m_active = false;
                    GameManager.Instance.CurrentBuildsInfo.kingComponents[GameManager.Instance.currentPlayerIndex][d.kingId].m_waitingForActivation = false;
                }
                d.slot.ReturnBuildOnSlot();
                Object.Destroy(gameObject);
            }
            else
            {
                transform.position = d.oldPos;
            }
        }
    }
}
