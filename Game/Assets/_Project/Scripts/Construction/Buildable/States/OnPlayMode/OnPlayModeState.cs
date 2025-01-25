using UnityEngine;
using UnityEngine.UI;

namespace BuildState
{
    public class OnPlayModeState : State
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

        protected override void OnUpdate()
        {
            if (transform.position.y < -Camera.main.orthographicSize)
            {
                GameMusics.Instance?.DestroyObjectSounds(build.Data.materialData.material);
                build.EnablePhysics(false);
                Object.Destroy(build.gameObject);
            }
        }

        public void LoseHp(int hp)
        {
            d.currentHp -= hp;
            if (d.currentHp <= 0)
            {
                GameMusics.Instance?.DestroyObjectSounds(build.Data.materialData.material);
                build.EnablePhysics(false);

                Drawer.DisplayScore();
                GameManager.Instance.CurrentPlayer.AddScore(d.materialData.score);

                if (d.materialData.material == BuildMaterialData.Material.PROT)
                {
                    GameManager.Instance.CurrentBuildsInfo.nbKingsAlive[GameManager.Instance.currentPlayerIndex]--;
                    Image image = null;
                    switch (d.kingId)
                    {
                        case 0:
                            image = (GameManager.Instance.currentPlayerIndex == 0 ? GameHudManager.Instance.m_player1King1 : GameHudManager.Instance.m_player2King1);
                            break;
                        case 1:
                            image = (GameManager.Instance.currentPlayerIndex == 0 ? GameHudManager.Instance.m_player1King2 : GameHudManager.Instance.m_player2King2);
                            break;
                        case 2:
                            image = (GameManager.Instance.currentPlayerIndex == 0 ? GameHudManager.Instance.m_player1King3 : GameHudManager.Instance.m_player2King3);
                            break;
                        default:
                            break;
                    }
                    if (image != null)
                    {
                        image.color = new Color(image.color.r, image.color.g, image.color.b, 1.0f);
                        GameHudManager.Instance.CheckForWinner();
                    }
                }
                Object.Destroy(build.gameObject);
            }
        }
    }
}
