# Glossary Map

## Contexts

- [公司管理](./docs/company/GLOSSARY.md): 我方旗下的法人實體（公司主檔、啟用與停用、單據歸屬哪間公司）
- [庫存管理](./docs/inventory/GLOSSARY.md): 物料如何進入、停留在、離開公司倉庫（進貨、領料、庫存異動）
- [差勤](./docs/attendance/GLOSSARY.md): 員工因公離開公司的申請與簽核（目前為出差）

## Relationships

- **差勤 ↔ 庫存管理**: 目前無直接關聯。兩者共用通用簽核引擎與員工主檔，但各自的單據與術語互不依賴。
- **公司管理 → 庫存管理、差勤**: 領料單與出差單各自引用一間公司作為單據公司；公司管理不依賴這兩個 context。
