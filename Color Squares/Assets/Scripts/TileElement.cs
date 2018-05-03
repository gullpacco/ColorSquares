using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMadness

{

    public class TileElement : MonoBehaviour
    {

        private Color color = Color.None;
        private SpriteRenderer spriteRenderer;
        private Sprite currentSprite;
        public int Section;
        BoxCollider2D myCollider; 

        public Color Color
        {
            get { return color; }
        }

        private void Awake()
        {
            myCollider = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (Input.touchCount == 1)
            {
                Vector3 wp = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                Vector2 touchPos = new Vector2(wp.x, wp.y);
                if (myCollider == Physics2D.OverlapPoint(touchPos))
                {
                    GameManager.Instance.CheckValidTile(this);
                }
            }
        }

        public void Spawn(Color newColor)
        {
            color = newColor;
            spriteRenderer.sprite = TileManager.Instance.tileSprites[(int)color];
        }

        public void DeSpawn()
        {
            color = Color.None;
            gameObject.SetActive(false);
        }
    }
}