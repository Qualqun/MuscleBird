using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace BuildState
{
    public abstract class State
    {
        protected Build build;
        protected BuildData d;

        public void OnStateEnter(Build build)
        {
            this.build = build;
            d = build.Data;
            OnEnter();
        }

        protected virtual void OnEnter()
        {
        }

        public void OnStateTriggerEnter(Collider2D collision)
        {
            TriggerEnter(collision);
        }

        protected virtual void TriggerEnter(Collider2D collision)
        {
        }

        public void OnStateTriggerExit(Collider2D collision)
        {
            TriggerExit(collision);
        }

        protected virtual void TriggerExit(Collider2D collision)
        {
        }

        public void OnStateCollisionEnter(Collision2D collision)
        {
            CollisionEnter(collision);
        }

        protected virtual void CollisionEnter(Collision2D collision)
        {
        }

        public void OnStateUpdate()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
        }

        public void OnStateFixedUpdate()
        {
            OnFixedUpdate();
        }

        protected virtual void OnFixedUpdate()
        {
        }

        public void OnStateExit()
        {
            OnExit();
        }

        protected virtual void OnExit()
        {
        }

        public void OnStateBeginDrag()
        {
            OnBeginDrag();
        }
        protected virtual void OnBeginDrag()
        {
        }

        public void OnStateDrag(Vector3 dragPos)
        {
            OnDrag(dragPos);
        }
        protected virtual void OnDrag(Vector3 dragPos)
        {
        }

        public void OnStateEndDrag()
        {
            OnEndDrag();
        }
        protected virtual void OnEndDrag()
        {
        }

        public void OnStateMouseButtonDown()
        {

            OnMouseButtonDown();
        }
        protected virtual void OnMouseButtonDown()
        {
        }

        public void OnStateMouseButtonUp()
        {

            OnMouseButtonUp();
        }
        protected virtual void OnMouseButtonUp()
        {
        }

        public bool CheckState<T>() where T : State
        {
            return this is T;
        }

        public T GetComponent<T>()
        {
            return build.GetComponent<T>();
        }

        public Build Build { get => build; }
        public BuildData Data { get => d; }
        public GameObject gameObject { get => build.gameObject; }
        public Transform transform { get => build.transform; }
        public BuildDrawer Drawer { get => build.Drawer; }
    }
}

