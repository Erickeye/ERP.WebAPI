import http, { getHeaders } from './http';


export const getLeaveTypes = () => {
  return http.get('/_1030DayOff/GetLeaveType')
}

export const createDayOff = (data) => {
  return http.post('/_1030DayOff/Create', data, {
    headers: getHeaders(),
  });
};

export const GetStaffSelect = () => {
  return http.get('/Shared/GetStaffSelect', {
    headers: getHeaders(),
  });
};