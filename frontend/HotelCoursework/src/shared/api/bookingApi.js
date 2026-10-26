const clientURL = 'http://localhost:5262/api/client/booking'
const publicURL='http://localhost:5262/api/public/booking'

const headers = {
    'Content-Type': 'application/json',
}

const bookingApi={
    getOccupiedDates: (roomId,year,month,user)=>{
        if(user){
            return fetch(`${clientURL}/occupied-dates/${roomId}?year=${year}&month=${month}`,{
                method: 'GET',
                headers,
                credentials: 'include'
            })
            .then((response)=> response.json())
        }
        else{
             return fetch(`${publicURL}/occupied-dates/${roomId}?year=${year}&month=${month}`)
            .then((response)=> response.json())
        }
        
    },
    bookingByUser : (userId,roomId,checkIn,checkOut)=>{
        return fetch(`${clientURL}`,{
            method: 'POST',
            headers,
            body: JSON.stringify({
                userId,
                roomId,
                checkIn,
                checkOut
            }),
            credentials: 'include'
        })
    },
    booking : (phoneNumber,firstName,lastName,patronymic,roomId,checkIn,checkOut)=>{
        fetch(`${publicURL}`,{
                method: 'POST',
                headers,
                body: JSON.stringify({
                    phoneNumber,
                    firstName,
                    lastName,
                    patronymic,
                    roomId,
                    checkIn,
                    checkOut
                }),
                credentials: 'include'
            })
    }
    
}

export default bookingApi