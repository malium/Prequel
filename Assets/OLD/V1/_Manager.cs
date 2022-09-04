/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System.Collections.Generic;
//using UnityEngine;

//using Assets;
//using System.IO;

//public class _Manager : MonoBehaviour
//{
//    Map m_CurrentMap = null;
//    int m_LastSeed = 0;
//    Vector2 m_CenterSlider = new Vector2(8.0f, 8.0f);
//    Vector2 m_SigmaSlider = new Vector2(15.9f, 15.9f);
//    List<Tile> Selected = new List<Tile>();
//    Tile tileOver = null;
//    bool IsMouseHeld = false;
//    bool RandomInGeneration = true;
//    MapGeneration Generation = MapGeneration.SQUARE;

//    List<GameObject> enemies = new List<GameObject>();
//    public Camera m_Camera;

//    public Shader tileShader;
//    public MeshFilter importedMesh;
//    public GameObject oddGO;
//    public Sprite enemySprite;

//    // Start is called before the first frame update
//    void Start()
//    {
//        if (importedMesh != null)
//        {
//            string path = "Assets/Mesh.txt";
//            StreamWriter writer = new StreamWriter(path, false);
//            writer.WriteLine("vertices:");
//            for (int i = 0; i < importedMesh.sharedMesh.vertices.Length; i++)
//                writer.WriteLine($"new Vector3({importedMesh.sharedMesh.vertices[i].x}f, {importedMesh.sharedMesh.vertices[i].y}f, {importedMesh.sharedMesh.vertices[i].z}f),");
//            writer.WriteLine("normals:");
//            for (int i = 0; i < importedMesh.sharedMesh.normals.Length; i++)
//                writer.WriteLine($"new Vector3({importedMesh.sharedMesh.normals[i].x}f, {importedMesh.sharedMesh.normals[i].y}f, {importedMesh.sharedMesh.normals[i].z}f),");
//            writer.WriteLine("uvs:");
//            for (int i = 0; i < importedMesh.sharedMesh.uv.Length; i++)
//                writer.WriteLine($"new Vector2({importedMesh.sharedMesh.uv[i].x}f, {importedMesh.sharedMesh.uv[i].y}f),");
//            writer.WriteLine("indices:");
//            for (int i = 0; i < importedMesh.sharedMesh.triangles.Length; i += 3)
//                writer.WriteLine($"{importedMesh.sharedMesh.triangles[i]}, {importedMesh.sharedMesh.triangles[i + 1]}, {importedMesh.sharedMesh.triangles[i + 2]},");

//            writer.Flush();
//            writer.Close();
//        }
//        Tile.TileShader = tileShader;
//        m_CurrentMap = new Map(new Vector3());
//        Regenerate(Generation);

//        var enemyGO = new GameObject("succubus");
//        enemyGO.transform.position = new Vector3(10.0f, 0.1f, 0.0f);
//        var enemy = enemyGO.AddComponent<Enemy>();
//        enemy.Player = oddGO;
//        enemy.m_Camera = m_Camera;
//        enemy.SetEnemyInfo(EnemyDB.Enemies["succubus"]);
//        var spriteRND = enemyGO.AddComponent<SpriteRenderer>();
//        spriteRND.sprite = enemySprite;
//        spriteRND.drawMode = SpriteDrawMode.Simple;
//        spriteRND.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
//        spriteRND.spriteSortPoint = SpriteSortPoint.Pivot;
//        enemies.Add(enemyGO);
//    }

//    //private void OnDrawGizmos()
//    //{
//    //    //Gizmos.color = new Color(1.0f, 1.0f, 0.0f);
//    //    //for (int i = 0; i < Selected.Count; ++i)
//    //    //{
//    //    //    var go = Selected[i].GO;
//    //    //    var box = go.GetComponent<BoxCollider>();

//    //    //    Gizmos.DrawWireCube(go.transform.position + box.center, box.size);
//    //    //}

//    //    //if (tileOver != null)
//    //    //{
//    //    //    Gizmos.color = new Color(1.0f, 1.0f, 1.0f);
//    //    //    var box = tileOver.GO.GetComponent<BoxCollider>();

//    //    //    Gizmos.DrawWireCube(tileOver.GO.transform.position + box.center, box.size);
//    //    //}
//    //}

//    private bool IsSelected(Vector3 position)
//    {
//        for (int i = 0; i < Selected.Count; ++i)
//        {
//            if (Selected[i].GO.transform.position == position)
//                return true;
//        }
//        return false;
//    }

//    private void RemoveSelected(Vector3 position)
//    {
//        for (int i = 0; i < Selected.Count; ++i)
//        {
//            if (Selected[i].GO.transform.position == position)
//            {
//                Selected.RemoveAt(i);
//                return;
//            }
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetKey(KeyCode.Delete))
//        {
//            for (int i = 0; i < Selected.Count; ++i)
//            {
//                for (int j = 0; j < m_CurrentMap.Tiles.Count; ++j)
//                {
//                    if (m_CurrentMap.Tiles[j].Pos == Selected[i].Pos)
//                    {
//                        m_CurrentMap.Tiles[j].SetVisible(false);
//                        break;
//                    }
//                }
//            }
//            Selected.Clear();
//            tileOver = null;
//            return;
//        }
//        bool shiftPressed = Input.GetKey(KeyCode.LeftShift);
//        bool leftHeldDown = Input.GetMouseButton(0);
//        bool rightHeldDown = Input.GetMouseButton(1);

//        bool wasMouseHeld = IsMouseHeld;
//        IsMouseHeld = leftHeldDown || rightHeldDown;
//        bool mouseLeftClick = Input.GetMouseButtonDown(0);
//        bool mouseRightClick = Input.GetMouseButtonDown(1);

