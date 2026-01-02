<template>
  <div v-if="visible" :class="`notification ${type}`">
    {{ message }}
  </div>
</template>

<script>
import mitt from 'mitt'

// 1. 在組件外部定義 emitter，確保它是單例（Singleton）
const emitter = mitt()

// 2. 直接導出這個輔助函式，讓其他地方可以直接 import
export const showNotification = (msg, notificationType = 'success', duration = 3000) => {
  emitter.emit('notify', { msg, notificationType, duration })
}

export default {
  name: 'Notification',
  data() {
    return {
      visible: false,
      message: '',
      type: 'success',
      timer: null
    }
  },
  mounted() {
    // 3. 監聽同一個 emitter
    emitter.on('notify', ({ msg, notificationType, duration }) => {
      if (this.timer) clearTimeout(this.timer)

      this.message = msg
      this.type = notificationType
      this.visible = true

      this.timer = setTimeout(() => {
        this.visible = false
      }, duration)
    })
  },
  beforeUnmount() {
    emitter.off('notify')
    if (this.timer) clearTimeout(this.timer)
  }
}
</script>

<style>
.notification {
  position: fixed;
  bottom: 10px;
  right: 10px;
  padding: 10px;
  border-radius: 5px;
  color: white;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  transition: opacity 0.3s ease, transform 0.3s ease;
}
.notification.success {
  background-color: #4caf50;
}
.notification.error {
  background-color: #f44336;
}
.notification.enter-active,
.notification.leave-active {
  opacity: 0;
  transform: translateY(-10px);
}
</style>