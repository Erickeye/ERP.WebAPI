# 02: 合約掃描檔上傳與下載（後端）

**Spec:** `.scratch/contract-management/spec.md`

**What to build:** 建單人可以在草稿或已退回的合約單上傳或更換一份 PDF 合約掃描檔，簽核人與有檢視權限的人可以下載。檔案存在 SFTP，資料表只存遠端路徑。

**Blocked by:** 01

**Status:** resolved

- [x] 只接受 PDF（副檔名與內容類型皆檢查），大小上限 10 MB，不合規定回明確訊息
- [x] 草稿與已退回的合約單可上傳或更換；簽核中、已核准、已作廢不可更換
- [x] 更換掃描檔時舊檔的處理方式已決定並記錄在 Comments
- [x] 下載走合約單自己的端點，套用檢視權限與「待我簽核」可見範圍
- [x] 資料表只存 SFTP 遠端路徑，不存檔案內容
- [x] 需要「有檔案」判斷時可由路徑得知（供送簽 ticket 使用）
- [x] 測試以假的 SFTP 服務隔離，不實際連線，覆蓋格式、大小、可更換狀態與可見範圍
- [x] 專案編譯與全部測試通過
- [ ] SFTP 真實上傳與下載已在真實環境驗證（交由使用者，記錄於 Comments；未驗證則保持不勾並說明）

## Comments

### 完成記錄（02）

- 驗證：`dotnet test src/ERP.Tests/ERP.Tests.csproj` 全套 569 筆通過（`ContractServiceTests` 88 筆，本 ticket 新增 19 筆）。編譯輸出仍以 `-p:OutDir=<暫存資料夾>` 導出，原因同 ticket 01（Visual Studio 執行中的 `ERP.WebAPI` 鎖住 `bin`）。
- 未勾：SFTP 真實連線的上傳與下載未驗證，測試用 `FakeSftpService`（記憶體）。留給 ticket 07。
- 舊檔處理（本 ticket 的決定）：更換成功並儲存後，刪除舊的遠端檔案；刪除失敗不回報錯誤，只會留下孤兒檔案，不影響單據。資料庫儲存失敗時，刪除剛上傳的新檔再把例外丟出。
- 遠端路徑：由系統產生為 `/contract-{單號}-{亂數}.pdf`，放在 SFTP 根目錄，不使用上傳者提供的檔名（避免互相覆蓋與路徑穿越），也不需要建立子目錄。
- PDF 判斷：副檔名 `.pdf`、內容類型 `application/pdf` 與檔頭 `%PDF-` 三者都要符合。
- 為了指定遠端路徑，在 `ISftpService` 新增 `UploadStream(Stream, remotePath)`；既有 `UploadFile(IFormFile)` 一律存根目錄並沿用原檔名，不適合直接使用，沒有改動它。
- 下載端點目前要「合約單檢視」權限（與列表、單筆相同）。ticket 04 讓簽核人沒有檢視權限也能看待簽核的單時，需要一併放寬這個端點的可見範圍；規格的「待我簽核可見範圍」在 ticket 04 補齊。
- 上傳端點為 `UploadAttachment`（表單欄位 `Id`、`File`），下載端點為 `DownloadAttachment?id=`；控制器屬性（權限、`[Log]`、`[ValidateModel]`）以讀程式確認，沒有自動測試。
- 單檔大小也受 Kestrel／IIS 的請求大小上限影響（預設 30 MB 左右，高於 10 MB），未調整；10 MB 的檢查在服務層。
