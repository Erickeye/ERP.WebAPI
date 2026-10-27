// 檔案下載的共用處理：Blob 轉成瀏覽器下載、判斷後端其實回了 ResultModel 失敗訊息

// 後端下載失敗時回的是 JSON 的 ResultModel（HTTP 200），axios 以 blob 接到後內容是 JSON 文字；
// 這時回傳錯誤訊息，呼叫端顯示出來，不可下載出一個壞檔案。成功的檔案回傳 null。
export async function readBlobErrorMessage(blob, fallback = '下載失敗') {
  if (!blob || !String(blob.type).includes('application/json')) return null

  try {
    const result = JSON.parse(await blob.text())
    return result?.errorMessage ?? fallback
  } catch {
    return fallback
  }
}

// 從 Content-Disposition 解析檔名（支援 filename*=UTF-8''...），解析不到用 fallbackName
export function parseDownloadFileName(contentDisposition, fallbackName) {
  if (!contentDisposition) return fallbackName

  const encoded = /filename\*=UTF-8''([^;]+)/i.exec(contentDisposition)
  if (encoded) {
    try {
      return decodeURIComponent(encoded[1])
    } catch {
      return fallbackName
    }
  }

  const plain = /filename="?([^";]+)"?/i.exec(contentDisposition)
  return plain ? plain[1] : fallbackName
}

// 觸發瀏覽器下載
export function saveBlob(blob, fileName) {
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = fileName
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
  URL.revokeObjectURL(url)
}
