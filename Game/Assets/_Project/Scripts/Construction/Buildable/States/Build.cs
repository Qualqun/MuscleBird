using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using BuildState;

[Serializable]
public class KingComponent
{
    public bool m_waitingForActivation = false;
    public bool m_active = false;
    public bool m_waitingForSprite = false;
}
public enum KingId
{
    PLACEHOLDER,
    SHAKER,
    POWER,
    KETTLEBELL,
}
public class BuildData
{
    public BuildData() { }
    public BuildData(BuildData c)
    {
        materialData = c.materialData;
        collidedGrid = c.collidedGrid;
        slot = c.slot;
        oldPos = c.oldPos;
        oldRotation = c.oldRotation;
        nbColliding = c.nbColliding;
        currentHp = c.currentHp;
    }

    public BuildMaterialData materialData;
    public BuildingGrid collidedGrid;
    public NewBuildSlot slot;
    public Vector3 oldPos;
    public Quaternion oldRotation;
    public int nbColliding;
    public int currentHp;
    public int kingId;
}

public class Build : MonoBehaviour, BuildCursor.Behavior
{
    [ReadOnly] public BuildMaterialData materialDataViewer;
    [ReadOnly] public BuildDrawerData drawerDataViewer;


    State currentState;
    Dictionary<Type, State> statesInstance = new Dictionary<Type, State>();
    BuildData data = new BuildData();
    public GameObject particuleImpactPrefabs;

    public void CopyTo(Build builToCopy)
    {
        //currentState = builToCopy.currentState;
        //statesInstance = new Dictionary<Type, State>(builToCopy.statesInstance);
        data = new BuildData(builToCopy.data);
    }

    void Awake()
    {

        // Get all children type of State
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type[] types = assembly.GetTypes();
        IEnumerable<Type> stateTypes = types.Where(t => t.IsSubclassOf(typeof(State)) && !t.IsAbstract);

        // Instanciate each type and add to dictionary
        foreach (Type type in stateTypes)
        {
            State instance = (State)Activator.CreateInstance(type);
            statesInstance[type] = instance;
        }
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentState != null)
        {
            currentState.OnStateTriggerEnter(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (currentState != null)
        {
            currentState.OnStateTriggerExit(collision);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState != null)
        {
            currentState.OnStateCollisionEnter(collision);
        }

        if(collision.relativeVelocity.sqrMagnitude > 3)
        {
            Instantiate(particuleImpactPrefabs, collision.contacts[0].point, Quaternion.LookRotation((-collision.relativeVelocity.normalized)));
        }

    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.OnStateUpdate();
        }
    }

    void FixedUpdate()
    {
        if (currentState != null)
        {
            currentState.OnStateFixedUpdate();
            materialDataViewer = data.materialData;
            drawerDataViewer = Drawer.BaseDataUnused;
        }
    }

    public void OnMouseButtonDown()
    {
        if (currentState != null)
        {
            currentState.OnStateMouseButtonDown();
        }
    }

    public void OnMouseButtonUp()
    {
        if (currentState != null)
        {
            currentState.OnStateMouseButtonUp();
        }
    }

    public void OnBeginDrag()
    {
        if (currentState != null)
        {
            currentState.OnStateBeginDrag();
        }
    }

    public void OnDrag(Vector3 dragPos)
    {
        if (currentState != null)
        {
            currentState.OnStateDrag(dragPos);
        }
    }

    public void OnEndDrag()
    {
        if (currentState != null)
        {
            currentState.OnStateEndDrag();
        }
    }

    public T LoadState<T>() where T : State
    {
        if (statesInstance.TryGetValue(typeof(T), out State state))
        {
            state.OnStateEnter(this);
            currentState = state;
            //Debug.Log(currentState.GetType().Name);
            return currentState as T;
        }
        return null;
    }

    public T ChangeState<T>() where T : State
    {
        if (statesInstance.TryGetValue(typeof(T), out State state))
        {
            if (currentState != null)
            {
                currentState.OnStateExit();
                //Debug.Log(currentState.GetType().Name + " ----> " + state.GetType().Name);
            }

            state.OnStateEnter(this);
            currentState = state;
            return currentState as T;
        }
        return null;
    }

    public T GetCurrentState<T>() where T : State
    {
        if (currentState is T)
        {
            return currentState as T;
        }
        return null;
    }

    public T GetState<T>() where T : State
    {
        if (statesInstance.TryGetValue(typeof(T), out State state))
        {
            return state as T;
        }
        return null;
    }

    public bool CheckCurrentState<T>() where T : State
    {
        return currentState is T;
    }

    public void ResetPreviousTransform()
    {
        transform.position = data.oldPos;
        transform.rotation = data.oldRotation;
    }

    public void EnablePhysics(bool enable = true)
    {
        if (enable)
        {
            ChangeState<OnTestPhysics>();
        }
        else
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            GetComponent<Rigidbody2D>().angularVelocity = 0f;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            GetComponent<CompositeCollider2D>().isTrigger = true;
        }
    }

    public BuildsStack GetBuildsStack()
    {
        Transform constructZone = data.collidedGrid.transform.parent;
        return constructZone.GetComponentInChildren<BuildsStack>();
    }

    public BuildData Data { get => data; }
    public BuildDrawer Drawer { get => GetComponent<BuildDrawer>(); }
    public BuildMaterialData.Material Material { get => data.materialData.material; }
}


