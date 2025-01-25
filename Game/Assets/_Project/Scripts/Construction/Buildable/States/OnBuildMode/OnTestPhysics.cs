using UnityEngine;

namespace BuildState
{
    public class OnTestPhysics : State
    {
        protected override void CollisionEnter(Collision2D collision)
        {
            LoseHp(Mathf.FloorToInt(collision.relativeVelocity.magnitude));
            GameMusics.Instance.CollideObjectSounds(build.Data.materialData.material);
        }

        protected override void OnEnter()
        {
            Drawer.SetColor(d.materialData.color);

            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            GetComponent<CompositeCollider2D>().isTrigger = false;
        }

        protected override void OnExit()
        {
            d.currentHp = d.materialData.maxHp;
        }

        protected override void OnUpdate()
        {
            if (transform.position.y < -Camera.main.orthographicSize)
            {
                GameMusics.Instance?.DestroyObjectSounds(build.Data.materialData.material);
                build.EnablePhysics(false);
                build.gameObject.SetActive(false);
            }
        }

        public void LoseHp(int hp)
        {
            d.currentHp -= hp;
            if (d.currentHp <= 0)
            {
                GameMusics.Instance?.DestroyObjectSounds(build.Data.materialData.material);
                build.EnablePhysics(false);
                build.gameObject.SetActive(false);
            }
        }
    }
}
