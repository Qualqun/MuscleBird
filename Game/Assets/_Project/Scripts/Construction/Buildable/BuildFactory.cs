using UnityEngine;
using BuildState;
public class BuildFactory : MonoBehaviour
{
    [SerializeField] GameObject buildPrefab;

    public Build CreateBuildOnUI(BuildDrawerData drawerData, BuildMaterialData materialData, NewBuildSlot slot)
    {
        GameObject obj = Instantiate(buildPrefab);
        Build build = obj.GetComponent<Build>();
        OnUIState state = build.LoadState<OnUIState>();
        state.LoadNewData(drawerData, materialData);
        state.Data.slot = slot;
        return build;
    }

    public Build CreateBuildOnDrag(Build buildOnUI)
    {
        GameObject obj = Instantiate(buildPrefab);
        obj.GetComponent<OrthographicSpriteScaler>().enabled = false;
        obj.transform.localScale = Vector3.one;

        Build build = obj.GetComponent<Build>();
        BuildDrawer drawer = obj.GetComponent<BuildDrawer>();
        build.tag = "Build";
        build.gameObject.layer = LayerMask.NameToLayer("Build");
        build.CopyTo(buildOnUI);
        drawer.CopyTo(buildOnUI.GetComponent<BuildDrawer>());
        OnDragState state = build.ChangeState<OnDragState>();
        //state.EnableColliders();

        BuildCursor.Instance.SetDragBuild(build);

        return build;
    }
}
