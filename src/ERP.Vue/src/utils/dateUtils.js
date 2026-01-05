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