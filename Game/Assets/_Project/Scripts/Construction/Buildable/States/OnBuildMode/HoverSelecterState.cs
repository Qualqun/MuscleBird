using UnityEngine;
using UnityEngine.EventSystems;

namespace BuildState
{
    public class HoverSelecterState : State
    {
        protected override void OnEnter()
        {
            Drawer.SetColor(0f, 0f, 1f, 0.5f);
        }

        protected override void OnDrag(Vector3 dragPos)
        {
            build.transform.position = dragPos;
        }

        protected override void OnEndDrag()
        {
            if (d.materialData.material == BuildMaterialData.Material.PROT)
            {
                //Debug.Log($"HoverSleceter EndDrag for id {d.kingId}");
                Tiling.placed[GameManager.Instance.currentPlayerIndex][d.kingId + 1] = false;
                GameManager.Instance.CurrentBuildsInfo.kingComponents[GameManager.Instance.currentPlayerIndex][d.kingId].m_active = false;
                GameManager.Instance.CurrentBuildsInfo.kingComponents[GameManager.Instance.currentPlayerIndex][d.kingId].m_waitingForActivation = false;
            }
            d.slot.ReturnBuildOnSlot();
            Object.Destroy(gameObject);
        }
    }
}

