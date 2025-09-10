using System;
using UI;
using UnityEngine;

public static class AssetsManager
{
    private const string ResourcesUiAssets = "UiAssets";
    private const string UnitsAssets = "UnitAssets";

    public static UIAssets LoadAndInitUIAssets()
    {
        var uiAssets = Resources.Load<UIAssets>(ResourcesUiAssets);
        if (uiAssets == null)
        {
            throw new Exception("Need create UiAssets first");
        }

        uiAssets.Init();

        return uiAssets;
    }

    public static UIAssets LoadAndInitUnitsAssets()
    {
        var unitsAssets = Resources.Load<UIAssets>(UnitsAssets);
        if (unitsAssets == null)
        {
            throw new Exception("Need create UnitsAssets first");
        }

        unitsAssets.Init();

        return unitsAssets;
    }
}