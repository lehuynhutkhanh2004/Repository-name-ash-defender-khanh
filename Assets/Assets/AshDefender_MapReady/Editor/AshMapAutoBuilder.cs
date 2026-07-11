using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor tool that builds a complete ready-to-use 2D defense map from the included PNG.
/// Menu: Tools/Ash Defender/Build Map 01 - Ash Forest
/// </summary>
public static class AshMapAutoBuilder
{
    private const string MapSpritePath = "Assets/AshDefender_MapReady/Art/Maps/Map_01_AshForest.png";
    private const string RootName = "Map_01_AshForest_Root";
    private const float PixelsPerUnit = 100f;
    private const float ImageWidth = 1920f;
    private const float ImageHeight = 1080f;

    [MenuItem("Tools/Ash Defender/Build Map 01 - Ash Forest")]
    public static void BuildMap()
    {
        PrepareSpriteImportSettings();

        Sprite mapSprite = AssetDatabase.LoadAssetAtPath<Sprite>(MapSpritePath);
        if (mapSprite == null)
        {
            EditorUtility.DisplayDialog(
                "Map Sprite Missing",
                "Không tìm thấy Map_01_AshForest.png. Hãy kiểm tra file nằm ở:\n" + MapSpritePath,
                "OK"
            );
            return;
        }

        GameObject oldRoot = GameObject.Find(RootName);
        if (oldRoot != null)
        {
            bool replace = EditorUtility.DisplayDialog(
                "Replace Existing Map?",
                "Scene đã có " + RootName + ". Bạn có muốn xóa map cũ và tạo lại không?",
                "Tạo lại",
                "Hủy"
            );

            if (!replace) return;
            Object.DestroyImmediate(oldRoot);
        }

        GameObject root = new GameObject(RootName);
        Undo.RegisterCreatedObjectUndo(root, "Build Ash Forest Map");

        CreateBackground(root.transform, mapSprite);
        CreateMapObjects(root.transform);
        SetupCamera();

        Selection.activeGameObject = root;
        EditorGUIUtility.PingObject(root);

        EditorUtility.DisplayDialog(
            "Map Created",
            "Đã tạo xong Map_01_AshForest.\n\nBạn có thể nhấn Play để xem camera. Nếu muốn enemy chạy, hãy dùng Path_A_LeftToCore / Path_B_TopToCore / BossPath_BottomToCore cho wave system của bạn.",
            "OK"
        );
    }