//        bool mouseClicked = mouseLeftClick || mouseRightClick;

//        bool select = (leftHeldDown || mouseLeftClick) && (!rightHeldDown || !mouseRightClick);
//        bool unselect = (!leftHeldDown || !mouseLeftClick) && (rightHeldDown || mouseRightClick);

//        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//        RaycastHit mouseHit;
//        GameObject goOver = null;
//        if (Physics.Raycast(ray, out mouseHit, 10000.0f))
//        {
//            goOver = mouseHit.transform.gameObject;
//            if (goOver.tag == Tile.TileTag)
//            {
//                for (int i = 0; i < m_CurrentMap.Tiles.Count; ++i)
//                {
//                    if (m_CurrentMap.Tiles[i].Pos == mouseHit.transform.position)
//                    {
//                        tileOver = m_CurrentMap.Tiles[i];
//                        break;
//                    }
//                }
//            }
//        }
//        else
//        {
//            tileOver = null;
//        }

//        if (tileOver == null)
//        {
//            if (mouseClicked && !wasMouseHeld && !shiftPressed)
//                Selected.Clear();
//            return;
//        }

//        if (mouseRightClick)
//        {
//            oddGO.GetComponent<OddScript>().MoveTo(new Vector2(mouseHit.point.x, mouseHit.point.z));
//        }
//        if (mouseLeftClick)
//        {
//            oddGO.GetComponent<OddScript>().Attack(new Vector2(mouseHit.point.x, mouseHit.point.z), enemies);
//        }

//        if (shiftPressed && IsMouseHeld) // Drag edit
//        {
//            if (select && !IsSelected(tileOver.GO.transform.position))
//            {
//                Selected.Add(tileOver);
//            }
//            else if (unselect)
//            {
//                RemoveSelected(tileOver.GO.transform.position);
//            }
//        }
//        else if (!shiftPressed && mouseClicked) // Clicky edit
//        {
//            if (select && !IsSelected(tileOver.GO.transform.position))
//            {
//                Selected.Add(tileOver);
//            }
//            else if (unselect)
//            {
//                RemoveSelected(tileOver.GO.transform.position);
//            }
//        }

//    }

//    private void FixedUpdate()
//    {
//        Color curColor = new Color(1.0f, 1.0f, 0.0f);
//        for (int i = 0; i < Selected.Count; ++i)
//        {
//            var go = Selected[i].GO;
//            var box = go.GetComponent<BoxCollider>();

//            MapUtils.DrawWireCube(go.transform.position + box.center, box.size, curColor);
//        }

//        if (tileOver != null)
//        {
//            curColor = new Color(1.0f, 1.0f, 1.0f);
//            var box = tileOver.GO.GetComponent<BoxCollider>();

//            MapUtils.DrawWireCube(tileOver.GO.transform.position + box.center, box.size, curColor);
//        }
//    }

//    private void Regenerate(MapGeneration mapGeneration, bool usingLastSeed = false)
//    {
//        m_LastSeed = usingLastSeed ? m_LastSeed : System.DateTime.Now.Millisecond;
//        m_CurrentMap.ClearMap();
//        m_CurrentMap.Generate(mapGeneration, m_LastSeed, m_CenterSlider, m_SigmaSlider, RandomInGeneration);
//    }

//    private void OnGUI()
//    {
//        if (GUI.Button(new Rect(0, 0, 160, 30), "Generate"))
//        {
//            Regenerate(Generation);
//        }
//        Vector2 tempCenterSlider = new Vector2();
//        Vector2 tempSigmaSlider = new Vector2();
//        GUI.Label(new Rect(0, 31, 160, 20), "Map Center:");
//        tempCenterSlider.x = GUI.HorizontalSlider(new Rect(0, 31 + 20, 160, 15), m_CenterSlider.x, 0.0f, Map.MapSize.x - 0.1f);
//        tempCenterSlider.y = GUI.HorizontalSlider(new Rect(0, 31 + 20 + 16, 160, 15), m_CenterSlider.y, 0.0f, Map.MapSize.y - 0.1f);
//        GUI.Label(new Rect(0, 31 + 20 + 16 * 2, 160, 20), "Map deviation:");
//        tempSigmaSlider.x = GUI.HorizontalSlider(new Rect(0, 31 + 20 * 2 + 16 * 2, 160, 15), m_SigmaSlider.x, 0.1f, Map.MapSize.x - 0.1f);
//        tempSigmaSlider.y = GUI.HorizontalSlider(new Rect(0, 31 + 20 * 2 + 16 * 3, 160, 15), m_SigmaSlider.y, 0.1f, Map.MapSize.x - 0.1f);
//        bool tempRandom = GUI.Toggle(new Rect(0, 31 + 20 * 2 + 16 * 4, 160, 15), RandomInGeneration, "RandomGeneration");
//        var tempGeneration = GUI.SelectionGrid(new Rect(0, 31 + 20 * 2 + 16 * 5, 160, 30), (int)Generation, new string[] { "SQUARE", "CIRCLE" }, 1);
//        if (tempCenterSlider != m_CenterSlider || tempSigmaSlider != m_SigmaSlider || tempRandom != RandomInGeneration
//            || tempGeneration != (int)Generation)
//        {
//            m_CenterSlider = tempCenterSlider;
//            m_SigmaSlider = tempSigmaSlider;
//            RandomInGeneration = tempRandom;
//            Generation = (MapGeneration)tempGeneration;
//            Regenerate(Generation, true);
//        }
//    }
//}
