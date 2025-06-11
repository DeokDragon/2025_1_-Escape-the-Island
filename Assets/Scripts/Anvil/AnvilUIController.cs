using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class AnvilUIController : MonoBehaviour
{
    [SerializeField] private Slot[] materialSlots;       
    [SerializeField] private Slot resultSlot;            
    [SerializeField] private Slot toolSlot;
    [SerializeField] private Slot previewSlot;

    [SerializeField] private Slot[] recipeDisplaySlots;

    [SerializeField] private TextMeshProUGUI statusText;

    [SerializeField] private Button[] filterButtons;       

    private bool isUpgrading = false;

    [SerializeField] private GameObject previewPanel;

    [SerializeField] private GameObject go_AnvilUIBase;


    void Start()
    {
        Debug.Log("Anvil UI 시작");
        var dummy = ToolUpgradeDatabase.AllRecipes.Count; // 강제 참조
    }
    IEnumerator UpgradeCoroutine(ToolUpgradeRecipe recipe)
    {
        isUpgrading = true;
        statusText.text = "업그레이드 중...";

        yield return new WaitForSeconds(2f); // 업그레이드 대기 시간

        // 재료 소비
        ConsumeMaterials(recipe);

        // 결과 아이템 로드
        Item upgradedItem = Instantiate(Resources.Load<Item>("Item/" + recipe.resultItemName));

        // 강화된 아이템 속성 수정
        upgradedItem.attackPower += 10;  // 공격력 증가 (예시)
        upgradedItem.materialType = Item.MaterialType.Diamond;  // 머터리얼 변경 (enum 기준)

        // 결과 슬롯에 추가
        resultSlot.ForceAddItem(upgradedItem, 1);

        statusText.text = "업그레이드 완료!";
        isUpgrading = false;
    }

    bool HasRequiredMaterials(ToolUpgradeRecipe recipe)
    {
        foreach (var requirement in recipe.requiredMaterials)
        {
            bool found = false;
            foreach (var slot in materialSlots)
            {
                if (slot.item != null &&
                    slot.item.itemName == requirement.Key &&
                    slot.itemCount >= requirement.Value)
                {
                    found = true;
                    break;
                }
            }
            if (!found) return false;
        }
        return true;
    }

    void ConsumeMaterials(ToolUpgradeRecipe recipe)
    {
        foreach (var key in recipe.requiredMaterials.Keys.ToList()) // ToList()로 먼저 복사
        {
            int remaining = recipe.requiredMaterials[key];

            foreach (var slot in materialSlots)
            {
                if (slot.item != null && slot.item.itemName == key)
                {
                    int consume = Mathf.Min(slot.itemCount, remaining);
                    slot.SetSlotCount(-consume);
                    remaining -= consume;

                    if (recipe.requiredMaterials.ContainsKey(key))
                    {
                        recipe.requiredMaterials[key] = remaining;
                    }

                    if (remaining <= 0)
                        break;
                }
            }
        }
    }
    public void FilterList(string type)
    {
        // 예: type == "Axe" 또는 "Pickaxe"
        // 이 함수는 5번 버튼에서 onClick으로 연결됨
        // 목록 표시를 갱신하는 용도
    }
    public void OnUpgradeButton()
    {
        Debug.Log("[검사용] toolSlot.item.itemName = " + toolSlot.item.itemName);
        Item baseTool = toolSlot.item;
        if (baseTool == null)
        {
            Debug.Log("[강화 실패] 도구 없음");
            statusText.text = "도구를 넣어주세요.";
            return;
        }

        Debug.Log("[강화 시도] 도구 이름: " + baseTool.itemName);

        ToolUpgradeRecipe recipe = ToolUpgradeDatabase.AllRecipes.Find(r => r.inputItemName == baseTool.itemName);
        if (recipe == null)
        {
            Debug.Log("[강화 실패] 레시피 없음: " + baseTool.itemName);
            statusText.text = "이 도구는 강화할 수 없습니다.";
            return;
        }

        if (!HasEnoughMaterials(recipe))
        {
            Debug.Log("[강화 실패] 재료 부족");
            statusText.text = "재료가 부족합니다.";
            return;
        }

        Debug.Log("[강화 성공 조건 만족]");

        // 강화 실행
        recipe.Craft(toolSlot, materialSlots, resultSlot);
        statusText.text = "강화 완료!";
    }
    bool HasEnoughMaterials(ToolUpgradeRecipe recipe)
    {
        foreach (var req in recipe.requiredMaterials)
        {
            int totalInSlots = 0;
            foreach (var slot in materialSlots)
            {
                if (slot.item != null && slot.item.itemName == req.Key)
                {
                    totalInSlots += slot.itemCount;
                }
            }

            if (totalInSlots < req.Value)
                return false;
        }
        return true;
    }
    private ToolUpgradeRecipe FindMatchingRecipe(string baseItemName)
    {
        foreach (var recipe in ToolUpgradeDatabase.AllRecipes)
        {
            if (recipe.inputItemName == baseItemName)
                return recipe;
        }
        return null;
    }

    private void UpdatePreview()
    {
        previewPanel.SetActive(false);
        previewSlot.ClearSlot();

        if (toolSlot.item == null) return;

        // 레시피 찾기
        var recipe = FindMatchingRecipe(toolSlot.item.itemName);
        if (recipe != null && recipe.CheckRecipe(toolSlot, materialSlots))
        {
            Item previewItem = recipe.GetPreviewResult();
            if (previewItem != null)
            {
                previewSlot.ForceAddItem(previewItem, 1);
                previewPanel.SetActive(true);
            }
        }
    }
    public void RefreshPreview()
    {
        UpdatePreview();
    }
    public void UpdateAnvilPreview()
    {
        Debug.Log("--- [1] UpdateAnvilPreview 함수 시작 ---");

        // --- 슬롯 연결 확인 ---
        if (toolSlot == null) { Debug.LogError("오류: Tool Slot 자체가 인스펙터에 연결되지 않았습니다!"); return; }
        if (previewSlot == null) { Debug.LogError("오류: Preview Slot (빨간칸)이 연결되지 않았습니다!"); return; }
        if (recipeDisplaySlots == null) { Debug.LogError("오류: Recipe Display Slots (핑크칸) 배열이 연결되지 않았습니다!"); return; }

        // --- 기존 정보 초기화 ---
        previewSlot.ClearSlot();
        foreach (var slot in recipeDisplaySlots)
        {
            if (slot != null) slot.ClearSlot();
            else { Debug.LogError("오류: Recipe Display Slots 배열에 비어있는(None) 칸이 있습니다!"); return; }
        }
        Debug.Log("[2] 모든 미리보기/재료 표시 슬롯을 초기화했습니다.");

        // --- 도구 확인 ---
        if (toolSlot.item == null)
        {
            Debug.LogWarning("[3] Tool Slot에 아이템이 없습니다. 여기서 함수를 종료합니다.");
            return;
        }
        Debug.Log($"[3] Tool Slot 아이템 확인: '{toolSlot.item.itemName}'");

        // --- 레시피 검색 ---
        ToolUpgradeRecipe recipe = ToolUpgradeDatabase.AllRecipes.Find(r => r.inputItemName == toolSlot.item.itemName);
        if (recipe == null)
        {
            Debug.LogError($"[4] 오류: '{toolSlot.item.itemName}'에 대한 레시피를 데이터베이스에서 찾을 수 없습니다!");
            return;
        }
        Debug.Log($"[4] 레시피 발견! 결과물: '{recipe.resultItemName}'");

        // --- 미리보기(빨간칸) 업데이트 ---
        Debug.Log("[5] 미리보기(빨간칸) 업데이트를 시도합니다...");
        Item resultItem = Resources.Load<Item>("Item/" + recipe.resultItemName);
        if (resultItem == null)
        {
            Debug.LogError($"[5] 오류: Resources 폴더에서 결과물 '{recipe.resultItemName}'을 로드할 수 없습니다! 경로와 파일명을 확인하세요.");
        }
        else
        {
            previewSlot.SetItem(resultItem, 1);
            Debug.Log($"[5] 성공: 미리보기에 '{resultItem.itemName}'을 표시했습니다.");
        }

        // --- 필요 재료(핑크칸) 업데이트 ---
        Debug.Log($"[6] 필요 재료(핑크칸) 업데이트를 시도합니다... (총 {recipe.requiredMaterials.Count} 종류)");
        int i = 0;
        foreach (var requiredMaterial in recipe.requiredMaterials)
        {
            if (i >= recipeDisplaySlots.Length)
            {
                Debug.LogWarning($"[6] 경고: 재료 표시 슬롯(핑크칸)이 부족하여 모든 재료를 표시할 수 없습니다.");
                break;
            }
            string materialName = requiredMaterial.Key;
            int materialAmount = requiredMaterial.Value;
            Item materialItem = Resources.Load<Item>("Item/" + materialName);

            if (materialItem == null)
            {
                // 아이템 로드에 실패하면 에러 로그만 남기고, 슬롯에 아무 작업도 하지 않습니다.
                Debug.LogError($"[6] 오류: Resources 폴더에서 재료 '{materialName}'을 로드할 수 없습니다!");
            }
            else
            {
                // --- 바로 이 부분입니다! ---
                // SetItem을 ForceAddItem으로 변경하여 'CanReceive' 검사를 무시하고 강제로 아이템을 표시합니다.
                recipeDisplaySlots[i].ForceAddItem(materialItem, materialAmount);
                Debug.Log($"  - {i}번 핑크 슬롯에 '{materialName}' {materialAmount}개 표시 완료.");
            }
            i++;
        }

        Debug.Log("--- [7] UpdateAnvilPreview 함수 정상 종료 ---");
    }
}

