// 後端回傳一律是 ResultModel：{ isSuccess, errorCode, errorMessage, data }
// 業務失敗也是 HTTP 200 加 isSuccess: false，所以不能只靠 catch 判斷失敗。

function getResult(response) {
  return response?.data ?? response
}

export function unwrap(response) {
  return getResult(response)?.data ?? null
}

export function isSuccess(response) {
  return getResult(response)?.isSuccess === true
}

// 欄位驗證失敗時 data 是 { 欄位名: [錯誤訊息...] }，把訊息接在 errorMessage 後面一併顯示
function getFieldErrorText(data) {
  if (!data || typeof data !== 'object' || Array.isArray(data)) return ''
  return Object.values(data)
    .flat()
    .filter((message) => typeof message === 'string')
    .join('；')
}

export function getApiErrorMessage(error, fallback = '操作失敗') {
  const result = error?.response?.data ?? error?.data ?? error
  const message = result?.errorMessage ?? fallback
  const fieldErrors = getFieldErrorText(result?.data)
  return fieldErrors ? `${message}：${fieldErrors}` : message
}
