export function formatDate(date) {
  if (!date) return '';
  const options = { year: 'numeric', month: '2-digit', day: '2-digit' };
  return new Intl.DateTimeFormat('zh-TW', options).format(new Date(date));
}

export function formatDateTime(date) {
  if (!date) return '';
  const options = { year: 'numeric', month: '2-digit', day: '2-digit',
     hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: false
   };
  return new Intl.DateTimeFormat('zh-TW', options).format(new Date(date));
}

// 'YYYY-MM-DD' 字串（或 ISO 字串開頭）轉成 'YYYY-MM-DD'，不經過時區換算
export function toDateOnly(value) {
  return value ? String(value).slice(0, 10) : '';
}

// 起訖日涵蓋的日曆天數（含起訖兩天、不扣假日）；任一為空或迄日早於起日回傳 0
export function countInclusiveDays(start, end) {
  const startDate = toDateOnly(start);
  const endDate = toDateOnly(end);
  if (!startDate || !endDate) return 0;
  const dayMs = 24 * 60 * 60 * 1000;
  const diff = (Date.parse(`${endDate}T00:00:00Z`) - Date.parse(`${startDate}T00:00:00Z`)) / dayMs;
  return Number.isFinite(diff) && diff >= 0 ? diff + 1 : 0;
}
