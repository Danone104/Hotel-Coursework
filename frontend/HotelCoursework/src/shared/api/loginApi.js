const URL = 'http://localhost:5262/api/authentication'

const headers = {
    'Content-Type': 'application/json',
}

const loginApi={
    login: (userName, password)=>{
        return fetch(`${URL}/login`,{
                method: 'POST',
                headers,
                body: JSON.stringify({
                    userName,
                    password,
                }),
                credentials: 'include'
            })
    },  
    getProfile : ()=>{
        return fetch(`${URL}/user-profile`,{
        method: 'GET',
        headers,
        credentials: 'include'
    }).then((response)=> response.json())
    },
    userNameAvailable : (userName)=>{
        return fetch(`${URL}/name-availability?userName=${userName}`).then((response)=> response.json())
    },
    logout : ()=>{
        return fetch(`${URL}/logout`,{
        method: 'GET', 
        credentials: 'include'  
    })
    },
    register : (userName,password,phone,firstName,lastName,patronymic)=>{
        return fetch(`${URL}/register`,{
                method: 'POST',
                headers,
                body: JSON.stringify({
                    userName,
                    password,   
                    phone,
                    firstName,
                    lastName,
                    patronymic
                })
            }).then((response)=>response.json())
        
    }
    
}

export default loginApi
