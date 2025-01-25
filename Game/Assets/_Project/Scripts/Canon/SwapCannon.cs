using UnityEngine;

public class SwapCannon : MonoBehaviour
{
    [SerializeField]
    private LlamaScript llamaCannon;

    [SerializeField]
    private KiwiScript kiwiCannon;

    [SerializeField]
    private Transform cannonPos;

    private LlamaScript instateLlamaCannon;
    private KiwiScript instateKiwiCannon;

    


    private void Start()
    {
        //CannonPos = Camera.main.ScreenToWorldPoint(new Vector3(100, 400, 0));
        instateKiwiCannon = Instantiate(kiwiCannon, cannonPos.position, Quaternion.identity, GameObject.Find("CannonPosition").transform);
        //Debug.Log(transform.name);
    }

    public void CreateCannon()
    {
        if (instateKiwiCannon != null)
        {
            Destroy(instateKiwiCannon.gameObject);
            instateLlamaCannon = Instantiate(llamaCannon, cannonPos.position, Quaternion.identity, GameObject.Find("CannonPosition").transform);
        }
        else
        {
            Destroy(instateLlamaCannon.gameObject);
            instateKiwiCannon = Instantiate(kiwiCannon, cannonPos.position, Quaternion.identity, GameObject.Find("CannonPosition").transform);
        }
    }
}
