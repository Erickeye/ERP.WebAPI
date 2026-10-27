# 06: 合約單前端

**Spec:** `.scratch/contract-management/spec.md`

**What to build:** 使用者可以在畫面上完成合約單的整個流程：列表查詢、建立與編輯草稿、上傳與下載 PDF、送簽、撤銷、簽核人核准與拒絕、最後一關簽核人作廢。依專案前端規範撰寫（使用 `vue-coding-style` skill）。

**Blocked by:** 02、03、04

**Status:** resolved

- [x] 列表頁：欄位、篩選（關鍵字、類型、對象、簽約公司、狀態、生效中／已到期、日期區間、待我簽核）、預設排序、分頁、新增與進入檢視入口，並顯示生效中／已到期標籤
- [x] 建立、編輯、檢視共用一頁：類型切換時對象選單隨之改變，簽約公司選單排除停用者
- [x] 草稿與已退回可儲存、上傳或更換 PDF、送簽、刪除（草稿）；簽核中可撤銷；簽核中、已核准、已作廢唯讀
- [x] 輪到目前使用者簽核時顯示核准與拒絕（拒絕必填原因）；已核准且目前使用者為最後一關簽核人時顯示作廢
- [x] 可下載 PDF；簽核進度沿用現有簽核進度查詢
- [x] 新增路由與選單入口，依合約單檢視、編輯、刪除權限顯示頁面與按鈕
- [x] API 封裝與錯誤處理比照現有頁面
- [x] 前端建置通過
- [ ] 在瀏覽器手動驗證完整流程（交由使用者；未驗證則保持不勾並在 Comments 說明）

## Comments

### 完成記錄（06）

- 驗證：`npm run build` 通過（只有既有的 chunk 大於 500 kB 警告）；靜態檢查：新檔案沒有直接 `import axios`、`window.confirm`／`alert`、`console.log`、寫死 `localhost`、直接讀寫 `localStorage`，`git diff --check` 通過，`node --check` 通過。
- 未勾：沒有在瀏覽器實際操作過畫面（沒有連真實後端與資料庫），所有互動（列表篩選、儲存草稿、上傳與下載、簽核按鈕、唯讀切換）都只靠建置與讀程式確認，需要使用者手動驗證。留給 ticket 07。
- 新增檔案：`views/ContractList.vue`、`views/ContractEdit.vue`、`api/contractApi.js`、`utils/contract.js`、`utils/file.js`（Blob 下載、檔名解析、判斷後端以 JSON 回失敗訊息）。修改：`router/index.js`（三個路由）、`DashboardLayout.vue`（新增「合約管理」卡片）、`utils/permissions.js`（三個合約單權限碼）。
- 路由權限：三個合約單路由要「合約單檢視」（3050）才能進入，符合 spec 第 60 條；代價是**沒有檢視權限的簽核人進不了頁面**（後端 API 不要求檢視權限，簽核人其實看得到待簽核的單）。上線時簽核人需要被授予「合約單檢視」，或之後把路由權限改成「檢視或簽核人」的判斷（目前前端沒有簽核人身分的資訊）。這是前端 UX 限制，真正限制在後端。
- 按鈕顯示全部取自後端回傳的旗標（`canEdit`、`canDelete`、`canRevoke`、`canVoid`、`canApprove`），前端不自行推算；「新增合約單」按鈕用 JWT 的編輯權限判斷。
- 送簽：先以目前畫面內容儲存草稿，再送簽（比照出差單）；沒上傳掃描檔、簽核設定問題等後端錯誤顯示在頁面上，單據維持原狀態。新增尚未儲存時不能上傳掃描檔，頁面提示先儲存草稿。
- 上傳前端先擋非 PDF 與超過 10 MB（後端仍會檢查）；下載走 `contractApi.downloadAttachment`（blob），後端失敗回 JSON 時顯示錯誤訊息而不是下載壞檔。
- 重複程式：簽核動作（核准、拒絕、撤銷、作廢）在 `ContractEdit.vue` 內以 `runApprovalAction` 共用；與出差單頁面仍是兩份類似的簽核區塊與進度表，等第三個簽核單據頁面出現再抽共用元件。
- 沒有新增前端自動化測試（專案沒有設定）。

### code-review 結果（06）

- Standards 軸（`vueCodingStyle.md`）：2 項硬違規、約 9 項判斷。已修正：(1) `ContractEdit.vue` 載入中切換單據時 `loading` 會卡住（舊請求的 finally 因 token 不符不恢復）——`loadContract` 開頭先恢復 `loading` 與 `progressLoading`；(2) 列表的下拉載入失敗只有警告提示——改為頁面上可見的錯誤區加 `ElMessage.error`；另修正：新增路由改要「合約單編輯」權限、下載中其他按鈕也鎖定（`downloading` 納入 `busy`）、頁面先載入客戶選單再載入單據（避免 `formError` 被蓋掉）、移除沒用到的 `CONTRACT_DELETE`。未處理的判斷：供應商、公司、客戶選單載入在列表與編輯頁各寫一份（第二次出現，可抽 composable）、`sendApproval` 沒走 `runApprovalAction`、狀態選項文字與後端 `statusDisplay` 重複、`CONTRACT_TABLE_TYPE` 是魔術數字（以註解對應後端）、下載遇 HTTP 4xx 時錯誤訊息只有預設文字（blob 內容讀不到）、儲存後 `unwrap(response).id` 沒有 null 防護。
- Spec 軸：前後端契約（Route、查詢參數、儲存欄位、上傳表單欄位 `Id`／`File`、簽核 body、旗標欄位、權限碼、`TableType` 11）逐項比對沒有不一致，畫面沒有用到詞彙表 Avoid 的詞。已知限制：列表的供應商與公司篩選選單沿用填單用選單（只列啟用中），已停用對象的歷史合約單無法用這兩個篩選找出（仍可用關鍵字、類型、日期區間找到），要改需新增「含停用」的選單端點；導覽卡片沒有依權限隱藏（與供應商卡片現況一致）；沒有檢視權限的簽核人進不了頁面（見上面路由權限說明）。
- 排序欄位（單號、合約編號、起日、迄日、合約總金額、狀態）是否都被後端排序白名單接受，由 `SortHelper` 依 `ContractVM` 屬性產生，測試只驗證了合約編號；金額排序須在真實資料庫驗證。
