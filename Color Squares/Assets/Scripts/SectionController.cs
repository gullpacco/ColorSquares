using UnityEngine;
using System.Collections;

public class SectionController : MonoBehaviour {

    Tile[] tiles;
    Tile[] activeTiles = new Tile[3];
    int counter = 0;
    int toClick = 0;
    int bgValue = 0;
    int[] tilePositions;
    public static Color[] colors = new Color[5] {
                      new Color(0.25f, 0.8f,0.25f),
         new Color (0.8f, 0.25f, 0.25f),
             new Color(1,1,1),
             new Color(0,0,0),
            new Color(0.4f, 0.9f, 9f)};
    SpriteRenderer bgCol;
    class Tile
    {
        GameObject tile_reference, star;
        int val = 0, tileNumber;
        bool clicked, locked;
        SpriteRenderer renderer, childRenderer;

        public int Value
        {
            get { return val; }
            set { val = value; }

        }

        public bool Clicked
        {
            get { return clicked; }
            set { clicked = value; }
        }

        public GameObject Tile_Reference
        {
            get { return tile_reference; }
        }

        public int TileNumber
        {
            get { return tileNumber; }
        }

        public void SetColour(int bgValue)
        {
            renderer.color = colors[bgValue];
            for (int k=0; k<colors.Length; k++)
            {
                if (k == val && val!=bgValue)
                {
                    renderer.transform.GetChild(2).GetChild(k).gameObject.SetActive(true);
                }
                else
                {
                    renderer.transform.GetChild(2).GetChild(k).gameObject.SetActive(false);
                }
            }
        }

        public Tile(GameObject go, int number)
        {
            tile_reference = go;
            renderer = go.GetComponent<SpriteRenderer>();
            childRenderer = go.transform.GetChild(0).GetComponent<SpriteRenderer>();
            star = go.transform.GetChild(1).gameObject;
            tileNumber = number;
        }

        public bool Locked
        {
            get { return locked; }
            set
            {
                locked = value;
                star.SetActive(locked);
            }
        }

        public bool TryLock(float chance)
        {
            if (chance > 8)
                Locked = true;
            return locked;
        }


    }

    void Awake()
    {
        bgCol = GetComponent<SpriteRenderer>();
        tiles = new Tile[transform.childCount];
        {
            for (int p = 0; p < tiles.Length; p++)
            {
                tiles[p] = new Tile(transform.GetChild(p).gameObject, counter++);
            }
        }
    }


