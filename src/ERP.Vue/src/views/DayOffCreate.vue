<script setup>
import { ref, onMounted } from 'vue'
import { getLeaveTypes, createDayOff, GetStaffSelect } from '@/api/dayoff'
import { useRouter } from 'vue-router';

// 假別下拉
const leaveTypes = ref([])
//員工下拉
const staffs = ref([])

const today = () => new Date().toISOString().slice(0, 10)

const todayTime = (hh, mm) => {
    return `${today()}T${hh}:${mm}`
}

// 表單資料（對應 DayOffInputVM）
const form = ref({
    applicationDate: today(),
    beginDate: todayTime('08', '30'), // ✅ 今天 08:30
    endDate: todayTime('17', '30'),   // ✅ 今天 17:30
    leaveTaker: 1,   // 請假人（必填） 
    applicant: null, // 申請者
    proxy: null, // 代理人
    leaveType: '', //假別
    reason: '', //理由
    staff: '',
})

// 取得假別下拉選單
const loadLeaveTypes = async () => {
    const res = await getLeaveTypes()
    leaveTypes.value = res.data.data.items
}
// 取得員工下拉選單
const loadStaffs = async () => {
    const res = await GetStaffSelect()
    staffs.value = res.data.data.items
}
// 送出
const submit = async () => {
    await createDayOff(form.value)
    alert('請假單新增成功')
}

onMounted(() => {
  loadLeaveTypes()
  loadStaffs()
})

const router = useRouter();

const goBack = () => {
  router.back();
};
</script>

<template>
    
    <div>
        <button @click="goBack" style="margin-bottom: 1rem;">返回</button>
        <h2>新增請假單</h2>

        <form>
            <div>
                <label>申請日期</label>
                <input type="date" v-model="form.applicationDate" readonly />
            </div>

            <div>
                <label>申請者</label>
                <input type="number" v-model="form.applicant" placeholder="申請者員工ID" />
            </div>

            <div>
                <label>代理人</label>
                <el-select v-model="selectedStaff" placeholder="請選擇員工" clearable>
                <el-option
                    v-for="staff in staffs"
                    :key="staff.value"
                    :label="staff.text"
                    :value="staff.value"
                />
                </el-select>
            </div>

            <div>
                <label>假別</label>
                <select v-model="form.leaveType">
                    <option value="">請選擇假別</option>
                    <option v-for="item in leaveTypes" :key="item.value" :value="item.value">
                        {{ item.text }}
                    </option>
                </select>
            </div>

            <div>
                <label>開始日期</label>
                <input type="datetime-local" v-model="form.beginDate" />
            </div>

            <div>
                <label>結束日期</label>
                <input type="datetime-local" v-model="form.endDate" />
            </div>

            <div>
                <label>請假原因</label>
                <textarea v-model="form.reason"></textarea>
            </div>

            <button @click="submit">送出申請</button>
        </form>
    </div>
</template>
