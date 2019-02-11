using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TileMadness
{
    public enum BackGroundState { Full, HorizontalSplit, VerticalSplit }
    public enum ElementPostion { None, Left, Right, Up, Down }
    public enum Color { White, Black, Green, Red, Blue, None }

    public class TileManager : MonoBehaviour
    {
        public Sprite[] tileSprites;
        public TileElement[] tiles;
        private static TileManager instance;
        [Header("Backgrounds")]
        public SpriteRenderer fullBackground;
        public SpriteRenderer leftBackground;
        public SpriteRenderer rightBackground;
        public SpriteRenderer upBackground;
        public SpriteRenderer downBackground;
        [Header("Colors")]
        public UnityEngine.Color[] colors;

        [Header("Sections")]
        [SerializeField]
        public LevelSection[] sections;
        public TutorialLevel[] tutorials;
        private LevelSection currentSection;
        private int currentSectionIndex;
        private List<Color> defaultAllowedColors = new List<Color> { Color.White, Color.Black, Color.Green, Color.Red, Color.Blue };
        private List<Color> allowedColors;

        private BackGroundState backGroundState = BackGroundState.Full;
        private Color fullBackgroundColor = Color.None;
        private Color leftBackgroundColor = Color.None;
        private Color rightBackgroundColor = Color.None;
        private Color upBackgroundColor = Color.None;
        private Color downBackgroundColor = Color.None;
        private int currentValidTiles = 0;
        bool tutorialDone = false;

        private int[] defaultTileIndex = new int[] { -1, -1, -1 };

        [Serializable]
        public struct LevelSection
        {
            public int minLevel;
            public int tileNumber;
            public bool horizontalSplit;
            public bool verticalSplit;
            public bool blueTile;
            public Color[] allowedBackgroundColors;
            public int tutorialIndex;
        }

        [Serializable]
        public struct TutorialLevel
        {
            public BackGroundState backgroundState;
            public Color[] tileColors;
            public Color[] backGroundColors;
            public string tutorialText;
            public Color tutorialBackgroundColor;
            public Color tutorialContinueColor;
        }


        public BackGroundState BackgroundState
        {
            get
            {
                return backGroundState;
            }
        }

        public static TileManager Instance
        {
            get { return instance; }
        }

        public Color FullBackgroundColor
        {
            get
            {
                return fullBackgroundColor;
            }
        }
        public Color LeftBackgroundColor
        {
            get
            {
                return leftBackgroundColor;
            }
        }
        public Color RightBackgroundColor
        {
            get
            {
                return rightBackgroundColor;
            }
        }
        public Color UpBackgroundColor
        {
            get
            {
                return upBackgroundColor;
            }
        }
        public Color DownBackgroundColor
        {
            get
            {
                return downBackgroundColor;
            }
        }

        private void Awake()
        {
            instance = this;
            currentSection = sections[0];
        }
        public void ResetTiles()
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].DeSpawn();
            }
        }
        public void CheckTutorialCompleted()
        {
            if (!tutorialDone)
            {
                MobileFramework.FirebaseEventsHandler.Instance.TutorialComplete(currentSection.tutorialIndex);
                SaveManager.Instance.SetTutorialDone(currentSection.tutorialIndex);
            }
        }
        public int SpawnTileset()
        {
            currentValidTiles = 0;
            ResetTiles();
            ResetBackgroundColors();
            CheckSectionIncrease();
            if (!tutorialDone)
            {
                tutorialDone = true;
                int tutorialIndex = currentSection.tutorialIndex;
                TutorialLevel currentTutorial = tutorials[tutorialIndex];
                backGroundState = currentTutorial.backgroundState;
                SetBackgrounds(currentTutorial.backGroundColors);
                SpawnTiles(currentTutorial.tileColors);
                GUIManager.instance.ShowTutorial(currentTutorial.tutorialText, currentTutorial.tutorialBackgroundColor, currentTutorial.tutorialContinueColor);

            }
            else
            {
                SetBackgroundState();
                SetBackgrounds();
                SpawnTiles();
            }
            return currentValidTiles;
        }

        void CheckSectionIncrease()
        {
            if (currentSectionIndex < sections.Length - 1)
            {
                if (GameManager.Instance.CurrentLevel > sections[currentSectionIndex + 1].minLevel)
                {
                    currentSectionIndex++;
                    if (sections[currentSectionIndex].tutorialIndex > SaveManager.Instance.LastTutorialDone())
                    {
                        tutorialDone = false;
                    }
                }
            }
            currentSection = sections[currentSectionIndex];
        }

        int SetTilesForBackgroundOne()
        {
            if (currentSection.horizontalSplit)
            {
                int tilesToReturn = UnityEngine.Random.Range(1, currentSection.tileNumber);
                return tilesToReturn == currentSection.tileNumber ? tilesToReturn - 1 : tilesToReturn;
            }
            else
            {
                return currentSection.tileNumber;
            }
        }

        void SetBackgroundState()
        {
            int backgroundTempState;
            if (currentSection.verticalSplit)
            {
                backgroundTempState = UnityEngine.Random.Range(0, 3);
            }
            else if (currentSection.verticalSplit)
            {
                backgroundTempState = UnityEngine.Random.Range(0, 2);
            }
            else
            {
                backgroundTempState = 0;
            }
            if (backgroundTempState == 2)
            {
                backGroundState = BackGroundState.HorizontalSplit;
            }
            else if (backgroundTempState == 1)
            {
                backGroundState = BackGroundState.VerticalSplit;
            }
            else
            {
                backGroundState = BackGroundState.Full;
            }
        }

        Color SetColor(List<Color> _allowedColors)
        {
            int listElement = UnityEngine.Random.Range(0, _allowedColors.Count);
            return _allowedColors[listElement];
        }

        void SetBackground(ElementPostion _position, Color _color)
        {
            switch (_position)
            {
                case ElementPostion.None:
                    fullBackground.color = colors[(int)_color];
                    fullBackgroundColor = _color;
                    break;
                case ElementPostion.Left:
                    leftBackground.color = colors[(int)_color];
                    leftBackgroundColor = _color;
                    break;
                case ElementPostion.Right:
                    rightBackground.color = colors[(int)_color];
                    rightBackgroundColor = _color;
                    break;
                case ElementPostion.Up:
                    upBackground.color = colors[(int)_color];
                    upBackgroundColor = _color;
                    break;
                case ElementPostion.Down:
                    downBackground.color = colors[(int)_color];
                    downBackgroundColor = _color;
                    break;
            }
        }

        void SetBackgrounds(Color[] mandatoryColors = null)
        {
            if (mandatoryColors == null)
            {
                allowedColors = new List<Color>(currentSection.allowedBackgroundColors);
            }
            else
            {
                allowedColors = new List<Color>(mandatoryColors);
            }
            Color firstColor;
            switch (backGroundState)
            {
                case BackGroundState.Full:
                    SetBackground(ElementPostion.None, SetColor(allowedColors));
                    break;
                case BackGroundState.HorizontalSplit:
                    firstColor = SetColor(allowedColors);
                    SetBackground(ElementPostion.Left, firstColor);
                    allowedColors.Remove(firstColor);
                    SetBackground(ElementPostion.Right, SetColor(allowedColors));
                    break;
                case BackGroundState.VerticalSplit:
                    firstColor = SetColor(allowedColors);
                    SetBackground(ElementPostion.Up, firstColor);
                    allowedColors.Remove(firstColor);
                    SetBackground(ElementPostion.Down, SetColor(allowedColors));
                    break;
            }
        }

        void ResetBackgroundColors()
        {
            SetBackground(ElementPostion.None, Color.None);
            SetBackground(ElementPostion.Left, Color.None);
            SetBackground(ElementPostion.Right, Color.None);
            SetBackground(ElementPostion.Up, Color.None);
            SetBackground(ElementPostion.Down, Color.None);
        }

        void SpawnTiles(Color[] mandatoryColors = null)
        {
            int[] spawnedTilesIndex = defaultTileIndex;
            Color[] tileColors = new Color[currentSection.tileNumber];
            List<Color> allowedTileColors;
            int tilesForSectionOne = SetTilesForBackgroundOne();
            for (int tileToSpawn = 0; tileToSpawn < currentSection.tileNumber; tileToSpawn++)
            {
                int tileIndex = -1;
                bool allowed = false;
                if (mandatoryColors == null || mandatoryColors.Length < tileToSpawn + 1)
                {
                    allowedTileColors = new List<Color>(defaultAllowedColors);
                    if (!currentSection.blueTile)
                    {
                        allowedTileColors.Remove(Color.Blue);
                    }
                }
                else
                {
                    allowedTileColors = new List<Color>();
                    allowedTileColors.Add(mandatoryColors[tileToSpawn]);
                }

                switch (backGroundState)
                {
                    case BackGroundState.Full:
                        while (!allowed)
                        {
                            allowed = true;
                            tileIndex = UnityEngine.Random.Range(0, tiles.Length);
                            for (int i = 0; i < currentSection.tileNumber; i++)
                            {
                                if (spawnedTilesIndex[i] == tileIndex)
                                {
                                    allowed = false;
                                }
                            }
                        }
                        spawnedTilesIndex[tileToSpawn] = tileIndex;
                        allowedTileColors = GetAllowedColorsOnBackground(FullBackgroundColor, allowedTileColors);
                        tileColors[tileToSpawn] = SetColor(allowedTileColors);
                        CheckIncreaseValidTiles(tileColors[tileToSpawn], fullBackgroundColor);
                        break;
                    case BackGroundState.HorizontalSplit:
                        while (!allowed)
                        {
                            allowed = true;
                            tileIndex = UnityEngine.Random.Range(0, tiles.Length / 2);
                            if (tileToSpawn < tilesForSectionOne)
                            {
                                if (tileIndex > 1)
                                {
                                    if (tileIndex < 4)
                                    {
                                        tileIndex += 2;
                                    }
                                    else if (tileIndex < 6)
                                    {
                                        tileIndex += 4;
                                    }
                                    else if (tileIndex < 8)
                                    {
                                        tileIndex += 6;
                                    }
                                    else if (tileIndex < 10)
                                    {
                                        tileIndex += 8;
                                    }
                                    else
                                    {
                                        tileIndex += 10;
                                    }
                                }
                            }
                            else
                            {
                                if (tileIndex < 2)
                                {
                                    tileIndex += 2;
                                }
                                else if (tileIndex < 4)
                                {
                                    tileIndex += 4;
                                }
                                else if (tileIndex < 6)
                                {
                                    tileIndex += 6;
                                }
                                else if (tileIndex < 8)
                                {
                                    tileIndex += 8;
                                }
                                else if (tileIndex < 10)
                                {
                                    tileIndex += 10;
                                }
                                else
                                {
                                    tileIndex += 12;
                                }
                            }
                            for (int i = 0; i < currentSection.tileNumber; i++)
                            {
                                if (spawnedTilesIndex[i] == tileIndex)
                                {
                                    allowed = false;
                                }
                            }
                        }
                        spawnedTilesIndex[tileToSpawn] = tileIndex;
                        if (tileToSpawn < tilesForSectionOne)
                        {
                            allowedTileColors = GetAllowedColorsOnBackground(LeftBackgroundColor, allowedTileColors);
                            tileColors[tileToSpawn] = SetColor(allowedTileColors);
                            CheckIncreaseValidTiles(tileColors[tileToSpawn], LeftBackgroundColor);
                        }
                        else
                        {
                            allowedTileColors = GetAllowedColorsOnBackground(RightBackgroundColor, allowedTileColors);
                            tileColors[tileToSpawn] = SetColor(allowedTileColors);
                            CheckIncreaseValidTiles(tileColors[tileToSpawn], RightBackgroundColor);
                        }
                        break;
                    case BackGroundState.VerticalSplit:
                        while (!allowed)
                        {
                            allowed = true;
                            tileIndex = UnityEngine.Random.Range(0, tiles.Length / 2);
                            if (tileToSpawn >= tilesForSectionOne)
                            {
                                tileIndex += tiles.Length / 2;
                            }
                            for (int i = 0; i < currentSection.tileNumber; i++)
                            {
                                if (spawnedTilesIndex[i] == tileIndex)
                                {
                                    allowed = false;
                                }
                            }
                        }
                        spawnedTilesIndex[tileToSpawn] = tileIndex;
                        if (tileToSpawn < tilesForSectionOne)
                        {
                            allowedTileColors = GetAllowedColorsOnBackground(UpBackgroundColor, allowedTileColors);
                            tileColors[tileToSpawn] = SetColor(allowedTileColors);
                            CheckIncreaseValidTiles(tileColors[tileToSpawn], UpBackgroundColor);
                        }
                        else
                        {
                            allowedTileColors = GetAllowedColorsOnBackground(DownBackgroundColor, allowedTileColors);
                            tileColors[tileToSpawn] = SetColor(allowedTileColors);
                            CheckIncreaseValidTiles(tileColors[tileToSpawn], DownBackgroundColor);
                        }
                        break;
                }

            }
            for (int i = 0; i < tileColors.Length; i++)
            {
                SpawnTile(tileColors[i], spawnedTilesIndex[i]);
            }
        }

        public void CheckIncreaseValidTiles(Color tileColor, Color backgroundColor)
        {
            if (IsTileValid(tileColor, backgroundColor))
            {
                currentValidTiles++;
            }
        }

        List<Color> GetAllowedColorsOnBackground(Color backgroundColor, List<Color> allowedColors)
        {
            allowedColors.Remove(backgroundColor);
            switch (backgroundColor)
            {
                case Color.Black:
                    allowedColors.Remove(Color.White);
                    break;
                case Color.White:
                    allowedColors.Remove(Color.Black);
                    break;
                case Color.Green:
                    allowedColors.Remove(Color.Red);
                    break;
                case Color.Red:
                    allowedColors.Remove(Color.Green);
                    break;
            }
            return allowedColors;
        }

        bool IsTileValid(Color tileColor, Color backgroundColor)
        {
            if (tileColor == Color.Blue)
            {
                return true;
            }
            else
            {
                switch (backgroundColor)
                {
                    case Color.Black:
                        if (tileColor == Color.Red)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case Color.White:
                        if (tileColor == Color.Green)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case Color.Green:
                        return true;
                    case Color.Red:
                        return false;
                    default:
                        return false;
                }
            }
        }

        void SpawnTile(Color tileColor, int tileIndex)
        {
            tiles[tileIndex].gameObject.SetActive(true);
            tiles[tileIndex].Spawn(tileColor);
        }
    }
}
