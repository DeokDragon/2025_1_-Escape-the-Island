using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMaterialChanger : MonoBehaviour
{
    [Header("µµ³¢ ·»´õ·¯")]
    public Renderer axeRenderer;

    [Header("°î±ªÀÌ ·»´õ·¯")]
    public Renderer pickaxeRenderer;

    [Header("µµ³¢ ÀçÁú")]
    public Material rockAxeMaterial;
    public Material ironAxeMaterial;
    public Material diamondAxeMaterial;

    [Header("°î±ªÀÌ ÀçÁú")]
    public Material rockPickaxeMaterial;
    public Material ironPickaxeMaterial;
    public Material diamondPickaxeMaterial;

    public void ApplyMaterial(string weaponName)
    {
        switch (weaponName)
        {
            case "RockAxe":
                axeRenderer.material = rockAxeMaterial;
                break;
            case "IronAxe":
                axeRenderer.material = ironAxeMaterial;
                break;
            case "DiamondAxe":
                axeRenderer.material = diamondAxeMaterial;
                break;

            case "RockPickaxe":
                pickaxeRenderer.material = rockPickaxeMaterial;
                break;
            case "IronPickaxe":
                pickaxeRenderer.material = ironPickaxeMaterial;
                break;
            case "DiamondPickaxe":
                pickaxeRenderer.material = diamondPickaxeMaterial;
                break;

            default:
                Debug.LogWarning($"[WeaponMaterialChanger] {weaponName}¿¡ ÇØ´çÇÏ´Â ÀçÁú ¾øÀ½");
                break;
        }
    }
}