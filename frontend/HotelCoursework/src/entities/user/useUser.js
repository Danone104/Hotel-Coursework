import { useState } from "react"

const useUser = ()=>{
    const [user,setUser]=useState(null)
    return {user,setUser}
}
export default useUser 