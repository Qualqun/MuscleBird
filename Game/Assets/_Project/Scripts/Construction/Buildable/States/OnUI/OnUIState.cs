using UnityEngine;

namespace BuildState
{
    public class OnUIState : State
    {
        public void LoadNewData(BuildDrawerData drawerData, BuildMaterialData materialData)
        {
            Drawer.SetData(drawerData);

            Drawer.Sprite = materialData.sprite;
            if (materialData.alternatSprite)
            {
                Drawer.Sprite = GameManager.Instance.currentPlayerIndex == 1 ? materialData.sprite : materialData.alternatSprite;
            }

            d.materialData = materialData;

            Drawer.RefreshDisplay();
            Drawer.ChangeDefaultColor(materialData.color);

            d.currentHp = materialData.maxHp;
        }

        protected override void OnMouseButtonUp()
        {
            Drawer.TurnQuarterDegreeClockwise();
        }

        protected override void OnBeginDrag()
        {
            if (!GameManager.Instance.inEditMode) return;

           
            Build newBuild = GameManager.Instance.BuildFactory.CreateBuildOnDrag(build);
            d.slot.RemoveBuildOnSlot(1);
            if (d.materialData.material == BuildMaterialData.Material.PROT)
            {
                if (GameManager.Instance.CurrentBuildsInfo.kingComponents == null)
                {// ? Shoudln't be there but not enough time to refactor
                    GameManager.Instance.CurrentBuildsInfo.kingComponents = new KingComponent[2][];
                    for (int i = 0; i < GameManager.Instance.CurrentBuildsInfo.kingComponents.Length; i++)
                    {
                        GameManager.Instance.CurrentBuildsInfo.kingComponents[i] = new KingComponent[4];
                        for (int j = 0; j < GameManager.Instance.CurrentBuildsInfo.kingComponents[i].Length; j++)
                        {
                            GameManager.Instance.CurrentBuildsInfo.kingComponents[i][j] = new KingComponent();
                        }
                    }
                }
                for (global::System.Int32 i = 0; i < GameManager.Instance.CurrentBuildsInfo.kingComponents[GameManager.Instance.currentPlayerIndex].Length; i++)
                {
                    if (!GameManager.Instance.CurrentBuildsInfo.kingComponents[GameManager.Instance.currentPlayerIndex][i].m_active
                        && !GameManager.Instance.CurrentBuildsInfo.kingComponents[GameManager.Instance.currentPlayerIndex][i].m_waitingForActivation)
                    {
                        GameManager.Instance.CurrentBuildsInfo.kingComponents[GameManager.Instance.currentPlayerIndex][i].m_waitingForActivation = true;
                        newBuild.Data.kingId = i;
                        break;
                    }
                }
                //Debug.Log("King dragged out with id " + newBuild.Data.kingId);
            }
        }
    }
}