    // Use this for initialization

    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        bgCol.color = colors[ bgValue];
	}

    public int SwitchColour(int tileCount)
    {
        toClick = 0;
        tilePositions = new int[tileCount];

        GenerateBackground();
        if (tileCount > 0)
        {

            ActivateTiles(tileCount);
        }
        SetTilesColour();



        return toClick;
    }

    void ActivateTiles(int tileCount)
    {

        PickTilesPosition(tileCount);

    }

    void PickTilesPosition(int tileCount)
    {
        tilePositions[0] = Random.Range(0, tiles.Length);
        if (tileCount > 1)
        {
            do { tilePositions[1] = Random.Range(0, tiles.Length); }
            while (tilePositions[1] == tilePositions[0]);
            if (tileCount > 2)
            {
                do { tilePositions[2] = Random.Range(0, tiles.Length); }
                while (tilePositions[2] == tilePositions[1] || tilePositions[2] == tilePositions[0]);
            }
        }

    }

    void SetTilesColour()
    {
        for (int k = 0; k < tiles.Length; k++)
        {
            bool found = false;
            tiles[k].Clicked = false;
            tiles[k].Locked = false;

            for (int t = 0; t < tilePositions.Length; t++)
            {
                if (tilePositions[t] == k)
                {
                    tiles[k].Value = GenerateTile();
                    found = true;


                }
            }
            if (!found) tiles[k].Value = bgValue;
            tiles[k].SetColour(bgValue);

        }
    }

    int GenerateTile()
    {
        int tileVal;
        if (GameController.instance.level <= 5)
        {
            tileVal = ColourFromBackground(false);
        }
        else
        {
            tileVal = ColourFromBackground(true);
        }
        IncreaseClicks(tileVal);
        return tileVal;
    }

    void GenerateBackground()
    {
        switch (GameController.instance.level)
        {

           
            case 3:
                bgValue = Random.Range(1, 4);
                if (bgValue == 1)
                    bgValue = 0;
                break;
            case 2:
                bgValue = Random.Range(2, 4);
                break;
            case 1:
                bgValue = 2;
                break;
            default:
                bgValue = Random.Range(0, 4);
                break;
        }

    }

    int ColourFromBackground(bool fifth)
    {
        int tileColour = 0;

        if (fifth)
        {
            if (bgValue > 1)
            {
                float tempCol = Random.Range(0, 5);
                if (tempCol >= 0 && tempCol < 2)
                    tileColour = 0;
                else if (tempCol >= 2 && tempCol < 4)
                    tileColour = 1;
                else  tileColour = 4;
                return tileColour;
            }
            else
            {
                float tempCol = Random.Range(4, 9);
                if (tempCol >= 4 && tempCol < 6)
                    tileColour = 2;
                else if (tempCol >= 6 && tempCol < 8)
                    tileColour = 3;
                else tileColour = 4;
                return tileColour;
            }


        }
        else
        {
            if (bgValue > 1)
                return Random.Range(0, 2);
            else
                return Random.Range(2, 4);
        }

    }

    void IncreaseClicks(int tileValue)
    {
        if (tileValue == 4)
        {
            toClick++;
            return;
        }
        switch (bgValue)
        {
            case 0:
                toClick++;
                break;
            case 1: break;
            case 2:
                if (tileValue == 0)
                    toClick++;
                break;
            case 3:
                if (tileValue == 1)
                    toClick++;
                break;
        }
    }

    public void ManualGeneration(int tileCount, int [] tileColors, int background)
    {
        bgValue = background;
        tilePositions = new int[tileCount];

        PickTilesPosition(tileCount);
        ManualColourSet(tileColors);
    }

    void ManualColourSet(int [] tileColors)
    {
        for (int k = 0; k < tiles.Length; k++)
        {
            bool found = false;
            tiles[k].Clicked = false;
            tiles[k].Locked = false;

            for (int t = 0; t < tilePositions.Length; t++)
            {
                if (tilePositions[t] == k)
                {
                    tiles[k].Value = tileColors[t];
                    found = true;

                }
            }
            if (!found) tiles[k].Value = bgValue;
            tiles[k].SetColour(bgValue);

        }
    }


   public  int CheckCorrect(RaycastHit2D hit)
    {

        Tile clickedTile;
        if (!GameController.instance.pause && !GameController.instance.tutorial)
        {
            for (int j = 0; j < tiles.Length; j++)
            {
                if (tiles[j].Tile_Reference == hit.transform.gameObject)
                {
                    clickedTile = tiles[j];

                    if (!clickedTile.Clicked)
                    {
                        if (clickedTile.Value == 4)
                        {
                            if (!clickedTile.Locked)
                            {
                                GameController.instance.ToClick -= 1;
                                clickedTile.Clicked = true;
                                //coin.Play();
                                clickedTile.Value = bgValue;
                                clickedTile.SetColour(bgValue);
                                return 1;
                            }
                            { clickedTile.Locked = false; }

                        }
                        else
                        {
                            switch (bgValue)
                            {
                                case 0:
                                    if (!clickedTile.Locked)
                                    {
                                        GameController.instance.ToClick -= 1;
                                        clickedTile.Clicked = true;
                                      //  coin.Play();
                                        clickedTile.Value = bgValue;
                                        clickedTile.SetColour(bgValue);
                                        return 1;
                                    }
                                    { clickedTile.Locked = false; }
                                    break;
                                case 1:
                                    //GameOver(wrong);
                                    //error.Play();
                                    return -1;
                                  //  break;
                                case 2:
                                    if (clickedTile.Value == 0)
                                    {

                                        if (!clickedTile.Locked)
                                        {
                                            GameController.instance.ToClick -= 1;
                                            clickedTile.Clicked = true;
                                           // coin.Play();
                                            clickedTile.Value = bgValue;
                                            clickedTile.SetColour(bgValue);
                                            return 1;
                                        }
                                        { clickedTile.Locked = false; }
                                        break;

                                    }
                                    else
                                    {
                                        //GameOver(wrong);
                                        //error.Play();
                                        return -1;
                                    }

                                   // break;
                                case 3:
                                    if (clickedTile.Value == 1)
                                    {

                                        if (!clickedTile.Locked)
                                        {
                                            GameController.instance.ToClick -= 1;
                                            clickedTile.Clicked = true;
                                            //    coin.Play();
                                            clickedTile.Value = bgValue;
                                            clickedTile.SetColour(bgValue);
                                            return 1;
                                        }
                                        { clickedTile.Locked = false; }
                                        break;

                                    }
                                    else
                                        {
                                        //    GameOver(wrong);
                                        //    error.Play();
                                        return -1;
                                    }
                                   // break;


                            }
                            break;
                        }
                    }
                }
            }
        }
        return 0;
    }

}
