// [UNITY-SKILL:SPRITEATLAS]
// [TYPE:PREBUILD]
// 빌드 직전에 Assets/Art/Sprites 하위의 각 폴더를 스캔해서
// 폴더 1개당 SpriteAtlas 1개를 자동 생성/갱신하는 스크립트.
// 새 카테고리 폴더를 추가해도 이 스크립트를 다시 수정할 필요 없음.
//
// 배치 위치: Assets/Editor/SpriteAtlas/SpriteAtlasPrebuildGenerator.cs

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using System.Linq;
using System.Collections.Generic;
using System.IO;

public class SpriteAtlasPrebuildGenerator : IPreprocessBuildWithReport
{
    // ── 프로젝트에 맞게 수정할 설정값 ──
    private const string SpriteRootFolder = "Assets/Art/Sprites";
    private const string AtlasOutputFolder = "Assets/Atlases";

    public int callbackOrder
    {
        get { return 0; }
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("[SpriteAtlas] 프리빌드 아틀라스 생성을 시작합니다.");

        // 1. 스프라이트 패커 모드 활성화
        EditorSettings.spritePackerMode = SpritePackerMode.SpriteAtlasV2;

        // 2. 설정이 실제로 반영됐는지 읽어서 확인 (Disabled면 팩킹이 안 됨)
        SpritePackerMode currentMode = EditorSettings.spritePackerMode;
        if (currentMode == SpritePackerMode.Disabled)
        {
            Debug.LogError("[SpriteAtlas] spritePackerMode가 Disabled 상태입니다. 아틀라스가 패킹되지 않습니다.");
            return;
        }
        else
        {
            Debug.Log("[SpriteAtlas] spritePackerMode 확인됨: " + currentMode);
        }

        if (Directory.Exists(SpriteRootFolder) == false)
        {
            Debug.LogWarning("[SpriteAtlas] 스프라이트 루트 폴더가 없습니다: " + SpriteRootFolder);
            return;
        }

        // 3. 루트 폴더 바로 아래의 하위 폴더 목록을 가져와서 폴더당 아틀라스 1개씩 생성
        string[] subFolders = Directory.GetDirectories(SpriteRootFolder);

        for (int i = 0; i < subFolders.Length; i++)
        {
            string subFolder = subFolders[i].Replace("\\", "/");
            string categoryName = Path.GetFileName(subFolder);
            string atlasPath = AtlasOutputFolder + "/" + categoryName + "_Atlas.spriteatlasv2";

            GenerateAtlasByFolder(subFolder, atlasPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[SpriteAtlas] 프리빌드 아틀라스 생성이 완료됐습니다.");
    }

    // 프로젝트 Assets 폴더 내부 스프라이트만 허용 (Packages, 내장 리소스 제외)
    private bool IsValidProjectSprite(string assetPath)
    {
        if (assetPath.StartsWith("Assets/") == false)
        {
            return false;
        }
        else if (assetPath.Contains("/Packages/") == true)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void GenerateAtlasByFolder(string spriteFolder, string atlasPath)
    {
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new string[] { spriteFolder });

        List<Object> spriteList = new List<Object>();
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            if (IsValidProjectSprite(path) == false)
            {
                continue;
            }

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null)
            {
                spriteList.Add(sprite);
            }
        }

        if (spriteList.Count == 0)
        {
            Debug.LogWarning("[SpriteAtlas] 스프라이트를 찾지 못했습니다: " + spriteFolder);
            return;
        }

        Object[] sprites = spriteList.ToArray();

        // 기존 아틀라스가 있으면 갱신, 없으면 새로 생성
        SpriteAtlasAsset atlasAsset;
        if (File.Exists(atlasPath) == true)
        {
            atlasAsset = SpriteAtlasAsset.Load(atlasPath);
            SpriteAtlas existingAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
            if (existingAtlas != null)
            {
                atlasAsset.Remove(existingAtlas.GetPackables());
            }
        }
        else
        {
            atlasAsset = new SpriteAtlasAsset();
            string outputDir = Path.GetDirectoryName(atlasPath);
            if (Directory.Exists(outputDir) == false)
            {
                Directory.CreateDirectory(outputDir);
            }
        }

        atlasAsset.Add(sprites);
        SpriteAtlasAsset.Save(atlasAsset, atlasPath);
        AssetDatabase.ImportAsset(atlasPath);

        ConfigureAtlasImporter(atlasPath);

        Debug.Log("[SpriteAtlas] 생성 완료: " + atlasPath + " (스프라이트 " + sprites.Length + "개)");
    }

    private void ConfigureAtlasImporter(string atlasPath)
    {
        SpriteAtlasImporter importer = AssetImporter.GetAtPath(atlasPath) as SpriteAtlasImporter;
        if (importer == null)
        {
            return;
        }

        // ── 텍스처 설정: 도트 리소스는 Point 필터 + 밉맵 비활성화가 필수 ──
        SpriteAtlasTextureSettings textureSettings = importer.textureSettings;
        textureSettings.filterMode = FilterMode.Point;
        textureSettings.generateMipMaps = false;
        importer.textureSettings = textureSettings;

        // ── 패킹 설정: 도트 그리드가 깨지지 않도록 회전 비활성화 ──
        SpriteAtlasPackingSettings packingSettings = importer.packingSettings;
        packingSettings.padding = 4;
        packingSettings.enableRotation = false;
        packingSettings.enableTightPacking = false;
        packingSettings.enableAlphaDilation = true;
        importer.packingSettings = packingSettings;

        // ── 플랫폼 설정: 프로젝트가 Android 타겟이므로 ASTC 적용 ──
        // 압축으로 인한 색상 밴딩이 눈에 띄면 format을 RGBA32로 바꿔서 비교해보세요.
        SetPlatformSettings(importer, "Android", TextureImporterFormat.ASTC_6x6);

        // 빌트인 방식이므로 빌드에 포함
        importer.includeInBuild = true;
        importer.SaveAndReimport();
    }

    private void SetPlatformSettings(SpriteAtlasImporter importer, string platformName, TextureImporterFormat format)
    {
        TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
        settings.name = platformName;
        settings.overridden = true;
        settings.maxTextureSize = 2048;
        settings.format = format;

        importer.SetPlatformSettings(settings);
    }
}