    private static void PrepareSpriteImportSettings()
    {
        TextureImporter importer = AssetImporter.GetAtPath(MapSpritePath) as TextureImporter;
        if (importer == null) return;

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

        if (Mathf.Abs(importer.spritePixelsPerUnit - PixelsPerUnit) > 0.01f)
        {
            importer.spritePixelsPerUnit = PixelsPerUnit;
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

    private static void CreateBackground(Transform parent, Sprite mapSprite)
    {
        GameObject background = new GameObject("Map_Background");
        background.transform.SetParent(parent);
        background.transform.position = Vector3.zero;

        SpriteRenderer renderer = background.AddComponent<SpriteRenderer>();
        renderer.sprite = mapSprite;
        renderer.sortingOrder = -20;
    }

    private static void CreateMapObjects(Transform parent)
    {
        GameObject important = new GameObject("Important_Points");
        important.transform.SetParent(parent);

        GameObject paths = new GameObject("Enemy_Paths");
        paths.transform.SetParent(parent);

        GameObject deployRoot = new GameObject("DeployPoints");
        deployRoot.transform.SetParent(parent);

        GameObject spawnA = CreateMarker("SpawnPointA_Left", important.transform, FromImage(95, 520), AshMapMarker.MarkerType.Spawn, new Color(0.75f, 0.2f, 1f), 0.18f);
        GameObject spawnB = CreateMarker("SpawnPointB_Top", important.transform, FromImage(485, 120), AshMapMarker.MarkerType.Spawn, new Color(0.75f, 0.2f, 1f), 0.18f);
        GameObject bossSpawn = CreateMarker("BossSpawnPoint_BottomLeft", important.transform, FromImage(135, 930), AshMapMarker.MarkerType.BossSpawn, new Color(1f, 0.2f, 0.1f), 0.25f);

        GameObject core = CreateMarker("Core_LastFlame", important.transform, FromImage(1665, 520), AshMapMarker.MarkerType.Core, new Color(1f, 0.75f, 0.1f), 0.35f);
        CircleCollider2D coreCollider = core.AddComponent<CircleCollider2D>();
        coreCollider.radius = 0.45f;
        coreCollider.isTrigger = true;

        CreatePath(
            "Path_A_LeftToCore",
            paths.transform,
            new Vector2[]
            {
                FromImage(95,520),
                FromImage(210,520),
                FromImage(410,470),
                FromImage(610,430),
                FromImage(810,490),
                FromImage(1020,555),
                FromImage(1220,515),
                FromImage(1440,520),
                FromImage(1665,520)
            }
        );

        CreatePath(
            "Path_B_TopToCore",
            paths.transform,
            new Vector2[]
            {
                FromImage(485,120),
                FromImage(510,130),
                FromImage(575,275),
                FromImage(700,360),
                FromImage(810,490),
                FromImage(1020,555),
                FromImage(1220,515),
                FromImage(1440,520),
                FromImage(1665,520)
            }
        );

        CreatePath(
            "BossPath_BottomToCore",
            paths.transform,
            new Vector2[]
            {
                FromImage(135,930),
                FromImage(160,950),
                FromImage(260,790),
                FromImage(450,680),
                FromImage(650,610),
                FromImage(810,490),
                FromImage(1020,555),
                FromImage(1220,515),
                FromImage(1440,520),
                FromImage(1665,520)
            }
        );

        Vector2[] deployPoints = new Vector2[]
        {
            FromImage(420,320),
            FromImage(640,280),
            FromImage(950,350),
            FromImage(1180,660),
            FromImage(1390,355),
            FromImage(330,680),
            FromImage(610,760),
            FromImage(950,720),
            FromImage(1260,350),
            FromImage(1450,700)
        };

        for (int i = 0; i < deployPoints.Length; i++)
        {
            GameObject point = CreateMarker("DeployPoint_" + (i + 1).ToString("00"), deployRoot.transform, deployPoints[i], AshMapMarker.MarkerType.DeployPoint, new Color(0.1f, 0.8f, 1f), 0.14f);
            point.AddComponent<AshDeployPoint>();

            CircleCollider2D deployCollider = point.AddComponent<CircleCollider2D>();
            deployCollider.radius = 0.35f;
            deployCollider.isTrigger = true;
        }

        // Empty containers to keep the scene clean and easy to connect to your existing systems.
        GameObject managers = new GameObject("Managers_ConnectYourWaveAndDeployScriptsHere");
        managers.transform.SetParent(parent);

        // Keep created spawn references visible in hierarchy, even though variables are not used later.
        spawnA.transform.SetSiblingIndex(0);
        spawnB.transform.SetSiblingIndex(1);
        bossSpawn.transform.SetSiblingIndex(2);
    }

    private static GameObject CreatePath(string name, Transform parent, Vector2[] points)
    {
        GameObject pathObject = new GameObject(name);
        pathObject.transform.SetParent(parent);

        Transform[] waypointTransforms = new Transform[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            GameObject waypoint = CreateMarker("Waypoint_" + (i + 1).ToString("00"), pathObject.transform, points[i], AshMapMarker.MarkerType.Waypoint, Color.yellow, 0.08f);
            waypointTransforms[i] = waypoint.transform;
        }

        AshPathContainer path = pathObject.AddComponent<AshPathContainer>();
        path.SetWaypoints(waypointTransforms);

        return pathObject;
    }

    private static GameObject CreateMarker(string name, Transform parent, Vector2 position, AshMapMarker.MarkerType type, Color color, float radius)
    {
        GameObject marker = new GameObject(name);
        marker.transform.SetParent(parent);
        marker.transform.position = new Vector3(position.x, position.y, 0f);

        AshMapMarker markerComponent = marker.AddComponent<AshMapMarker>();
        markerComponent.Setup(type, color, radius);

        return marker;
    }

    private static Vector2 FromImage(float x, float y)
    {
        float worldX = (x - ImageWidth / 2f) / PixelsPerUnit;
        float worldY = (ImageHeight / 2f - y) / PixelsPerUnit;
        return new Vector2(worldX, worldY);
    }

    private static void SetupCamera()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            camera = cameraObject.AddComponent<Camera>();
        }

        camera.transform.position = new Vector3(0f, 0f, -10f);
        camera.orthographic = true;
        camera.orthographicSize = 5.8f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.08f, 0.08f, 0.1f);
    }
}
