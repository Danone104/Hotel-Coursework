const URL = 'http://localhost:5262/api/rooms'

const headers = {
    'Content-Type': 'application/json',
}

const roomApi={
    getByPage: (page, pageSize)=>{
        return fetch(`${URL}?page=${page}&pageSize=${pageSize}`).then((response)=> response.json())
    },
    getCount : ()=>{
        return fetch(`${URL}/count`).then((response)=> response.json())
    },
    getById : (id)=>{
        return fetch(`${URL}/${id}`).then((response)=>response.json())
    }
    
}

export default roomApi
