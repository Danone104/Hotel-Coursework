const BASE_URL = 'http://localhost:5262/api/client/rent-history'

const headers = {
    'Content-Type': 'application/json',
}

const rentHistoryApi = {
    getActiveBookings: (userId,page,pageSize) => {
        return fetch(`${BASE_URL}/${userId}/bookings?page=${page}&pageSize=${pageSize}`,{
                method: 'GET',
                headers,
                credentials: 'include'
            })
            .then((response) => {
                if (!response.ok) throw new Error('Ошибка загрузки активных бронирований');
                return response.json();
            });
    },

    getCancelledBookings: (userId,page,pageSize) => {
        return fetch(`${BASE_URL}/${userId}/cancelled?page=${page}&pageSize=${pageSize}`,{
                method: 'GET',
                headers,
                credentials: 'include'
            })
            .then((response) => {
                if (!response.ok) throw new Error('Ошибка загрузки отменённых бронирований');
                return response.json();
            });
    },

    getRentsHistory: (userId,page,pageSize) => {
        return fetch(`${BASE_URL}/${userId}/rents?page=${page}&pageSize=${pageSize}`,{
                method: 'GET',
                headers,
                credentials: 'include'
            })
            .then((response) => {
                if (!response.ok) throw new Error('Ошибка загрузки истории аренд');
                return response.json();
            });
    },

    cancelBooking: (userId, bookingId) => {
        return fetch(`${BASE_URL}/${userId}/bookings/cancel-booking/${bookingId}`, {
            method: 'PATCH',
            headers: headers,
            credentials:'include'
            
        }).then((response) => {
            if (!response.ok) throw new Error('Ошибка отмены бронирования');
            return response;
        });
    }
}

export default rentHistoryApi;