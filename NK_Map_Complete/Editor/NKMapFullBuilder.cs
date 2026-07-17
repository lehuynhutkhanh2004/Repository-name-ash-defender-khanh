#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public static class NKMapFullBuilder
{
    private const string RootPath = "Assets/NK_Map_Complete";

    [MenuItem("Tools/Ash Defender/Build NK Full Map")]
    public static void BuildNKFullMap()
    {
        if (!EditorUtility.DisplayDialog("Build NK Full Map", "Script sẽ tạo map đầy đủ giống cấu trúc NK-Map. Các object cũ cùng tên sẽ được xóa và tạo lại. Tiếp tục?", "Build", "Cancel"))
        {
            return;
        }

        DeleteIfExists("Grid_Dungeon");
        DeleteIfExists("GameplayObjects_Map03");
        DeleteIfExists("Map03_GameManager");
        DeleteIfExists("UIController");
        DeleteIfExists("EventSystem");

        SetupMainCamera();

        Grid grid = CreateGridDungeon();
        Dictionary<string, Tilemap> tilemaps = CreateTilemaps(grid.transform);
        FillDungeonTiles(tilemaps);

        GameObject gameplay = new GameObject("GameplayObjects_Map03");
        GameObject spawnPoints = CreateChild(gameplay.transform, "SpawnPoints");
        GameObject waypoints = CreateChild(gameplay.transform, "Waypoints");
        GameObject baseObj = CreateChild(gameplay.transform, "Base");
        GameObject heroArea = CreateChild(gameplay.transform, "HeroArea");
        GameObject heroSlots = CreateChild(gameplay.transform, "HeroSlots");
        GameObject enemyContainer = CreateChild(gameplay.transform, "EnemyContainer");
        GameObject bossContainer = CreateChild(gameplay.transform, "BossContainer");
        GameObject projectileContainer = CreateChild(gameplay.transform, "ProjectileContainer");
        GameObject itemDropContainer = CreateChild(gameplay.transform, "ItemDropContainer");
        GameObject trapObjects = CreateChild(gameplay.transform, "TrapObjects");
        GameObject uiFolder = CreateChild(gameplay.transform, "UI");

        AddMarker(enemyContainer, NKMapMarker.MarkerType.Container, "Container chứa enemy runtime");
        AddMarker(bossContainer, NKMapMarker.MarkerType.Container, "Container chứa boss runtime");
        AddMarker(projectileContainer, NKMapMarker.MarkerType.Container, "Container chứa projectile runtime");
        AddMarker(itemDropContainer, NKMapMarker.MarkerType.Container, "Container chứa item drop runtime");
        AddMarker(uiFolder, NKMapMarker.MarkerType.UI, "Folder UI trong gameplay");

        CreateSpawnPoint(spawnPoints.transform, "SpawnPointA_Left", new Vector3(-15f, 0f, 0f), false);
        CreateSpawnPoint(spawnPoints.transform, "SpawnPointB_Top", new Vector3(-4f, 8f, 0f), false);
        CreateSpawnPoint(spawnPoints.transform, "BossSpawnPoint_BottomLeft", new Vector3(-14f, -7f, 0f), true);

        NKPathContainer pathA = CreatePath(waypoints.transform, "Path_A_LeftToCore", new Vector3[]
        {
            new Vector3(-15f, 0f, 0f),
            new Vector3(-10f, 0f, 0f),
            new Vector3(-6f, 1.5f, 0f),
            new Vector3(-2f, 0.5f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(8f, 0.5f, 0f),
            new Vector3(12.5f, 0f, 0f),
        });

        NKPathContainer pathB = CreatePath(waypoints.transform, "Path_B_TopToCore", new Vector3[]
        {
            new Vector3(-4f, 8f, 0f),
            new Vector3(-4f, 5f, 0f),
            new Vector3(0f, 4f, 0f),
            new Vector3(4f, 3f, 0f),
            new Vector3(8f, 1.5f, 0f),
            new Vector3(12.5f, 0f, 0f),
        });

        NKPathContainer bossPath = CreatePath(waypoints.transform, "BossPath_BottomToCore", new Vector3[]
        {
            new Vector3(-14f, -7f, 0f),
            new Vector3(-10f, -5f, 0f),
            new Vector3(-5f, -3.5f, 0f),
            new Vector3(0f, -2.5f, 0f),
            new Vector3(5f, -1.5f, 0f),
            new Vector3(12.5f, 0f, 0f),
        });

        CreateCore(baseObj.transform, new Vector3(13.5f, 0f, 0f));
        CreateHeroArea(heroArea.transform);
        CreateHeroSlots(heroSlots.transform);
        CreateTrapObjects(trapObjects.transform);
        CreateEventSystem();

        GameObject gameManager = new GameObject("Map03_GameManager");
        NKSimpleWaveSpawner spawner = gameManager.AddComponent<NKSimpleWaveSpawner>();
        spawner.pathA = pathA;
        spawner.pathB = pathB;
        spawner.bossPath = bossPath;
        spawner.enemyContainer = enemyContainer.transform;
        spawner.bossContainer = bossContainer.transform;
        spawner.autoStart = false;

        GameObject uiController = new GameObject("UIController");
        uiController.AddComponent<NKMapMarker>().markerType = NKMapMarker.MarkerType.UI;

        Selection.activeGameObject = gameplay;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("NK Full Map đã tạo xong. Hãy gán Enemy Prefab và Boss Prefab trong Map03_GameManager -> NKSimpleWaveSpawner.");
    }

    private static void DeleteIfExists(string name)
    {
        GameObject existing = GameObject.Find(name);
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }
    }

    private static void SetupMainCamera()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            camera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
        }

        camera.orthographic = true;
        camera.orthographicSize = 9f;
        camera.transform.position = new Vector3(0f, 0f, -10f);
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.07f, 0.07f, 0.09f, 1f);
    }

    private static Grid CreateGridDungeon()
    {
        GameObject gridObj = new GameObject("Grid_Dungeon");
        Grid grid = gridObj.AddComponent<Grid>();
        grid.cellSize = Vector3.one;
        return grid;
    }

    private static Dictionary<string, Tilemap> CreateTilemaps(Transform parent)
    {
        Dictionary<string, Tilemap> maps = new Dictionary<string, Tilemap>();
        CreateTilemap(parent, maps, "Ground_Tilemap", 0, false);
        CreateTilemap(parent, maps, "Path_Tilemap", 1, false);
        CreateTilemap(parent, maps, "Wall_Tilemap", 2, true);
        CreateTilemap(parent, maps, "Obstacle_Tilemap", 3, true);
        CreateTilemap(parent, maps, "Decoration_Tilemap", 4, false);
        CreateTilemap(parent, maps, "Trap_Tilemap", 5, true);
        CreateTilemap(parent, maps, "BuildSlot_Tilemap", 6, false);
        return maps;
    }

    private static void CreateTilemap(Transform parent, Dictionary<string, Tilemap> maps, string name, int sortingOrder, bool collider)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);
        Tilemap tilemap = obj.AddComponent<Tilemap>();
        TilemapRenderer renderer = obj.AddComponent<TilemapRenderer>();
        renderer.sortingOrder = sortingOrder;
        if (collider)
        {
            obj.AddComponent<TilemapCollider2D>();
        }
        maps[name] = tilemap;
    }

    private static void FillDungeonTiles(Dictionary<string, Tilemap> maps)
    {
        Tile floor = GetOrCreateTile("NK_Floor", "NK_Floor.png");
        Tile path = GetOrCreateTile("NK_Path", "NK_Path.png");
        Tile wall = GetOrCreateTile("NK_Wall", "NK_Wall.png");
        Tile obstacle = GetOrCreateTile("NK_Obstacle", "NK_Obstacle.png");
        Tile decoration = GetOrCreateTile("NK_Decoration", "NK_Decoration.png");
        Tile trap = GetOrCreateTile("NK_Trap", "NK_Trap.png");
        Tile buildSlot = GetOrCreateTile("NK_BuildSlot", "NK_BuildSlot.png");

        for (int x = -17; x <= 17; x++)
        {
            for (int y = -9; y <= 9; y++)
            {
                maps["Ground_Tilemap"].SetTile(new Vector3Int(x, y, 0), floor);
            }
        }

        for (int x = -17; x <= 17; x++)
        {
            maps["Wall_Tilemap"].SetTile(new Vector3Int(x, -10, 0), wall);
            maps["Wall_Tilemap"].SetTile(new Vector3Int(x, 10, 0), wall);
        }
        for (int y = -9; y <= 9; y++)
        {
            maps["Wall_Tilemap"].SetTile(new Vector3Int(-18, y, 0), wall);
            maps["Wall_Tilemap"].SetTile(new Vector3Int(18, y, 0), wall);
        }

        // Main path A, left to core.
        SetThickLine(maps["Path_Tilemap"], path, new Vector2Int(-17, 0), new Vector2Int(13, 0), 1);
        SetThickLine(maps["Path_Tilemap"], path, new Vector2Int(-8, 1), new Vector2Int(-4, 2), 1);
        SetThickLine(maps["Path_Tilemap"], path, new Vector2Int(-4, 2), new Vector2Int(1, 1), 1);

        // Path B, top to core.
        SetThickLine(maps["Path_Tilemap"], path, new Vector2Int(-4, 9), new Vector2Int(-4, 4), 1);
        SetThickLine(maps["Path_Tilemap"], path, new Vector2Int(-4, 4), new Vector2Int(4, 3), 1);
        SetThickLine(maps["Path_Tilemap"], path, new Vector2Int(4, 3), new Vector2Int(13, 0), 1);

        // Boss path, bottom to core.
        SetThickLine(maps["Path_Tilemap"], path, new Vector2Int(-15, -7), new Vector2Int(-10, -5), 1);
        SetThickLine(maps["Path_Tilemap"], path, new Vector2Int(-10, -5), new Vector2Int(-2, -3), 1);
        SetThickLine(maps["Path_Tilemap"], path, new Vector2Int(-2, -3), new Vector2Int(5, -2), 1);
        SetThickLine(maps["Path_Tilemap"], path, new Vector2Int(5, -2), new Vector2Int(13, 0), 1);

        Vector3Int[] obstacles =
        {
            new Vector3Int(-12,5,0), new Vector3Int(-11,5,0), new Vector3Int(-10,5,0),
            new Vector3Int(-12,4,0), new Vector3Int(-10,4,0),
            new Vector3Int(2,6,0), new Vector3Int(3,6,0), new Vector3Int(4,6,0),
            new Vector3Int(7,6,0), new Vector3Int(8,6,0),
            new Vector3Int(-14,-3,0), new Vector3Int(-13,-3,0),
            new Vector3Int(0,-6,0), new Vector3Int(1,-6,0), new Vector3Int(2,-6,0),
            new Vector3Int(9,-5,0), new Vector3Int(10,-5,0)
        };
        foreach (Vector3Int cell in obstacles)
        {
            maps["Obstacle_Tilemap"].SetTile(cell, obstacle);
        }

        Vector3Int[] decorCells =
        {
            new Vector3Int(-15,7,0), new Vector3Int(-13,8,0), new Vector3Int(-9,7,0),
            new Vector3Int(-1,7,0), new Vector3Int(6,7,0), new Vector3Int(12,6,0),
            new Vector3Int(-16,-5,0), new Vector3Int(-8,-7,0), new Vector3Int(-3,-8,0),
            new Vector3Int(4,-7,0), new Vector3Int(12,-6,0), new Vector3Int(15,-3,0)
        };
        foreach (Vector3Int cell in decorCells)
        {
            maps["Decoration_Tilemap"].SetTile(cell, decoration);
        }

        Vector3Int[] trapCells =
        {
            new Vector3Int(-1,0,0), new Vector3Int(0,0,0),
            new Vector3Int(6,1,0), new Vector3Int(7,1,0),
            new Vector3Int(-6,-3,0), new Vector3Int(-5,-3,0)
        };
        foreach (Vector3Int cell in trapCells)
        {
            maps["Trap_Tilemap"].SetTile(cell, trap);
        }

        Vector3Int[] slots =
        {
            new Vector3Int(-11,3,0), new Vector3Int(-7,4,0), new Vector3Int(-2,4,0), new Vector3Int(3,5,0),
            new Vector3Int(-11,-2,0), new Vector3Int(-6,-2,0), new Vector3Int(0,-2,0), new Vector3Int(6,-3,0),
            new Vector3Int(10,3,0), new Vector3Int(10,-2,0)
        };
        foreach (Vector3Int cell in slots)
        {
            maps["BuildSlot_Tilemap"].SetTile(cell, buildSlot);
        }
    }

    private static void SetThickLine(Tilemap map, Tile tile, Vector2Int from, Vector2Int to, int radius)
    {
        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);
        int sx = from.x < to.x ? 1 : -1;
        int sy = from.y < to.y ? 1 : -1;
        int err = dx - dy;
        int x = from.x;
        int y = from.y;

        while (true)
        {
            for (int ox = -radius; ox <= radius; ox++)
            {
                for (int oy = -radius; oy <= radius; oy++)
                {
                    map.SetTile(new Vector3Int(x + ox, y + oy, 0), tile);
                }
            }

            if (x == to.x && y == to.y) break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y += sy;
            }
        }
    }

    private static Tile GetOrCreateTile(string tileName, string spriteFileName)
    {
        string tilePath = RootPath + "/GeneratedTiles/" + tileName + ".asset";
        Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
        if (tile != null) return tile;

        Sprite sprite = LoadSprite(RootPath + "/Art/Tiles/" + spriteFileName);
        if (sprite == null)
        {
            Debug.LogError("Không tìm thấy sprite: " + RootPath + "/Art/Tiles/" + spriteFileName + ". Hãy đặt thư mục NK_Map_Complete trực tiếp trong Assets.");
            return null;
        }

        tile = ScriptableObject.CreateInstance<Tile>();
        tile.name = tileName;
        tile.sprite = sprite;
        AssetDatabase.CreateAsset(tile, tilePath);
        AssetDatabase.SaveAssets();
        return tile;
    }

    private static Sprite LoadSprite(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            bool changed = false;
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                changed = true;
            }
            if (importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                changed = true;
            }
            if (importer.filterMode != FilterMode.Point)
            {
                importer.filterMode = FilterMode.Point;
                changed = true;
            }
            if (importer.textureCompression != TextureImporterCompression.Uncompressed)
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                changed = true;
            }
            if (changed)
            {
                importer.SaveAndReimport();
            }
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
    }

    private static GameObject CreateChild(Transform parent, string name)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(parent);
        child.transform.localPosition = Vector3.zero;
        return child;
    }

    private static void AddMarker(GameObject obj, NKMapMarker.MarkerType markerType, string note)
    {
        NKMapMarker marker = obj.GetComponent<NKMapMarker>();
        if (marker == null) marker = obj.AddComponent<NKMapMarker>();
        marker.markerType = markerType;
        marker.note = note;
    }

    private static GameObject CreateSpawnPoint(Transform parent, string name, Vector3 position, bool boss)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);
        obj.transform.position = position;
        NKMapMarker marker = obj.AddComponent<NKMapMarker>();
        marker.markerType = boss ? NKMapMarker.MarkerType.BossSpawn : NKMapMarker.MarkerType.Spawn;
        marker.note = boss ? "Điểm xuất hiện boss" : "Điểm xuất hiện enemy";
        return obj;
    }

    private static NKPathContainer CreatePath(Transform parent, string name, Vector3[] positions)
    {
        GameObject pathObj = new GameObject(name);
        pathObj.transform.SetParent(parent);
        NKPathContainer container = pathObj.AddComponent<NKPathContainer>();

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject waypoint = new GameObject("Waypoint_" + (i + 1).ToString("00"));
            waypoint.transform.SetParent(pathObj.transform);
            waypoint.transform.position = positions[i];
            NKMapMarker marker = waypoint.AddComponent<NKMapMarker>();
            marker.markerType = NKMapMarker.MarkerType.Waypoint;
            marker.gizmoSize = 0.22f;
        }
        return container;
    }

    private static void CreateCore(Transform parent, Vector3 position)
    {
        GameObject core = new GameObject("Core_LastFlame");
        core.transform.SetParent(parent);
        core.transform.position = position;
        SpriteRenderer renderer = core.AddComponent<SpriteRenderer>();
        renderer.sprite = LoadSprite(RootPath + "/Art/Tiles/NK_Core.png");
        renderer.sortingOrder = 20;
        core.AddComponent<BoxCollider2D>();
        NKMapMarker marker = core.AddComponent<NKMapMarker>();
        marker.markerType = NKMapMarker.MarkerType.Core;
        marker.note = "Core/Base cần bảo vệ";
    }

    private static void CreateHeroArea(Transform parent)
    {
        GameObject top = new GameObject("HeroArea_Top");
        top.transform.SetParent(parent);
        top.transform.position = new Vector3(-1f, 4.8f, 0f);
        NKMapMarker topMarker = top.AddComponent<NKMapMarker>();
        topMarker.markerType = NKMapMarker.MarkerType.HeroArea;
        topMarker.note = "Vùng đặt hero phía trên đường đi";

        GameObject bottom = new GameObject("HeroArea_Bottom");
        bottom.transform.SetParent(parent);
        bottom.transform.position = new Vector3(-1f, -3.3f, 0f);
        NKMapMarker bottomMarker = bottom.AddComponent<NKMapMarker>();
        bottomMarker.markerType = NKMapMarker.MarkerType.HeroArea;
        bottomMarker.note = "Vùng đặt hero phía dưới đường đi";
    }

    private static void CreateHeroSlots(Transform parent)
    {
        Vector3[] positions =
        {
            new Vector3(-11f, 3f, 0f), new Vector3(-7f, 4f, 0f), new Vector3(-2f, 4f, 0f), new Vector3(3f, 5f, 0f),
            new Vector3(-11f, -2f, 0f), new Vector3(-6f, -2f, 0f), new Vector3(0f, -2f, 0f), new Vector3(6f, -3f, 0f),
            new Vector3(10f, 3f, 0f), new Vector3(10f, -2f, 0f)
        };

        Sprite slotSprite = LoadSprite(RootPath + "/Art/Tiles/NK_BuildSlot.png");
        for (int i = 0; i < positions.Length; i++)
        {
            GameObject slot = new GameObject("HeroSlot_" + (i + 1).ToString("00"));
            slot.transform.SetParent(parent);
            slot.transform.position = positions[i];
            SpriteRenderer renderer = slot.AddComponent<SpriteRenderer>();
            renderer.sprite = slotSprite;
            renderer.sortingOrder = 30;
            slot.AddComponent<BoxCollider2D>().isTrigger = true;
            slot.AddComponent<NKBuildSlot>();
        }
    }

    private static void CreateTrapObjects(Transform parent)
    {
        Vector3[] positions =
        {
            new Vector3(-0.5f, 0f, 0f),
            new Vector3(6.5f, 1f, 0f),
            new Vector3(-5.5f, -3f, 0f)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject trap = new GameObject("Trap_" + (i + 1).ToString("00"));
            trap.transform.SetParent(parent);
            trap.transform.position = positions[i];
            BoxCollider2D collider = trap.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            NKMapMarker marker = trap.AddComponent<NKMapMarker>();
            marker.markerType = NKMapMarker.MarkerType.Trap;
            marker.note = "Trap trigger area";
        }
    }

    private static void CreateEventSystem()
    {
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }
}
#endif
